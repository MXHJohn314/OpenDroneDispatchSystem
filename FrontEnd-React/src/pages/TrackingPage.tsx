import { useState } from 'react';
import { useQuery } from '@tanstack/react-query';
import {
  APIProvider,
  Map,
  AdvancedMarker,
  Pin,
  Polyline,
} from '@vis.gl/react-google-maps';
import { getFleet, getHomeLocation } from '../api/client';
import { DRONE_STATE_COLORS } from '../utils/droneColors';
import type { DroneState, DroneRecord } from '../types';

const DRONE_STATES: DroneState[] = [
  'Ready',
  'Delivering',
  'Returning',
  'Dead',
  'Charging',
  'Assigned',
  'Unititialized',
];

type FilterState = 'All' | DroneState;

function DroneCard({ drone }: { drone: DroneRecord }) {
  const color = DRONE_STATE_COLORS[drone.state];
  const loc = drone.currentLocation;
  return (
    <div className="border border-gray-200 rounded-lg p-3 shadow-sm bg-white flex flex-col gap-1">
      <div className="flex items-center justify-between gap-2">
        <span className="font-bold text-base">#{drone.badgeNumber}</span>
        <span
          className="text-xs font-semibold px-2 py-0.5 rounded-full text-white"
          style={{ backgroundColor: color }}
        >
          {drone.state}
        </span>
      </div>
      <p className="text-xs text-gray-500">
        {loc
          ? `${loc.latitude.toFixed(4)}, ${loc.longitude.toFixed(4)}`
          : 'Unknown'}
      </p>
    </div>
  );
}

function DroneMarker({ drone }: { drone: DroneRecord }) {
  if (!drone.currentLocation) return null;
  const color = DRONE_STATE_COLORS[drone.state];
  const pos = {
    lat: drone.currentLocation.latitude,
    lng: drone.currentLocation.longitude,
  };

  return (
    <AdvancedMarker position={pos}>
      <svg width="20" height="20" viewBox="0 0 20 20">
        <polygon
          points="10,2 18,18 2,18"
          fill={color}
          stroke="white"
          strokeWidth="1.5"
          transform={`rotate(${drone.direction}, 10, 10)`}
        />
      </svg>
    </AdvancedMarker>
  );
}

function DestinationMarker({ drone }: { drone: DroneRecord }) {
  if (drone.state !== 'Delivering' || !drone.destination) return null;
  const color = DRONE_STATE_COLORS[drone.state];
  const pos = {
    lat: drone.destination.latitude,
    lng: drone.destination.longitude,
  };

  return (
    <AdvancedMarker position={pos}>
      <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: '2px' }}>
        <div
          style={{
            width: '10px',
            height: '10px',
            borderRadius: '50%',
            backgroundColor: color,
            opacity: 0.7,
            border: '1px solid white',
          }}
        />
        <span style={{ fontSize: '9px', color: '#333', fontWeight: 'bold', whiteSpace: 'nowrap' }}>
          {drone.droneId.slice(0, 8)}
        </span>
      </div>
    </AdvancedMarker>
  );
}

function FlightPath({ drone }: { drone: DroneRecord }) {
  if (
    drone.state !== 'Delivering' ||
    !drone.currentLocation ||
    !drone.destination
  )
    return null;

  const path = [
    { lat: drone.currentLocation.latitude, lng: drone.currentLocation.longitude },
    { lat: drone.destination.latitude, lng: drone.destination.longitude },
  ];

  return (
    <Polyline
      path={path}
      strokeColor={DRONE_STATE_COLORS[drone.state]}
      strokeOpacity={0}
      strokeWeight={2}
      icons={[
        {
          icon: { path: 'M 0,-1 0,1', strokeOpacity: 1, scale: 3 },
          offset: '0',
          repeat: '12px',
        },
      ]}
    />
  );
}

export default function TrackingPage() {
  const [filter, setFilter] = useState<FilterState>('All');

  const { data: fleet } = useQuery({
    queryKey: ['fleet'],
    queryFn: getFleet,
    refetchInterval: 2000,
  });

  const { data: homeLocation } = useQuery({
    queryKey: ['homeLocation'],
    queryFn: getHomeLocation,
  });

  const mapCenter = homeLocation
    ? { lat: homeLocation.latitude, lng: homeLocation.longitude }
    : { lat: 0, lng: 0 };

  const filteredDrones =
    filter === 'All'
      ? (fleet ?? [])
      : (fleet ?? []).filter((d) => d.state === filter);

  const apiKey = import.meta.env.VITE_GOOGLE_MAPS_API_KEY ?? '';

  return (
    <div className="flex flex-col h-full" style={{ minHeight: '100vh' }}>
      {/* Top bar */}
      <div className="flex items-center gap-4 p-4 border-b border-gray-200 bg-white">
        <h1 className="text-2xl font-bold">Tracking</h1>
        <div className="flex items-center gap-2">
          <label htmlFor="state-filter" className="text-sm font-medium text-gray-700">
            Filter:
          </label>
          <select
            id="state-filter"
            value={filter}
            onChange={(e) => setFilter(e.target.value as FilterState)}
            className="border border-gray-300 rounded px-2 py-1 text-sm"
          >
            <option value="All">All</option>
            {DRONE_STATES.map((s) => (
              <option key={s} value={s}>
                {s}
              </option>
            ))}
          </select>
        </div>
      </div>

      {/* Body: left panel + map */}
      <div className="flex flex-1 overflow-hidden">
        {/* Left panel */}
        <div
          className="w-64 border-r border-gray-200 bg-gray-50 overflow-y-auto flex flex-col gap-2 p-3"
          style={{ minWidth: '16rem' }}
        >
          <p className="text-xs text-gray-500 font-medium uppercase tracking-wide">
            {filteredDrones.length} drone{filteredDrones.length !== 1 ? 's' : ''}
          </p>
          {filteredDrones.map((drone) => (
            <DroneCard key={drone.id} drone={drone} />
          ))}
          {fleet && filteredDrones.length === 0 && (
            <p className="text-sm text-gray-400 text-center mt-4">No drones match filter.</p>
          )}
        </div>

        {/* Map */}
        <div className="flex-1" style={{ minHeight: '500px' }}>
          <APIProvider apiKey={apiKey}>
            <Map
              defaultCenter={mapCenter}
              defaultZoom={14}
              mapId="drone-tracking-map"
              style={{ width: '100%', height: '100%', minHeight: '500px' }}
            >
              {/* Home marker */}
              {homeLocation && (
                <AdvancedMarker
                  position={{ lat: homeLocation.latitude, lng: homeLocation.longitude }}
                >
                  <span style={{ fontSize: '24px' }}>🏠</span>
                </AdvancedMarker>
              )}

              {/* Per-drone markers */}
              {(fleet ?? []).map((drone) => (
                <span key={drone.id}>
                  <DroneMarker drone={drone} />
                  <DestinationMarker drone={drone} />
                  <FlightPath drone={drone} />
                </span>
              ))}
            </Map>
          </APIProvider>
        </div>
      </div>
    </div>
  );
}
