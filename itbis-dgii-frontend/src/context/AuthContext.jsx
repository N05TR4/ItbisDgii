import { createContext, useContext, useState, useEffect } from 'react'
import { authService } from '../services/authService'
import toast from 'react-hot-toast'

const AuthContext = createContext()

export const useAuth = () => {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth debe ser usado dentro de un AuthProvider')
  }
  return context
}

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null)
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    checkAuth()
  }, [])

  const checkAuth = () => {
    const token = localStorage.getItem('token')
    const userData = localStorage.getItem('user')
    
    if (token && userData) {
      try {
        const parsedUser = JSON.parse(userData)
        setUser(parsedUser)
        authService.setAuthToken(token)
      } catch (error) {
        console.error('Error parsing user data:', error)
        localStorage.removeItem('token')
        localStorage.removeItem('user')
      }
    }
    setLoading(false)
  }

  const login = async (email, password) => {
    try {
      setLoading(true)
      const response = await authService.login(email, password)
      
      if (response.succeeded) {
        const { data } = response
        localStorage.setItem('token', data.jwToken)
        localStorage.setItem('user', JSON.stringify(data))
        authService.setAuthToken(data.jwToken)
        setUser(data)
        toast.success(`¡Bienvenido, ${data.userName}!`)
        return { success: true }
      } else {
        toast.error(response.message || 'Error en las credenciales')
        return { success: false, error: response.message }
      }
    } catch (error) {
      console.error('Login error:', error)
      toast.error('Error al iniciar sesión')
      return { success: false, error: error.message }
    } finally {
      setLoading(false)
    }
  }

  const logout = () => {
    localStorage.removeItem('token')
    localStorage.removeItem('user')
    authService.setAuthToken(null)
    setUser(null)
    toast.success('Sesión cerrada exitosamente')
  }

  const register = async (userData) => {
    try {
      setLoading(true)
      const response = await authService.register(userData)
      
      if (response.succeeded) {
        toast.success('Usuario registrado exitosamente')
        return { success: true }
      } else {
        toast.error(response.message || 'Error al registrar usuario')
        return { success: false, error: response.message }
      }
    } catch (error) {
      console.error('Register error:', error)
      toast.error('Error al registrar usuario')
      return { success: false, error: error.message }
    } finally {
      setLoading(false)
    }
  }

  const value = {
    user,
    login,
    logout,
    register,
    loading
  }

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  )
}