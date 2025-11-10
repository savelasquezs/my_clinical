import dayjs from 'dayjs'
import 'dayjs/locale/es'

dayjs.locale('es')

export function useDate() {
  const formatDate = (date, format = 'DD/MM/YYYY') => {
    if (!date) return ''
    return dayjs(date).format(format)
  }
  
  const formatDateTime = (date) => {
    if (!date) return ''
    return dayjs(date).format('DD/MM/YYYY HH:mm')
  }
  
  const parseDate = (dateString) => {
    if (!dateString) return null
    return dayjs(dateString).toDate()
  }
  
  const isValidDate = (date) => {
    if (!date) return false
    return dayjs(date).isValid()
  }
  
  const addDays = (date, days) => {
    if (!date) return null
    return dayjs(date).add(days, 'day').toDate()
  }
  
  const isBefore = (date1, date2) => {
    if (!date1 || !date2) return false
    return dayjs(date1).isBefore(dayjs(date2))
  }
  
  const isAfter = (date1, date2) => {
    if (!date1 || !date2) return false
    return dayjs(date1).isAfter(dayjs(date2))
  }
  
  const today = () => {
    return dayjs().toDate()
  }
  
  const now = () => {
    return dayjs().toDate()
  }
  
  return {
    formatDate,
    formatDateTime,
    parseDate,
    isValidDate,
    addDays,
    isBefore,
    isAfter,
    today,
    now
  }
}

