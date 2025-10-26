
// ==========================================
// src/components/LoadingSpinner.tsx
// ==========================================
import { Loader2 } from 'lucide-react'

export const LoadingSpinner = () => {
  return (
    <div className="flex items-center justify-center h-full">
      <Loader2 className="w-8 h-8 animate-spin text-primary" />
    </div>
  )
}