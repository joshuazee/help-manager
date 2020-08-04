<template>
  <Modal v-model="initObj.show" :title="initObj.title" ref="modal"
  :loading="initObj.loading" :mask-closable="initObj.maskClosable"
  :closable="initObj.closable"
  @on-ok="confirm" @on-on-cancel="clear">
    <Form :model="form" :label-width="form.labelWidth">
      <FormItem label="角色名">
        <i-input v-model="initObj.data.name"></i-input>
      </FormItem>
      <FormItem label="角色级别">
        <i-input v-model="initObj.data.level"></i-input>
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
      if (!this.initObj.data.name) {
        this.$Message.warning('请输入角色名')
        return false
      }
      if (!this.initObj.data.level) {
        this.$Message.warning('请输入角色级别')
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

<style>

</style>
