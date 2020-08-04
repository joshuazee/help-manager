<template>
  <div class="container">
    <div class="left">
      <div class="title">
        <div style="float: left; height: 50px; line-height: 50px; padding-left: 20px;">
          <span style="font-size: 24px; font-weight: bolder;">角色列表</span>
        </div>
        <div style="float: right; padding-right: 20px;">
          <Button type="primary" @click="openRoleOperModal('add')">添加角色</Button>
        </div>
      </div>
      <div class="content" ref="content">
        <Table :columns="table.columns" :data="table.data" :height="table.height" @on-row-click="refreshRoleMenuRelationship" highlight-row>
          <template slot="index" slot-scope="{ row, index }">
            {{ index + 1 }}
          </template>
          <template slot="action" slot-scope="{ row }">
            <Button @click="openRoleDepRelationshipModal(row)" type="info" size="small" ghost>所属部门</Button>
            <Button @click="openRoleOperModal('edit', row)" type="warning" size="small" ghost>编辑</Button>
            <Button @click="deleteRole(row)" type="error" size="small" ghost>删除</Button>
          </template>
        </Table>
        <Page :total="page.total" prev-text="上一页" next-text="下一页" :page-size="page.size" :current="page.current" :page-size-opts="page.sizeOptions"
        show-sizer show-elevator show-total @on-page-size-change="pageSizeChange" @on-change="pageChange"></Page>
      </div>
    </div>
    <div class="right">
      <div class="title">
        <div style="float: left; height: 50px; line-height: 50px; padding-left: 20px;">
          <span style="font-size: 24px; font-weight: bolder;">菜单列表</span>
        </div>
        <div style="float: right; padding-right: 20px;">
          <Button type="primary" @click="updateRoleMenuRelationship">保存</Button>
        </div>
      </div>
      <div class="content">
        <Row v-for="group in menus" :key="group.id" class="row">
          <span class="group">{{ group.name }}</span>
          <Checkbox v-for="menu in group.children" :key="menu.id" border v-model="menu.check" class="menu" size="large">{{ menu.name }}</Checkbox>
        </Row>
      </div>
    </div>
    <role-open-modal :initObj="modalData" v-on:refreshList="refreshData"></role-open-modal>
    <dep-selector-modal :initObj="depSelectorModal" v-on:confirm="updateRoleDepRelationship"></dep-selector-modal>
  </div>
</template>

<script>
import utilities from '../components/utilities'
import roleOperModal from './role-oper-modal'
import depSelectorModal2 from './dep-selector-modal2'
export default {
  name: 'role',
  components: {
    'role-open-modal': roleOperModal,
    'dep-selector-modal': depSelectorModal2
  },
  data () {
    return {
      table: {
        height: 480,
        columns: [{
          title: '序号',
          slot: 'index',
          align: 'center'
        }, {
          title: '角色名',
          key: 'name',
          align: 'center'
        }, {
          title: '角色级别',
          key: 'level',
          align: 'center'
        }, {
          title: '操作',
          slot: 'action',
          align: 'center'
        }],
        data: [],
        total: []
      },
      page: {
        total: 0,
        size: 20,
        current: 1,
        sizeOptions: [20, 30, 50]
      },
      menus: [],
      selectedRole: 0,
      modalData: {
        show: false,
        title: '',
        loading: true,
        closable: false,
        maskClosable: false,
        success: '',
        uri: '',
        data: {}
      },
      depSelectorModal: {
        list: [],
        title: '选择部门',
        show: false,
        loading: false,
        closable: false,
        maskClosable: false,
        multiple: true
      }
    }
  },
  mounted () {
    this.refreshData()

    this.table.height = this.$refs.content.offsetHeight - 34
  },
  methods: {
    refreshData () {
      this.refreshRole()

      this.refreshMenu()
    },
    refreshRole () {
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_role').then((response) => {
        if (response.success === true) {
          this.table.total = response.data
          this.page.total = response.data.length
          this.refreshTable(1, this.page.size)
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    refreshMenu () {
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_menu').then((response) => {
        if (response.success === true) {
          this.menus = response.data
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    refreshTable (current, size) {
      this.table.data = []
      let start = (current - 1) * size
      let end = this.table.total.length >= current * size ? current * size : this.table.total.length
      for (let i = start; i < end; i++) {
        this.table.data.push(this.table.total[i])
      }
    },
    pageSizeChange (size) {
      this.page.size = size
      this.page.current = 1
      this.pageChange(1)
    },
    pageChange (current) {
      this.refreshTable(current, this.page.size)
    },
    refreshRoleMenuRelationship (data) {
      this.selectedRole = data.id
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_role_menu_relationship', {
        roles: data.id
      }).then((response) => {
        if (response.success === true) {
          this.updateMenuForm(response.data)
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })

      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_role_dep_relationship', {
        role: data.id
      }).then((response) => {
        if (response.success === true) {
          this.updateTreeNode(response.data)
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    updateTreeNode (deps) {
      let childrenArr = {
        0: []
      }
      for (let i = 0; i < deps.length; i++) {
        if (Object.keys(childrenArr).indexOf(deps[i].id.toString()) < 0) {
          childrenArr[deps[i].id] = []
          deps[i].children = childrenArr[deps[i].id]
        }
        if (Object.keys(childrenArr).indexOf(deps[i].pId.toString()) < 0) {
          childrenArr[0].push(deps[i])
        } else {
          childrenArr[deps[i].pId].push(deps[i])
        }
        deps[i].title = deps[i].name
        deps[i].expand = deps[i].open
        delete deps[i].name
        delete deps[i].open
        delete deps[i].pId
      }
      this.depSelectorModal.list = childrenArr[0]
    },
    updateMenuForm (rights) {
      this.menus.forEach((group) => {
        group.children.forEach((menu) => {
          if (rights.indexOf(menu.code) > -1) {
            menu.check = true
          } else if (menu.check === true) {
            menu.check = false
          }
        })
      })
    },
    getCheckedMenu () {
      let menus = []
      this.menus.forEach((group) => {
        let children = []
        group.children.forEach((menu) => {
          if (menu.check === true) {
            children.push(menu.id)
          }
        })
        if (children.length > 0) {
          menus.push(group.id)
          menus = menus.concat(children)
        }
      })
      return menus
    },
    openRoleOperModal (type, data) {
      this.modalData.show = true
      if (type === 'add') {
        this.modalData.title = '添加角色'
        this.modalData.uri = utilities.config.service_base + utilities.config.permission_url + '/add_role'
        this.modalData.success = '添加角色成功'
      } else {
        this.modalData.title = '编辑角色'
        this.modalData.uri = utilities.config.service_base + utilities.config.permission_url + '/update_role'
        this.modalData.success = '编辑角色成功'
      }
      if (data !== undefined) {
        this.modalData.data = data
      }
    },
    deleteRole (data) {
      this.$Modal.confirm({
        title: '请确认',
        content: '<p>是否删除角色 ' + data.name + '</p>',
        onOk: () => {
          this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/delete_role', {
            id: data.id
          }).then((response) => {
            if (response.success === true && response.data === true) {
              this.$Message.success('角色删除成功')
              this.refreshData()
            } else {
              this.$Message.info('后台更新中，请稍后重试')
              console.error(response.message)
            }
          }).catch((error) => {
            this.$Message.info('后台更新中，请稍后重试')
            console.error(error)
          })
        }
      })
    },
    updateRoleMenuRelationship () {
      let param = {
        roleId: this.selectedRole,
        menuId: this.getCheckedMenu().join(',')
      }
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/update_role_menu_relationship', param).then((response) => {
        if (response.success === true) {
          this.$Message.success('保存成功')
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    openRoleDepRelationshipModal (data) {
      this.currentRoleId = data.id
      this.depSelectorModal.checkStrictly = true
      this.depSelectorModal.show = true
    },
    updateRoleDepRelationship (data) {
      let p = {
        role: this.currentRoleId,
        deps: []
      }
      for (let i = 0; i < data.length; i++) {
        p.deps.push(data[i].id)
      }
      p.deps = p.deps.join(',')
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/update_role_dep_relationship', p).then((response) => {
        if (response.success === true && response.data === true) {
          this.$Message.success('修改成功')
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

<style lang="less" scoped>
.container{
  padding: 20px;
  .left{
    float: left;
    width: 48%;
    height: 100%;
    border: 1px solid rgba(185, 185, 185, 0.8);
    border-radius: 10px;
    .title{
      height: 50px;
      line-height: 50px;
    }
    .content{
      height: calc(100% - 50px);
    }
  }
  .right{
    float: right;
    width: 48%;
    height: 100%;
    border: 1px solid rgba(185, 185, 185, 0.8);
    border-radius: 10px;
    .title{
      height: 50px;
      line-height: 50px;
    }
    .content{
      height: calc(100% - 50px);
      .row{
        height: 64px;
        line-height: 64px;
        .group{
          padding: 0 20px;
          font-size: 18px;
          font-weight: bolder;
        }
        .menu{
          font-size: 12px;
        }
      }
    }
  }
}
</style>
