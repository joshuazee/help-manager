<template>
  <div class="container">
    <div class="left">
      <div class="title">
        <div class="logo">
          <span class="content">部门列表</span>
        </div>
      </div>
      <div ref="treeContainer" class="content">
        <Tree :data="deps" :render="treeNodeRender" ref="depTree"></Tree>
      </div>
    </div>
    <div class="right">
      <div class="title">
        <div class="logo">
          <span class="content">用户列表</span>
        </div>
        <div class="search">
          <Row :gutter="10">
            <i-col span="16">
              <i-input placeholder="输入姓名进行查询" v-model="query.name"></i-input>
            </i-col>
            <i-col span="8">
              <Button type="primary" @click="refreshUser">查询</Button>
            </i-col>
          </Row>
        </div>
        <div class="addUser">
          <Button type="primary" @click="openUserOperModal('add')">添加用户</Button>
        </div>
      </div>
      <div class="content" ref="content">
        <Table :columns="table.columns" :data="table.data" :height="table.height">
          <template slot="action" slot-scope="{ row }">
            <Button type="info" size="small" ghost @click="unbindMobile(row)" v-if="row.roles[0].level === level1">解绑</Button>
            <Button type="success" size="small" ghost @click="translateUser(row)" v-if="currentLevel > level3 && row.roles[0].level < currentLevel">调动</Button>
            <Button type="primary" size="small" ghost @click="openUserOperModal('edit_password',row.id)" v-if="row.roles[0].level >= level3">修改密码</Button>
            <Button type="warning" size="small" ghost @click="openUserOperModal('edit', row)">编辑</Button>
            <Button type="error" size="small" ghost @click="deleteUser(row)">删除</Button>
          </template>
        </Table>
        <Page :total="page.total" prev-text="上一页" next-text="下一页" :page-size="page.size" :current="page.current" :page-size-opts="page.sizeOptions"
          show-sizer show-elevator show-total @on-page-size-change="pageSizeChange" @on-change="pageChange"></Page>
      </div>
    </div>
    <dep-oper-modal :initObj="depOperModalData" v-on:refreshList="refreshDep"></dep-oper-modal>
    <dep-selector-modal :initObj="depSelectorModalData" v-on:refreshList="refreshUser"></dep-selector-modal>
    <user-oper-modal :initObj="userOperModalData" v-on:refreshList="refreshUser"></user-oper-modal>
  </div>
</template>

<script>
import depOperModal from './dep-oper-modal'
import depSelectorModal from './dep-selector-modal'
import userOperModal from './user-oper-modal'
import utilities from '../components/utilities'
export default {
  components: {
    'dep-oper-modal': depOperModal,
    'dep-selector-modal': depSelectorModal,
    'user-oper-modal': userOperModal
  },
  data () {
    return {
      lastDep: {},
      level1: utilities.service_config.level1,
      level3: utilities.service_config.level3,
      currentLevel: utilities.user_data.roles[0].level,
      table: {
        height: 480,
        columns: [{
          title: '姓名',
          key: 'name',
          align: 'center',
          width: 120
        }, {
          title: '身份证号',
          key: 'identity',
          align: 'center',
          width: 180
        }, {
          title: '手机号',
          key: 'mobile',
          align: 'center',
          width: 150
        }, {
          title: '年龄',
          key: 'age',
          align: 'center',
          width: 70
        }, {
          title: '性别',
          key: 'sex',
          align: 'center',
          width: 70
        }, {
          title: '角色',
          key: 'role_name',
          align: 'center',
          width: 160
        }, {
          title: '所属社工',
          key: 'parent_name',
          align: 'center',
          width: 100
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
      query: {
        name: ''
      },
      depOperModalData: {
        show: false,
        title: '',
        loading: true,
        closable: false,
        maskClosable: false,
        success: '',
        uri: '',
        data: {}
      },
      depSelectorModalData: {
        show: false,
        title: '',
        loading: true,
        closable: false,
        maskClosable: false,
        success: '',
        uri: '',
        data: {
          deps: [],
          user: {
            roles: [{
              level: 0
            }]
          }
        }
      },
      userOperModalData: {
        show: false,
        title: '',
        loading: true,
        closable: false,
        maskClosable: false,
        success: '',
        uri: '',
        roleEditable: false,
        editPassword: false,
        data: {
          user: {
            name: '',
            role: 0,
            roleLevel: 0,
            admin: 0,
            mobile: '',
            identity: '',
            sex: '',
            age: ''
          },
          password: {
            old: '',
            new: '',
            confirm: ''
          },
          roles: [],
          level2Users: []
        }
      },
      deps: []
    }
  },
  mounted () {
    this.table.height = this.$refs.content.offsetHeight - 34
    this.$refs.treeContainer.style.height = this.$refs.content.offsetHeight + 'px'
    this.refreshDep()
  },
  methods: {
    refreshDep () {
      let p = {
        parent: utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].id : 0
      }
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_department', p).then((response) => {
        if (response.success === true) {
          this.updateTreeNode(response.data)
        } else {
          throw response.message
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
      this.deps = childrenArr[0]
    },
    treeNodeRender (h, { root, node, data }) {
      return h('span', {
        style: {
          display: 'inline-block',
          width: '95%'
        },
        on: {
          click: () => {
            if (data.id !== this.lastDep.id) {
              if (this.lastDep.selected === true) {
                this.$set(this.lastDep, 'selected', false)
              }
              this.lastDep = data
              this.refreshRoleList()
              this.refreshUser()
              this.$set(data, 'selected', true)
            }
          }
        }
      }, [
        h('span', {
          class: {
            'tree_node': true,
            'tree_node_selected': data.selected
          }
        }, data.title),
        h('span', {
          style: {
            display: 'inline-block',
            float: 'right',
            marginRight: '24px'
          }
        }, [
          data.level > 1 && h('Button', {
            props: {
              type: 'success',
              icon: 'ios-add',
              size: 'small',
              ghost: true
            },
            attrs: {
              title: '添加下级部门'
            },
            on: {
              click: () => {
                this.openDepOperModal('add', { parentName: data.title, parent: data.id, level: data.level - 1 })
              }
            }
          }),
          utilities.user_data.roles[0].level > utilities.service_config.level3 && h('Button', {
            props: {
              type: 'warning',
              icon: 'ios-build-outline',
              size: 'small',
              ghost: true
            },
            attrs: {
              title: '编辑本部门'
            },
            on: {
              click: () => {
                // console.log(this.$refs.depTree.flatState[node.parent]) // 借用iview框架内部机制获取父节点
                data.name = data.title
                if (node.parent) {
                  data.parentName = this.$refs.depTree.flatState[node.parent].node.title
                  data.parent = this.$refs.depTree.flatState[node.parent].node.id
                } else {
                  data.parent = 0
                }
                this.openDepOperModal('edit', data)
              }
            }
          }),
          utilities.user_data.roles[0].level > utilities.service_config.level3 && h('Button', {
            props: {
              type: 'error',
              icon: 'ios-remove',
              size: 'small',
              ghost: true
            },
            attrs: {
              title: '删除本部门'
            },
            on: {
              click: () => {
                this.deleteDep(data)
              }
            }
          })
        ])
      ])
    },
    refreshUser () {
      let option = {}
      Object.assign(option, {
        dep: this.lastDep.id,
        level: ''
      }, this.query)
      option.level = 'ltoet#' + utilities.user_data.roles[0].level
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_user', option).then((response) => {
        if (response.success === true) {
          this.table.total = response.data
          this.page.total = response.data.length
          this.userOperModalData.data.level2Users = []
          for (let i = 0; i < response.data.length; i++) {
            if (response.data[i].roles[0].level === utilities.service_config.level2) {
              this.userOperModalData.data.level2Users.push(response.data[i])
            }
          }
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
    refreshRoleList () {
      let dep = 0
      if (this.lastDep.id !== 0) {
        dep = this.lastDep.id
      } else {
        dep = utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].id : 0
      }
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_role2', {
        level: 'lt#' + utilities.user_data.roles[0].level,
        dep: dep
      }).then((response) => {
        if (response.success === true) {
          this.userOperModalData.data.roles = response.data
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
    openUserOperModal (type, user) {
      if (type === 'add') {
        if (!this.lastDep.id) {
          this.$Message.warning('请在左侧选择一个部门后进行该操作')
          return
        }
        this.userOperModalData.data.user.dep = this.lastDep.id
        this.userOperModalData.roleEditable = true
        this.userOperModalData.title = '添加新用户'
        this.userOperModalData.uri = utilities.config.service_base + utilities.config.permission_url + '/add_user'
        this.userOperModalData.success = '新用户已添加'
      } else if (type === 'edit') {
        this.userOperModalData.title = '编辑用户信息'
        this.userOperModalData.uri = utilities.config.service_base + utilities.config.permission_url + '/update_user'
        this.userOperModalData.success = '用户信息已更新'
        this.userOperModalData.data.user = user
        this.userOperModalData.data.user.roleLevel = user.roles[0].level
        this.userOperModalData.data.user.admin = user.parent
        this.userOperModalData.data.user.role = user.roles[0].id
        this.userOperModalData.data.user.dep = user.deps[0].id
      } else if (type === 'edit_password') {
        this.userOperModalData.editPassword = true
        this.userOperModalData.title = '修改密码'
        this.userOperModalData.uri = utilities.config.service_base + utilities.config.permission_url + '/update_user'
        this.userOperModalData.success = '用户密码已更新'
        this.userOperModalData.data.password.id = user
      }
      this.userOperModalData.show = true
    },
    unbindMobile (data) {
      this.$Modal.confirm({
        title: '请确认',
        content: '<p>是否解除 <span style="font-weight: bolder;">' + data.name + '</span> 账号与其手机的绑定?</p><p>(本功能只用于一次解除，下次解绑请重复该操作。另:若同一人员已多次请求解绑，请慎重考虑)</p>',
        onOk: () => {
          // this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/clear_user_pin', {
          //   id: data.id
          // }).then((response) => {
          //   if (response.data === true) {
          //     this.$Message.success('解绑成功')
          //   } else {
          //     throw new Error('后台错误，请检查')
          //   }
          // })
          this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/clear_user_pin', {
            id: data.id
          }).then((response) => {
            if (response.success === true && response.data === true) {
              this.$Message.success('解绑成功')
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
    deleteUser (data) {
      this.$Modal.confirm({
        title: '请确认',
        content: '是否确认删除 ' + data.name + ' ?',
        onOk: () => {
          this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/delete_user', {
            id: data.id
          }).then((response) => {
            if (response.success === true && response.data === true) {
              this.$Message.success('成功删除该用户')
              this.refreshUser()
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
    translateUser (data) {
      this.depSelectorModalData.data.deps = JSON.parse(JSON.stringify(this.deps))
      this.depSelectorModalData.data.user = data
      this.depSelectorModalData.uri = utilities.config.service_base + utilities.config.permission_url + '/update_user_dep'
      this.depSelectorModalData.title = '人员调动'
      this.depSelectorModalData.success = '人员调动成功'
      this.depSelectorModalData.show = true
    },
    openDepOperModal (type, data) {
      this.depOperModalData.show = true
      if (type === 'add') {
        this.depOperModalData.title = '添加子部门'
        this.depOperModalData.uri = utilities.config.service_base + utilities.config.permission_url + '/add_department'
        this.depOperModalData.success = '添加部门成功'
      } else if (type === 'edit') {
        this.depOperModalData.title = '编辑部门信息'
        this.depOperModalData.uri = utilities.config.service_base + utilities.config.permission_url + '/update_department'
        this.depOperModalData.success = '编辑部门成功'
      }
      if (data) {
        this.depOperModalData.data = data
      }
    },
    deleteDep (data) {
      this.$Modal.confirm({
        title: '请确认',
        content: '是否确认删除 ' + data.title + ' ?',
        onOk: () => {
          this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/delete_department', {
            id: data.id
          }).then((response) => {
            if (response.success === true && response.data === true) {
              this.$Message.success('成功删除该部门')
              this.refreshDep()
            } else {
              this.$Message.info(response.message)
              // console.error(response.message)
            }
          }).catch((error) => {
            this.$Message.info('后台更新中，请稍后重试')
            console.error(error)
          })
        }
      })
    }
  }
}
</script>

<style lang="less" scoped>
@titleHeight: 50px;
.container{
  padding: 20px;
  .left{
    float: left;
    width: 25%;
    height: 100%;
    border: 1px solid rgba(185, 185, 185, 0.8);
    border-radius: 10px;
    .title{
      height: @titleHeight;
      line-height: @titleHeight;
      .logo{
        float: left;
        height: @titleHeight;
        line-height: @titleHeight;
        padding-left: 20px;
        .content{
          font-size: 24px;
          font-weight: bolder;
        }
      }
    }
    .content{
      overflow: auto;
    }
    .content::-webkit-scrollbar {
      width: 2px;
    }
    .content::-webkit-scrollbar-thumb {
      border-radius: 10px;
      background: rgb(0, 238, 255);
    }
    .content::-webkit-scrollbar-track {
      border-radius: 0;
      background: rgb(255, 255, 255);
    }
  }
  .right{
    float: right;
    width: 70%;
    height: 100%;
    border: 1px solid rgba(185, 185, 185, 0.8);
    border-radius: 10px;
    .title{
      height: @titleHeight;
      line-height: @titleHeight;
      .logo{
        float: left;
        height: @titleHeight;
        line-height: @titleHeight;
        padding-left: 20px;
        .content{
          font-size: 24px;
          font-weight: bolder;
        }
      }
      .search{
        float: left;
        margin-left: 20px;
      }
      .addUser{
        float: right;
        margin-right: 10px;
      }
    }
    .content{
      height: calc(100% - @titleHeight);
    }
  }
}
/deep/.tree_node{
  cursor: pointer;
  &:hover{
    background: #eaf4fe;
  }
}
/deep/.tree_node_selected{
  background: #d5e8fc;
  &:hover{
    background: #d5e8fc;
  }
}
</style>
