import type { DroneState } from '../types';

export const DRONE_STATE_COLORS: Record<DroneState, string> = {
  Ready: '#13FF00',
  Delivering: '#98f5ff',
  Returning: '#8A2BE2',
  Dead: '#FF0000',
  Charging: '#FF9A08',
  Assigned: '#FF9A08',
  Unititialized: '#964B00',
};
