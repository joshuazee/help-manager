<template>
  <Modal v-model="initObj.show" :title="initObj.title" ref="modal" width="450"
  :loading="initObj.loading" :mask-closable="initObj.maskClosable"
  :closable="initObj.closable" @on-cancel="clear"
  @on-ok="confirm">
  <Form :model="initObj.data" :label-width="form.labelWidth">
    <FormItem label="名称">
      <i-input v-model="initObj.data.name"></i-input>
    </FormItem>
    <FormItem label="类型">
      <Select v-model="initObj.data.type">
        <Option value="重点区域" key="重点区域">重点区域</Option>
        <Option value="管控区域" key="管控区域">管控区域</Option>
      </Select>
    </FormItem>
  </Form>
  </Modal>
</template>

<script>export default {
  props: {
    initObj: Object
  },
  data () {
    return {
      form: {
        labelWidth: 40
      }
    }
  },
  methods: {
    confirm () {
      if (this.validate() === true) {
        this.$http.post(this.initObj.uri, this.initObj.data).then((response) => {
          if (response.success === true && response.data === true) {
            this.$emit('success')
            this.initObj.show = false
            this.clear()
          } else {
            if (this.initObj.editPassword === true) {
              this.$Message.info(response.message)
            } else {
              this.$Message.info('后台更新中，请稍后重试')
              console.error(response.message)
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
      if (!this.initObj.data.name) {
        this.$Message.warning('请输入围栏名称')
        return false
      }
      if (!this.initObj.data.type) {
        this.$Message.warning('请选择围栏类型')
        return false
      }
      return true
    },
    clear () {
      this.initObj.data = {}
      this.$emit('cancel')
    }
  }
}
</script>

<style scoped lang="less">

</style>
