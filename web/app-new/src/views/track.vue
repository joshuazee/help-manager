<template>
  <div class="container">
    <div class="query">
      <Form :model="queryForm" label-position="right" :label-width="80" inline>
        <FormItem label="所属社区" v-if="communityModal.list.length > 0">
          <i-input v-model="queryForm.community" placeholder="请选择" readonly @on-focus="openDepSelectorModal" width=""></i-input>
        </FormItem>
        <FormItem label="被点名人">
          <Select v-model="queryForm.user" placeholder="请选择" filterable clearable>
            <Option v-for="item in userList" :value="item.id" :key="item.value">{{ item.name }}</Option>
          </Select>
        </FormItem>
        <FormItem label="日期" :label-width="50">
          <DatePicker type="date" v-model="queryForm.date"></DatePicker>
        </FormItem>
        <FormItem :label-width="0">
          <Button type="primary" @click="queryTrackData">查询</Button>
          <!-- <Button type="primary" @click="clearTrack" style="margin-left: 5px;">清除</Button> -->
          <Button type="primary" @click="playTrack" style="margin-left: 5px;" :disabled="btnGrp.play">播放</Button>
          <Button type="primary" @click="pauseTrack" style="margin-left: 5px;" :disabled="btnGrp.pause">暂停</Button>
          <Button type="primary" @click="stopTrack" style="margin-left: 5px;" :disabled="btnGrp.stop">停止</Button>
        </FormItem>
      </Form>
    </div>
    <div class="content">
      <iframe src="/map/" ref="frame"></iframe>
    </div>
    <dep-select-modal :initObj="communityModal" v-on:confirm="confirmCommunity"></dep-select-modal>
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
        time: '',
        date: null,
        leader: utilities.user_data.userid
      },
      btnGrp: {
        play: true,
        pause: true,
        stop: true
      },
      playControl: {}
    }
  },
  mounted () {
    this.queryCommunities()
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
    queryTrackData () {
      this.queryForm.time = this.queryForm.date.getFullYear() + '-' + (this.queryForm.date.getMonth() + 1) + '-' + this.queryForm.date.getDate()
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/query_trace', this.queryForm).then((response) => {
        if (response.success === true) {
          this.trackData = response.data
          if (this.trackData.points.length > 0) {
            this.refreshData()
          } else {
            this.$Message.info('该时段无该人员轨迹数据')
          }
        } else {
          this.$Message.info('后台更新中，请稍后重试')
          console.error(response.message)
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    refreshData () {
      this.btnGrp.play = false
      this.btnGrp.pause = false
      this.btnGrp.stop = false

      let data = {
        taskName: 'track',
        callback: this.trackCallback,
        data: this.trackData
      }
      this.$refs.frame.contentWindow.task.callInterface(data)
    },
    trackCallback (track) {
      this.track = track
      // this.playTrack = track.play
      // this.pauseTrack = track.pause
      // this.stopTrack = track.stop
    },
    playTrack () {
      this.track.play()
    },
    pauseTrack () {
      this.track.pause()
    },
    stopTrack () {
      this.track.stop()
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
    iframe{
      height: calc(100% - 10px);
      width: 100%;
      border: 1px solid #ccc;
    }
  }
}
</style>
