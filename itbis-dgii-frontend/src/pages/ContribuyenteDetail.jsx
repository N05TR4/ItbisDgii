import { useState, useEffect } from 'react'
import { useParams, Link } from 'react-router-dom'
import { contribuyentesService, comprobantesFiscalesService } from '../services/apiService'
import {
  ArrowLeftIcon,
  DocumentTextIcon,
  CurrencyDollarIcon,
  CalendarIcon,
  PlusIcon
} from '@heroicons/react/24/outline'
import toast from 'react-hot-toast'
import CreateComprobanteModal from '../components/CreateComprobanteModal'

export default function ContribuyenteDetail() {
  const { rncCedula } = useParams()
  const [contribuyente, setContribuyente] = useState(null)
  const [comprobantes, setComprobantes] = useState([])
  const [totalITBIS, setTotalITBIS] = useState(0)
  const [loading, setLoading] = useState(true)
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)

  useEffect(() => {
    if (rncCedula) {
      loadContribuyenteData()
    }
  }, [rncCedula])

  const loadContribuyenteData = async () => {
    try {
      setLoading(true)
      
      // Cargar datos del contribuyente y sus comprobantes en paralelo
      const [contribuyenteResponse, comprobantesResponse, totalResponse] = await Promise.all([
        contribuyentesService.getByRncCedula(rncCedula),
        comprobantesFiscalesService.getByContribuyente(rncCedula),
        comprobantesFiscalesService.getTotalITBIS(rncCedula)
      ])

      if (contribuyenteResponse.succeeded) {
        setContribuyente(contribuyenteResponse.data)
      } else {
        toast.error('Contribuyente no encontrado')
      }

      if (comprobantesResponse.succeeded) {
        setComprobantes(comprobantesResponse.data || [])
      }

      setTotalITBIS(totalResponse || 0)

    } catch (error) {
      console.error('Error loading contribuyente data:', error)
      toast.error('Error al cargar información del contribuyente')
    } finally {
      setLoading(false)
    }
  }

  const handleCreateSuccess = () => {
    loadContribuyenteData()
    setIsCreateModalOpen(false)
  }

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('es-DO', {
      style: 'currency',
      currency: 'DOP'
    }).format(amount)
  }

  const formatDate = (dateString) => {
    return new Date(dateString).toLocaleDateString('es-DO', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    })
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-dgii-600"></div>
      </div>
    )
  }

  if (!contribuyente) {
    return (
      <div className="text-center py-12">
        <h3 className="mt-2 text-sm font-medium text-gray-900">Contribuyente no encontrado</h3>
        <p className="mt-1 text-sm text-gray-500">
          El contribuyente con RNC/Cédula {rncCedula} no existe.
        </p>
        <div className="mt-6">
          <Link to="/contribuyentes" className="btn-primary">
            Volver a Contribuyentes
          </Link>
        </div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex items-center space-x-4">
        <Link
          to="/contribuyentes"
          className="flex items-center text-dgii-600 hover:text-dgii-700"
        >
          <ArrowLeftIcon className="h-5 w-5 mr-1" />
          Volver
        </Link>
      </div>

      {/* Información del contribuyente */}
      <div className="card">
        <div className="px-4 py-5 sm:p-6">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
            <div>
              <h1 className="text-2xl font-bold text-gray-900">
                {contribuyente.nombre}
              </h1>
              <div className="mt-2 flex flex-wrap items-center gap-4 text-sm text-gray-600">
                <span className="font-mono">{contribuyente.rncCedula}</span>
                <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                  contribuyente.tipo === 'PersonaFisica' 
                    ? 'bg-blue-100 text-blue-800' 
                    : 'bg-purple-100 text-purple-800'
                }`}>
                  {contribuyente.tipo === 'PersonaFisica' ? 'Persona Física' : 'Persona Jurídica'}
                </span>
                <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                  contribuyente.estatus === 'Activo' 
                    ? 'bg-green-100 text-green-800' 
                    : 'bg-red-100 text-red-800'
                }`}>
                  {contribuyente.estatus}
                </span>
              </div>
            </div>
            <div className="mt-4 sm:mt-0">
              <button
                onClick={() => setIsCreateModalOpen(true)}
                className="btn-primary inline-flex items-center"
                disabled={contribuyente.estatus !== 'Activo'}
              >
                <PlusIcon className="h-5 w-5 mr-2" />
                Nuevo Comprobante
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* Estadísticas */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="card">
          <div className="p-6">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <DocumentTextIcon className="h-8 w-8 text-blue-600" />
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Comprobantes</h3>
                <p className="text-2xl font-bold text-gray-900">{comprobantes.length}</p>
              </div>
            </div>
          </div>
        </div>

        <div className="card">
          <div className="p-6">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <CurrencyDollarIcon className="h-8 w-8 text-green-600" />
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Total ITBIS</h3>
                <p className="text-2xl font-bold text-gray-900">{formatCurrency(totalITBIS)}</p>
              </div>
            </div>
          </div>
        </div>

        <div className="card">
          <div className="p-6">
            <div className="flex items-center">
              <div className="flex-shrink-0">
                <CurrencyDollarIcon className="h-8 w-8 text-yellow-600" />
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Promedio ITBIS</h3>
                <p className="text-2xl font-bold text-gray-900">
                  {comprobantes.length > 0 ? formatCurrency(totalITBIS / comprobantes.length) : formatCurrency(0)}
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Lista de comprobantes */}
      <div className="card">
        <div className="px-4 py-5 sm:p-6">
          <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
            Comprobantes Fiscales
          </h3>
          
          {comprobantes.length === 0 ? (
            <div className="text-center py-12">
              <DocumentTextIcon className="mx-auto h-12 w-12 text-gray-400" />
              <h3 className="mt-2 text-sm font-medium text-gray-900">No hay comprobantes</h3>
              <p className="mt-1 text-sm text-gray-500">
                Este contribuyente no tiene comprobantes fiscales registrados.
              </p>
              {contribuyente.estatus === 'Activo' && (
                <div className="mt-6">
                  <button
                    onClick={() => setIsCreateModalOpen(true)}
                    className="btn-primary inline-flex items-center"
                  >
                    <PlusIcon className="h-5 w-5 mr-2" />
                    Crear Primer Comprobante
                  </button>
                </div>
              )}
            </div>
          ) : (
            <div className="overflow-hidden">
              <table className="min-w-full divide-y divide-gray-200">
                <thead className="bg-gray-50">
                  <tr>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      NCF
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Monto
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      ITBIS (18%)
                    </th>
                    <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                      Fecha
                    </th>
                  </tr>
                </thead>
                <tbody className="bg-white divide-y divide-gray-200">
                  {comprobantes.map((comprobante, index) => (
                    <tr key={index} className="hover:bg-gray-50">
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm font-medium text-gray-900 font-mono">
                          {comprobante.ncf}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-900 font-medium">
                          {formatCurrency(comprobante.monto)}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-900 font-medium">
                          {formatCurrency(comprobante.itbis18)}
                        </div>
                      </td>
                      <td className="px-6 py-4 whitespace-nowrap">
                        <div className="text-sm text-gray-500">
                          {/* Aquí podrías mostrar la fecha de creación si la tienes en el API */}
                          <CalendarIcon className="inline h-4 w-4 mr-1" />
                          Reciente
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
                <tfoot className="bg-gray-50">
                  <tr>
                    <td className="px-6 py-3 text-sm font-medium text-gray-900" colSpan="2">
                      Total ({comprobantes.length} comprobantes)
                    </td>
                    <td className="px-6 py-3 text-sm font-bold text-gray-900">
                      {formatCurrency(totalITBIS)}
                    </td>
                    <td></td>
                  </tr>
                </tfoot>
              </table>
            </div>
          )}
        </div>
      </div>

      {/* Modal de crear comprobante */}
      <CreateComprobanteModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSuccess={handleCreateSuccess}
        rncCedula={rncCedula}
      />
    </div>
  )
}