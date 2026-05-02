import type { DroneRecord } from '../types';
import { DRONE_STATE_COLORS } from '../utils/droneColors';

interface DetailedDroneProps {
  drone: DroneRecord | null;
  onClose: () => void;
}

export default function DetailedDrone({ drone, onClose }: DetailedDroneProps) {
  if (!drone) return null;

  const color = DRONE_STATE_COLORS[drone.state];

  function fmtLocation(
    loc: { latitude: number; longitude: number } | null | undefined,
    fallback: string
  ): string {
    if (!loc) return fallback;
    return `${loc.latitude.toFixed(4)}, ${loc.longitude.toFixed(4)}`;
  }

  return (
    <div
      className="fixed inset-0 bg-black/50 flex items-center justify-center z-50"
      onClick={(e) => {
        if (e.target === e.currentTarget) onClose();
      }}
    >
      <div
        role="dialog"
        aria-modal="true"
        aria-label={`Drone #${drone.badgeNumber} details`}
        className="bg-white rounded-xl shadow-xl p-6 w-full max-w-md mx-4 flex flex-col gap-3"
      >
        <div className="flex items-center justify-between mb-1">
          <h2 className="text-xl font-bold">Drone #{drone.badgeNumber}</h2>
          <span
            className="text-xs font-semibold px-2 py-0.5 rounded-full text-white"
            style={{ backgroundColor: color }}
          >
            {drone.state}
          </span>
        </div>

        <dl className="grid grid-cols-[auto_1fr] gap-x-4 gap-y-2 text-sm">
          <dt className="font-medium text-gray-500">ID</dt>
          <dd className="font-mono break-all">{drone.droneId}</dd>

          <dt className="font-medium text-gray-500">Badge</dt>
          <dd>#{drone.badgeNumber}</dd>

          <dt className="font-medium text-gray-500">State</dt>
          <dd>{drone.state}</dd>

          <dt className="font-medium text-gray-500">Location</dt>
          <dd>{fmtLocation(drone.currentLocation, 'Unknown')}</dd>

          <dt className="font-medium text-gray-500">Destination</dt>
          <dd>{fmtLocation(drone.destination, 'None')}</dd>

          <dt className="font-medium text-gray-500">Home</dt>
          <dd>{fmtLocation(drone.homeLocation, 'Unknown')}</dd>

          <dt className="font-medium text-gray-500">Order ID</dt>
          <dd className="font-mono break-all">
            {drone.orderId ?? 'No active order'}
          </dd>

          <dt className="font-medium text-gray-500">Direction</dt>
          <dd>{drone.direction}°</dd>
        </dl>

        <button
          type="button"
          onClick={onClose}
          className="mt-2 self-end px-4 py-1.5 bg-gray-800 text-white text-sm rounded-lg hover:bg-gray-700 transition-colors"
        >
          Close
        </button>
      </div>
    </div>
  );
}
