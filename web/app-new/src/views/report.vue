<template>
<div class="container">
  <div class="first">
    <Form :model="queryForm" :label-width="80" label-position="right" inline>
      <FormItem label="统计类型">
        <Select v-model="queryForm.type" placeholder="请选择" style="width: 180px;">
          <Option value="装机">App装机情况</Option>
          <Option value="签到">签到情况</Option>
          <Option value="尿检">尿检情况</Option>
        </Select>
      </FormItem>
      <FormItem label="年份">
        <Select v-model="queryForm.year" placeholder="请选择" style="width: 180px;">
            <Option v-for="item in yearList" :value="item.value" :key="item.value">{{ item.label }}</Option>
          </Select>
      </FormItem>
      <FormItem label="月份">
          <Select v-model="queryForm.month" placeholder="请选择" style="width: 180px;">
            <Option v-for="item in monthList" :value="item.value" :key="item.value">{{ item.label }}</Option>
          </Select>
        </FormItem>
        <FormItem :label-width="0">
          <Button type="primary" @click="queryButtonClick">查询</Button>
        </FormItem>
    </Form>
  </div>
  <div class="second">
    <div class="statistic_map" ref="statistic_map"></div>
  </div>
</div>
</template>

<script>
import utilities from '../components/utilities'
import echarts from 'echarts'

export default {
  data () {
    return {
      queryForm: {
        year: 0,
        month: 0,
        type: ''
      },
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
      chartOption: {
        graphic: {
          position: {
            leftPlus: 60,
            leftCur: 0,
            left: 0,
            top: 50
          },
          style: {
            font: '18px "Microsoft YaHei", sans-serif',
            textColor: '#eee',
            lineColor: 'rgba(147, 235, 248, .8)'
          }
        }
      },
      chartBuffer: []
    }
  },
  mounted () {
    this.initQueryCondition()
    this.initEcharts()

    let regionCode
    if (utilities.user_data.deps.length > 0) {
      regionCode = utilities.user_data.deps[0].code
    } else {
      regionCode = utilities.config.default_region_code
    }
    this.mapGoDown(regionCode)
  },
  methods: {
    initQueryCondition () {
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
      this.queryForm.type = '装机'
    },
    initEcharts () {
      this.chart = echarts.init(this.$refs['statistic_map'])

      this.chart.on('click', (event) => {
        if (event.componentType === 'series' && event.data && event.data.code) {
          this.mapGoDown(event.data.code)
        }
      })
    },
    createChartBufferData (regionCode) {
      this.chartBuffer.push({
        regionCode: regionCode,
        graphic: []
      })
    },
    mapGoDown (regionCode) {
      this.createChartBufferData(regionCode)
      this.queryMapByRegionCode(regionCode)
      this.queryStatisticByQueryForm(regionCode)
    },
    queryMapByRegionCode (regionCode) {
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/query_region_map_data', {
        code: regionCode
      }).then((response) => {
        if (response.success === true) {
          let mapdata = JSON.parse(response.data)
          // mapdata register to echarts
          echarts.registerMap(regionCode, mapdata)

          this.createBreadCrumb(regionCode, mapdata)
        } else {
          throw response.message
        }
      }).catch((error) => {
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    // 地图下钻时新建面包屑
    createBreadCrumb (regionCode, mapdata) {
      let graphic
      if (this.chartBuffer.length > 1) {
        graphic = [].concat(this.chartBuffer[this.chartBuffer.length - 2].graphic)
      } else {
        graphic = []
      }

      let left = 0
      let graphicOption = {
        type: 'group',
        left: this.chartOption.graphic.position.leftCur + this.chartOption.graphic.position.leftPlus,
        top: this.chartOption.graphic.position.top,
        children: []
      }
      if (graphic.length !== 0) {
        graphicOption.children.push({
          type: 'polyline',
          left: 0,
          top: -10,
          shape: {
            points: [[0, 0], [4, 8], [0, 16]]
          },
          style: {
            stroke: '#fff',
            key: mapdata.properties.name
          }
        })
        left += 20
      }
      graphicOption.children.push({
        type: 'text',
        left: left,
        top: 'middle',
        style: {
          text: mapdata.properties.name,
          textAlign: 'center',
          fill: this.chartOption.graphic.style.textColor,
          font: this.chartOption.graphic.style.font
        },
        info: {
          regionCode: regionCode
        },
        onclick: this.crumbClick
      })
      graphic.push(graphicOption)
      this.chartBuffer[this.chartBuffer.length - 1].graphic = graphic
      this.chartOption.graphic.position.leftCur += this.chartOption.graphic.position.leftPlus + left
    },
    MapGoBack (index) {
      // 更新chartBuffer数据到当前层级
      this.chartOption.graphic.position.leftCur = this.chartOption.graphic.position.left + (this.chartOption.graphic.position.leftPlus * (index + 1)) + 20 * index
      this.chartBuffer = this.chartBuffer.slice(0, index + 1)
      let regionCode = this.chartBuffer[index].regionCode
      this.queryStatisticByQueryForm(regionCode)
    },
    queryStatisticByQueryForm (regionCode) {
      let startTime = this.queryForm.year + '-' + this.queryForm.month + '-1 0:0:0'
      let endTime = this.queryForm.year + '-' + (this.queryForm.month + 1) + '-1 0:0:0'
      let queryOption = {
        startTime: startTime,
        endTime: endTime,
        dep: regionCode
      }
      switch (this.queryForm.type) {
        case '装机':
          this.queryStatisticAjax({ dep: regionCode }, 'count_active_user_by_dep')
          break
        case '签到':
          this.queryStatisticAjax(queryOption, 'count_sign_by_dep')
          break
        case '尿检':
          this.queryStatisticAjax(queryOption, 'count_urinalysis_by_dep')
          break
      }
    },
    queryStatisticAjax (queryOption, functionName) {
      this.$Loading.start()
      this.$http.post(utilities.config.service_base + utilities.config.business_url + '/' + functionName, queryOption).then((response) => {
        this.$Loading.finish()
        if (response.success === true) {
          let statData = JSON.parse(response.data)
          this.updateEcharts(queryOption.dep, statData)
        } else {
          throw response.error
        }
      }).catch((error) => {
        this.$Loading.finish()
        this.$Message.info('后台更新中，请稍后重试')
        console.error(error)
      })
    },
    updateEcharts (regionCode, statisticData) {
      if (echarts.getMap(regionCode)) {
        let buffer = this.chartBuffer[this.chartBuffer.length - 1]
        let option = {
          backgroundColor: '#154e90',
          graphic: buffer.graphic,
          series: [{
            type: 'map',
            map: buffer.regionCode,
            label: {
              normal: {
                formatter: (params) => {
                  if (params.data) {
                    let label
                    switch (this.queryForm.type) {
                      case '装机':
                        label = params.name + '\n'
                        label += '已装App ' + params.data.active_user_count + ' 人'
                        return label
                      case '签到':
                        label = params.name + '\n'
                        label += '已签到任务 ' + params.data.signed + '人/次'
                        return label
                      case '尿检':
                        label = params.name + '\n'
                        label += '上报尿检' + params.data.total + '人/次'
                        return label
                    }
                  } else {
                    return params.name
                  }
                },
                show: true,
                textStyle: {
                  color: '#fff'
                }
              },
              emphasis: {
                formatter: (params) => {
                  if (params.data) {
                    switch (this.queryForm.type) {
                      case '装机':
                        let label = params.name + '\n'
                        if (params.data.code) {
                          label += '录入下属社区 ' + params.data.community_count + ' 个\n'
                        }
                        label += '录入戒毒人员 ' + params.data.total_user_count + ' 人\n'
                        label += '已装App ' + params.data.active_user_count + ' 人'
                        return label
                      case '签到':
                        label = params.name + '\n'
                        label += '总签到任务 ' + params.data.total + ' 人/次\n'
                        label += '已签到 ' + params.data.signed + ' 人/次\n'
                        label += '未签到 ' + params.data.unsign + ' 人/次\n'
                        label += '已审核 ' + params.data.checked + ' 人/次\n'
                        label += '未审核 ' + params.data.uncheck + ' 人/次'
                        return label
                      case '尿检':
                        label = params.name + '\n'
                        label += '上报尿检 ' + params.data.total + ' 人/次\n'
                        label += '已审核 ' + params.data.checked + ' 人/次\n'
                        label += '未审核 ' + params.data.uncheck + ' 人/次'
                        return label
                    }
                  } else {
                    return params.name
                  }
                },
                textStyle: {
                  color: '#fff'
                }
              }
            },
            itemStyle: {
              normal: {
                borderColor: 'rgba(147, 235, 248, 1)',
                borderWidth: 1,
                areaColor: {
                  type: 'radial',
                  x: 0.5,
                  y: 0.5,
                  r: 0.8,
                  colorStops: [{
                    offset: 0,
                    color: 'rgba(147, 235, 248, 0)' // 0% 处的颜色
                  }, {
                    offset: 1,
                    color: 'rgba(147, 235, 248, .2)' // 100% 处的颜色
                  }],
                  globalCoord: false // 缺省为 false
                },
                shadowColor: 'rgba(128, 217, 248, 1)',
                shadowOffsetX: -2,
                shadowOffsetY: 2,
                shadowBlur: 10
              },
              emphasis: {
                areaColor: '#389BB7',
                borderWidth: 0
              }
            },
            data: statisticData
          }]
        }
        this.chart.clear()
        this.chart.setOption(option)
      } else {
        setTimeout(() => {
          this.updateEcharts(regionCode, statisticData)
        }, 500)
      }
    },
    crumbClick (evt) {
      let target = evt.target
      for (let index = 0; index < this.chartBuffer.length; index++) {
        const element = this.chartBuffer[index]
        if (element.regionCode === target.info.regionCode) {
          this.MapGoBack(index)
          return
        }
      }
    },
    queryButtonClick () {
      let regionCode = this.chartBuffer[this.chartBuffer.length - 1].regionCode
      this.queryStatisticByQueryForm(regionCode)
    }
  }
}
</script>

<style lang="less" scoped>
@queryHeight: 64px;
.container{
  .first{
    height: @queryHeight;
    padding-left: 10px;
    padding-top: 15px;
    .row{
      margin: 0!important;
    }
  }
  .second{
    height: calc(100% - @queryHeight);
    .statistic_map{
      height: 100%;
    }
  }
}
</style>
