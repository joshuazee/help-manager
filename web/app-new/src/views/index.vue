<template>
  <div class="container">
    <div class="first">
      <div class="welcome">
        <div style="text-align: left;">
          <span>你好，{{ username }}</span>
        </div>
        <div style="text-align: right; margin-left: 200px;">
          <span>欢迎来到{{ systemTitle }}</span>
        </div>
      </div>
      <div class="weather">
        <iframe id="fancybox-frame" name="fancybox-frame1576574919558" frameborder="0" scrolling="no" hspace="0"  src="http://i.tianqi.com/index.php?c=code&a=getcode&id=25&h=100&w=155"></iframe>
        </div>
    </div>
    <div class="second" ref="secondFloor">
      <Card class="video card" ref="videoCard">
        <div slot="title" style="height: 16px; font-size: 16px; color: #17233d;">
            <img src="../assets/video_small.png">
            <span style="margin-left: 5px;">教育视频</span>
        </div>
        <video controlsList = "nodownload" controls="controls"></video>
      </Card>
      <Card class="news card">
        <div slot="title" style="height: 16px; font-size: 16px; color: #17233d;">
            <img src="../assets/news_small.png">
            <span style="margin-left: 5px;">新闻快报</span>
        </div>
        <ul>
          <template v-for="(item, index) in listData.news">
            <li v-if="index < 10" :key="item.id" style="list-style:none; height: 24px; line-height: 24px;">
              <div style="float: left;">
                <a @click="previewNews(item)" :title="item.title">{{ item.title.length &gt; 18 ? item.title.substr(0, 18) + '...' : item.title }}</a>
              </div>
              <div style="float: right;">
                <span>{{ item.publish_time.substr(0, 10) }}</span>
              </div>
            </li>
          </template>
        </ul>
      </Card>
      <Card class="community card">
        <div slot="title" style="height: 16px; font-size: 16px; color: #17233d;">
            <img src="../assets/community_small.png">
            <span style="margin-left: 5px;">社区新鲜事</span>
        </div>
        <ul>
          <template v-for="(item, index) in listData.communities">
            <li v-if="index < 10" :key="item.id" style="list-style:none; height: 24px; line-height: 24px;">
              <div style="float: left;">
                <a @click="previewNews(item)" :title="item.title">{{ item.title.length &gt; 18 ? item.title.substr(0, 18) + '...' : item.title }}</a>
              </div>
              <div style="float: right;">
                <span>{{ item.publish_time.substr(0, 10) }}</span>
              </div>
            </li>
          </template>
        </ul>
      </Card>
      <Card class="help-record card ex">
        <div slot="title" style="height: 16px; font-size: 16px; color: #17233d;">
            <img src="../assets/handshake_small.png">
            <span style="margin-left: 5px;">帮扶记录</span>
        </div>
        <ul>
          <template v-for="(item, index) in listData.helprecords">
           <li v-if="index < 10" :key="item.id" style="list-style:none; height: 24px; line-height: 24px;">
              <div style="float: left;">
                <a @click="previewNews(item)" :title="item.title">{{ item.title.length &gt; 18 ? item.title.substr(0, 18) + '...' : item.title }}</a>
              </div>
              <div style="float: right;">
                <span>{{ item.publish_time.substr(0, 10) }}</span>
              </div>
            </li>
          </template>
        </ul>
      </Card>
    </div>
    <div class="third">
      <div class="stat_info" ref="stat"></div>
    </div>
    <news-preview-modal :initObj="newsPreviewData"></news-preview-modal>
  </div>
</template>

<script>
import utilities from '../components/utilities'
import newsPreviewModal from './news-preview-modal'
import echarts from 'echarts'
export default {
  components: {
    newsPreviewModal: newsPreviewModal
  },
  data () {
    return {
      username: utilities.user_data.username,
      systemTitle: utilities.config.system_title,
      currentStatistic: 0,
      listData: {
        news: [],
        communities: [],
        help: [],
        video: '',
        statInfo: {}
      },
      newsPreviewData: {
        show: false,
        title: '新闻详情',
        data: {}
      }
    }
  },
  mounted () {
    this.refreshData()
  },
  methods: {
    refreshData () {
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/query_index_other_content', {
        userid: utilities.user_data.userid,
        dep: utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].id : 0
      }).then((response) => {
        this.resolveQueryInfo(response)
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })

      this.querySignStatistics()
    },
    resolveQueryInfo (response) {
      if (response.success === true) {
        if (response.data.news) {
          this.listData.news = JSON.parse(response.data.news)
        }
        if (response.data.communities) {
          this.listData.communities = JSON.parse(response.data.communities)
        }
        if (response.data.helprecords) {
          this.listData.helprecords = JSON.parse(response.data.helprecords)
        }
        if (response.data.video) {
          this.updateVideo(response.data.video)
        }
      } else {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(response.message)
      }
    },
    updateVideo (data) {
      this.listData.video = utilities.config.buffer_url + data.substring(7)
      let vDom = document.getElementsByTagName('video')[0]
      vDom.parentElement.style.padding = 0
      let height = vDom.parentElement.parentElement.offsetHeight - 45
      let width = vDom.parentElement.parentElement.offsetWidth - 1
      vDom.style.width = width + 'px'
      vDom.style.height = height + 'px'
      vDom.style.objectFit = 'fill'
      vDom.src = this.listData.video
    },
    querySignStatistics () {
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/count_sign_by_dep', {
        dep: utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].code || utilities.user_data.deps[0].id : utilities.config.default_region_code
      }).then((response) => {
        if (response.success === true) {
          const data = JSON.parse(response.data)
          let keys = []
          let total = []
          let signed = []
          let unsign = []
          let checked = []
          let uncheck = []
          let legends = []
          let series = []

          for (let i = 0; i < data.length; i++) {
            const element = data[i]
            keys.push(element.name)
            total.push(element.total)
            signed.push(element.signed)
            unsign.push(element.unsign)
            checked.push(element.checked)
            uncheck.push(element.uncheck)
          }

          if (utilities.user_data.deps && utilities.user_data.deps.length > 0) {
            if (utilities.user_data.deps[0].level > 1) {
              legends = ['签到任务总数', '已签到', '已审核']
              series = [{
                type: 'bar',
                data: total,
                name: '签到任务总数'
              }, {
                type: 'bar',
                data: signed,
                name: '已签到'
              }, {
                type: 'bar',
                data: checked,
                name: '已审核'
              }]
            } else {
              legends = ['已签到']
              series = [{
                type: 'bar',
                data: signed,
                name: '已签到'
              }]
            }
          } else {
            legends = ['签到任务总数', '已签到', '未签到', '已审核', '未审核']
            series = [{
              type: 'bar',
              data: total,
              name: '签到任务总数'
            }, {
              type: 'bar',
              data: signed,
              stack: '签到情况',
              name: '已签到'
            }, {
              type: 'bar',
              data: unsign,
              stack: '签到情况',
              name: '未签到'
            }, {
              type: 'bar',
              data: checked,
              stack: '审核情况',
              name: '已审核'
            }, {
              type: 'bar',
              data: uncheck,
              stack: '审核情况',
              name: '未审核'
            }]
          }
          this.updateChart('本月签到情况总览', keys, series, legends)
        } else {
          throw response.message
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    queryUrinalysisStatistics () {
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/count_urinalysis_by_dep', {
        dep: utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].code : utilities.config.default_region_code
      }).then((response) => {
        if (response.success === true) {
          const data = JSON.parse(response.data)
          let keys = []
          let total = []
          let checked = []
          let uncheck = []
          let legends = []
          let series = []

          for (let i = 0; i < data.length; i++) {
            const element = data[i]
            keys.push(element.name)
            total.push(element.total)
            checked.push(element.checked)
            uncheck.push(element.uncheck)
          }

          if (utilities.user_data.deps && utilities.user_data.deps.length > 0) {
            if (utilities.user_data.deps[0].level > 1) {
              legends = ['尿检上报总数', '已审核', '未审核']
              series = [{
                type: 'bar',
                data: total,
                name: '尿检上报总数'
              }, {
                type: 'bar',
                data: checked,
                stack: '审核情况',
                name: '已审核'
              }, {
                type: 'bar',
                data: uncheck,
                stack: '审核情况',
                name: '未审核'
              }]
            } else {
              legends = ['尿检上报总数']
              series = [{
                type: 'bar',
                data: total,
                name: '尿检上报总数'
              }]
            }
          } else {
            legends = ['尿检上报总数', '已审核', '未审核']
            series = [{
              type: 'bar',
              data: total,
              name: '尿检上报总数'
            }, {
              type: 'bar',
              data: checked,
              stack: '审核情况',
              name: '已审核'
            }, {
              type: 'bar',
              data: uncheck,
              stack: '审核情况',
              name: '未审核'
            }]
          }
          this.updateChart('本月尿检情况总览', keys, series, legends)
        } else {
          throw response.message
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    updateChart (title, keys, series, legends) {
      let chart = echarts.init(this.$refs.stat)
      chart.clear()
      chart.setOption({
        title: {
          text: title,
          padding: 10
        },
        toolbox: {
          show: true,
          orient: 'horizontal',
          itemSize: 16,
          showTitle: true,
          feature: {
            myToolForChange: {
              show: true,
              title: '切换结果显示',
              icon: `image://data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAQAAADZc7J/AAAABGdBTUEAALGPC/
              xhBQAAACBjSFJNAAB6JgAAgIQAAPoAAACA6AAAdTAAAOpgAAA6mAAAF3CculE8AAAAAmJLR0QA/4ePzL8AAAAHdElNRQ
              fkBBcBDC2R50OcAAAAuElEQVRIx93Uuw0CMRAE0PEJCdGKPwWQQRNUQHU0cUREkICPJgjJWQJIuPPBrFcCCTuz5KcNZscJbKcx/
              meB1KZd9JYJppg3mxJBArJ2nYQiIeQNPuUoKQf/+k4DY4QCKBMqoEQogSGhBvqEEwCpxUKfQdcdoynKcgHwmMByvrVMfw1Mg
              NocYHtaPidwM8MI73aduR/qggbqCaK0aKCOoHpPAeiJ0caJew4oRDmfbysccOVy9PtCuQMuf8elS2sXXAAAACV0RVh0ZGF0ZTpjc
              mVhdGUAMjAyMC0wNC0yMlQxNzoxMjo0NSswODowMICpP9UAAAAldEVYdGRhdGU6bW9kaWZ5ADIwMjAtMDQtMjJUMTc6M
              TI6NDUrMDg6MDDx9IdpAAAAAElFTkSuQmCC`,
              onclick: () => {
                if (this.currentStatistic === 0) {
                  this.queryUrinalysisStatistics()
                  this.currentStatistic = 1
                } else {
                  this.querySignStatistics()
                  this.currentStatistic = 0
                }
              }
            }
          }
        },
        legend: {
          data: legends
        },
        tooltip: {
          trigger: 'axis',
          axisPointer: {
            type: 'shadow'
          }
        },
        xAxis: {
          type: 'category',
          data: keys,
          axisTick: {
            alignWithLabel: true
          }
        },
        yAxis: {
          type: 'value'
        },
        series: series
      })
    },
    previewNews (data) {
      this.newsPreviewData.data = data
      this.newsPreviewData.show = true
    }
  }
}
</script>

<style lang="less" scoped>
.container{
  height: 100%;
  .first{
    height: 15%;
    .welcome{
      float: left;
      font-size: 30px;
      font-weight: bold;
      margin: 10px 10px;
    }
    .weather{
      margin: 10px 10px;
      float: right;
      height: 100%;
      width: 150px;
    }
  }
  .second{
    height: 40%;
    width: 100%;
    .card{
      float: left;
      vertical-align:top;
      width: 25%;
      height: 100%;
    }
    .ex{
      margin-top: -20px;
    }
  }
  .third{
    height: 45%;
    .stat_info{
      height: 100%;
    }
  }
}
</style>
