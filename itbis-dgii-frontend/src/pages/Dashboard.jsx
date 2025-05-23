import { useState, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { contribuyentesService, comprobantesFiscalesService } from '../services/apiService'
import { useAuth } from '../context/AuthContext'
import {
  UsersIcon,
  DocumentTextIcon,
  CurrencyDollarIcon,
  ChartBarIcon
} from '@heroicons/react/24/outline'
import toast from 'react-hot-toast'

export default function Dashboard() {
  const { user } = useAuth()
  const [stats, setStats] = useState({
    totalContribuyentes: 0,
    totalComprobantes: 0,
    totalITBIS: 0,
    loading: true
  })
  const [recentContribuyentes, setRecentContribuyentes] = useState([])
  const [recentComprobantes, setRecentComprobantes] = useState([])

  useEffect(() => {
    loadDashboardData()
  }, [])

  const loadDashboardData = async () => {
    try {
      setStats(prev => ({ ...prev, loading: true }))
      
      // Cargar datos en paralelo
      const [contribuyentesResponse, comprobantesResponse] = await Promise.all([
        contribuyentesService.getAll(),
        comprobantesFiscalesService.getAll()
      ])

      // Procesar contribuyentes
      const contribuyentes = contribuyentesResponse.data?.items || contribuyentesResponse.items || []
      const comprobantes = comprobantesResponse.data?.items || comprobantesResponse.items || []

      // Calcular estadísticas
      const totalITBIS = contribuyentes.reduce((sum, contrib) => sum + (contrib.totalITBIS || 0), 0)

      setStats({
        totalContribuyentes: contribuyentes.length,
        totalComprobantes: comprobantes.length,
        totalITBIS: totalITBIS,
        loading: false
      })

      // Tomar los más recientes para mostrar en el dashboard
      setRecentContribuyentes(contribuyentes.slice(0, 5))
      setRecentComprobantes(comprobantes.slice(0, 5))

    } catch (error) {
      console.error('Error loading dashboard:', error)
      toast.error('Error al cargar datos del dashboard')
      setStats(prev => ({ ...prev, loading: false }))
    }
  }

  const formatCurrency = (amount) => {
    return new Intl.NumberFormat('es-DO', {
      style: 'currency',
      currency: 'DOP'
    }).format(amount)
  }

  const statCards = [
    {
      title: 'Total Contribuyentes',
      value: stats.totalContribuyentes,
      icon: UsersIcon,
      color: 'bg-blue-500',
      link: '/contribuyentes'
    },
    {
      title: 'Comprobantes Fiscales',
      value: stats.totalComprobantes,
      icon: DocumentTextIcon,
      color: 'bg-green-500',
      link: '/comprobantes'
    },
    {
      title: 'Total ITBIS Recaudado',
      value: formatCurrency(stats.totalITBIS),
      icon: CurrencyDollarIcon,
      color: 'bg-yellow-500',
      isMonetary: true
    },
    {
      title: 'Promedio ITBIS',
      value: stats.totalContribuyentes > 0 ? formatCurrency(stats.totalITBIS / stats.totalContribuyentes) : formatCurrency(0),
      icon: ChartBarIcon,
      color: 'bg-purple-500',
      isMonetary: true
    }
  ]

  if (stats.loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-dgii-600"></div>
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div>
        <h1 className="text-2xl font-bold text-gray-900">Dashboard</h1>
        <p className="mt-1 text-sm text-gray-600">
          Bienvenido de vuelta, {user?.userName}. Aquí tienes un resumen de las estadísticas del sistema.
        </p>
      </div>

      {/* Tarjetas de estadísticas */}
      <div className="grid grid-cols-1 gap-5 sm:grid-cols-2 lg:grid-cols-4">
        {statCards.map((stat, index) => (
          <div key={index} className="card">
            <div className="p-5">
              <div className="flex items-center">
                <div className="flex-shrink-0">
                  <div className={`w-8 h-8 ${stat.color} rounded-md flex items-center justify-center`}>
                    <stat.icon className="w-5 h-5 text-white" />
                  </div>
                </div>
                <div className="ml-5 w-0 flex-1">
                  <dl>
                    <dt className="text-sm font-medium text-gray-500 truncate">
                      {stat.title}
                    </dt>
                    <dd className={`text-lg font-medium text-gray-900 ${stat.isMonetary ? 'text-base' : ''}`}>
                      {stat.value}
                    </dd>
                  </dl>
                </div>
              </div>
              {stat.link && (
                <div className="mt-3">
                  <Link
                    to={stat.link}
                    className="text-sm text-dgii-600 hover:text-dgii-500 font-medium"
                  >
                    Ver todos →
                  </Link>
                </div>
              )}
            </div>
          </div>
        ))}
      </div>

      {/* Secciones de contenido */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Contribuyentes recientes */}
        <div className="card">
          <div className="px-4 py-5 sm:p-6">
            <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
              Contribuyentes Recientes
            </h3>
            {recentContribuyentes.length > 0 ? (
              <div className="space-y-3">
                {recentContribuyentes.map((contribuyente, index) => (
                  <div key={index} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                    <div>
                      <p className="text-sm font-medium text-gray-900">
                        {contribuyente.nombre}
                      </p>
                      <p className="text-xs text-gray-500">
                        {contribuyente.rncCedula} • {contribuyente.tipo}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="text-sm font-medium text-gray-900">
                        {formatCurrency(contribuyente.totalITBIS || 0)}
                      </p>
                      <span className={`inline-flex px-2 py-1 text-xs rounded-full ${
                        contribuyente.estatus === 'Activo' 
                          ? 'bg-green-100 text-green-800' 
                          : 'bg-red-100 text-red-800'
                      }`}>
                        {contribuyente.estatus}
                      </span>
                    </div>
                  </div>
                ))}
                <div className="mt-4">
                  <Link
                    to="/contribuyentes"
                    className="btn-primary w-full text-center"
                  >
                    Ver todos los contribuyentes
                  </Link>
                </div>
              </div>
            ) : (
              <p className="text-gray-500 text-sm">No hay contribuyentes disponibles</p>
            )}
          </div>
        </div>

        {/* Comprobantes recientes */}
        <div className="card">
          <div className="px-4 py-5 sm:p-6">
            <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
              Comprobantes Fiscales Recientes
            </h3>
            {recentComprobantes.length > 0 ? (
              <div className="space-y-3">
                {recentComprobantes.map((comprobante, index) => (
                  <div key={index} className="flex items-center justify-between p-3 bg-gray-50 rounded-lg">
                    <div>
                      <p className="text-sm font-medium text-gray-900">
                        NCF: {comprobante.ncf}
                      </p>
                      <p className="text-xs text-gray-500">
                        RNC: {comprobante.rncCedula}
                      </p>
                    </div>
                    <div className="text-right">
                      <p className="text-sm font-medium text-gray-900">
                        {formatCurrency(comprobante.monto)}
                      </p>
                      <p className="text-xs text-gray-500">
                        ITBIS: {formatCurrency(comprobante.itbis18)}
                      </p>
                    </div>
                  </div>
                ))}
                <div className="mt-4">
                  <Link
                    to="/comprobantes"
                    className="btn-primary w-full text-center"
                  >
                    Ver todos los comprobantes
                  </Link>
                </div>
              </div>
            ) : (
              <p className="text-gray-500 text-sm">No hay comprobantes disponibles</p>
            )}
          </div>
        </div>
      </div>

      {/* Acciones rápidas */}
      <div className="card">
        <div className="px-4 py-5 sm:p-6">
          <h3 className="text-lg leading-6 font-medium text-gray-900 mb-4">
            Acciones Rápidas
          </h3>
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <Link
              to="/contribuyentes"
              className="p-4 border border-gray-200 rounded-lg hover:border-dgii-300 hover:shadow-md transition-all"
            >
              <UsersIcon className="h-8 w-8 text-dgii-600 mb-2" />
              <h4 className="font-medium text-gray-900">Gestionar Contribuyentes</h4>
              <p className="text-sm text-gray-500 mt-1">
                Ver, crear y administrar contribuyentes
              </p>
            </Link>
            
            <Link
              to="/comprobantes"
              className="p-4 border border-gray-200 rounded-lg hover:border-dgii-300 hover:shadow-md transition-all"
            >
              <DocumentTextIcon className="h-8 w-8 text-dgii-600 mb-2" />
              <h4 className="font-medium text-gray-900">Comprobantes Fiscales</h4>
              <p className="text-sm text-gray-500 mt-1">
                Revisar y gestionar comprobantes
              </p>
            </Link>
            
            <div className="p-4 border border-gray-200 rounded-lg">
              <ChartBarIcon className="h-8 w-8 text-dgii-600 mb-2" />
              <h4 className="font-medium text-gray-900">Reportes</h4>
              <p className="text-sm text-gray-500 mt-1">
                Generar reportes de ITBIS
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}