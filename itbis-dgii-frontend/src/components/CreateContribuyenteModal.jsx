import { useState } from 'react'
import { contribuyentesService } from '../services/apiService'
import { XMarkIcon } from '@heroicons/react/24/outline'
import toast from 'react-hot-toast'

export default function CreateContribuyenteModal({ isOpen, onClose, onSuccess }) {
  const [formData, setFormData] = useState({
    rncCedula: '',
    nombre: '',
    tipo: 'PersonaFisica',
    estatus: 'Activo'
  })
  const [loading, setLoading] = useState(false)
  const [errors, setErrors] = useState({})

  const handleChange = (e) => {
    const { name, value } = e.target
    setFormData(prev => ({
      ...prev,
      [name]: value
    }))
    // Limpiar error cuando el usuario empiece a escribir
    if (errors[name]) {
      setErrors(prev => ({
        ...prev,
        [name]: ''
      }))
    }
  }

  const validateForm = () => {
    const newErrors = {}

    if (!formData.rncCedula.trim()) {
      newErrors.rncCedula = 'RNC/Cédula es requerido'
    } else if (formData.tipo === 'PersonaFisica' && formData.rncCedula.length !== 11) {
      newErrors.rncCedula = 'Cédula debe tener 11 dígitos'
    } else if (formData.tipo === 'PersonaJuridica' && formData.rncCedula.length !== 9) {
      newErrors.rncCedula = 'RNC debe tener 9 dígitos'
    }

    if (!formData.nombre.trim()) {
      newErrors.nombre = 'Nombre es requerido'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (!validateForm()) {
      return
    }

    try {
      setLoading(true)
      const response = await contribuyentesService.create(formData)
      
      if (response.succeeded) {
        toast.success('Contribuyente creado exitosamente')
        setFormData({
          rncCedula: '',
          nombre: '',
          tipo: 'PersonaFisica',
          estatus: 'Activo'
        })
        onSuccess()
      } else {
        toast.error(response.message || 'Error al crear contribuyente')
      }
    } catch (error) {
      console.error('Error creating contribuyente:', error)
      toast.error(error.message || 'Error al crear contribuyente')
    } finally {
      setLoading(false)
    }
  }

  const handleClose = () => {
    setFormData({
      rncCedula: '',
      nombre: '',
      tipo: 'PersonaFisica',
      estatus: 'Activo'
    })
    setErrors({})
    onClose()
  }

  if (!isOpen) return null

  return (
    <div className="fixed inset-0 z-50 overflow-y-auto">
      <div className="flex items-end justify-center min-h-screen pt-4 px-4 pb-20 text-center sm:block sm:p-0">
        <div className="fixed inset-0 transition-opacity" onClick={handleClose}>
          <div className="absolute inset-0 bg-gray-500 opacity-75"></div>
        </div>

        <span className="hidden sm:inline-block sm:align-middle sm:h-screen">&#8203;</span>

        <div className="inline-block align-bottom bg-white rounded-lg text-left overflow-hidden shadow-xl transform transition-all sm:my-8 sm:align-middle sm:max-w-lg sm:w-full">
          <div className="bg-white px-4 pt-5 pb-4 sm:p-6 sm:pb-4">
            <div className="flex items-center justify-between mb-4">
              <h3 className="text-lg leading-6 font-medium text-gray-900">
                Crear Nuevo Contribuyente
              </h3>
              <button
                onClick={handleClose}
                className="rounded-md text-gray-400 hover:text-gray-500 focus:outline-none focus:ring-2 focus:ring-dgii-500"
              >
                <XMarkIcon className="h-6 w-6" />
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label htmlFor="nombre" className="block text-sm font-medium text-gray-700">
                  Nombre *
                </label>
                <input
                  type="text"
                  id="nombre"
                  name="nombre"
                  value={formData.nombre}
                  onChange={handleChange}
                  className={`input-field ${errors.nombre ? 'border-red-300' : ''}`}
                  placeholder="Nombre del contribuyente"
                />
                {errors.nombre && (
                  <p className="mt-1 text-sm text-red-600">{errors.nombre}</p>
                )}
              </div>

              <div>
                <label htmlFor="tipo" className="block text-sm font-medium text-gray-700">
                  Tipo *
                </label>
                <select
                  id="tipo"
                  name="tipo"
                  value={formData.tipo}
                  onChange={handleChange}
                  className="input-field"
                >
                  <option value="PersonaFisica">Persona Física</option>
                  <option value="PersonaJuridica">Persona Jurídica</option>
                </select>
              </div>

              <div>
                <label htmlFor="rncCedula" className="block text-sm font-medium text-gray-700">
                  {formData.tipo === 'PersonaFisica' ? 'Cédula (11 dígitos)' : 'RNC (9 dígitos)'} *
                </label>
                <input
                  type="text"
                  id="rncCedula"
                  name="rncCedula"
                  value={formData.rncCedula}
                  onChange={handleChange}
                  className={`input-field ${errors.rncCedula ? 'border-red-300' : ''}`}
                  placeholder={formData.tipo === 'PersonaFisica' ? '00000000000' : '000000000'}
                  maxLength={formData.tipo === 'PersonaFisica' ? 11 : 9}
                />
                {errors.rncCedula && (
                  <p className="mt-1 text-sm text-red-600">{errors.rncCedula}</p>
                )}
              </div>

              <div>
                <label htmlFor="estatus" className="block text-sm font-medium text-gray-700">
                  Estado *
                </label>
                <select
                  id="estatus"
                  name="estatus"
                  value={formData.estatus}
                  onChange={handleChange}
                  className="input-field"
                >
                  <option value="Activo">Activo</option>
                  <option value="Inactivo">Inactivo</option>
                </select>
              </div>

              <div className="flex justify-end space-x-3 pt-4">
                <button
                  type="button"
                  onClick={handleClose}
                  className="btn-secondary"
                  disabled={loading}
                >
                  Cancelar
                </button>
                <button
                  type="submit"
                  className="btn-primary"
                  disabled={loading}
                >
                  {loading ? (
                    <>
                      <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                      Creando...
                    </>
                  ) : (
                    'Crear Contribuyente'
                  )}
                </button>
              </div>
            </form>
          </div>
        </div>
      </div>
    </div>
  )
}