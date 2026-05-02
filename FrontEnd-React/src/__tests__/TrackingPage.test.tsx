import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import TrackingPage from '../pages/TrackingPage';
import type { DroneRecord, GeoLocation } from '../types';

// Mock Google Maps — avoids browser API requirements in jsdom
vi.mock('@vis.gl/react-google-maps', () => ({
  APIProvider: ({ children }: any) => <div data-testid="api-provider">{children}</div>,
  Map: ({ children }: any) => <div data-testid="google-map">{children}</div>,
  AdvancedMarker: ({ children, position }: any) => (
    <div data-testid="marker" data-lat={position?.lat} data-lng={position?.lng}>
      {children}
    </div>
  ),
  Pin: () => <div data-testid="pin" />,
  Polyline: ({ path }: any) => (
    <div
      data-testid="polyline"
      data-from={JSON.stringify(path?.[0])}
      data-to={JSON.stringify(path?.[1])}
    />
  ),
  useMap: () => null,
}));

// Mock API client
vi.mock('../api/client', () => ({
  getFleet: vi.fn(),
  getHomeLocation: vi.fn(),
}));

import { getFleet, getHomeLocation } from '../api/client';

const mockGetFleet = getFleet as ReturnType<typeof vi.fn>;
const mockGetHomeLocation = getHomeLocation as ReturnType<typeof vi.fn>;

const makeLocation = (lat: number, lng: number): GeoLocation => ({
  latitude: lat,
  longitude: lng,
});

const makeDrone = (overrides: Partial<DroneRecord> = {}): DroneRecord => ({
  id: 'id-1',
  droneId: 'aabbccddeeff00112233445566778899',
  orderId: null,
  badgeNumber: 1,
  destination: null,
  currentLocation: makeLocation(40.1234, -75.5678),
  homeLocation: makeLocation(40.0, -75.0),
  state: 'Ready',
  droneUrl: 'http://drone:8080',
  dispatchUrl: 'http://dispatch:83',
  direction: 90,
  ...overrides,
});

const mockHomeLocation: GeoLocation = makeLocation(40.0, -75.0);

const mockFleet: DroneRecord[] = [
  makeDrone({ id: 'id-1', badgeNumber: 1, state: 'Ready' }),
  makeDrone({
    id: 'id-2',
    badgeNumber: 2,
    droneId: 'bbccddee001122334455667788990011',
    state: 'Delivering',
    destination: makeLocation(40.2, -75.2),
  }),
  makeDrone({ id: 'id-3', badgeNumber: 3, state: 'Charging' }),
];

function renderTrackingPage() {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
    },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <TrackingPage />
    </QueryClientProvider>
  );
}

beforeEach(() => {
  vi.clearAllMocks();
  mockGetFleet.mockResolvedValue([]);
  mockGetHomeLocation.mockResolvedValue(mockHomeLocation);
});

describe('TrackingPage', () => {
  it('renders "Tracking" heading', () => {
    renderTrackingPage();
    expect(screen.getByRole('heading', { name: /tracking/i })).toBeInTheDocument();
  });

  it('state filter dropdown renders with "All" option and all 7 drone states', () => {
    renderTrackingPage();
    const select = screen.getByRole('combobox');
    expect(select).toBeInTheDocument();

    const options = screen.getAllByRole('option');
    const optionValues = options.map((o) => (o as HTMLOptionElement).value);

    expect(optionValues).toContain('All');
    expect(optionValues).toContain('Ready');
    expect(optionValues).toContain('Delivering');
    expect(optionValues).toContain('Returning');
    expect(optionValues).toContain('Dead');
    expect(optionValues).toContain('Charging');
    expect(optionValues).toContain('Assigned');
    expect(optionValues).toContain('Unititialized');
    // 1 "All" + 7 states = 8 total
    expect(options).toHaveLength(8);
  });

  it('drone list shows drone cards after data loads', async () => {
    mockGetFleet.mockResolvedValue(mockFleet);
    renderTrackingPage();

    await waitFor(() => {
      expect(screen.getByText('#1')).toBeInTheDocument();
      expect(screen.getByText('#2')).toBeInTheDocument();
      expect(screen.getByText('#3')).toBeInTheDocument();
    });
  });

  it('filtering by "Delivering" hides non-delivering drones in the list', async () => {
    const user = userEvent.setup();
    mockGetFleet.mockResolvedValue(mockFleet);
    renderTrackingPage();

    // Wait for drones to load
    await waitFor(() => {
      expect(screen.getByText('#1')).toBeInTheDocument();
    });

    // Change filter to Delivering
    const select = screen.getByRole('combobox');
    await user.selectOptions(select, 'Delivering');

    // Only badge #2 (Delivering) should be in the list
    expect(screen.getByText('#2')).toBeInTheDocument();
    expect(screen.queryByText('#1')).not.toBeInTheDocument();
    expect(screen.queryByText('#3')).not.toBeInTheDocument();
  });

  it('markers are rendered for drones with currentLocation', async () => {
    mockGetFleet.mockResolvedValue(mockFleet);
    renderTrackingPage();

    await waitFor(() => {
      const markers = screen.getAllByTestId('marker');
      // 3 drone position markers + 1 destination marker for delivering drone + 1 home marker = 5
      // but we only assert at least 3 (one per drone with location)
      expect(markers.length).toBeGreaterThanOrEqual(3);
    });
  });

  it('map wrapper is in the DOM', () => {
    renderTrackingPage();
    expect(screen.getByTestId('google-map')).toBeInTheDocument();
  });

  it('api provider wrapper is in the DOM', () => {
    renderTrackingPage();
    expect(screen.getByTestId('api-provider')).toBeInTheDocument();
  });

  it('renders a polyline for a delivering drone that has both currentLocation and destination', async () => {
    const deliveringDrone = makeDrone({
      id: 'id-del',
      badgeNumber: 5,
      state: 'Delivering',
      currentLocation: makeLocation(40.1, -75.1),
      destination: makeLocation(40.2, -75.2),
    });
    mockGetFleet.mockResolvedValue([deliveringDrone]);
    renderTrackingPage();

    await waitFor(() => {
      expect(screen.getByTestId('polyline')).toBeInTheDocument();
    });
  });

  it('does not render a polyline for a non-delivering drone', async () => {
    mockGetFleet.mockResolvedValue([makeDrone({ state: 'Ready' })]);
    renderTrackingPage();

    await waitFor(() => {
      expect(screen.queryByTestId('polyline')).not.toBeInTheDocument();
    });
  });
});
