
// ==========================================
// src/layouts/MainLayout.tsx
// ==========================================
import { Outlet } from 'react-router-dom'
import { Header } from '@/components/Header'
import { SidebarProvider } from '@/contexts/SidebarContext'

export const MainLayout = () => {
  return (
    <SidebarProvider>
      <div className="flex h-screen bg-background overflow-hidden">
        <div className="flex flex-col flex-1 overflow-hidden w-full">
          <Header />
          <main className="flex-1 overflow-hidden">
            <Outlet />
          </main>
        </div>
      </div>
    </SidebarProvider>
  )
}