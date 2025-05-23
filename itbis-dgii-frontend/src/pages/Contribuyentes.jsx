import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { contribuyentesService } from '../services/apiService'
import { PlusIcon, MagnifyingGlassIcon, UserIcon } from '@heroicons/react/24/outline'
import toast from 'react-hot-toast'
import CreateContribuyenteModal from '../components/CreateContribuyenteModal'

export default function Contribuyentes() {
  const [contribuyentes, setContribuyentes] = useState([])
  const [loading, setLoading] = useState(true)
  const [searchTerm, setSearchTerm] = useState('')
  const [isCreateModalOpen, setIsCreateModalOpen] = useState(false)

  useEffect(() => {
    loadContribuyentes()
  }, [])

  const loadContribuyentes = async () => {
    try {
      setLoading(true)
      const response = await contribuyentesService.getAll()
      const data = response.data?.items || response.items || []
      setContribuyentes(data)
    } catch (error) {
      console.error('Error loading contribuyentes:', error)
      toast.error('Error al cargar contribuyentes')
    } finally {
      setLoading(false)
    }
  }

  const handleCreateSuccess = () => {
    loadContribuyentes()
    setIsCreateModalOpen(false)
  }

  const filteredContribuyentes = contribuyentes.filter(contribuyente =>
    contribuyente.nombre.toLowerCase().includes(searchTerm.toLowerCase()) ||
    contribuyente.rncCedula.includes(searchTerm) ||
    contribuyente.tipo.toLowerCase().includes(searchTerm.toLowerCase())
  )

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('es-DO', {
      style: 'currency',
      currency: 'DOP'
    }).format(amount)
  }

  const getEstadoBadge = (estatus) => {
    return estatus === 'Activo' 
      ? 'bg-green-100 text-green-800' 
      : 'bg-red-100 text-red-800'
  }

  const getTipoBadge = (tipo) => {
    return tipo === 'PersonaFisica' 
      ? 'bg-blue-100 text-blue-800' 
      : 'bg-purple-100 text-purple-800'
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
          <h1 className="text-2xl font-bold text-gray-900">Contribuyentes</h1>
          <p className="mt-1 text-sm text-gray-600">
            Gestiona todos los contribuyentes registrados en el sistema
          </p>
        </div>
        <div className="mt-4 sm:mt-0">
          <button
            onClick={() => setIsCreateModalOpen(true)}
            className="btn-primary inline-flex items-center"
          >
            <PlusIcon className="h-5 w-5 mr-2" />
            Nuevo Contribuyente
          </button>
        </div>
      </div>

      {/* Barra de búsqueda y filtros */}
      <div className="card">
        <div className="p-4">
          <div className="flex flex-col sm:flex-row gap-4">
            <div className="flex-1">
              <div className="relative">
                <MagnifyingGlassIcon className="absolute left-3 top-1/2 transform -translate-y-1/2 h-5 w-5 text-gray-400" />
                <input
                  type="text"
                  placeholder="Buscar por nombre, RNC/Cédula o tipo..."
                  className="pl-10 input-field"
                  value={searchTerm}
                  onChange={(e) => setSearchTerm(e.target.value)}
                />
              </div>
            </div>
            <div className="flex items-center space-x-2 text-sm text-gray-600">
              <span>Total: {filteredContribuyentes.length} contribuyentes</span>
            </div>
          </div>
        </div>
      </div>

      {/* Lista de contribuyentes */}
      <div className="card">
        {filteredContribuyentes.length === 0 ? (
          <div className="text-center py-12">
            <UserIcon className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">No hay contribuyentes</h3>
            <p className="mt-1 text-sm text-gray-500">
              {searchTerm ? 'No se encontraron contribuyentes con ese criterio de búsqueda.' : 'Comienza creando un nuevo contribuyente.'}
            </p>
            {!searchTerm && (
              <div className="mt-6">
                <button
                  onClick={() => setIsCreateModalOpen(true)}
                  className="btn-primary inline-flex items-center"
                >
                  <PlusIcon className="h-5 w-5 mr-2" />
                  Nuevo Contribuyente
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
                    Contribuyente
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    RNC/Cédula
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Tipo
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Estado
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Total ITBIS
                  </th>
                  <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                    Acciones
                  </th>
                </tr>
              </thead>
              <tbody className="bg-white divide-y divide-gray-200">
                {filteredContribuyentes.map((contribuyente, index) => (
                  <tr key={index} className="hover:bg-gray-50">
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div>
                        <div className="text-sm font-medium text-gray-900">
                          {contribuyente.nombre}
                        </div>
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900 font-mono">
                        {contribuyente.rncCedula}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${getTipoBadge(contribuyente.tipo)}`}>
                        {contribuyente.tipo === 'PersonaFisica' ? 'Persona Física' : 'Persona Jurídica'}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <span className={`inline-flex px-2 py-1 text-xs font-medium rounded-full ${getEstadoBadge(contribuyente.estatus)}`}>
                        {contribuyente.estatus}
                      </span>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap">
                      <div className="text-sm text-gray-900 font-medium">
                        {formatCurrency(contribuyente.totalITBIS || 0)}
                      </div>
                    </td>
                    <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                      <Link
                        to={`/contribuyentes/${contribuyente.rncCedula}`}
                        className="text-dgii-600 hover:text-dgii-900"
                      >
                        Ver detalles
                      </Link>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>

      {/* Modal de crear contribuyente */}
      <CreateContribuyenteModal
       isOpen={isCreateModalOpen}
       onClose={() => setIsCreateModalOpen(false)}
       onSuccess={handleCreateSuccess}
     />
   </div>
 )
}