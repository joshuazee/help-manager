<template>
  <div class="container" ref="content">
    <div class="map">
      <iframe src="/map/" ref="frame" @load="mapLoaded"></iframe>
    </div>
    <div class="table">
      <div class="title">
        <span>围栏列表</span>
        <Button type="info" style="float: right;" @click="startAddFence">添加围栏</Button>
      </div>
      <Table :columns="table.columns" :data="table.data" :height="table.height" highlight-row @on-row-click="fenceHighLight">
        <template slot="no" slot-scope="{ row, index }">
          {{ index + 1 }}
        </template>
        <template slot="name" slot-scope="{ row }">
          <span :title="row.name">
            {{ row.name.length > 10 ? row.name.substr(0, 9) + '...' : row.name }}
          </span>
        </template>
        <template slot="action" slot-scope="{ row }">
          <Button type="info" size="small" ghost @click="queryRelatedDep(row)" v-if="row.type === exType">关联人员</Button>
          <Button type="error" size="small" ghost @click="deleteFence(row)">删除</Button>
        </template>
      </Table>
      <Page :total="page.total" prev-text="上一页" next-text="下一页" :page-size="page.size" :current="page.current"
      show-sizer show-total @on-page-size-change="pageSizeChange" @on-change="pageChange"></Page>
    </div>
    <fence-oper-modal :initObj="fenceOperModalData" v-on:success="AddFenceSuccess" v-on:cancel="addFenceCancel"></fence-oper-modal>
    <dep-select-modal :initObj="depSelectorModalData" v-on:confirm="confirmCommunity"></dep-select-modal>
  </div>
</template>

<script>
import utilities from '../components/utilities'
import fenceOperModal from './fence-oper-modal'
import depSelectorModal2 from './dep-selector-modal2'
export default {
  components: {
    'fence-oper-modal': fenceOperModal,
    'dep-select-modal': depSelectorModal2
  },
  data () {
    return {
      exType: '管控区域',
      fenceOperModalData: {
        show: false,
        loading: true,
        closable: false,
        maskClosable: false,
        title: '',
        data: {}
      },
      depSelectorModalData: {
        show: false,
        loading: false,
        closable: false,
        maskClosable: false,
        multiple: true,
        list: []
      },
      table: {
        height: 519,
        columns: [{
          title: '序号',
          slot: 'no',
          width: 70,
          align: 'center'
        }, {
          title: '名称',
          slot: 'name',
          align: 'center'
        }, {
          title: '操作',
          slot: 'action',
          width: 155,
          align: 'center'
        }],
        total: [],
        data: []
      },
      page: {
        size: 20,
        current: 1,
        total: 0,
        sizeOptions: [20, 30, 50]
      }
    }
  },
  mounted () {
    this.table.height = this.$refs.content.offsetHeight - 84
  },
  methods: {
    mapLoaded () {
      this.$refs.frame.contentWindow.task.callInterface({
        taskName: 'efence',
        callback: this.mapCallback,
        data: {}
      })
      this.queryFenceData()
    },
    mapCallback (fenceMap) {
      this.map = fenceMap
    },
    queryFenceData () {
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/query_fence').then((response) => {
        if (response.success === true) {
          this.table.total = response.data
          this.page.total = response.data.length
          this.refreshTable(1, this.page.size)
          this.showOnMap()
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
    fenceHighLight (row, index) {
      this.currentRow = row
      this.map.highLight(row)
    },
    showOnMap () {
      if (this.map) {
        this.map.refreshMap(this.table.total)
      } else {
        setTimeout(this.showOnMap.bind(this), 500)
      }
    },
    startAddFence () {
      this.$Message.info('请在地图上选择范围，单击选点，双击结束')
      this.map.drawPolygon('polygon', this.drawEnd)
    },
    drawEnd (e) {
      let extent = ''
      let points = e.overlay.getPath()
      for (let i = 0; i < points.length; i++) {
        if (extent) {
          extent += ';'
        }
        extent += points[i].lng + ',' + points[i].lat
      }
      this.fenceOperModalData.data.extent = extent
      this.openFenceOperModal()
    },
    openFenceOperModal () {
      this.fenceOperModalData.show = true
      this.fenceOperModalData.title = '添加围栏'
      this.fenceOperModalData.uri = utilities.config.service_base + utilities.config.business_url + '/add_fence'
    },
    AddFenceSuccess () {
      this.$Message.success('围栏添加成功')
      this.map.removeDrawOverlay()
      this.queryFenceData()
    },
    addFenceCancel () {
      this.map.removeDrawOverlay()
    },
    queryRelatedDep (data) {
      let p = {
        id: data.id
      }
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/query_related_dep', p).then((response) => {
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
      this.depSelectorModalData.list = childrenArr[0]
      this.relateDep()
    },
    relateDep () {
      this.depSelectorModalData.show = true
    },
    confirmCommunity (data) {
      var deps = []
      for (let i = 0; i < data.length; i++) {
        if (data[i].children.length === 0) {
          deps.push(data[i].id)
        }
      }
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/fence_relate_dep', {
        deps: deps.join(','),
        id: this.currentRow.id
      }).then((response) => {
        if (response.success === true) {
          this.$Message.success('关联人员成功')
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    deleteFence (data) {
      this.$Modal.confirm({
        title: '请确认',
        content: '<p>是否删除围栏 <span style="font-weight: bolder;">' + data.name + '</span> ?</p>',
        onOk: () => {
          this.$http.post(utilities.config.service_base + utilities.config.business_url + '/delete_fence', {
            id: data.id
          }).then((response) => {
            if (response.success === true && response.data === true) {
              this.$Message.success('删除围栏成功')
              this.queryFenceData()
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
    }
  }
}
</script>

<style lang="less" scoped>
.container{
  height: 100%;
  .map{
    width: 75%;
    height: 100%;
    float: left;
    iframe{
      height: 100%;
      width: 100%;
      border: 1px solid #ccc;
    }
  }
  .table{
    padding: 0 5px;
    width: 25%;
    height: 100%;
    float: left;
    .title{
      font-size: 20px;
      font-weight: bolder;
      padding: 10px;
    }
  }
}
</style>
