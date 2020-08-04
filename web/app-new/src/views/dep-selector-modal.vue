<template>
  <Modal v-model="initObj.show" title="部门调动" ref="modal" width="420"
  :loading="initObj.loading" :mask-closable="initObj.maskClosable"
  :closable="initObj.closable" :scrollable="true"
  @on-ok="confirm" @on-cancel="clear">
    <Tree :data="initObj.data.deps" ref="depTree" @on-select-change="depSelectChange" style="height: 500px; overflow: auto;" :check-strictly="initObj.data.checkStrictly"></Tree>
    <div v-if="initObj.data.user.roles[0].level == level1">
      <Select v-model="initObj.data.user.parent" placeholder="请选择一个社工">
        <Option v-for="admin in admins" :value="admin.id" :key="admin.name">{{ admin.name }}</Option>
      </Select>
    </div>
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
      admins: [],
      level1: utilities.service_config.level1
    }
  },
  methods: {
    confirm () {
      if (this.validate() === true) {
        this.$http.post(this.initObj.uri, this.initObj.data.user).then((response) => {
          if (response.success === true) {
            this.$Message.success(this.initObj.success)
            this.$emit('refreshList')
            this.initObj.show = false
            this.clear()
          } else {
            this.$Message.info('后台更新中，请稍后重试')
            console.error(response.message)
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
      if (!this.initObj.data.user.id) {
        this.$Message.warning('内部错误，请联系维护人员')
        return false
      }
      if (!this.initObj.data.user.dep) {
        this.$Message.warning('请选择转入部门')
        return false
      }
      if (this.initObj.data.user.roles[0].level === this.level1 && !this.initObj.data.user.parent) {
        this.$Message.warning('请选择一个新的社工')
        return false
      }
      return true
    },
    clear () {
      this.initObj.data.user = {
        roles: [{
          level: 0
        }]
      }
    },
    depSelectChange (selection, node) {
      this.initObj.data.user.dep = node.id
      this.initObj.data.user.roles[0].level === this.level1 && this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_user', {
        dep: node.id,
        level: 'eq#' + utilities.service_config.level2
      }).then((response) => {
        if (response.success === true) {
          this.admins = response.data
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    }
  }
}
</script>

<style scoped>
::-webkit-scrollbar {
  width: 2px;
}
::-webkit-scrollbar-thumb {
  border-radius: 10px;
  background: rgb(0, 238, 255);
}
::-webkit-scrollbar-track {
  border-radius: 0;
  background: rgb(255, 255, 255);
}
</style>
