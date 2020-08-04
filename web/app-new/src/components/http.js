import axios from 'axios'
import utilities from './utilities'

const http = axios.create({
  responseType: 'json',
  withCredentials: false,
  headers: {
    'Content-Type': 'application/x-www-form-urlencoded;charset=utf-8'
  },
  validateStatus: function (status) {
    return status >= 200 && status < 500
  }
})

function transformRequest (data) {
  let requestData = ''
  Object.keys(data).forEach((key) => {
    if (requestData !== '') {
      requestData += '&'
    }
    if (typeof data[key] === 'object') {
      requestData += key + '=' + JSON.stringify(data[key])
    } else {
      requestData += key + '=' + data[key]
    }
  })
  return requestData
}

http.interceptors.request.use(
  config => {
    if (config.headers['Content-Type'].indexOf('application/x-www-form-urlencoded') >= 0) {
      config.data = config.data || {}
      if (utilities.user_data) {
        config.data.operation_user = utilities.user_data.userid
        config.data.token = utilities.user_data.token
      }
      config.data = transformRequest(config.data)
    }
    return config
  },
  error => {
    return error
  }
)

http.interceptors.response.use(
  res => {
    if (res.status === 200) {
      return res.data
    } else {
      return Promise.reject(res.statusText)
    }
  },
  error => {
    console.error('连接失败')
    return Promise.reject(error)
  }
)

export default {
  install: function (Vue) {
    Object.defineProperty(Vue.prototype, '$http', { value: http })
  }
}
