import { NavLink } from 'react-router-dom';

export default function NavBar() {
  const linkClass = ({ isActive }: { isActive: boolean }) =>
    `px-4 py-2 rounded font-medium transition-colors ${
      isActive
        ? 'bg-blue-600 text-white'
        : 'text-gray-700 hover:bg-gray-100'
    }`;

  return (
    <nav className="flex items-center gap-2 px-6 py-3 border-b border-gray-200 bg-white shadow-sm">
      <span className="mr-4 font-bold text-lg text-blue-700">
        Drone Dispatch
      </span>
      <NavLink to="/" end className={linkClass}>
        Fleet View
      </NavLink>
      <NavLink to="/orders" className={linkClass}>
        Orders
      </NavLink>
      <NavLink to="/tracking" className={linkClass}>
        Tracking
      </NavLink>
    </nav>
  );
}
