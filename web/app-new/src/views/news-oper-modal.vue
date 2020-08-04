<template>
  <Modal v-model="modalData.show" width="40"
    :title="modalData.title"
    :loading="modalData.loading"
    :mask-closable="modalData.maskClosable"
    @on-ok="submit(modalData.type)"
    @on-cancel="cancel"
    @on-visible-change="visibleChange">
    <div v-if="modalData.type == 'delete'">
      {{ '是否删除选中的' + modalData.newsType }}
    </div>
    <div v-else-if="modalData.type == 'add' || modalData.type == 'edit'">
      <Form :model="newsForm">
        <FormItem>
          <Row>
            <i-col span="11">
              <i-input type="text" :placeholder="'请输入' + modalData.newsType + '标题'" v-model="newsForm.title">
                <span slot="prepend">标题</span>
              </i-input>
            </i-col>
            <i-col span="5" style="margin-left: 20px">
              <i-input type="text" v-model="newsForm.publish_user" disabled>
                <span slot="prepend">发布人</span>
              </i-input>
            </i-col>
            <i-col span="3" style="margin-left: 20px">
              <i-switch size="large" v-model="newsForm.topSwitch">
                <span slot="open">置顶</span>
                <span slot="close">置顶</span>
              </i-switch>
            </i-col>
          </Row>
        </FormItem>
        <FormItem>
          <quill-editor class="editor" v-model="newsForm.html" ref="myQuillEditor" :options="editorOption">
            <div id="toolbar" slot="toolbar">
              <!-- Add a bold button -->
              <button class="ql-bold" title="加粗">Bold</button>
              <button class="ql-italic" title="斜体">Italic</button>
              <button class="ql-underline" title="下划线">underline</button>
              <button class="ql-strike" title="删除线">strike</button>
              <button class="ql-blockquote" title="引用"></button>
              <button class="ql-code-block" title="代码"></button>
              <button class="ql-header" value="1" title="标题1"></button>
              <button class="ql-header" value="2" title="标题2"></button>
              <!--Add list -->
              <button class="ql-list" value="ordered" title="有序列表"></button>
              <button class="ql-list" value="bullet" title="无序列表"></button>
              <!-- Add font size dropdown -->
              <select class="ql-header" title="段落格式">
                <option selected>段落</option>
                <option value="1">标题1</option>
                <option value="2">标题2</option>
                <option value="3">标题3</option>
                <option value="4">标题4</option>
                <option value="5">标题5</option>
                <option value="6">标题6</option>
              </select>
              <select class="ql-size" title="字体大小">
                <option value="10px">10px</option>
                <option value="12px">12px</option>
                <option value="14px" selected>14px</option>
                <option value="16px">16px</option>
                <option value="18px">18px</option>
                <option value="20px">20px</option>
              </select>
              <select class="ql-font" title="字体">
                <option value="SimSun">宋体</option>
                <option value="SimHei">黑体</option>
                <option value="Microsoft-YaHei">微软雅黑</option>
                <option value="KaiTi">楷体</option>
                <option value="FangSong">仿宋</option>
                <option value="Arial">Arial</option>
              </select>
              <!-- Add subscript and superscript buttons -->
              <select class="ql-color" value="color" title="字体颜色"></select>
              <select class="ql-background" value="background" title="背景颜色"></select>
              <select class="ql-align" value="align" title="对齐"></select>
              <!-- <button class="ql-clean" title="还原"></button> -->
              <button class="ql-image" title="插入图片"></button>
              <!-- You can also add your own -->
            </div>
          </quill-editor>
        </FormItem>
      </Form>
    </div>
  </Modal>
</template>

<script>
import utilities from '../components/utilities'
import { Quill } from 'vue-quill-editor'
export default {
  data () {
    return {
      modalData: {
        show: false,
        type: 'add',
        title: '',
        loading: true,
        maskClosable: false,
        newsType: '',
        data: {}
      },
      newsForm: {
        id: 0,
        title: '',
        html: '',
        content: '',
        publish_user: '',
        user: 0,
        dep: 0,
        top: 0,
        topSwitch: false
      },
      editorOption: {
        placeholder: '请输入内容...',
        theme: 'snow',
        modules: {
          toolbar: {
            container: '#toolbar'
          }
        }
      }
    }
  },
  mounted () {
    this.editorRegister()
  },
  methods: {
    checkForm () {
      if (!this.newsForm.title) {
        this.$Message.warning('请输入' + this.modalData.newsType + '标题')
        return false
      }
      if (!this.newsForm.html) {
        this.$Message.warning('请输入' + this.modalData.newsType + '内容')
        return false
      }
      return true
    },
    submit (type) {
      this.modalData.loading = true
      new Promise((resolve, reject) => {
        let success = ''
        let uri = ''
        let images = []
        if (type === 'add') {
          images = this.validateNewsContent()
          success = '发布' + this.modalData.newsType + '成功'
          uri = '/add_news'
          if (!this.checkForm()) {
            this.modalData.loading = false
            return
          }
        } else if (type === 'edit') {
          images = this.validateNewsContent()
          success = '编辑' + this.modalData.newsType + '成功'
          uri = '/update_news'
          if (!this.checkForm()) {
            this.modalData.loading = false
            return
          }
        } else if (type === 'delete') {
          uri = '/delete_news'
          success = '删除' + this.modalData.newsType + '成功'
          resolve({ uri, success })
          return
        }
        this.newsForm.top = this.newsForm.topSwitch ? 1 : 0
        if (images.length > 0) {
          this.$http.post(utilities.config.service_base + utilities.config.business_url + '/upload_news_images', { images: images.join('###') }).then((response) => {
            if (response.success) {
              this.updateNewsContent(response.data)
              resolve({ uri, success })
            } else {
              this.$Message.error(response.message)
              this.modalData.loading = false
            }
          }).catch((error) => {
            this.$Message.info('后台更新中，请稍后重试')
            this.modalData.loading = false
            console.error(error)
          })
        } else {
          this.updateNewsContent()
          resolve({ uri, success })
        }
      }).then((option) => {
        this.$http.post(utilities.config.service_base + utilities.config.business_url + option.uri, this.newsForm).then((response) => {
          if (response.success === true) {
            this.$emit('refreshList', { msg: option.success })
          } else {
            this.$Message.error(response.message)
            this.modalData.loading = false
          }
        }).catch((error) => {
          this.$Message.info('后台更新中，请稍后重试')
          this.modalData.loading = false
          console.error(error)
        })
      })
    },
    cancel () {
      this.newsForm.html = ''
      this.newsForm.title = ''
      this.newsForm.topSwitch = false
      this.modalData.show = false
    },
    visibleChange (flag) {
      if (flag === true) {
        this.newsForm.publish_user = utilities.user_data.username
        this.newsForm.user = utilities.user_data.userid
        this.newsForm.dep = utilities.user_data.deps.length > 0 ? utilities.user_data.deps[0].id : 0
        this.newsForm.type = this.modalData.newsType
        if (this.modalData.data) {
          this.newsForm.id = this.modalData.data.id || 0
          this.newsForm.html = this.modalData.data.html || ''
          // let idx = this.newsForm.html.indexOf('{')
          // while (idx >= 0) {
          //   let t = this.newsForm.content.substring(idx + 1, this.newsForm.content.indexOf('}'))
          //   this.newsForm.content = this.newsForm.content.replace('{' + t + '}', utilities.config.buffer_url + '/upload/' + t)
          //   idx = this.newsForm.content.indexOf('{')
          // }
          this.newsForm.topSwitch = this.modalData.data.top === 1
          this.newsForm.title = this.modalData.data.title || ''
        }
      }
    },
    validateNewsContent () {
      let images = []
      let html = ''.concat(this.newsForm.html)
      let idx = html.indexOf('<img')
      while (idx >= 0) {
        let img = html.substring(idx, html.indexOf('>', idx) + 1)
        let idx2 = img.indexOf('src="') + 5
        let data = img.substring(idx2, img.indexOf('"', idx2))
        if (data.indexOf('base64') >= 0) {
          images.push(data)
        }
        idx = html.indexOf('<img', idx + img.length)
      }
      return images
    },
    updateNewsContent (paths) {
      let html = ''.concat(this.newsForm.html)
      let idx = html.indexOf('<img')
      let i = 0
      while (idx >= 0) {
        let img = html.substring(idx, html.indexOf('>', idx) + 1)
        if (img.indexOf('base64') >= 0) {
          let imageHtml = '<img src="' + utilities.config.buffer_url + '/upload/' + paths[i++] + '" style="max-width: 100%;"/>'
          html = html.replace(img, imageHtml)
          idx = html.indexOf('<img', idx + imageHtml.length)
        } else {
          if (img.indexOf('style') < 0) {
            let imageHtml = img.substr(0, img.length - 1) + ' style="max-width: 100%;"/>'
            html = html.replace(img, imageHtml)
          }
          idx = html.indexOf('<img', idx + img.length)
        }
      }
      this.newsForm.content = html
    },
    editorRegister () {
      let size = Quill.import('attributors/style/size')
      size.whitelist = [ '10px', '12px', '14px', '16px', '18px', '20px' ]
      Quill.register(size, true)
      let fonts = ['SimSun', 'SimHei', 'Microsoft-YaHei', 'KaiTi', 'FangSong', 'Arial', 'Times-New-Roman', 'sans-serif',
        '宋体', '黑体'
      ]
      let font = Quill.import('formats/font')
      font.whitelist = fonts
      Quill.register(font, true)

      // let t = this.$refs.myQuillEditor.quill.getModule('toolbar')
      // t.addHandler('image', this.imageUpload)
    },
    imageUpload () {
      console.error(arguments)
    }
  }
}
</script>

<style lang="less" scoped>

</style>
