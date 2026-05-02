import { describe, it, expect } from 'vitest';
import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import NavBar from '../components/NavBar';

function renderNavBar(initialPath = '/') {
  return render(
    <MemoryRouter initialEntries={[initialPath]}>
      <NavBar />
    </MemoryRouter>
  );
}

describe('NavBar', () => {
  it('renders the app brand name', () => {
    renderNavBar();
    expect(screen.getByText('Drone Dispatch')).toBeInTheDocument();
  });

  it('renders a link to Fleet View', () => {
    renderNavBar();
    const fleetLink = screen.getByRole('link', { name: /fleet view/i });
    expect(fleetLink).toBeInTheDocument();
    expect(fleetLink).toHaveAttribute('href', '/');
  });

  it('renders a link to Orders', () => {
    renderNavBar();
    const ordersLink = screen.getByRole('link', { name: /orders/i });
    expect(ordersLink).toBeInTheDocument();
    expect(ordersLink).toHaveAttribute('href', '/orders');
  });

  it('renders a link to Tracking', () => {
    renderNavBar();
    const trackingLink = screen.getByRole('link', { name: /tracking/i });
    expect(trackingLink).toBeInTheDocument();
    expect(trackingLink).toHaveAttribute('href', '/tracking');
  });

  it('renders all 3 navigation links', () => {
    renderNavBar();
    const links = screen.getAllByRole('link');
    expect(links).toHaveLength(3);
  });

  it('marks Fleet View as active when on root path', () => {
    renderNavBar('/');
    const fleetLink = screen.getByRole('link', { name: /fleet view/i });
    // NavLink applies aria-current="page" when active
    expect(fleetLink).toHaveAttribute('aria-current', 'page');
  });

  it('does not mark Fleet View as active when on /orders', () => {
    renderNavBar('/orders');
    const fleetLink = screen.getByRole('link', { name: /fleet view/i });
    expect(fleetLink).not.toHaveAttribute('aria-current', 'page');
  });

  it('marks Orders as active when on /orders path', () => {
    renderNavBar('/orders');
    const ordersLink = screen.getByRole('link', { name: /orders/i });
    expect(ordersLink).toHaveAttribute('aria-current', 'page');
  });

  it('marks Tracking as active when on /tracking path', () => {
    renderNavBar('/tracking');
    const trackingLink = screen.getByRole('link', { name: /tracking/i });
    expect(trackingLink).toHaveAttribute('aria-current', 'page');
  });
});
