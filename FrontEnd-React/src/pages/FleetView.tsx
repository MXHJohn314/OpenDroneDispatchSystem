import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import { getFleet } from '../api/client';
import { useAppStore } from '../store';
import { DRONE_STATE_COLORS } from '../utils/droneColors';
import type { DroneRecord } from '../types';
import DetailedDrone from '../components/DetailedDrone';

export default function FleetView() {
  const [modalOpen, setModalOpen] = useState(false);

  const { data: fleet, isLoading, isError } = useQuery({
    queryKey: ['fleet'],
    queryFn: getFleet,
    refetchInterval: 3000,
  });

  function handleMoreInfo(drone: DroneRecord) {
    useAppStore.getState().setCurrentDrone(drone);
    setModalOpen(true);
  }

  function handleClose() {
    useAppStore.getState().setCurrentDrone(null);
    setModalOpen(false);
  }

  const currentDrone = useAppStore((s) => s.currentDrone);

  return (
    <div className="p-4">
      <div className="flex items-center gap-3 mb-6">
        <h1 className="text-2xl font-bold">Fleet</h1>
        {fleet && (
          <span className="text-sm text-gray-500 font-medium">
            ({fleet.length} drone{fleet.length !== 1 ? 's' : ''})
          </span>
        )}
        <span
          className="inline-block w-3 h-3 rounded-full"
          style={{ backgroundColor: isError ? '#dc3545' : fleet ? '#28a745' : '#6c757d' }}
          title={isError ? 'Connection error' : fleet ? 'Connected' : 'Connecting…'}
        />
      </div>

      {isLoading && (
        <p className="text-gray-500">Loading fleet…</p>
      )}

      {isError && (
        <p className="text-red-600 font-medium">Connection error</p>
      )}

      {fleet && (
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
          {fleet.map((drone) => {
            const color = DRONE_STATE_COLORS[drone.state];
            const hasLocation =
              drone.currentLocation !== null && drone.currentLocation !== undefined;

            return (
              <div
                key={drone.id}
                className="border border-gray-200 rounded-lg p-4 shadow-sm bg-white flex flex-col gap-2"
              >
                <div className="flex items-center justify-between">
                  <span className="font-bold text-lg">#{drone.badgeNumber}</span>
                  <span
                    className="text-xs font-semibold px-2 py-0.5 rounded-full text-white"
                    style={{ backgroundColor: color }}
                  >
                    {drone.state}
                  </span>
                </div>

                <p className="text-sm text-gray-600">
                  {hasLocation
                    ? `${drone.currentLocation!.latitude.toFixed(4)}, ${drone.currentLocation!.longitude.toFixed(4)}`
                    : 'Location unknown'}
                </p>

                <button
                  type="button"
                  onClick={() => handleMoreInfo(drone)}
                  className="mt-auto self-start text-sm font-medium text-blue-600 hover:underline"
                >
                  More Info
                </button>
              </div>
            );
          })}
        </div>
      )}

      <DetailedDrone
        drone={modalOpen ? currentDrone : null}
        onClose={handleClose}
      />
    </div>
  );
}
