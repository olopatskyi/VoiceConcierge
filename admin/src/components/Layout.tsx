import { NavLink, Outlet } from 'react-router-dom';

const links = [
  { to: '/playground', label: 'Playground', emoji: '🎙️' },
  { to: '/faq', label: 'FAQs', emoji: '📚' },
  { to: '/unanswered', label: 'Unanswered Queue', emoji: '❓' },
  { to: '/voices', label: 'Voices', emoji: '🗣️' },
];

export function Layout() {
  return (
    <div className="layout">
      <aside className="sidebar">
        <div className="brand">
          <div className="brand-title">Meridian</div>
          <div className="brand-subtitle">Concierge Admin</div>
        </div>
        <nav>
          {links.map((l) => (
            <NavLink
              key={l.to}
              to={l.to}
              className={({ isActive }) => `nav-link${isActive ? ' active' : ''}`}
            >
              <span className="nav-emoji" aria-hidden>
                {l.emoji}
              </span>
              {l.label}
            </NavLink>
          ))}
        </nav>
      </aside>
      <main className="content">
        <Outlet />
      </main>
    </div>
  );
}
