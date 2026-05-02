import { useState } from 'react';
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { getOrders, getFleet, createOrder, cancelOrder, addDrone } from '../api/client';
import { useAppStore } from '../store';
import { DRONE_STATE_COLORS } from '../utils/droneColors';
import type { Order, DroneRecord } from '../types';

function generateOrderId(): string {
  return Array.from(crypto.getRandomValues(new Uint8Array(12)))
    .map(b => b.toString(16).padStart(2, '0'))
    .join('');
}

export default function OrderPage() {
  const queryClient = useQueryClient();

  // --- Order Manager state ---
  const [customerName, setCustomerName] = useState('');
  const [deliveryAddress, setDeliveryAddress] = useState('');
  const [cancelOrderId, setCancelOrderId] = useState('');
  const [selectedOrder, setSelectedOrder] = useState<Order | null>(null);

  // --- Drone Manager state ---
  const [droneUrl, setDroneUrl] = useState('');
  const [removeDroneId, setRemoveDroneId] = useState('');

  // --- Queries ---
  const {
    data: orders,
    isLoading: ordersLoading,
    isError: ordersError,
  } = useQuery({
    queryKey: ['orders'],
    queryFn: getOrders,
    refetchInterval: 3000,
  });

  const {
    data: fleet,
    isLoading: fleetLoading,
    isError: fleetError,
  } = useQuery({
    queryKey: ['fleet'],
    queryFn: getFleet,
    refetchInterval: 3000,
  });

  // --- Mutations ---
  const createOrderMutation = useMutation({
    mutationFn: createOrder,
    onSuccess: () => {
      setCustomerName('');
      setDeliveryAddress('');
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });

  const cancelOrderMutation = useMutation({
    mutationFn: cancelOrder,
    onSuccess: () => {
      setCancelOrderId('');
      queryClient.invalidateQueries({ queryKey: ['orders'] });
    },
  });

  const addDroneMutation = useMutation({
    mutationFn: addDrone,
    onSuccess: () => {
      setDroneUrl('');
      queryClient.invalidateQueries({ queryKey: ['fleet'] });
    },
  });

  // --- Handlers ---
  function handleCreateOrder(e: React.FormEvent) {
    e.preventDefault();
    const newId = generateOrderId();
    createOrderMutation.mutate({
      orderId: newId,
      customerName,
      deliveryAddress,
      droneId: null,
      timeOrdered: new Date().toISOString(),
      deliveryLocation: null,
      state: 'Waiting',
    });
  }

  function handleCancelOrder(e: React.FormEvent) {
    e.preventDefault();
    cancelOrderMutation.mutate({ orderId: cancelOrderId });
  }

  function handleSelectOrder(order: Order) {
    setSelectedOrder(order);
    useAppStore.getState().setCurrentOrder(order);
  }

  function handleAddDrone(e: React.FormEvent) {
    e.preventDefault();
    addDroneMutation.mutate(droneUrl);
  }

  function handleRemoveDrone(e: React.FormEvent) {
    e.preventDefault();
    alert('Remove not yet implemented');
  }

  return (
    <div className="p-6">
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-8">
        {/* ===== Left column: Order Manager ===== */}
        <div>
          <h2 className="text-2xl font-bold mb-4">Order Manager</h2>

          {/* Create Order form */}
          <form onSubmit={handleCreateOrder} className="mb-4 flex flex-col gap-3">
            <input
              type="text"
              placeholder="Customer Name"
              value={customerName}
              onChange={e => setCustomerName(e.target.value)}
              required
              aria-label="Customer Name"
              className="border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
            />
            <input
              type="text"
              placeholder="Delivery Address"
              value={deliveryAddress}
              onChange={e => setDeliveryAddress(e.target.value)}
              required
              aria-label="Delivery Address"
              className="border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
            />
            <button
              type="submit"
              disabled={createOrderMutation.isPending}
              className="bg-blue-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
            >
              {createOrderMutation.isPending ? 'Creating…' : 'Create Order'}
            </button>
            {createOrderMutation.isError && (
              <p className="text-red-600 text-sm">
                Error: {(createOrderMutation.error as Error)?.message ?? 'Failed to create order'}
              </p>
            )}
          </form>

          {/* Cancel Order form */}
          <form onSubmit={handleCancelOrder} className="mb-6 flex flex-col gap-3">
            <input
              type="text"
              placeholder="Order ID"
              value={cancelOrderId}
              onChange={e => setCancelOrderId(e.target.value)}
              required
              aria-label="Cancel Order ID"
              className="border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-red-400"
            />
            <button
              type="submit"
              disabled={cancelOrderMutation.isPending}
              className="bg-red-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-red-700 disabled:opacity-50"
            >
              {cancelOrderMutation.isPending ? 'Cancelling…' : 'Cancel Order'}
            </button>
            {cancelOrderMutation.isError && (
              <p className="text-red-600 text-sm">
                Error: {(cancelOrderMutation.error as Error)?.message ?? 'Failed to cancel order'}
              </p>
            )}
          </form>

          {/* Selected order info panel */}
          {selectedOrder && (
            <div className="mb-6 border border-blue-300 rounded-lg p-4 bg-blue-50">
              <h3 className="font-semibold text-sm mb-2 text-blue-800">Selected Order</h3>
              <dl className="text-sm grid grid-cols-[auto_1fr] gap-x-3 gap-y-1">
                <dt className="text-gray-500 font-medium">Order ID</dt>
                <dd className="font-mono">{selectedOrder.orderId.slice(0, 8)}</dd>
                <dt className="text-gray-500 font-medium">Customer</dt>
                <dd>{selectedOrder.customerName}</dd>
                <dt className="text-gray-500 font-medium">Address</dt>
                <dd>{selectedOrder.deliveryAddress}</dd>
                <dt className="text-gray-500 font-medium">Ordered</dt>
                <dd>{new Date(selectedOrder.timeOrdered).toLocaleString()}</dd>
                <dt className="text-gray-500 font-medium">State</dt>
                <dd>{selectedOrder.state}</dd>
              </dl>
            </div>
          )}

          {/* Order list */}
          {ordersLoading && <p className="text-gray-500 text-sm">Loading orders…</p>}
          {ordersError && <p className="text-red-600 text-sm font-medium">Failed to load orders</p>}
          {orders && (
            <div className="overflow-y-auto max-h-96 flex flex-col gap-2">
              {orders.length === 0 && (
                <p className="text-gray-400 text-sm">No orders yet.</p>
              )}
              {orders.map(order => (
                <button
                  key={order.id}
                  type="button"
                  onClick={() => handleSelectOrder(order)}
                  className={`w-full text-left border rounded-lg px-4 py-3 text-sm transition-colors ${
                    selectedOrder?.id === order.id
                      ? 'border-blue-500 bg-blue-50'
                      : 'border-gray-200 bg-white hover:bg-gray-50'
                  }`}
                >
                  <span className="font-mono font-semibold">{order.orderId.slice(0, 8)}</span>
                  <span className="mx-2 text-gray-400">—</span>
                  <span className="text-gray-700">{order.deliveryAddress}</span>
                </button>
              ))}
            </div>
          )}
        </div>

        {/* ===== Right column: Drone Manager ===== */}
        <div>
          <h2 className="text-2xl font-bold mb-4">Drone Manager</h2>

          {/* Add Drone form */}
          <form onSubmit={handleAddDrone} className="mb-4 flex flex-col gap-3">
            <input
              type="text"
              placeholder="Drone URL (e.g. http://localhost:5000)"
              value={droneUrl}
              onChange={e => setDroneUrl(e.target.value)}
              required
              aria-label="Drone URL"
              className="border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-400"
            />
            <button
              type="submit"
              disabled={addDroneMutation.isPending}
              className="bg-blue-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-blue-700 disabled:opacity-50"
            >
              {addDroneMutation.isPending ? 'Adding…' : 'Add Drone'}
            </button>
            {addDroneMutation.isError && (
              <p className="text-red-600 text-sm">
                Error: {(addDroneMutation.error as Error)?.message ?? 'Failed to add drone'}
              </p>
            )}
          </form>

          {/* Remove Drone form */}
          <form onSubmit={handleRemoveDrone} className="mb-6 flex flex-col gap-3">
            <input
              type="text"
              placeholder="Drone ID"
              value={removeDroneId}
              onChange={e => setRemoveDroneId(e.target.value)}
              aria-label="Remove Drone ID"
              className="border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-red-400"
            />
            <button
              type="submit"
              className="bg-red-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-red-700"
            >
              Remove Drone
            </button>
          </form>

          {/* Drone list */}
          {fleetLoading && <p className="text-gray-500 text-sm">Loading fleet…</p>}
          {fleetError && <p className="text-red-600 text-sm font-medium">Failed to load fleet</p>}
          {fleet && (
            <div className="overflow-y-auto max-h-96 flex flex-col gap-2">
              {fleet.length === 0 && (
                <p className="text-gray-400 text-sm">No drones registered.</p>
              )}
              {fleet.map((drone: DroneRecord) => {
                const color = DRONE_STATE_COLORS[drone.state];
                return (
                  <div
                    key={drone.id}
                    className="border border-gray-200 rounded-lg px-4 py-3 bg-white flex items-center gap-3 text-sm"
                  >
                    <span className="font-bold text-base">#{drone.badgeNumber}</span>
                    <span
                      className="text-xs font-semibold px-2 py-0.5 rounded-full text-white"
                      style={{ backgroundColor: color }}
                    >
                      {drone.state}
                    </span>
                    <span className="text-gray-500 truncate">{drone.droneUrl}</span>
                  </div>
                );
              })}
            </div>
          )}
        </div>
      </div>
    </div>
  );
}
