import { defineStore } from 'pinia'
import { ref } from 'vue'

export const useSupportStore = defineStore('support', () => {
  const medications = ref([])
  const procedures = ref([])
  const diagnosticAids = ref([])
  const loading = ref(false)
  
  const setMedications = (data) => {
    medications.value = data
  }
  
  const addMedication = (medication) => {
    medications.value.push(medication)
  }
  
  const updateMedicationInList = (id, updatedMedication) => {
    const index = medications.value.findIndex(m => m.id === id)
    if (index !== -1) {
      medications.value[index] = updatedMedication
    }
  }
  
  const setProcedures = (data) => {
    procedures.value = data
  }
  
  const addProcedure = (procedure) => {
    procedures.value.push(procedure)
  }
  
  const updateProcedureInList = (id, updatedProcedure) => {
    const index = procedures.value.findIndex(p => p.id === id)
    if (index !== -1) {
      procedures.value[index] = updatedProcedure
    }
  }
  
  const setDiagnosticAids = (data) => {
    diagnosticAids.value = data
  }
  
  const addDiagnosticAid = (diagnosticAid) => {
    diagnosticAids.value.push(diagnosticAid)
  }
  
  const updateDiagnosticAidInList = (id, updatedDiagnosticAid) => {
    const index = diagnosticAids.value.findIndex(d => d.id === id)
    if (index !== -1) {
      diagnosticAids.value[index] = updatedDiagnosticAid
    }
  }
  
  const setLoading = (value) => {
    loading.value = value
  }
  
  return {
    medications,
    procedures,
    diagnosticAids,
    loading,
    setMedications,
    addMedication,
    updateMedicationInList,
    setProcedures,
    addProcedure,
    updateProcedureInList,
    setDiagnosticAids,
    addDiagnosticAid,
    updateDiagnosticAidInList,
    setLoading
  }
})

