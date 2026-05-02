import { describe, it, expect, vi, beforeEach } from 'vitest';

// Mock axios before importing the client
vi.mock('axios', () => {
  const mockClient = {
    get: vi.fn(),
    post: vi.fn(),
  };
  return {
    default: {
      create: vi.fn(() => mockClient),
    },
    __mockClient: mockClient,
  };
});

import axios from 'axios';
import {
  getFleet,
  getOrders,
  createOrder,
  cancelOrder,
  getDrone,
  getOrder,
  getHomeLocation,
  addDrone,
  getDispatchUrl,
} from '../api/client';
import type { DroneRecord, Order } from '../types';

// Access the mock client via the create mock
const mockClient = (axios.create as ReturnType<typeof vi.fn>)();

beforeEach(() => {
  vi.clearAllMocks();
});

const mockDrone: DroneRecord = {
  id: 'abc123',
  droneId: 'drone-1',
  orderId: null,
  badgeNumber: 1,
  destination: null,
  currentLocation: { latitude: 0, longitude: 0 },
  homeLocation: { latitude: 0, longitude: 0 },
  state: 'Ready',
  droneUrl: 'http://drone:8080',
  dispatchUrl: 'http://dispatch:83',
  direction: 0,
};

const mockOrder: Order = {
  id: 'order-abc',
  orderId: 'order-1',
  droneId: null,
  state: 'Waiting',
  items: [],
  customerName: 'Test Customer',
  deliveryAddress: '123 Main St',
  deliveryLocation: null,
  timeOrdered: new Date().toISOString(),
  timeDelivered: null,
  hasBeenDelivered: false,
};

describe('getFleet', () => {
  it('returns an array of DroneRecords', async () => {
    mockClient.get.mockResolvedValueOnce({ data: [mockDrone] });
    const result = await getFleet();
    expect(Array.isArray(result)).toBe(true);
    expect(result).toHaveLength(1);
    expect(result[0].droneId).toBe('drone-1');
    expect(mockClient.get).toHaveBeenCalledWith('/GetFleet');
  });

  it('returns empty array when fleet is empty', async () => {
    mockClient.get.mockResolvedValueOnce({ data: [] });
    const result = await getFleet();
    expect(result).toEqual([]);
  });
});

describe('getOrders', () => {
  it('returns an array of Orders', async () => {
    mockClient.get.mockResolvedValueOnce({ data: [mockOrder] });
    const result = await getOrders();
    expect(Array.isArray(result)).toBe(true);
    expect(result).toHaveLength(1);
    expect(result[0].orderId).toBe('order-1');
    expect(mockClient.get).toHaveBeenCalledWith('/GetOrders');
  });

  it('returns empty array when no orders', async () => {
    mockClient.get.mockResolvedValueOnce({ data: [] });
    const result = await getOrders();
    expect(result).toEqual([]);
  });
});

describe('createOrder', () => {
  it('posts to CreateOrder and returns response', async () => {
    const req = {
      orderId: 'new-order',
      customerName: 'Jane',
      deliveryAddress: '456 Oak Ave',
      droneId: null,
      timeOrdered: new Date().toISOString(),
      deliveryLocation: null,
      state: 'Waiting' as const,
    };
    mockClient.post.mockResolvedValueOnce({
      data: { orderId: 'new-order', success: true },
    });
    const result = await createOrder(req);
    expect(result.success).toBe(true);
    expect(mockClient.post).toHaveBeenCalledWith('/CreateOrder', req);
  });
});

describe('cancelOrder', () => {
  it('posts to CancelOrder and returns response', async () => {
    mockClient.post.mockResolvedValueOnce({
      data: { orderId: 'order-1', isCancelled: true },
    });
    const result = await cancelOrder({ orderId: 'order-1' });
    expect(result.isCancelled).toBe(true);
  });
});

describe('getDrone', () => {
  it('calls GetDrone with id param and returns DroneRecord', async () => {
    mockClient.get.mockResolvedValueOnce({ data: mockDrone });
    const result = await getDrone('abc123');
    expect(result.droneId).toBe('drone-1');
    expect(mockClient.get).toHaveBeenCalledWith('/GetDrone', {
      params: { id: 'abc123' },
    });
  });
});

describe('getOrder', () => {
  it('calls GetOrderById with id param and returns Order', async () => {
    mockClient.get.mockResolvedValueOnce({ data: mockOrder });
    const result = await getOrder('order-abc');
    expect(result.orderId).toBe('order-1');
    expect(mockClient.get).toHaveBeenCalledWith('/GetOrderById', {
      params: { id: 'order-abc' },
    });
  });
});

describe('getHomeLocation', () => {
  it('returns a GeoLocation', async () => {
    mockClient.get.mockResolvedValueOnce({
      data: { latitude: 39.9, longitude: -75.1 },
    });
    const result = await getHomeLocation();
    expect(result.latitude).toBe(39.9);
    expect(result.longitude).toBe(-75.1);
  });
});

describe('addDrone', () => {
  it('posts to AddDrone with message body', async () => {
    mockClient.post.mockResolvedValueOnce({ data: undefined });
    await addDrone('http://newdrone:8080');
    expect(mockClient.post).toHaveBeenCalledWith('/AddDrone', {
      message: 'http://newdrone:8080',
    });
  });
});

describe('getDispatchUrl', () => {
  it('returns the dispatch URL string', async () => {
    mockClient.get.mockResolvedValueOnce({ data: 'http://dispatch:83' });
    const result = await getDispatchUrl();
    expect(result).toBe('http://dispatch:83');
    expect(mockClient.get).toHaveBeenCalledWith('/GetDispatchUrl');
  });
});
