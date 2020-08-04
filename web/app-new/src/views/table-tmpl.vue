<template>
  <div class="container">
    <div class="query">
      <Row gutter="20" class="row">
        <i-col span="6">
          <Select v-model="queryForm.user" placeholder="请选择尿检人员..." filterable>
            <Option v-for="item in userList" :value="item.value" :key="item.value">{{ item.label }}</Option>
          </Select>
        </i-col>
        <i-col span="6">
          <Select v-model="queryForm.year" placeholder="请选择年份..." filterable>
            <Option v-for="item in yearList" :value="item.value" :key="item.value">{{ item.label }}</Option>
          </Select>
        </i-col>
        <i-col span="4">
          <Button @click="refreshData" type="info">查询</Button>
        </i-col>
      </Row>
    </div>
    <div class="content">
      <Table :columns="table.columns" :data="table.data" :height="table.height">
        <template slot="action">
          <Button @click="showDetail(row)" type="primary" size="small" ghost>查看详情</Button>
        </template>
      </Table>
      <Page :total="page.total" prev-text="上一页" next-text="下一页" :page-size="page.size" :current="page.current"
      show-sizer show-elevator show-total @on-page-size-change="pageSizeChange" @on-change="pageChange"></Page>
    </div>
  </div>
</template>

<script>
import utilities from '../components/utilities'
export default {
  data () {
    return {
      tableHeight: 519,
      queryForm: {
        user: 0,
        year: 0,
        month: 0
      },
      page: {
        size: 10,
        current: 1,
        total: 0
      },
      table: {
        columns: [{
          title: '被点名人',
          key: 'name1',
          align: 'center'
        }, {
          title: '签到内容',
          key: 'remark',
          align: 'center'
        }, {
          title: '完成时间',
          key: 'time1',
          align: 'center'
        }, {
          title: '签到类型',
          key: 'type',
          align: 'center'
        }, {
          title: '完成情况',
          key: 'state',
          align: 'center'
        }, {
          title: '点名人',
          key: 'name2',
          align: 'center'
        }, {
          title: '点名时间',
          key: 'appointment_time',
          align: 'center'
        }, {
          title: '间距',
          key: 'distance',
          align: 'center'
        }, {
          title: '操作',
          slot: 'action',
          align: 'center'
        }],
        total: [],
        data: []
      }
    }
  },
  mounted () {
    this.refreshData()
  },
  methods: {
    refreshData () {
      this.$http(utilities.config.service_base + utilities.config.business_url + '/query_sign', this.queryForm).then((response) => {
        if (response.success === true) {
          this.totalData = response.data
          this.page.total = response.data.length
          this.refreshTable(1, this.page.size)
        } else {
          this.$Message.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    refreshTable (current, size) {
      this.tableData = []
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
    showDetail (row) {
      console.error(row)
    }
  }
}
</script>

<style lang="less" scoped>
@queryHeight: 64px;
.container{
  .query{
    height: @queryHeight;
    line-height: 64px;
    .row{
      margin: 0!important;
    }
  }
}
</style>
