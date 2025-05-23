import { useState } from 'react'
import { useAuth } from '../context/AuthContext'
import { EyeIcon, EyeSlashIcon } from '@heroicons/react/24/outline'

export default function Login() {
  const [formData, setFormData] = useState({
    email: '',
    password: ''
  })
  const [showPassword, setShowPassword] = useState(false)
  const [isRegistering, setIsRegistering] = useState(false)
  const [registerData, setRegisterData] = useState({
    nombre: '',
    apellido: '',
    email: '',
    userName: '',
    password: '',
    confirmPassword: ''
  })

  const { login, register, loading } = useAuth()

  const handleLoginSubmit = async (e) => {
    e.preventDefault()
    await login(formData.email, formData.password)
  }

  const handleRegisterSubmit = async (e) => {
    e.preventDefault()
    if (registerData.password !== registerData.confirmPassword) {
      toast.error('Las contraseñas no coinciden')
      return
    }
    const result = await register(registerData)
    if (result.success) {
      setIsRegistering(false)
      setRegisterData({
        nombre: '',
        apellido: '',
        email: '',
        userName: '',
        password: '',
        confirmPassword: ''
      })
    }
  }

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    })
  }

  const handleRegisterChange = (e) => {
    setRegisterData({
      ...registerData,
      [e.target.name]: e.target.value
    })
  }

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-md w-full space-y-8">
        <div>
          <div className="mx-auto h-16 w-16 bg-dgii-600 rounded-full flex items-center justify-center">
            <span className="text-white font-bold text-2xl">DGII</span>
          </div>
          <h2 className="mt-6 text-center text-3xl font-extrabold text-gray-900">
            {isRegistering ? 'Crear nueva cuenta' : 'Iniciar sesión'}
          </h2>
          <p className="mt-2 text-center text-sm text-gray-600">
            Sistema de Gestión ITBIS - DGII
          </p>
        </div>

        {!isRegistering ? (
          // Formulario de Login
          <form className="mt-8 space-y-6" onSubmit={handleLoginSubmit}>
            <div className="space-y-4">
              <div>
                <label htmlFor="email" className="block text-sm font-medium text-gray-700">
                  Correo electrónico
                </label>
                <input
                  id="email"
                  name="email"
                  type="email"
                  required
                  className="input-field"
                  placeholder="usuario@dgii.gov.do"
                  value={formData.email}
                  onChange={handleChange}
                />
              </div>
              
              <div>
                <label htmlFor="password" className="block text-sm font-medium text-gray-700">
                  Contraseña
                </label>
                <div className="relative">
                  <input
                    id="password"
                    name="password"
                    type={showPassword ? 'text' : 'password'}
                    required
                    className="input-field pr-10"
                    placeholder="••••••••"
                    value={formData.password}
                    onChange={handleChange}
                  />
                  <button
                    type="button"
                    className="absolute inset-y-0 right-0 pr-3 flex items-center"
                    onClick={() => setShowPassword(!showPassword)}
                  >
                    {showPassword ? (
                      <EyeSlashIcon className="h-5 w-5 text-gray-400" />
                    ) : (
                      <EyeIcon className="h-5 w-5 text-gray-400" />
                    )}
                  </button>
                </div>
              </div>
            </div>

            <div>
              <button
                type="submit"
                disabled={loading}
                className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-dgii-600 hover:bg-dgii-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-dgii-500 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? (
                  <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                ) : null}
                Iniciar sesión
              </button>
            </div>

            <div className="text-center">
              <button
                type="button"
                onClick={() => setIsRegistering(true)}
                className="text-dgii-600 hover:text-dgii-500 text-sm font-medium"
              >
                ¿No tienes cuenta? Regístrate aquí
              </button>
            </div>
          </form>
        ) : (
          // Formulario de Registro
          <form className="mt-8 space-y-6" onSubmit={handleRegisterSubmit}>
            <div className="space-y-4">
              <div className="grid grid-cols-2 gap-4">
                <div>
                  <label htmlFor="nombre" className="block text-sm font-medium text-gray-700">
                    Nombre
                  </label>
                  <input
                    id="nombre"
                    name="nombre"
                    type="text"
                    required
                    className="input-field"
                    placeholder="Juan"
                    value={registerData.nombre}
                    onChange={handleRegisterChange}
                  />
                </div>
                <div>
                  <label htmlFor="apellido" className="block text-sm font-medium text-gray-700">
                    Apellido
                  </label>
                  <input
                    id="apellido"
                    name="apellido"
                    type="text"
                    required
                    className="input-field"
                    placeholder="Pérez"
                    value={registerData.apellido}
                    onChange={handleRegisterChange}
                  />
                </div>
              </div>

              <div>
                <label htmlFor="register-email" className="block text-sm font-medium text-gray-700">
                  Correo electrónico
                </label>
                <input
                  id="register-email"
                  name="email"
                  type="email"
                  required
                  className="input-field"
                  placeholder="usuario@dgii.gov.do"
                  value={registerData.email}
                  onChange={handleRegisterChange}
                />
              </div>

              <div>
                <label htmlFor="userName" className="block text-sm font-medium text-gray-700">
                  Nombre de usuario
                </label>
                <input
                  id="userName"
                  name="userName"
                  type="text"
                  required
                  className="input-field"
                  placeholder="usuario123"
                  value={registerData.userName}
                  onChange={handleRegisterChange}
                />
              </div>

              <div>
                <label htmlFor="register-password" className="block text-sm font-medium text-gray-700">
                  Contraseña
                </label>
                <input
                  id="register-password"
                  name="password"
                  type="password"
                  required
                  className="input-field"
                  placeholder="••••••••"
                  value={registerData.password}
                  onChange={handleRegisterChange}
                />
              </div>

              <div>
                <label htmlFor="confirmPassword" className="block text-sm font-medium text-gray-700">
                  Confirmar contraseña
                </label>
                <input
                  id="confirmPassword"
                  name="confirmPassword"
                  type="password"
                  required
                  className="input-field"
                  placeholder="••••••••"
                  value={registerData.confirmPassword}
                  onChange={handleRegisterChange}
                />
              </div>
            </div>

            <div>
              <button
                type="submit"
                disabled={loading}
                className="group relative w-full flex justify-center py-2 px-4 border border-transparent text-sm font-medium rounded-md text-white bg-dgii-600 hover:bg-dgii-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-dgii-500 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? (
                  <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                ) : null}
                Crear cuenta
              </button>
            </div>

            <div className="text-center">
              <button
                type="button"
                onClick={() => setIsRegistering(false)}
                className="text-dgii-600 hover:text-dgii-500 text-sm font-medium"
              >
                ¿Ya tienes cuenta? Inicia sesión
              </button>
            </div>
          </form>
        )}

        {/* Usuarios de prueba */}
        <div className="mt-8 p-4 bg-blue-50 rounded-lg">
          <h3 className="text-sm font-medium text-blue-800 mb-2">Usuarios de prueba:</h3>
          <div className="text-xs text-blue-700 space-y-1">
            <div><strong>Admin:</strong> userAdmin@mail.com / Admin123!</div>
            <div><strong>Básico:</strong> userBasic@mail.com / Basic123!</div>
          </div>
        </div>
      </div>
    </div>
  )
}