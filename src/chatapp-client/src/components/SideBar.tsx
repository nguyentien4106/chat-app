
// ==========================================
// src/components/Sidebar.tsx
// ==========================================
import { NavLink } from 'react-router-dom'
import { 
  LayoutDashboard, 
  Monitor, 
  Users, 
  Clock, 
  FileText, 
  Settings,
  X
} from 'lucide-react'
import { cn } from '@/lib/utils'
import { useSidebar } from '@/contexts/SidebarContext'
import { Button } from '@/components/ui/button'

const navItems = [
  { to: '/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/devices', icon: Monitor, label: 'Devices' },
  { to: '/users', icon: Users, label: 'Users' },
  { to: '/attendance', icon: Clock, label: 'Attendance' },
  { to: '/reports', icon: FileText, label: 'Reports' },
  { to: '/settings', icon: Settings, label: 'Settings' },
]

export const Sidebar = () => {
  const { isOpen, closeSidebar } = useSidebar()

  return (
    <>
      {/* Overlay for mobile */}
      {isOpen && (
        <div 
          className="fixed inset-0 bg-black/50 z-40 md:hidden"
          onClick={closeSidebar}
        />
      )}

      {/* Sidebar */}
      <aside 
        className={cn(
          'fixed md:static inset-y-0 left-0 z-50 w-64 bg-card border-r border-border',
          'transform transition-transform duration-300 ease-in-out',
          isOpen ? 'translate-x-0' : '-translate-x-full md:translate-x-0',
          !isOpen && 'md:w-0 md:border-0 md:overflow-hidden'
        )}
      >
        <div className="flex items-center justify-between h-16 px-6 border-b border-border">
          <h1 className="text-xl font-bold text-primary">ChatApp Manager</h1>
          <Button
            variant="ghost"
            size="icon"
            className="md:hidden"
            onClick={closeSidebar}
          >
            <X className="w-5 h-5" />
          </Button>
        </div>
        <nav className="p-4 space-y-1">
          {navItems.map((item) => (
            <NavLink
              key={item.to}
              to={item.to}
              onClick={() => {
                // Close sidebar on mobile when navigating
                if (window.innerWidth < 768) {
                  closeSidebar()
                }
              }}
              className={({ isActive }) =>
                cn(
                  'flex items-center gap-3 px-4 py-3 rounded-lg transition-colors',
                  'hover:bg-accent hover:text-accent-foreground',
                  isActive
                    ? 'bg-primary text-primary-foreground'
                    : 'text-muted-foreground'
                )
              }
            >
              <item.icon className="w-5 h-5" />
              <span className="font-medium">{item.label}</span>
            </NavLink>
          ))}
        </nav>
      </aside>
    </>
  )
}
