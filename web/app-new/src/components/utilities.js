import Listener from './Listener'
import axios from 'axios'

const utilities = {
  AppEventListener: new Listener()
}

axios.get(process.env.BASE_URL + '/config.json').then((response) => {
  if (response.status === 200) {
    utilities.config = response.data
    if (!utilities.config.service_port || utilities.config.service_port === 80) {
      utilities.config.service_base = 'http://' + utilities.config.service_ip + utilities.config.service_base
    } else {
      utilities.config.service_base = 'http://' + utilities.config.service_ip + ':' + utilities.config.service_port + utilities.config.service_base
    }
    utilities.config.buffer_url = 'http://' + utilities.config.service_ip + ':' + utilities.config.service_port + '/buffer'
    axios.post(utilities.config.service_base + utilities.config.permission_url + '/service_config').then((response) => {
      if (response.status === 200) {
        utilities.service_config = response.data

        utilities.AppEventListener.dispatch('config_complete')
      } else {
        console.error(response.message)
      }
    }).catch((rest) => {
      console.error('后台配置读取错误')
    })
  } else {
    console.error(response.message)
  }
}).catch(() => {
  console.error('读取系统配置错误')
})

export default utilities
