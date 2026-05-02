import axios from 'axios';
import type {
  DroneRecord,
  Order,
  CreateOrderRequest,
  CreateOrderResponse,
  CancelDeliveryRequest,
  CancelDeliveryResponse,
  GeoLocation,
} from '../types';

const dbClient = axios.create({ baseURL: '/DatabaseAccess' });

export async function getFleet(): Promise<DroneRecord[]> {
  const res = await dbClient.get<DroneRecord[]>('/GetFleet');
  return res.data;
}

export async function getOrders(): Promise<Order[]> {
  const res = await dbClient.get<Order[]>('/GetOrders');
  return res.data;
}

export async function createOrder(
  req: CreateOrderRequest
): Promise<CreateOrderResponse> {
  const res = await dbClient.post<CreateOrderResponse>('/CreateOrder', req);
  return res.data;
}

export async function cancelOrder(
  req: CancelDeliveryRequest
): Promise<CancelDeliveryResponse> {
  const res = await dbClient.post<CancelDeliveryResponse>(
    `/CancelOrder?OrderId=${encodeURIComponent(req.orderId)}`
  );
  return res.data;
}

export async function getDrone(id: string): Promise<DroneRecord> {
  const res = await dbClient.get<DroneRecord>('/GetDrone', { params: { id } });
  return res.data;
}

export async function getOrder(id: string): Promise<Order> {
  const res = await dbClient.get<Order>('/GetOrderById', { params: { id } });
  return res.data;
}

export async function getHomeLocation(): Promise<GeoLocation> {
  const res = await dbClient.get<GeoLocation>('/GetHomeLocation');
  return res.data;
}

export async function addDrone(droneUrl: string): Promise<void> {
  await dbClient.post('/AddDrone', { message: droneUrl });
}

export async function getDispatchUrl(): Promise<string> {
  const res = await dbClient.get<string>('/GetDispatchUrl');
  return res.data;
}
