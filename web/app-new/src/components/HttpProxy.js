export default {
  post (url, data, config) {
    return new Promise((resolve, reject) => {
      this.http.post(url, data, config).then((response) => {
        if (response.success === true) {
          try {
            resolve(response)
          } catch (ex) {
            reject(ex)
          }
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    })
  }
}
