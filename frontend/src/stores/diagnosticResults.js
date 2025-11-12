import { defineStore } from 'pinia'

export const useDiagnosticResultsStore = defineStore('diagnosticResults', () => {
  // Resultados por defecto según el tipo de ayuda diagnóstica
  const defaultResults = {
    'Hemograma': 'Resultados anormales: Leucocitos elevados (12,500/μL), Hemoglobina baja (9.5 g/dL), requiere seguimiento clínico.',
    'Radiografía': 'Hallazgos: Opacidad en pulmón derecho, infiltrado intersticial. Requiere evaluación adicional y seguimiento.',
    'Tomografía': 'Hallazgos: Lesión focal identificada, requiere correlación clínica y posible seguimiento con estudios adicionales.',
    'Ecografía': 'Hallazgos: Presencia de líquido libre, engrosamiento de pared. Requiere evaluación clínica complementaria.',
    'Electrocardiograma': 'Resultados: Arritmia detectada, alteraciones en el segmento ST. Requiere evaluación cardiológica.',
    'Análisis de Sangre': 'Resultados anormales: Glucosa elevada (180 mg/dL), colesterol alto (250 mg/dL). Requiere seguimiento y tratamiento.',
    'Análisis de Orina': 'Hallazgos: Presencia de proteínas y leucocitos. Requiere evaluación adicional y posible tratamiento.',
    'Resonancia Magnética': 'Hallazgos: Lesión identificada, requiere correlación clínica y seguimiento especializado.',
    'Endoscopia': 'Hallazgos: Inflamación moderada, presencia de lesiones. Requiere evaluación y tratamiento específico.',
    'Biopsia': 'Resultados: Presencia de células anormales detectadas. Requiere evaluación histopatológica completa y seguimiento.'
  }

  const getDefaultResult = (diagnosticAidName) => {
    if (!diagnosticAidName) {
      return 'Resultados pendientes de revisión. Requiere evaluación clínica y seguimiento según corresponda.'
    }

    // Buscar resultado exacto por nombre
    const exactMatch = defaultResults[diagnosticAidName]
    if (exactMatch) {
      return exactMatch
    }

    // Buscar por coincidencia parcial (case-insensitive)
    const normalizedName = diagnosticAidName.toLowerCase()
    for (const [key, value] of Object.entries(defaultResults)) {
      if (key.toLowerCase().includes(normalizedName) || normalizedName.includes(key.toLowerCase())) {
        return value
      }
    }

    // Si no hay coincidencia, retornar resultado genérico
    return `Resultados de ${diagnosticAidName}: Hallazgos anormales detectados. Requiere evaluación clínica, correlación con síntomas y seguimiento según corresponda. Se recomienda crear un nuevo registro médico con diagnóstico actualizado.`
  }

  return {
    defaultResults,
    getDefaultResult
  }
})

