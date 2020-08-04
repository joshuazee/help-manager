<template>
  <Modal v-model="initObj.show" width="50" :title="initObj.title" class="preview-modal">
    <div class="title">
      <p>{{ initObj.data.title }}</p>
    </div>
    <div class="copyright">
      <p>{{ initObj.data.publish_time + ' ' + initObj.data.publish_user }}</p>
    </div>
    <div class="content" v-html="initObj.data.html"></div>
  </Modal>
</template>

<script>
export default {
  props: {
    initObj: Object
  }
}
</script>

<style lang="less" scoped>
.preview-modal{
  .title{
    font-size:24px;
    font-weight:bold;
    text-align: center;
  }
  .copyright{
    text-align: center;
    color: #aaa
  }
}
</style>
