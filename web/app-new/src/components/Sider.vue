<template>
  <Menu width="auto">
    <MenuGroup v-for="menu in menus" :key="menu.name" :title="menu.name">
      <MenuItem v-for="sub in menu.children" :key="sub.name" :name="sub.name" :to="sub.path">
        <img v-if="sub.img" :src="img_base + sub.img" style="vertical-align: middle;margin-right: 10px;"/>
        {{ sub.name }}
      </MenuItem>
    </MenuGroup>
    </Menu>
</template>

<script>
import utilities from './utilities'
export default {
  props: {
    menus: Array
  },
  components: {
    // MenuTree: MenuTree
  },
  data () {
    return {
      menu_index: 0,
      img_base: utilities.config.buffer_url + '/upload'
    }
  },
  methods: {
  }
}
</script>

<style lang="less" scoped>
/deep/ .ivu-menu-item-group-title{
  font-weight: bold;
  font-size: 16px;
}
</style>
