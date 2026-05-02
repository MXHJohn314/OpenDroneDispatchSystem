import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import FleetView from '../pages/FleetView';
import type { DroneRecord } from '../types';
import { useAppStore } from '../store';

// Mock the API client
vi.mock('../api/client', () => ({
  getFleet: vi.fn(),
}));

import { getFleet } from '../api/client';

const mockGetFleet = getFleet as ReturnType<typeof vi.fn>;

const makeDrone = (overrides: Partial<DroneRecord> = {}): DroneRecord => ({
  id: 'id-1',
  droneId: 'aabbccddeeff00112233445566778899',
  orderId: null,
  badgeNumber: 1,
  destination: null,
  currentLocation: { latitude: 40.1234, longitude: -75.5678 },
  homeLocation: { latitude: 40.0, longitude: -75.0 },
  state: 'Ready',
  droneUrl: 'http://drone:8080',
  dispatchUrl: 'http://dispatch:83',
  direction: 90,
  ...overrides,
});

const mockFleet: DroneRecord[] = [
  makeDrone({ id: 'id-1', badgeNumber: 1, droneId: 'aabbccddeeff00112233445566778899' }),
  makeDrone({ id: 'id-2', badgeNumber: 2, droneId: '112233445566778899aabbccddeeff00', state: 'Delivering' }),
  makeDrone({ id: 'id-3', badgeNumber: 3, droneId: '99aabbccddeeff0011223344556677aa', state: 'Charging' }),
];

function renderFleetView() {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: {
        retry: false,
      },
    },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <FleetView />
    </QueryClientProvider>
  );
}

beforeEach(() => {
  vi.clearAllMocks();
  // Reset zustand store
  useAppStore.setState({ currentDrone: null, currentOrder: null });
});

describe('FleetView', () => {
  it('renders "Loading fleet…" while fetching', async () => {
    // Never resolve so we stay in loading state
    mockGetFleet.mockReturnValue(new Promise(() => {}));
    renderFleetView();
    expect(screen.getByText('Loading fleet…')).toBeInTheDocument();
  });

  it('renders drone cards when data arrives', async () => {
    mockGetFleet.mockResolvedValue(mockFleet);
    renderFleetView();
    await waitFor(() => {
      expect(screen.getByText('#1')).toBeInTheDocument();
      expect(screen.getByText('#2')).toBeInTheDocument();
    });
  });

  it('renders badge numbers correctly', async () => {
    mockGetFleet.mockResolvedValue(mockFleet);
    renderFleetView();
    await waitFor(() => {
      expect(screen.getByText('#1')).toBeInTheDocument();
      expect(screen.getByText('#2')).toBeInTheDocument();
      expect(screen.getByText('#3')).toBeInTheDocument();
    });
  });

  it('fleet size count is correct', async () => {
    mockGetFleet.mockResolvedValue(mockFleet);
    renderFleetView();
    await waitFor(() => {
      expect(screen.getByText('(3 drones)')).toBeInTheDocument();
    });
  });

  it('clicking "More Info" opens the DetailedDrone modal', async () => {
    const user = userEvent.setup();
    mockGetFleet.mockResolvedValue(mockFleet);
    renderFleetView();

    await waitFor(() => {
      expect(screen.getByText('#1')).toBeInTheDocument();
    });

    const moreInfoButtons = screen.getAllByText('More Info');
    await user.click(moreInfoButtons[0]);

    // Modal should be open: role="dialog" present
    expect(screen.getByRole('dialog')).toBeInTheDocument();
  });

  it('shows "Connection error" on fetch error', async () => {
    mockGetFleet.mockRejectedValue(new Error('Network error'));
    renderFleetView();
    await waitFor(() => {
      expect(screen.getByText('Connection error')).toBeInTheDocument();
    });
  });
});
