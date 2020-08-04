<template>
  <Modal v-model="initObj.show" :title="initObj.title" ref="modal" width="360"
  :loading="initObj.loading" :mask-closable="initObj.maskClosable"
  :closable="initObj.closable"
  @on-ok="confirm" @on-on-cancel="clear">
    <Form :model="form" :label-width="form.labelWidth" :label-position="form.labelPosition">
      <FormItem label="上级部门">
        <i-input v-model="initObj.data.parentName" disabled></i-input>
      </FormItem>
      <FormItem label="部门名称">
        <i-input v-model="initObj.data.name"></i-input>
      </FormItem>
      <FormItem label="部门别名">
        <i-input v-model="initObj.data.alias" placeholder="用于在大屏报表，首页统计等情况下展示"></i-input>
      </FormItem>
      <FormItem label="行政代码">
        <i-input v-model="initObj.data.code" placeholder="用于在大屏报表中展示地图"></i-input>
      </FormItem>
    </Form>
  </Modal>
</template>

<script>
export default {
  props: {
    initObj: Object
  },
  data () {
    return {
      form: {
        labelWidth: 80,
        labelPosition: 'left',
        data: {}
      }
    }
  },
  methods: {
    confirm () {
      if (this.validate() === true) {
        this.$http.post(this.initObj.uri, this.initObj.data).then((response) => {
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
      if (this.initObj.data.parent === null || this.initObj.data.parent === void 0) {
        this.$Message.warning('内部错误，请联系维护人员')
        return false
      }
      if (!this.initObj.data.name) {
        this.$Message.warning('请输入部门名称')
        return false
      }
      return true
    },
    clear () {
      this.initObj.data = {}
    }
  }
}
</script>
