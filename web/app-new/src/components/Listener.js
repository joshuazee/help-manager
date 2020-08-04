class Listener {
  constructor () {
    this.events = []
    this.current = 0
  }
  on (name, callback) {
    if (typeof callback === 'function') {
      this.events.push({
        id: ++this.current,
        name: name,
        callback: callback
      })
    }
  }
  un (id) {
    for (let i = 0; i < this.events.length; i++) {
      if (this.events[i].id === id) {
        this.events.splice(i, 1)
      }
    }
  }
  dispatch (name, ...rest) {
    for (let i = 0; i < this.events.length; i++) {
      if (this.events[i].name === name) {
        this.events[i].callback(rest)
      }
    }
  }
}

export default Listener
