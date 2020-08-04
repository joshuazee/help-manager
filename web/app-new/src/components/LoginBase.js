import utilities from './utilities'
import layout from '../views/layout'
import notFound from '../views/notFound'
import index from '../views/index'
import news from '../views/news'
import eduVideo from '../views/edu-video'
import community from '../views/community'
import helpRecord from '../views/help-record'
import sign from '../views/sign'
import track from '../views/track'
import urinalysis from '../views/urinalysis'
import report from '../views/report'
import eleFence from '../views/ele-fence'
import departmentUser from '../views/department-user'
import role from '../views/role'

export default {
  data () {
    return {
      loginForm: {
        account: '',
        password: ''
      },
      loginRules: {
        account: [{
          required: true, message: '请输入手机号', trigger: 'blur'
        }],
        password: [{
          required: true, message: '请输入密码', trigger: 'blur'
        }]
      },
      defaultRoutes: [{
        code: 'xwkb',
        title: '新闻快报',
        path: '/a/news',
        component: news
      }, {
        code: 'jysp',
        title: '教育视频',
        path: '/a/edu-video',
        component: eduVideo
      }, {
        code: 'sqxxs',
        title: '社区新鲜事',
        path: '/a/community',
        component: community
      }, {
        code: 'bfjl',
        title: '帮扶记录',
        path: '/a/help-record',
        component: helpRecord
      }, {
        code: 'qdcx',
        title: '签到查询',
        path: '/a/sign',
        component: sign
      }, {
        code: 'rygj',
        title: '人员轨迹',
        path: '/a/track',
        component: track
      }, {
        code: 'njcx',
        title: '尿检查询',
        path: '/a/urinalysis',
        component: urinalysis
      }, {
        code: 'dpbb',
        title: '大屏报表',
        path: '/a/report',
        component: report
      }, {
        code: 'bmrygl',
        title: '部门/人员管理',
        path: '/a/department-user',
        component: departmentUser
      }, {
        code: 'jsgl',
        title: '角色管理',
        path: '/a/role',
        component: role
      }, {
        code: 'dzwl',
        title: '电子围栏',
        path: '/a/ele-fence',
        component: eleFence
      }]
    }
  },
  methods: {
    login () {
      if (this.input_check() === true) {
        this.$http.post(utilities.config.service_base + utilities.config.permission_url + '/login3', {
          mobile: this.loginForm.account,
          password: this.loginForm.password
        }).then((response) => {
          if (response.success === true && response.message === '') {
            utilities.user_data = response.data
            this.refreshRoutes(utilities.user_data.menus)
            this.save_login()
            this.$router.push('/a/index')
          } else {
            this.$Message.warning(response.message)
          }
        }).catch((error) => {
          console.error(error)
        })
      }
    },
    check_login: function () {
      let s = localStorage.getItem('help_manager_user')
      if (s) {
        return JSON.parse(localStorage.getItem('help_manager_user'))
      }
      return ''
    },
    save_login: function () {
      localStorage.setItem('help_manager_user', JSON.stringify(this.loginForm))
    },
    input_check: function () {
      if (!this.loginForm.account || !this.loginForm.password) {
        this.$Message.warning('请输入账号/密码')
        return false
      } else {
        return true
      }
    },
    refreshRoutes: function (menus) {
      let root = {
        path: '/a',
        component: layout,
        children: [{
          path: '/a/index',
          component: index,
          title: '首页'
        }]
      }
      let nf = {
        path: '*',
        name: 'Page not found',
        title: '页面丢了',
        component: notFound
      }

      let stack = [].concat(menus)
      while (stack.length > 0) {
        let menu = stack.pop()
        if (menu.children && menu.children.length > 0) {
          stack = stack.concat(menu.children)
        } else if (menu.code) {
          for (let i = 0; i < this.defaultRoutes.length; i++) {
            if (this.defaultRoutes[i].code === menu.code) {
              root.children.push(this.defaultRoutes[i])
              break
            }
          }
        }
      }
      root.children.push(nf)
      this.$router.addRoutes([root])
    }
  },
  beforeMount () {
    let lf = this.check_login()
    if (lf && lf.hasOwnProperty('account') && lf.hasOwnProperty('password')) {
      this.loginForm.account = lf.account
      this.loginForm.password = lf.password

      this.login()
    }
  }
}
