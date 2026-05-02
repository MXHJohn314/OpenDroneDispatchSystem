import { describe, it, expect, beforeEach } from 'vitest';
import { useAppStore } from '../store';
import type { DroneRecord, Order } from '../types';

const mockDrone: DroneRecord = {
  id: 'drone-id-1',
  droneId: 'drone-uuid-1',
  orderId: null,
  badgeNumber: 1,
  destination: null,
  currentLocation: { latitude: 40.0, longitude: -75.0 },
  homeLocation: { latitude: 40.0, longitude: -75.0 },
  state: 'Ready',
  droneUrl: 'http://drone:8080',
  dispatchUrl: 'http://dispatch:83',
  direction: 0,
};

const mockOrder: Order = {
  id: 'order-id-1',
  orderId: 'order-uuid-1',
  droneId: null,
  state: 'Waiting',
  items: [],
  customerName: 'Alice',
  deliveryAddress: '1 Test Lane',
  deliveryLocation: { latitude: 39.9, longitude: -75.1 },
  timeOrdered: new Date().toISOString(),
  timeDelivered: null,
  hasBeenDelivered: false,
};

// Reset store state between tests
beforeEach(() => {
  useAppStore.setState({ currentDrone: null, currentOrder: null });
});

describe('AppStore - currentDrone', () => {
  it('initializes with null currentDrone', () => {
    const { currentDrone } = useAppStore.getState();
    expect(currentDrone).toBeNull();
  });

  it('sets currentDrone via setCurrentDrone', () => {
    useAppStore.getState().setCurrentDrone(mockDrone);
    const { currentDrone } = useAppStore.getState();
    expect(currentDrone).not.toBeNull();
    expect(currentDrone?.droneId).toBe('drone-uuid-1');
    expect(currentDrone?.state).toBe('Ready');
  });

  it('clears currentDrone when set to null', () => {
    useAppStore.getState().setCurrentDrone(mockDrone);
    useAppStore.getState().setCurrentDrone(null);
    expect(useAppStore.getState().currentDrone).toBeNull();
  });

  it('replaces currentDrone with a new drone', () => {
    useAppStore.getState().setCurrentDrone(mockDrone);
    const anotherDrone: DroneRecord = { ...mockDrone, droneId: 'drone-uuid-2', badgeNumber: 2 };
    useAppStore.getState().setCurrentDrone(anotherDrone);
    expect(useAppStore.getState().currentDrone?.droneId).toBe('drone-uuid-2');
  });
});

describe('AppStore - currentOrder', () => {
  it('initializes with null currentOrder', () => {
    const { currentOrder } = useAppStore.getState();
    expect(currentOrder).toBeNull();
  });

  it('sets currentOrder via setCurrentOrder', () => {
    useAppStore.getState().setCurrentOrder(mockOrder);
    const { currentOrder } = useAppStore.getState();
    expect(currentOrder).not.toBeNull();
    expect(currentOrder?.orderId).toBe('order-uuid-1');
    expect(currentOrder?.customerName).toBe('Alice');
  });

  it('clears currentOrder when set to null', () => {
    useAppStore.getState().setCurrentOrder(mockOrder);
    useAppStore.getState().setCurrentOrder(null);
    expect(useAppStore.getState().currentOrder).toBeNull();
  });

  it('replaces currentOrder with a new order', () => {
    useAppStore.getState().setCurrentOrder(mockOrder);
    const anotherOrder: Order = { ...mockOrder, orderId: 'order-uuid-2', customerName: 'Bob' };
    useAppStore.getState().setCurrentOrder(anotherOrder);
    expect(useAppStore.getState().currentOrder?.customerName).toBe('Bob');
  });
});

describe('AppStore - independence', () => {
  it('setting currentDrone does not affect currentOrder', () => {
    useAppStore.getState().setCurrentOrder(mockOrder);
    useAppStore.getState().setCurrentDrone(mockDrone);
    expect(useAppStore.getState().currentOrder?.orderId).toBe('order-uuid-1');
  });

  it('setting currentOrder does not affect currentDrone', () => {
    useAppStore.getState().setCurrentDrone(mockDrone);
    useAppStore.getState().setCurrentOrder(mockOrder);
    expect(useAppStore.getState().currentDrone?.droneId).toBe('drone-uuid-1');
  });
});
