import { apiClient } from './authService'

export const contribuyentesService = {
  getAll: async () => {
    try {
      const response = await apiClient.get('/contribuyentes')
      return response.data
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al obtener contribuyentes')
    }
  },

  getByRncCedula: async (rncCedula) => {
    try {
      const response = await apiClient.get(`/contribuyentes/${rncCedula}`)
      return response.data
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al obtener contribuyente')
    }
  },

  create: async (contribuyenteData) => {
    try {
      const response = await apiClient.post('/contribuyentes', contribuyenteData)
      return response.data
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al crear contribuyente')
    }
  }
}

export const comprobantesFiscalesService = {
  getAll: async () => {
    try {
      const response = await apiClient.get('/comprobantesfiscales')
      return response.data
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al obtener comprobantes fiscales')
    }
  },

  getByContribuyente: async (rncCedula) => {
    try {
      const response = await apiClient.get(`/comprobantesfiscales/contribuyente/${rncCedula}`)
      return response.data
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al obtener comprobantes del contribuyente')
    }
  },

  getTotalITBIS: async (rncCedula) => {
    try {
      const response = await apiClient.get(`/comprobantesfiscales/totales/${rncCedula}`)
      return response.data
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al obtener total ITBIS')
    }
  },

  create: async (comprobanteData) => {
    try {
      const response = await apiClient.post('/comprobantesfiscales', comprobanteData)
      return response.data
    } catch (error) {
      throw new Error(error.response?.data?.message || 'Error al crear comprobante fiscal')
    }
  }
}