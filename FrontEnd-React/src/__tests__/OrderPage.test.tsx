import { describe, it, expect, vi, beforeEach } from 'vitest';
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import OrderPage from '../pages/OrderPage';
import type { Order, DroneRecord } from '../types';
import { useAppStore } from '../store';

// Mock the API client
vi.mock('../api/client', () => ({
  getOrders: vi.fn(),
  getFleet: vi.fn(),
  createOrder: vi.fn(),
  cancelOrder: vi.fn(),
  addDrone: vi.fn(),
}));

import { getOrders, getFleet, createOrder, cancelOrder, addDrone } from '../api/client';

const mockGetOrders = getOrders as ReturnType<typeof vi.fn>;
const mockGetFleet = getFleet as ReturnType<typeof vi.fn>;
const mockCreateOrder = createOrder as ReturnType<typeof vi.fn>;
const mockCancelOrder = cancelOrder as ReturnType<typeof vi.fn>;
const mockAddDrone = addDrone as ReturnType<typeof vi.fn>;

const makeOrder = (overrides: Partial<Order> = {}): Order => ({
  id: 'id-order-1',
  orderId: 'aabbccddeeff001122334455',
  droneId: null,
  state: 'Waiting',
  items: [],
  customerName: 'Alice Smith',
  deliveryAddress: '123 Main St',
  deliveryLocation: null,
  timeOrdered: '2024-01-01T12:00:00.000Z',
  timeDelivered: null,
  hasBeenDelivered: false,
  ...overrides,
});

const makeDrone = (overrides: Partial<DroneRecord> = {}): DroneRecord => ({
  id: 'id-drone-1',
  droneId: 'droneid001122334455667788',
  orderId: null,
  badgeNumber: 1,
  destination: null,
  currentLocation: { latitude: 40.0, longitude: -75.0 },
  homeLocation: { latitude: 40.0, longitude: -75.0 },
  state: 'Ready',
  droneUrl: 'http://drone:8080',
  dispatchUrl: 'http://dispatch:83',
  direction: 0,
  ...overrides,
});

const mockOrders: Order[] = [
  makeOrder({ id: 'id-order-1', orderId: 'aabbccddeeff001122334455', customerName: 'Alice Smith', deliveryAddress: '123 Main St' }),
  makeOrder({ id: 'id-order-2', orderId: 'ffeeddccbbaa998877665544', customerName: 'Bob Jones', deliveryAddress: '456 Oak Ave' }),
];

const mockFleet: DroneRecord[] = [
  makeDrone({ id: 'id-drone-1', badgeNumber: 1, droneUrl: 'http://drone1:8080' }),
  makeDrone({ id: 'id-drone-2', badgeNumber: 2, droneUrl: 'http://drone2:8080', state: 'Delivering' }),
];

function renderOrderPage() {
  const queryClient = new QueryClient({
    defaultOptions: {
      queries: { retry: false },
      mutations: { retry: false },
    },
  });
  return render(
    <QueryClientProvider client={queryClient}>
      <OrderPage />
    </QueryClientProvider>
  );
}

beforeEach(() => {
  vi.clearAllMocks();
  useAppStore.setState({ currentDrone: null, currentOrder: null });
  // Default: never-resolving promises so loading states are predictable,
  // but most tests override these.
  mockGetOrders.mockResolvedValue([]);
  mockGetFleet.mockResolvedValue([]);
});

describe('OrderPage', () => {
  it('renders "Order Manager" and "Drone Manager" headings', async () => {
    renderOrderPage();
    expect(screen.getByText('Order Manager')).toBeInTheDocument();
    expect(screen.getByText('Drone Manager')).toBeInTheDocument();
  });

  it('Create Order form has customer name input, address input, and submit button', () => {
    renderOrderPage();
    expect(screen.getByRole('textbox', { name: /customer name/i })).toBeInTheDocument();
    expect(screen.getByRole('textbox', { name: /delivery address/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /create order/i })).toBeInTheDocument();
  });

  it('Drone Manager has drone URL input and "Add Drone" button', () => {
    renderOrderPage();
    expect(screen.getByRole('textbox', { name: /drone url/i })).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /add drone/i })).toBeInTheDocument();
  });

  it('submitting Create Order form calls createOrder with correct shape', async () => {
    const user = userEvent.setup();
    mockCreateOrder.mockResolvedValue({ orderId: 'generated-id', success: true });
    renderOrderPage();

    const nameInput = screen.getByRole('textbox', { name: /customer name/i });
    const addressInput = screen.getByRole('textbox', { name: /delivery address/i });
    const submitBtn = screen.getByRole('button', { name: /create order/i });

    await user.type(nameInput, 'Jane Doe');
    await user.type(addressInput, '789 Elm St');
    await user.click(submitBtn);

    await waitFor(() => {
      expect(mockCreateOrder).toHaveBeenCalledTimes(1);
    });

    const callArg = mockCreateOrder.mock.calls[0][0];
    expect(callArg.customerName).toBe('Jane Doe');
    expect(callArg.deliveryAddress).toBe('789 Elm St');
    expect(typeof callArg.orderId).toBe('string');
    expect(callArg.orderId).toHaveLength(24);
    expect(callArg.state).toBe('Waiting');
    expect(callArg.droneId).toBeNull();
    expect(callArg.deliveryLocation).toBeNull();
    expect(typeof callArg.timeOrdered).toBe('string');
  });

  it('clicking an order card selects it and shows the info panel', async () => {
    const user = userEvent.setup();
    mockGetOrders.mockResolvedValue(mockOrders);
    renderOrderPage();

    await waitFor(() => {
      expect(screen.getByText('123 Main St')).toBeInTheDocument();
    });

    // Click the first order card
    const card = screen.getByText('123 Main St');
    await user.click(card);

    // Info panel should appear
    await waitFor(() => {
      expect(screen.getByText('Selected Order')).toBeInTheDocument();
      expect(screen.getByText('Alice Smith')).toBeInTheDocument();
    });
  });

  it('order list renders order cards with address text', async () => {
    mockGetOrders.mockResolvedValue(mockOrders);
    renderOrderPage();

    await waitFor(() => {
      expect(screen.getByText('123 Main St')).toBeInTheDocument();
      expect(screen.getByText('456 Oak Ave')).toBeInTheDocument();
    });
  });

  it('drone list renders drone cards with badge numbers', async () => {
    mockGetFleet.mockResolvedValue(mockFleet);
    renderOrderPage();

    await waitFor(() => {
      expect(screen.getByText('#1')).toBeInTheDocument();
      expect(screen.getByText('#2')).toBeInTheDocument();
    });
  });
});
