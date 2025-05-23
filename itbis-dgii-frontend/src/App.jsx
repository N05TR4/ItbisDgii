import { Routes, Route, Navigate } from 'react-router-dom'
import { useAuth } from './context/AuthContext'
import Layout from './components/Layout'
import Login from './pages/Login'
import Dashboard from './pages/Dashboard'
import Contribuyentes from './pages/Contribuyentes'
import ComprobantesFiscales from './pages/ComprobantesFiscales'
import ContribuyenteDetail from './pages/ContribuyenteDetail'

function App() {
  const { user, loading } = useAuth()

  if (loading) {
    return (
      <div className="min-h-screen flex items-center justify-center">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-dgii-600"></div>
      </div>
    )
  }

  return (
    <Routes>
      <Route 
        path="/login" 
        element={!user ? <Login /> : <Navigate to="/dashboard" replace />} 
      />
      
      {user ? (
        <Route path="/" element={<Layout />}>
          <Route index element={<Navigate to="/dashboard" replace />} />
          <Route path="dashboard" element={<Dashboard />} />
          <Route path="contribuyentes" element={<Contribuyentes />} />
          <Route path="contribuyentes/:rncCedula" element={<ContribuyenteDetail />} />
          <Route path="comprobantes" element={<ComprobantesFiscales />} />
        </Route>
      ) : (
        <Route path="*" element={<Navigate to="/login" replace />} />
      )}
    </Routes>
  )
}

export default App