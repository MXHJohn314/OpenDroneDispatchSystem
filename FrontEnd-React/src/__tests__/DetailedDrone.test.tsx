import { describe, it, expect, vi } from 'vitest';
import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import DetailedDrone from '../components/DetailedDrone';
import type { DroneRecord } from '../types';

const mockDrone: DroneRecord = {
  id: 'test-id',
  droneId: 'aabbccddeeff00112233445566778899',
  orderId: 'order-xyz',
  badgeNumber: 7,
  destination: { latitude: 39.9876, longitude: -75.1234 },
  currentLocation: { latitude: 40.1234, longitude: -75.5678 },
  homeLocation: { latitude: 40.0, longitude: -75.0 },
  state: 'Delivering',
  droneUrl: 'http://drone:8080',
  dispatchUrl: 'http://dispatch:83',
  direction: 127.5,
};

describe('DetailedDrone', () => {
  it('renders nothing when drone is null', () => {
    const { container } = render(
      <DetailedDrone drone={null} onClose={vi.fn()} />
    );
    expect(container).toBeEmptyDOMElement();
  });

  it('renders drone ID when drone is provided', () => {
    render(<DetailedDrone drone={mockDrone} onClose={vi.fn()} />);
    expect(screen.getByText('aabbccddeeff00112233445566778899')).toBeInTheDocument();
  });

  it('renders badge number when drone is provided', () => {
    render(<DetailedDrone drone={mockDrone} onClose={vi.fn()} />);
    // Badge number appears in heading and in the dl
    const badgeItems = screen.getAllByText(/#7/);
    expect(badgeItems.length).toBeGreaterThanOrEqual(1);
  });

  it('renders state when drone is provided', () => {
    render(<DetailedDrone drone={mockDrone} onClose={vi.fn()} />);
    const stateItems = screen.getAllByText('Delivering');
    expect(stateItems.length).toBeGreaterThanOrEqual(1);
  });

  it('clicking Close calls onClose', async () => {
    const user = userEvent.setup();
    const onClose = vi.fn();
    render(<DetailedDrone drone={mockDrone} onClose={onClose} />);
    const closeBtn = screen.getByRole('button', { name: /close/i });
    await user.click(closeBtn);
    expect(onClose).toHaveBeenCalledTimes(1);
  });

  it('has role="dialog" attribute', () => {
    render(<DetailedDrone drone={mockDrone} onClose={vi.fn()} />);
    expect(screen.getByRole('dialog')).toBeInTheDocument();
  });

  it('shows direction in degrees', () => {
    render(<DetailedDrone drone={mockDrone} onClose={vi.fn()} />);
    expect(screen.getByText('127.5°')).toBeInTheDocument();
  });

  it('shows order ID when assigned', () => {
    render(<DetailedDrone drone={mockDrone} onClose={vi.fn()} />);
    expect(screen.getByText('order-xyz')).toBeInTheDocument();
  });

  it('shows "No active order" when orderId is null', () => {
    const drone: DroneRecord = { ...mockDrone, orderId: null };
    render(<DetailedDrone drone={drone} onClose={vi.fn()} />);
    expect(screen.getByText('No active order')).toBeInTheDocument();
  });

  it('shows "None" for destination when null', () => {
    const drone: DroneRecord = { ...mockDrone, destination: null };
    render(<DetailedDrone drone={drone} onClose={vi.fn()} />);
    expect(screen.getByText('None')).toBeInTheDocument();
  });
});
