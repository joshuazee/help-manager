<template>
  <div class="container">
    <div class="query">
      <span style="margin-left: 25px; font-size: 20px; font-weight: bold;">视频列表</span>
      <div class="row">
        <Button @click="openAddModal" class="button" type="success">上传视频</Button>
      </div>
    </div>
    <div class="content">
      <Table :columns="tableCols" :data="tableData" :height="tableHeight">
        <template slot-scope="{ row, index }" slot="number">
          {{ index+1 }}
        </template>
        <template slot-scope="{ row }" slot="title">
          {{ row.title }}
        </template>
        <template slot-scope="{ row }" slot="action">
          <div>
            <Button @click="preview(row)" type="success" size="small" ghost>预览</Button>
            <Button @click="deleteVideo(row.id)" type="error" size="small" ghost>删除</Button>
          </div>
        </template>
      </Table>
        <Page :total="page.total" prev-text="上一页" next-text="下一页" show-sizer show-elevator show-total
        @on-page-size-change="pageSizeChange" @on-change="pageChange" :page-size="page.size" :current="page.current"></Page>
    </div>
    <Modal v-model="uploadModal.show" width="40" title="上传视频" @on-ok="uploadVideo"
    :mask-closable="uploadModal.maskClosable" :loading="uploadModal.loading">
      <Form :model="add_form" :label-width="add_form.labelWidth">
        <FormItem label="标题">
          <Input v-model="add_form.title" placeholder="请输入视频标题..." clearable />
        </FormItem>
        <FormItem label="上传人">
          <Input v-model="add_form.uploader" disabled />
        </FormItem>
        <FormItem label="文件">
          <Row>
            <input ref="uploadFile" type="file"/>
          </Row>
        </FormItem>
      </Form>
    </Modal>
    <Modal v-model="previewFlag" width="40" footer-hide
    @on-cancel="previewClose"
    :title="previewData.title" class="preview-modal">
      <video ref="videoPlayer" :src="previewData.url" autoplay>
        您的浏览器不支持该视频播放组件
      </video>
      <div v-if="previewData.url.indexOf('mp4') < 0 || previewData.url.indexOf('ogg') < 0">不支持播放该格式的视频</div>
    </Modal>
  </div>
</template>

<script>
import utilities from '../components/utilities'
export default {
  data () {
    return {
      uploadModal: {
        show: false,
        maskClosable: false,
        loading: true
      },
      previewFlag: false,
      add_form: {
        title: '',
        uploader: '',
        labelWidth: 80
      },
      previewData: {
        title: '',
        url: ''
      },
      modalData: {},
      tableHeight: 519,
      tableCols: [{
        title: '序号',
        slot: 'number',
        align: 'center',
        maxWidth: 180
      }, {
        title: '标题',
        slot: 'title',
        align: 'center'
      }, {
        title: '上传人',
        key: 'publish_user',
        align: 'center',
        maxWidth: 260
      }, {
        title: '上传时间',
        key: 'publish_time',
        align: 'center',
        maxWidth: 260
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
      }
    }
  },
  mounted () {
    this.refreshData()
  },
  methods: {
    refreshData () {
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/query_video', this.queryOption).then((response) => {
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
      let end = this.totalData.length >= current * size ? current * size : this.totalData.length
      for (let i = start; i < end; i++) {
        this.tableData.push(this.totalData[i])
      }
    },
    pageSizeChange (size) {
      this.page.size = size
      this.page.current = 1
      this.pageChange(1)
      // this.refreshTable(1, size)
    },
    pageChange (current) {
      this.refreshTable(current, this.page.size)
    },
    preview (data) {
      this.previewData.title = data.title
      this.previewData.uploader = data.uploader
      this.previewData.time = data.time
      this.previewData.url = utilities.config.buffer_url + data.url.substring(7)
      this.previewFlag = true
    },
    openAddModal () {
      this.add_form.uploader = utilities.user_data.username
      this.uploadModal.show = true
    },
    checkParams () {
      if (!this.add_form.title) {
        this.$Message.warning('请输入视频标题')
        return false
      }
      if (this.$refs.uploadFile.files.length < 1) {
        this.$Message.warning('请选择视频文件')
        return false
      }
      return true
    },
    uploadVideo () {
      if (this.checkParams()) {
        let formData = new FormData()
        formData.append('file', this.$refs.uploadFile.files[0])
        this.$http.post(utilities.config.service_base + '/Base.svc/upload2', formData, {
          headers: {
            'Content-Type': 'multipart/form-data'
          }
        }).then((response) => {
          if (response.success === true) {
            this.add_form.url = response.data[0]
            this.uploadRecord()
          } else {
            this.$Message.error(response.message)
            this.uploadModal.loading = false
          }
        }).catch((error) => {
          this.uploadModal.loading = false
          this.$Message.info('后台更新中，请稍后重试')
          console.error(error)
        })
      } else {
        this.uploadModal.loading = false
      }
    },
    uploadRecord () {
      this.add_form.dep = utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].name : ''
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/add_video', this.add_form).then((response) => {
        if (response.success === true) {
          this.$Message.success('视频上传成功')
          this.refreshData()
          this.clearForm()
          this.uploadModal.show = false
        } else {
          this.uploadModal.loading = false
          this.$Message.error(response.message)
        }
      }).catch((error) => {
        this.uploadModal.loading = false
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    deleteVideo (id) {
      this.$Modal.confirm({
        title: '请确认',
        content: '是否删除所选视频',
        maskClosable: false,
        onOk: () => {
          this.$http.post(utilities.config.service_base + utilities.config.business_url + '/delete_video', {
            id: id
          }).then((response) => {
            if (response.success === true) {
              this.$Message.success('视频删除成功')
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
    clearForm () {
      this.add_form.url = ''
      this.$refs.uploadFile.outerHTML = this.$refs.uploadFile.outerHTML
      this.add_form.title = ''
    },
    previewClose () {
      this.previewData.url = ''
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
    line-height: @queryHeight;
    div.row{
      overflow: hidden;
      float: right;
      margin-right: 25px;

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
