<template>
  <Layout class="layout">
    <Header>
      <sh></sh>
    </Header>
    <Layout class="center">
      <Sider class="sider">
        <ss :menus="menus"></ss>
      </Sider>
      <Layout>
        <Content style="padding: 15px;">
          <router-view style="background: #fff;border-radius: 5px;height: 100%;"></router-view>
        </Content>
      </Layout>
    </Layout>
  </Layout>
</template>

<script>
import Header from '../components/Header'
import Sider from '../components/Sider'
import utilities from '../components/utilities'

export default {
  components: {
    sh: Header,
    ss: Sider
  },
  data () {
    return {
      menus: utilities.user_data.menus
    }
  },
  beforeMount () {
  },
  methods: {
  }
}
</script>

<style lang="less" scoped>
@headerHeight: 64px;
@menuWidth: 240px;
.layout{
  height: 100%;
  .center{
    height: calc(100% - @headerHeight);
  }
  .sider{
    height: 100%;
    overflow:auto;
  }
  .sider::-webkit-scrollbar {
    width: 2px;
  }
  .sider::-webkit-scrollbar-thumb {
    border-radius: 10px;
    background: rgb(0, 238, 255);
  }
  .sider::-webkit-scrollbar-track {
    border-radius: 0;
    background: rgba(204, 204, 204, 0.8);
  }
}

/deep/ .ivu-layout-header{
  background: rgba(255,255,255,0);
  padding: 0;
}
/deep/ .ivu-layout-sider{
  background: #fff;
}
</style>
