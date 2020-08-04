<template>
  <Modal v-model="initObj.show" :title="initObj.title" ref="modal" width="360"
  :loading="initObj.loading" :mask-closable="initObj.maskClosable" @on-visible-change="visibleChange"
  :closable="initObj.closable" @on-cancel="clear"
  @on-ok="confirm">
    <Form :model="initObj.data.user" :label-width="form.labelWidth" v-if="initObj.editPassword === false">
      <FormItem label="角色">
        <Select v-model="initObj.data.user.role" :disabled="initObj.roleEditable !== true" clearable ref="roleSelect">
          <Option v-for="(role, idx) in initObj.data.roles" :value="role.id" :key="idx" @click.native="roleChange(role)">{{ role.name }}</Option>
        </Select>
      </FormItem>
      <FormItem label="所属社工" id="gridMember" style="display:none">
        <Select v-model="initObj.data.user.admin">
          <Option v-for="(admin, idx) in initObj.data.level2Users" :value="admin.id" :key="idx">{{ admin.name }}</Option>
        </Select>
      </FormItem>
      <FormItem label="姓名">
        <i-input v-model="initObj.data.user.name"></i-input>
      </FormItem>
      <FormItem label="手机号">
        <i-input v-model="initObj.data.user.mobile"></i-input>
      </FormItem>
      <FormItem label="身份证号">
        <i-input v-model="initObj.data.user.identity"></i-input>
      </FormItem>
      <FormItem label="性别">
        <Select v-model="initObj.data.user.sex">
          <Option value="男">男</Option>
          <Option value="女">女</Option>
        </Select>
      </FormItem>
      <FormItem label="年龄">
        <i-input v-model="initObj.data.user.age"></i-input>
      </FormItem>
    </Form>
    <Form :model="initObj.data.password" :label-width="form.labelWidth" v-if="initObj.editPassword === true">
      <FormItem label="原密码">
        <i-input v-model="initObj.data.password.old" type="password" password></i-input>
      </FormItem>
      <FormItem label="新密码">
        <i-input v-model="initObj.data.password.new" type="password" password></i-input>
      </FormItem>
      <FormItem label="确认密码">
        <i-input v-model="initObj.data.password.confirm" type="password" password></i-input>
      </FormItem>
    </Form>
  </Modal>
</template>

<script>
import utilities from '../components/utilities'
export default {
  props: {
    initObj: Object
  },
  data () {
    return {
      level1: utilities.service_config.level1,
      form: {
        labelWidth: 80
      }
    }
  },
  methods: {
    roleChange (role) {
      this.initObj.data.user.roleLevel = role.level
      if (this.initObj.data.user.roleLevel === this.level1) {
        document.getElementById('gridMember').style.display = ''
      } else {
        document.getElementById('gridMember').style.display = 'none'
      }
    },
    confirm () {
      if (this.validate() === true) {
        const data = this.initObj.editPassword === true ? this.initObj.data.password : this.initObj.data.user
        this.$http.post(this.initObj.uri, data).then((response) => {
          if (response.success === true) {
            this.$Message.success(this.initObj.success)
            this.$emit('refreshList')
            this.initObj.show = false
            this.clear()
          } else {
            if (this.initObj.editPassword === true) {
              this.$Message.info(response.message)
            } else {
              // this.$Message.info('后台更新中，请稍后重试')
              // console.error(response.message)
              this.$Message.info(response.message)
            }
            this.$refs.modal.buttonLoading = false
          }
        }).catch((error) => {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(error)
          this.$refs.modal.buttonLoading = false
        })
      } else {
        this.$refs.modal.buttonLoading = false
      }
    },
    validate () {
      if (this.initObj.editPassword === true) {
        if (!this.initObj.data.password.old) {
          this.$Message.warning('请输入原密码')
          return false
        }
        if (!this.initObj.data.password.new) {
          this.$Message.warning('请输入新密码')
          return false
        }
        if (!this.initObj.data.password.confirm) {
          this.$Message.warning('请再次输入新密码')
          return false
        }
        if (this.initObj.data.password.confirm !== this.initObj.data.password.new) {
          this.$Message.warning('两次输入的密码不一致，请重试')
          return false
        }
        this.initObj.data.password.password = this.initObj.data.password.old + '#' + this.initObj.data.password.new
      } else {
        if (!this.initObj.data.user.name) {
          this.$Message.warning('请输入用户姓名')
          return false
        }
        if (!this.initObj.data.user.mobile) {
          this.$Message.warning('请输入用户手机号')
          return false
        }
        if (!this.initObj.data.user.identity) {
          this.$Message.warning('请输入用户身份证号')
          return false
        } else if (this.initObj.data.user.identity.length !== 18) {
          this.$Message.warning('请输入正确的身份证号')
          return false
        }
        if (!this.initObj.data.user.sex) {
          this.$Message.warning('请输入用户性别')
          return false
        }
        if (!this.initObj.data.user.age) {
          this.$Message.warning('请输入用户年龄')
          return false
        }
        if (!this.initObj.data.user.role) {
          this.$Message.warning('请选择用户角色')
          return false
        }
        if (this.initObj.data.user.roleLevel === this.level1 && !this.initObj.data.user.admin) {
          this.$Message.warning('请选择所属社工')
          return false
        }
      }
      return true
    },
    visibleChange (visible) {
      if (visible) {
        if (this.initObj.data.user.roleLevel === this.level1) {
          document.getElementById('gridMember').style.display = ''
        } else {
          document.getElementById('gridMember').style.display = 'none'
        }
      }
    },
    clear () {
      this.initObj.roleEditable = false
      this.initObj.editPassword = false
      this.initObj.data.user = {}
      this.$refs.roleSelect.clearSingleSelect()
      document.getElementById('gridMember').style.display = 'none'
    }
  }
}
</script>
