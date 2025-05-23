import { useState } from 'react'
import { comprobantesFiscalesService } from '../services/apiService'
import { XMarkIcon } from '@heroicons/react/24/outline'
import toast from 'react-hot-toast'

export default function CreateComprobanteModal({ isOpen, onClose, onSuccess, rncCedula }) {
  const [formData, setFormData] = useState({
    rncCedula: rncCedula || '',
    ncf: '',
    monto: ''
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
    }

    if (!formData.ncf.trim()) {
      newErrors.ncf = 'NCF es requerido'
    } else if (!/^[a-zA-Z][0-9]{2}[0-9]{8}$/.test(formData.ncf)) {
      newErrors.ncf = 'NCF debe tener el formato correcto (ejemplo: E310000000001)'
    }

    if (!formData.monto) {
      newErrors.monto = 'Monto es requerido'
    } else if (parseFloat(formData.monto) <= 0) {
      newErrors.monto = 'Monto debe ser mayor que cero'
    }

    setErrors(newErrors)
    return Object.keys(newErrors).length === 0
  }

  const calculateITBIS = (monto) => {
    return (parseFloat(monto || 0) * 0.18).toFixed(2)
  }

  const handleSubmit = async (e) => {
    e.preventDefault()
    
    if (!validateForm()) {
      return
    }

    try {
      setLoading(true)
      const dataToSend = {
        ...formData,
        monto: parseFloat(formData.monto)
      }
      
      const response = await comprobantesFiscalesService.create(dataToSend)
      
      toast.success('Comprobante fiscal creado exitosamente')
      setFormData({
        rncCedula: rncCedula || '',
        ncf: '',
        monto: ''
      })
      onSuccess()
    } catch (error) {
      console.error('Error creating comprobante:', error)
      toast.error(error.message || 'Error al crear comprobante fiscal')
    } finally {
      setLoading(false)
    }
  }

  const handleClose = () => {
    setFormData({
      rncCedula: rncCedula || '',
      ncf: '',
      monto: ''
    })
    setErrors({})
    onClose()
  }

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('es-DO', {
      style: 'currency',
      currency: 'DOP'
    }).format(amount)
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
                Crear Nuevo Comprobante Fiscal
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
                <label htmlFor="rncCedula" className="block text-sm font-medium text-gray-700">
                  RNC/Cédula *
                </label>
                <input
                  type="text"
                  id="rncCedula"
                  name="rncCedula"
                  value={formData.rncCedula}
                  onChange={handleChange}
                  className={`input-field font-mono ${errors.rncCedula ? 'border-red-300' : ''}`}
                  placeholder="RNC/Cédula del contribuyente"
                  disabled={!!rncCedula}
                />
                {errors.rncCedula && (
                  <p className="mt-1 text-sm text-red-600">{errors.rncCedula}</p>
                )}
              </div>

              <div>
                <label htmlFor="ncf" className="block text-sm font-medium text-gray-700">
                  NCF (Número de Comprobante Fiscal) *
                </label>
                <input
                  type="text"
                  id="ncf"
                  name="ncf"
                  value={formData.ncf}
                  onChange={handleChange}
                  className={`input-field font-mono uppercase ${errors.ncf ? 'border-red-300' : ''}`}
                  placeholder="E310000000001"
                  maxLength={19}
                  style={{ textTransform: 'uppercase' }}
                />
                {errors.ncf && (
                  <p className="mt-1 text-sm text-red-600">{errors.ncf}</p>
                )}
                <p className="mt-1 text-xs text-gray-500">
                  Formato: E + 2 dígitos + 8 dígitos (ejemplo: E310000000001)
                </p>
              </div>

              <div>
                <label htmlFor="monto" className="block text-sm font-medium text-gray-700">
                  Monto *
                </label>
                <div className="relative">
                  <span className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-500">$</span>
                  <input
                    type="number"
                    id="monto"
                    name="monto"
                    value={formData.monto}
                    onChange={handleChange}
                    className={`input-field pl-8 ${errors.monto ? 'border-red-300' : ''}`}
                    placeholder="0.00"
                    step="0.01"
                    min="0"
                  />
                </div>
                {errors.monto && (
                  <p className="mt-1 text-sm text-red-600">{errors.monto}</p>
                )}
                {formData.monto && parseFloat(formData.monto) > 0 && (
                  <p className="mt-1 text-sm text-gray-600">
                    ITBIS (18%): {formatCurrency(calculateITBIS(formData.monto))}
                  </p>
                )}
              </div>

              {/* Resumen */}
              {formData.monto && parseFloat(formData.monto) > 0 && (
                <div className="bg-gray-50 p-4 rounded-lg">
                  <h4 className="text-sm font-medium text-gray-900 mb-2">Resumen:</h4>
                  <div className="space-y-1 text-sm">
                    <div className="flex justify-between">
                      <span>Monto base:</span>
                      <span>{formatCurrency(formData.monto)}</span>
                    </div>
                    <div className="flex justify-between">
                      <span>ITBIS (18%):</span>
                      <span>{formatCurrency(calculateITBIS(formData.monto))}</span>
                    </div>
                    <div className="flex justify-between font-medium border-t pt-1">
                      <span>Total con ITBIS:</span>
                      <span>{formatCurrency(parseFloat(formData.monto) + parseFloat(calculateITBIS(formData.monto)))}</span>
                    </div>
                  </div>
                </div>
              )}

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
                    'Crear Comprobante'
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