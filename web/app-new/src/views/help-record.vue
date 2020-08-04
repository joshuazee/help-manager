<template>
  <div class="container">
    <div class="query">
      <div class="row">
        <i-input type="text" placeholder="请输入查询内容" v-model="queryOption.title" clearable>
          <span slot="prepend">标题</span>
        </i-input>
      </div>
      <div class="row">
        <Button @click="refreshData" class="button" type="info">查询</Button>
        <Button @click="openAddModal" class="button" type="success">发布</Button>
      </div>
    </div>
    <div class="content">
      <Table :columns="tableCols" :data="tableData" :height="tableHeight">
        <template slot-scope="{ row, index }" slot="number">
          {{ index+1 }}
        </template>
        <template slot-scope="{ row }" slot="title">
          {{ row.title.length &lt; 18 ? row.title : row.title.substring(0, 18) + '...' }}
        </template>
        <template slot-scope="{ row }" slot="content">
          {{ row.content.length &lt; 45 ? row.content : row.content.substring(0, 45) + '...' }}
        </template>
        <template slot-scope="{ row }" slot="action">
          <div>
            <Button @click="preview(row)" type="success" size="small" ghost>预览</Button>
            <Button @click="openEditModal(row)" type="warning" size="small" ghost>编辑</Button>
            <Button @click="deleteNews(row.id)" type="error" size="small" ghost>删除</Button>
          </div>
        </template>
      </Table>
        <Page :total="page.total" prev-text="上一页" next-text="下一页" show-sizer show-elevator show-total
          @on-page-size-change="pageSizeChange" @on-change="pageChange" :page-size="page.size" :current="page.current"></Page>
    </div>
    <news-oper-modal ref="oper" v-on:refreshList="modalSuccessCallback"></news-oper-modal>
    <Modal v-model="previewFlag" width="40" title="帮扶记录预览" class="preview-modal">
      <div class="title">
        <p>{{ previewData.title }}</p>
      </div>
      <div class="copyright">
        <p>{{ previewData.publish_time + ' ' + previewData.publish_user }}</p>
      </div>
      <div class="content" v-html="previewData.html">
      </div>
    </Modal>
  </div>
</template>

<script>
import utilities from '../components/utilities'
import newsOperModal from './news-oper-modal'
export default {
  components: {
    'news-oper-modal': newsOperModal
  },
  data () {
    return {
      previewFlag: false,
      previewData: {
        title: '',
        html: '',
        publish_time: '',
        publish_user: ''
      },
      modalData: {},
      tableHeight: 519,
      tableCols: [{
        title: '序号',
        slot: 'number',
        align: 'center',
        maxWidth: 80
      }, {
        title: '标题',
        slot: 'title',
        align: 'center',
        maxWidth: 350
      }, {
        title: '内容',
        slot: 'content',
        align: 'center'
      }, {
        title: '发布人',
        key: 'publish_user',
        align: 'center',
        maxWidth: 150
      }, {
        title: '发布时间',
        key: 'publish_time',
        align: 'center',
        maxWidth: 250
      }, {
        title: '操作',
        slot: 'action',
        align: 'center',
        maxWidth: 180
      }],
      totalData: [],
      tableData: [],
      page: {
        total: 0,
        current: 1,
        size: 10
      },
      queryOption: {
        type: '帮扶记录',
        user: utilities.user_data.userid,
        title: ''
      }
    }
  },
  mounted () {
    this.refreshData()
  },
  methods: {
    refreshData () {
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/query_news', this.queryOption).then((response) => {
        if (response.success === true) {
          this.totalData = response.data
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
    refreshTable (current, size) {
      this.tableData = []
      let start = (current - 1) * size
      let end = this.totalData.length >= current * size ? current * size : this.totalData.length
      for (let i = start; i < end; i++) {
        this.tableData.push(this.totalData[i])
      }
    },
    preview (data) {
      this.previewData.title = data.title
      this.previewData.publish_user = data.publish_user
      this.previewData.publish_time = data.publish_time
      this.previewData.html = data.html
      this.previewFlag = true
    },
    openAddModal () {
      this.$refs.oper.modalData.type = 'add'
      this.$refs.oper.modalData.title = '发布帮扶记录'
      this.$refs.oper.modalData.data = {}
      this.$refs.oper.modalData.newsType = this.queryOption.type
      this.$refs.oper.modalData.show = true
    },
    openEditModal (row) {
      this.$refs.oper.modalData.type = 'edit'
      this.$refs.oper.modalData.title = '编辑帮扶记录'
      this.$refs.oper.modalData.data = row
      this.$refs.oper.modalData.newsType = this.queryOption.type
      this.$refs.oper.modalData.show = true
    },
    deleteNews (id) {
      this.$refs.oper.modalData.type = 'delete'
      this.$refs.oper.modalData.title = '请确认'
      this.$refs.oper.modalData.data = { id: id }
      this.$refs.oper.modalData.newsType = this.queryOption.type
      this.$refs.oper.modalData.show = true
    },
    modalSuccessCallback (data) {
      this.$Message.success(data.msg)
      this.refreshData()
      this.$refs.oper.modalData.show = false
    },
    pageSizeChange (size) {
      this.page.size = size
      this.page.current = 1
      this.pageChange(1)
    },
    pageChange (current) {
      this.refreshTable(current, this.page.size)
    }
  }
}
</script>

<style scoped lang="less">
@queryHeight: 64px;
.container{
  height: 100%;
  .query{
    height: @queryHeight;
    div.row{
      padding: 16px 0 16px 20px;
      overflow: hidden;
      float: left;

      .button{
        margin-left:20px;
      }
    }
  }
  .content{
    margin: 0 10px 0 10px;
    height: calc(100% - @queryHeight);
  }
}
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
