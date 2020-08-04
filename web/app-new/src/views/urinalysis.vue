<template>
  <div class="container">
    <div class="query">
      <Form :model="queryForm" label-position="right" :label-width="80" inline>
          <FormItem label="所属社区" v-if="communityModal.list.length > 0">
             <i-input v-model="queryForm.community" placeholder="请选择" readonly @on-focus="openDepSelectorModal" width=""></i-input>
          </FormItem>
          <FormItem label="尿检人员">
            <Select v-model="queryForm.user" placeholder="请选择" filterable clearable>
              <Option v-for="item in userList" :value="item.id" :key="item.id">{{ item.name }}</Option>
            </Select>
          </FormItem>
          <FormItem label="年份">
            <Select v-model="queryForm.year" placeholder="请选择" filterable>
              <Option v-for="item in yearList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
          </FormItem>
          <FormItem label="月份">
            <Select v-model="queryForm.month" placeholder="请选择" filterable>
              <Option v-for="item in monthList" :value="item.value" :key="item.value">{{ item.label }}</Option>
            </Select>
          </FormItem>
          <FormItem :label-width="0">
            <Button type="primary" @click="refreshTotalData">查询</Button>
          </FormItem>
        </Form>
    </div>
    <div class="content">
      <p>尿检列表</p>
      <Table :columns="table.columns" :data="table.data" :height="table.height" highlight-row>
        <template slot="action" slot-scope="{ row }">
          <Button @click="showDetail(row)" type="primary" size="small" ghost>查看详情</Button>
        </template>
      </Table>
      <Page :total="page.total" prev-text="上一页" next-text="下一页" :page-size="page.size" :current="page.current"
      show-sizer show-elevator show-total @on-page-size-change="pageSizeChange" @on-change="pageChange"></Page>
    </div>
    <dep-select-modal :initObj="communityModal" v-on:confirm="confirmCommunity"></dep-select-modal>
    <Modal v-model="detail.show" :width="60">
      <div class="image">
        <img v-if="detail.data.img1 !== ''" :src="detail.data.img1" style="max-width: 100%;" title="点击图片进行下载" @click="donwloadImg(detail.data.img1)">
        <img v-if="detail.data.img2  !== ''" :src="detail.data.img2" style="max-width: 100%;" title="点击图片进行下载" @click="donwloadImg(detail.data.img2)">
        <br/>
        <span style="font-weight:bolder;">上报人姓名:</span><span>{{ detail.data.name1 }}</span>
        <br/>
        <span style="font-weight:bolder;">上报时间:</span><span>{{ detail.data.time }}</span>
        <br/>
        <span style="font-weight:bolder;">尿检结果:</span><span>{{ detail.data.result }}</span>
        <br/>
        <span style="font-weight:bolder;">审核人:</span><span>{{ detail.data.name2 }}</span>
        <br/>
        <span style="font-weight:bolder;">审核状态:</span><span>{{ detail.data.state }}</span>
        <br/>
        <span style="font-weight:bolder;">审核时间:</span><span>{{ detail.data.update_time }}</span>
        <br/>
        <span style="font-weight:bolder;">备注:</span><span>{{ detail.data.remark }}</span>
      </div>
    </Modal>
  </div>
</template>

<script>
import utilities from '../components/utilities'
import depSelectModal2 from './dep-selector-modal2'
export default {
  components: {
    'dep-select-modal': depSelectModal2
  },
  data () {
    return {
      userList: [],
      communityModal: {
        list: [],
        title: '选择部门',
        show: false,
        loading: false,
        closable: false,
        maskClosable: false
      },
      queryForm: {
        community: '',
        dep: 0,
        user: 0,
        year: 0,
        month: 0,
        leader: utilities.user_data.userid
      },
      page: {
        size: 10,
        current: 1,
        total: 0
      },
      table: {
        data: [],
        total: [],
        columns: [{
          title: '上报人姓名',
          key: 'name1'
        }, {
          title: '上报时间',
          key: 'time'
        }, {
          title: '尿检结果',
          key: 'result'
        }, {
          title: '审核民警',
          key: 'name2'
        }, {
          title: '审核状态',
          key: 'state'
        }, {
          title: '审核时间',
          key: 'update_time'
        }, {
          title: '备注',
          key: 'remark'
        }, {
          title: '操作',
          slot: 'action'
        }],
        height: 519
      },
      totalData: [],
      tableData: [],
      yearList: [],
      monthList: [{
        label: '一月',
        value: 1
      }, {
        label: '二月',
        value: 2
      }, {
        label: '三月',
        value: 3
      }, {
        label: '四月',
        value: 4
      }, {
        label: '五月',
        value: 5
      }, {
        label: '六月',
        value: 6
      }, {
        label: '七月',
        value: 7
      }, {
        label: '八月',
        value: 8
      }, {
        label: '九月',
        value: 9
      }, {
        label: '十月',
        value: 10
      }, {
        label: '十一月',
        value: 11
      }, {
        label: '十二月',
        value: 12
      }],
      detail: {
        show: false,
        data: {
          img1: '',
          img2: '',
          name1: '',
          time: '',
          result: '',
          name2: '',
          remark: '',
          state: '',
          update_time: ''
        }
      }
    }
  },
  mounted () {
    let cY = new Date().getFullYear()
    for (let i = 0; i < 5; i++) {
      this.yearList.push({
        label: (cY - i) + '年',
        value: cY - i
      })
    }
    this.queryForm.year = cY
    let cM = new Date().getMonth() + 1
    this.queryForm.month = cM

    this.queryCommunities()
    this.refreshTotalData()
  },
  methods: {
    queryCommunities () {
      let p = {
        parent: utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].id : 0
      }
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_department', p).then((response) => {
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
      this.communityModal.list = childrenArr[0]

      if (this.communityModal.list.length === 1 && this.communityModal.list[0].level <= 1) {
        this.queryUserList()
      }
    },
    openDepSelectorModal () {
      this.communityModal.show = true
    },
    confirmCommunity (p) {
      this.queryForm.community = p.name
      this.queryForm.dep = p.id
      this.queryUserList()
    },
    queryUserList () {
      let dep
      if (this.queryForm.dep) {
        dep = this.queryForm.dep
      } else {
        if (utilities.user_data.deps.length > 0) {
          dep = utilities.user_data.deps[0].id
        } else {
          dep = 0
        }
      }
      this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/query_user', {
        dep: dep,
        level: 'eq#' + utilities.service_config.level1
      }).then((response) => {
        if (response.success === true) {
          this.userList = response.data
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    refreshTotalData () {
      this.$Loading.start()
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/query_urinalysis', this.queryForm).then((response) => {
        this.$Loading.finish()
        if (response.success === true) {
          this.table.total = response.data
          this.page.total = response.data.length
          this.refreshTable(1, this.page.size)
        } else {
          this.$Message.error(response.message)
        }
      }).catch((error) => {
        this.$Loading.finish()
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
    showDetail (data) {
      if (data.photo) {
        let p = data.photo.split('*')
        if (p[0]) {
          this.detail.data.img1 = utilities.config.buffer_url + p[0].substring(7)
        }
        if (p[1]) {
          this.detail.data.img2 = utilities.config.buffer_url + p[1].substring(7)
        }
      }
      Object.assign(this.detail.data, data)
      this.detail.show = true
    },
    donwloadImg (src) {
      window.open(src, '_blank')
    }
  }
}
</script>

<style lang="less" scoped>
@queryHeight: 64px;
.container{
  .query{
    height: @queryHeight;
    padding-left: 10px;
    padding-top: 15px;
    .row{
      margin: 0!important;
    }
  }
  .content{
    margin: 0 10px 0 10px;
    height: calc(100% - @queryHeight);
    p{
      margin: 10px;
      font-size: 20px;
      font-weight: bolder;
    }
  }
}
</style>
