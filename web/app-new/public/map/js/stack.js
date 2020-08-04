(function($){
	window.task = $
})({
	__tasks: [],
	intervalIdx: -1,
	push: function(option) {
		this.__task.push(option);

		this.pop();
	}.bind(this),
	pop: function() {
		if(!this._map) {
			if(this.intervalIdx == -1){
				intervalIdx = setInterval(this.pop, 500)
			}
			return;
		}
		else {
			clearInterval(this.intervalIdx);
			this.intervalIdx = -1;
		}
		var option = this.__task.pop()
		return this._interface[option.func].call(this)
	}.bind(this)
	// _interface: {
	// 	sign: function(param) {
	// 		this.photos = param.photos;

	// 		this.show = function() {
	// 			this.map
	// 		}.bind(this)
	// 		return this;
	// 	},
	// 	track: function(param) {
	// 		this.__points = param.points;

	// 		this.play = function() {}
	// 		this.pause = function() {}
	// 		this.stop = function() {}
	// 		return this;
	// 	}
	// }
})