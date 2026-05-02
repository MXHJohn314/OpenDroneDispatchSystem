import { create } from 'zustand';
import type { DroneRecord, Order } from '../types';

interface AppStore {
  currentDrone: DroneRecord | null;
  currentOrder: Order | null;
  setCurrentDrone: (drone: DroneRecord | null) => void;
  setCurrentOrder: (order: Order | null) => void;
}

export const useAppStore = create<AppStore>((set) => ({
  currentDrone: null,
  currentOrder: null,
  setCurrentDrone: (drone) => set({ currentDrone: drone }),
  setCurrentOrder: (order) => set({ currentOrder: order }),
}));
