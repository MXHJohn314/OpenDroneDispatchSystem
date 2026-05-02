export type DroneState =
  | 'Ready'
  | 'Delivering'
  | 'Returning'
  | 'Dead'
  | 'Charging'
  | 'Unititialized'
  | 'Assigned';

export type OrderState = 'Waiting' | 'Assigned' | 'Delivered';

export interface GeoLocation {
  latitude: number;
  longitude: number;
}

export interface DroneRecord {
  id: string;
  droneId: string;
  orderId: string | null;
  badgeNumber: number;
  destination: GeoLocation | null;
  currentLocation: GeoLocation | null;
  homeLocation: GeoLocation | null;
  state: DroneState;
  droneUrl: string;
  dispatchUrl: string;
  direction: number;
}

export interface Order {
  id: string;
  orderId: string;
  droneId: string | null;
  state: OrderState;
  items: unknown[];
  customerName: string;
  deliveryAddress: string;
  deliveryLocation: GeoLocation | null;
  timeOrdered: string;
  timeDelivered: string | null;
  hasBeenDelivered: boolean;
}

export interface CreateOrderRequest {
  orderId: string;
  customerName: string;
  deliveryAddress: string;
  droneId: string | null;
  timeOrdered: string;
  deliveryLocation: GeoLocation | null;
  state: OrderState;
}

export interface CreateOrderResponse {
  orderId: string;
  success: boolean;
}

export interface CancelDeliveryRequest {
  orderId: string;
}

export interface CancelDeliveryResponse {
  orderId: string;
  isCancelled: boolean;
}

export interface AddDroneRequest {
  droneId: string;
  homeLocation: GeoLocation | null;
  droneUrl: string;
  dispatchUrl: string;
}

export interface AddDroneResponse {
  droneId: string;
  success: boolean;
}

export interface BaseDto {
  message: string;
}
