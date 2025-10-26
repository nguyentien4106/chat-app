// ==========================================
// src/components/Header.tsx (Updated with Logout)
// ==========================================
import { Bell, Moon, Sun, LogOut, User, Settings, Menu } from 'lucide-react'
import { Button } from '@/components/ui/button'
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu'
import { useDarkMode } from '@/hooks/useDarkMode'
import { useAuth } from '@/contexts/AuthContext'
import { useSidebar } from '@/contexts/SidebarContext'
import { useNavigate } from 'react-router-dom'

export const Header = () => {
  const { isDark, toggleDark } = useDarkMode()
  const { user, logout } = useAuth()
  const { toggleSidebar } = useSidebar()
  console.log(user)
  const navigate = useNavigate()

  const handleLogout = async () => {
    await logout()
    navigate('/login')
  }

  const getInitials = (name: string) => {
    return name ? name
      .split(' ')
      .map((n) => n[0])
      .join('')
      .toUpperCase()
      .slice(0, 2) : "Admin"
  }

  return (
    <header className="h-16 border-b border-border bg-card px-4 md:px-6 flex items-center justify-between">
      <div className="flex items-center gap-4">
        <Button 
          variant="ghost" 
          size="icon"
          onClick={toggleSidebar}
          className="shrink-0"
        >
          <Menu className="w-5 h-5" />
        </Button>
        <h2 className="text-lg font-semibold hidden sm:block">
          {document.title || 'ChatApp Management'}
        </h2>
      </div>
      
      <div className="flex items-center gap-1 md:gap-2">
        <Button variant="ghost" size="icon" className="hidden sm:flex">
          <Bell className="w-5 h-5" />
        </Button>
        
        <Button variant="ghost" size="icon" onClick={toggleDark}>
          {isDark ? (
            <Sun className="w-5 h-5" />
          ) : (
            <Moon className="w-5 h-5" />
          )}
        </Button>

        <div className="h-6 w-px bg-border mx-2 hidden sm:block" />

        <DropdownMenu>
          <DropdownMenuTrigger asChild>
            <Button variant="ghost" className="flex items-center gap-2 md:gap-3">
              <div className="text-right hidden md:block">
                <p className="text-sm font-medium">{user?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']}</p>
                <p className="text-xs text-muted-foreground">{user?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress']}</p>
              </div>
              <div className="w-8 h-8 md:w-10 md:h-10 rounded-full bg-primary flex items-center justify-center text-primary-foreground font-semibold text-sm md:text-base">
                {user ? getInitials(user?.['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name']) : 'U'}
              </div>
            </Button>
          </DropdownMenuTrigger>
          <DropdownMenuContent align="end" className="w-56">
            <DropdownMenuLabel>My Account</DropdownMenuLabel>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={() => navigate('/settings')}>
              <User className="mr-2 h-4 w-4" />
              <span>Profile</span>
            </DropdownMenuItem>
            <DropdownMenuItem onClick={() => navigate('/settings')}>
              <Settings className="mr-2 h-4 w-4" />
              <span>Settings</span>
            </DropdownMenuItem>
            <DropdownMenuSeparator />
            <DropdownMenuItem onClick={handleLogout} className="text-destructive">
              <LogOut className="mr-2 h-4 w-4" />
              <span>Log out</span>
            </DropdownMenuItem>
          </DropdownMenuContent>
        </DropdownMenu>
      </div>
    </header>
  )
}