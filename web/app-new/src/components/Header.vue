<template>
  <div class="container">
      <div class="left" @click="toIndex">
        <img src="../assets/logo.png" class="logo"/>
        <img src="/web/images/title.png" class="title"/>
      </div>
      <div class="quit">
        <Button type="error" class="btn" @click="quit()">退出登录</Button>
      </div>
      <div class="personal">
        <div class="via">
          <img :src="user_via"/>
        </div>
        <div class="userinfo">
          <div class="name">
            <span>{{user_name}}</span>
          </div>
          <div class="other">
            <div class="dep">
              <span>{{user_dep}}</span>
            </div>
            <div class="role">
              <span>{{user_role}}</span>
            </div>
          </div>
        </div>
      </div>
  </div>
</template>

<script>
import utilities from './utilities'

export default {
  data () {
    return {
      user_name: utilities.user_data.username,
      user_dep: utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].name : '',
      user_role: utilities.user_data.roles[0].name,
      user_via: utilities.config.buffer_url + utilities.user_data.via
    }
  },
  methods: {
    quit () {
      localStorage.setItem('help_manager_user', '')
      window.location.reload()
    },
    toIndex () {
      this.$router.push('/a/index')
    }
  }
}
</script>

<style scoped lang="less">
.container{
  background: linear-gradient(to right, #34BDFF , white);
  height: 100%;
  .left{
    height: 100%;
    float: left;
    cursor: pointer;
    .logo{
      height: 100%;
    }
  }
  div.quit{
    float: right;
    width: 105px;
    height: 100%;
    .btn{
      height: 100%;
      font-size: 18px;
    }
  }
  div.personal{
    float: right;
    overflow: hidden;

    div.via{
      float: left;
      height: 100%;
      width: 64px;
      border-radius: 50%;
      cursor: pointer;
    }
    div.userinfo{
      float: left;
      margin-left: 10px;
      margin-right: 30px;
      font-size: 16px;
      div.name{
        text-align: center;
        height: 32px;
        line-height: 32px;
      }
      div.other{
        overflow: hidden;
        height: 32px;
        line-height: 32px;
        div.dep{
          font-weight: bold;
          float: left;
        }
        div.role{
          font-weight: bold;
          float: left;
          margin-left: 5px;
        }
      }
    }
  }
}
</style>
