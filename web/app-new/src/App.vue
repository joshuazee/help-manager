<template>
  <div id="app">
    <router-view />
  </div>
</template>

<script>
import utilities from './components/utilities'

export default {
  mounted () {
    utilities.AppEventListener.on('config_complete', this.config_complete)

    // this.$router.beforeEach((to, from, next) => {
    //   if (!from.name) {
    //     if (to.name === 'login') {
    //       next()
    //     } else {
    //       this.$router.push('/login')
    //     }
    //   } else if (from.name !== to.name) {
    //     next()
    //   }
    // })
  },
  methods: {
    config_complete: function () {
      let loginPath = utilities.config.login_path
      this.$router.addRoutes([{
        path: '/login',
        component (resolve) {
          require(['./' + loginPath], resolve)
        },
        name: 'login'
      }])
      this.$router.push('/login')
    }
  }
}
</script>

<style scoped lang="less">
#app{
  height: 100%;
}
</style>
