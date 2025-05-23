import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { comprobantesFiscalesService } from '../services/apiService'
import { 
  PlusIcon, 
  MagnifyingGlassIcon, 
  DocumentTextIcon,
  FunnelIcon 
} from '@heroicons/react/24/outline'
import toast from 'react-hot-toast'
import CreateComprobanteModal from '../components/CreateComprobanteModal'

export default function ComprobantesFiscales() {
  const [comprobantes, setComprobantes] = useState([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState('')
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)
  const [filterMonto, setFilterMonto] = useState('')

  useEffect(() => {
    loadComprobantes()
  }, [])

  const loadComprobantes = async () => {
    try {
      setLoading(true)
      const response = await comprobantesFiscalesService.getAll()
      const data = response.data?.items || response.items || []
      setComprobantes(data)
    } catch (error) {
      console.error('Error loading comprobantes:', error)
      toast.error('Error al cargar comprobantes fiscales')
    } finally {
      setLoading(false)
    }
  }

  const handleCreateSuccess = () => {
    loadComprobantes()
    setIsCreateModalOpen(false)
  }

  const filteredComprobantes = comprobantes.filter(comprobante => {
    const matchesSearch = 
      comprobante.ncf?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      comprobante.rncCedula?.includes(searchTerm)
    
    const matchesFilter = filterMonto === '' || 
      (filterMonto === 'low' && comprobante.monto < 500) ||
      (filterMonto === 'medium' && comprobante.monto >= 500 && comprobante.monto < 2000) ||
      (filterMonto === 'high' && comprobante.monto >= 2000)
    
    return matchesSearch && matchesFilter
  })

  const totalMonto = filteredComprobantes.reduce((sum, comp) => sum + (comp.monto || 0), 0)
  const totalITBIS = filteredComprobantes.reduce((sum, comp) => sum + (comp.itbis18 || 0), 0)

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('es-DO', {
      style: 'currency',
      currency: 'DOP'
    }).format(amount)
  }

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-dgii-600"></div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Comprobantes Fiscales</h1>
          <p className="mt-1 text-sm text-gray-600">
            Gestiona todos los comprobantes fiscales del sistema
          </p>
        </div>
        <div className="mt-4 sm:mt-0">
          <button
            onClick={() => setIsCreateModalOpen(true)}
            className="btn-primary inline-flex items-center"
          >
            <PlusIcon className="h-5 w-5 mr-2" />
            Nuevo Comprobante
          </button>
        </div>
      </div>

      {/* Estadísticas rápidas */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
        <div className="card">
          <div className="p-6">
            <div className="flex items-center">
              <DocumentTextIcon className="h-8 w-8 text-blue-600" />
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Total Comprobantes</h3>
                <p className="text-2xl font-bold text-gray-900">{filteredComprobantes.length}</p>
              </div>
            </div>
          </div>
        </div>

        <div className="card">
          <div className="p-6">
            <div className="flex items-center">
              <div className="h-8 w-8 bg-green-100 rounded-lg flex items-center justify-center">
                <span className="text-green-600 font-bold">$</span>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Monto Total</h3>
                <p className="text-xl font-bold text-gray-900">{formatCurrency(totalMonto)}</p>
              </div>
            </div>
          </div>
        </div>

        <div className="card">
          <div className="p-6">
            <div className="flex items-center">
              <div className="h-8 w-8 bg-yellow-100 rounded-lg flex items-center justify-center">
                <span className="text-yellow-600 font-bold">₮</span>
              </div>
              <div className="ml-4">
                <h3 className="text-lg font-medium text-gray-900">Total ITBIS</h3>
                <p className="text-xl font-bold text-gray-900">{formatCurrency(totalITBIS)}</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Filtros y búsqueda */}
      <div className="card">
        <div className="p-4">
          <div className="flex flex-col lg:flex-row gap-4">
            <div className="flex-1">
              <div className="relative">
                <MagnifyingGlassIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
                <input
                  type="text"
                  placeholder="Buscar por NCF o RNC/Cédula..."
                  className="pl-10 input-field"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
            </div>
            <div className="flex items-center space-x-4">
              <div className="flex items-center space-x-2">
                <FunnelIcon className="h-5 w-5 text-gray-400" />
                <select
                  value={filterMonto}
                  onChange={(e) => setFilterMonto(e.target.value)}
                  className="input-field min-w-0 w-48"
                >
                  <option value="">Todos los montos</option>
                  <option value="low">Menor a $500</option>
                  <option value="medium">$500 - $2,000</option>
                  <option value="high">Mayor a $2,000</option>
                </select>
              </div>
              <span className="text-sm text-gray-600">
                {filteredComprobantes.length} comprobantes
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Lista de comprobantes */}
      <div className="card">
        {filteredComprobantes.length === 0 ? (
          <div className="text-center py-12">
            <DocumentTextIcon className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No hay comprobantes</h3>
            <p className="mt-1 text-sm text-gray-500">
              {searchTerm || filterMonto 
                ? 'No se encontraron comprobantes con esos criterios de búsqueda.'
                : 'Comienza creando un nuevo comprobante fiscal.'
              }
            </p>
            {!searchTerm && !filterMonto && (
              <div className="mt-6">
                <button
                  onClick={() => setIsCreateModalOpen(true)}
                  className="btn-primary inline-flex items-center"
                >
                  <PlusIcon className="h-5 w-5 mr-2" />
                  Nuevo Comprobante
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
                    RNC/Cédula
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Monto
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    ITBIS (18%)
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Total con ITBIS
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Acciones
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredComprobantes.map((comprobante, index) => (
                  <tr key={index} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm font-medium text-gray-900 font-mono">
                        {comprobante.ncf}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900 font-mono">
                        {comprobante.rncCedula}
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
                      <div className="text-sm text-gray-900 font-medium">
                        {formatCurrency(comprobante.monto + comprobante.itbis18)}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <Link
                        to={`/contribuyentes/${comprobante.rncCedula}`}
                        className="text-dgii-600 hover:text-dgii-900"
                      >
                        Ver contribuyente
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
              {filteredComprobantes.length > 0 && (
                <tfoot className="bg-gray-50">
                  <tr>
                    <td className="px-6 py-3 text-sm font-medium text-gray-900" colSpan="2">
                      Totales ({filteredComprobantes.length} comprobantes)
                    </td>
                    <td className="px-6 py-3 text-sm font-bold text-gray-900">
                      {formatCurrency(totalMonto)}
                    </td>
                    <td className="px-6 py-3 text-sm font-bold text-gray-900">
                      {formatCurrency(totalITBIS)}
                    </td>
                    <td className="px-6 py-3 text-sm font-bold text-gray-900">
                      {formatCurrency(totalMonto + totalITBIS)}
                    </td>
                    <td></td>
                  </tr>
                </tfoot>
              )}
            </table>
          </div>
        )}
      </div>

      {/* Modal de crear comprobante */}
      <CreateComprobanteModal
        isOpen={isCreateModalOpen}
        onClose={() => setIsCreateModalOpen(false)}
        onSuccess={handleCreateSuccess}
      />
    </div>
  )
}