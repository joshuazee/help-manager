<template>
  <div class="container" @keyup.enter="login()">
    <div class="login_form">
      <div class="title">
        <img :src="titleImage" />
        <span>{{ system_title_login_inside }}</span>
      </div>
      <Form class="_form" ref="loginForm" :model="loginForm" :rules="loginRules">
        <FormItem prop="account" class="row">
          <i-input type="text" v-model="loginForm.account" placeholder="请输入手机号/账号">
            <Icon type="ios-person-outline" slot="prepend"></Icon>
          </i-input>
        </FormItem>
        <FormItem prop="password" class="row">
          <i-input type="password" v-model="loginForm.password" placeholder="请输入密码">
            <Icon type="ios-lock-outline" slot="prepend"></Icon>
          </i-input>
        </FormItem>
        <FormItem class="row">
          <Button type="primary" @click="login()"  class="confirm_button">登录</Button>
        </FormItem>
      </Form>
    </div>
    <div class="title1">
      <span>{{ system_title_login_up }}</span>
    </div>
    <div class="foot_right">
      <div>
        <span>{{ system_right_first_row }}</span>
      </div>
      <div>
        <span>{{ system_right_second_row }}</span>
      </div>
    </div>
    <div class="title2">
      <div>
        <img class="img1" :src="title2Image" />
        <span>{{ system_title_center }}</span>
      </div>
      <div>
        <img :src="loginTitleImage1" />
      </div>
    </div>
  </div>
</template>

<script>
import LoginBase from '../components/LoginBase'
import utilities from '../components/utilities'

export default {
  extends: LoginBase,
  data () {
    return {
      titleImage: require('@/assets/login/login_logo.png'),
      title2Image: require('@/assets/login/login_logo2.png'),
      loginTitleImage1: require('@/assets/login/login_title1.png'),
      system_title: utilities.config.system_title,
      system_title_center: utilities.config.system_title_center,
      system_title_login_up: utilities.config.system_title_login_up,
      system_title_login_inside: utilities.config.system_title_login_inside,
      system_right_first_row: utilities.config.system_right_first_row,
      system_right_second_row: utilities.config.system_right_second_row
    }
  }
}
</script>

<style lang="less" scoped>
.container{
  height: 100%;
  background: url('../assets/login/login_background.png') no-repeat;

  .login_form{
    width: 20%;
    height: 40%;
    position: absolute;
    top: 25%;
    right: 10%;
    background: rgba(216, 216, 216, 0.1);

    .title{
      text-align: center;
      font-size: 30px;
      margin-top: 40px;
      line-height: 58px;
      height: 58px;
      color: #fff;
      margin-left: 55px;

      img{
        float: left;
      }
      span{
        margin-left: 10px;
        float: left;
      }
    }
    ._form{
      margin-top: 40px;

      .row{
        width: 80%;
        margin-left: 10%;
      }

      .confirm_button{
        width: 100%;
      }
    }
  }

  .title1{
    text-align: center;
    font-size: 30px;
    color: #fff;
    position: absolute;
    width: 20%;
    top: 18%;
    right: 10%;
  }

  .title2{
    position: absolute;
    font-size: 30px;
    color: #fff;
    top: 18%;
    left: 40%;
    text-align: center;
    div{
      .img1{
        float: left;
        margin-left: 25px;
      }
      span{
        float: left;
        margin-left: 16px;
        line-height: 51px;
      }
    }
  }

  .foot_right{
    position: absolute;
    width: 20%;
    top: 66%;
    right: 10%;
    div{
      margin: 10px 0;
      text-align: center;
      color: #fff;
    }
  }
}
</style>
