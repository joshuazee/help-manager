<template>
  <Modal v-model="initObj.show" title="选择部门" ref="modal" width="420"
  :loading="initObj.loading" :mask-closable="initObj.maskClosable"
  :closable="initObj.closable" :scrollable="true"
  @on-ok="confirm" @on-cancel="clear">
    <Tree :data="initObj.list" ref="depTree" @on-select-change="depSelectChange" :show-checkbox="initObj.multiple || false" style="height: 500px; overflow: auto;" :check-strictly="initObj.checkStrictly"></Tree>
  </Modal>
</template>

<script>
export default {
  props: {
    initObj: Object
  },
  data () {
    return {
      dep: {
        id: 0,
        name: ''
      }
    }
  },
  methods: {
    confirm () {
      let data
      if (this.initObj.multiple === true) {
        data = this.$refs.depTree.getCheckedNodes()
      } else {
        data = {
          id: this.dep.id,
          name: this.dep.name
        }
      }
      this.$emit('confirm', data)
    },
    clear () {},
    depSelectChange (selection, row) {
      this.dep.id = row.id
      this.dep.name = row.title
    }
  }
}
</script>

<style>

</style>
