(function(){
	window.task = {
		__tasks: [],
		intervalIdx: -1,
		callInterface: function(option) {
			this.__tasks.push(option)
			
			this.interval()
		},
		interval: function(){
			if(!this._map) {
				if(this.intervalIdx == -1){
					this.intervalIdx = setInterval(function() { this.interval() }.bind(this), 500)
				}
				return;
			}
			else {
				if(this.intervalIdx != -1){
					clearInterval(this.intervalIdx);
					this.intervalIdx = -1;
				}
			}
			while(this.__tasks.length > 0) {
				var single = this.__tasks.splice(0, 1)[0]
				if(typeof single.callback == 'function' && this._list.hasOwnProperty(single.taskName)){
					var instance = new this._list[single.taskName](this._map, single.data)
					single.callback(instance)
				}
			}
		},
		_map: null,
		_list: {
			'sign': Sign,
			'track': Track,
			'efence': EFence
		}
	}

	window.onload = function() {
		var map = new BMap.Map('map',{
			minZoom: mapConfig.minZoom,
			maxZoom: mapConfig.maxZoom
		});

		var styleOptions = {
	        strokeColor: "#00DAFF",    //边线颜色。
	        fillColor: "#00DAFF",      //填充颜色。当参数为空时，圆形将没有填充效果。
	        strokeWeight: 4,       //边线的宽度，以像素为单位。
	        strokeOpacity: 1,    //边线透明度，取值范围0 - 1。
	        fillOpacity: 0.5,      //填充的透明度，取值范围0 - 1。
	        strokeStyle: 'solid' //边线的样式，solid或dashed。
	    }

	    var dm = new BMapLib.DrawingManager(map, {
	        isOpen: false, //是否开启绘制模式
	        enableDrawingTool: false, //是否显示工具栏
	        drawingToolOptions: {
	            anchor: BMAP_ANCHOR_TOP_RIGHT, //位置
	            offset: new BMap.Size(5, 5), //偏离值
	        },
	        circleOptions: styleOptions, //圆的样式
	        polylineOptions: styleOptions, //线的样式
	        polygonOptions: styleOptions, //多边形的样式
	        rectangleOptions: styleOptions //矩形的样式
	    });

	    map.drawTool = dm;
		map.addEventListener('onload', function(){
			task._map = map;

			map.removeEventListener('onload');
		});
		map.centerAndZoom(mapConfig.city);
		map.enableScrollWheelZoom();
		map.disableDoubleClickZoom();
	}

	function Sign(map, option) {
		this.initialize = function() {
			this._option = option;
			this._map = map;

			var arr= [];

			if(this._option.location1) {
				var pnt = new BMap.Point(this._option.location1.split(',')[0] * 1, this._option.location1.split(',')[1] * 1);
				arr.push(pnt);
				var marker = new BMap.Marker(pnt, {
					icon: new BMap.Icon('assets/location1.png', new BMap.Size(32, 32), {
						anchor: BMap.Size(32, 16)
					})
				});
				var content = "";
				if(this._option.photo){
					content += '<p><img src="' + this._option.photo + '" style="width: 300px;height: 375px;"></img></p>';
				}
				content += '<p><span style="font-weight: bolder;">人员名称:</span><span>' + this._option.name1 + '</span></p>';
				content += '<p><span style="font-weight: bolder;">地址:</span><span>' + this._option.address1 + '</span></p>';
				content += '<p><span style="font-weight: bolder;">签到时间:</span><span>' + this._option.time1 + '</span></p>';

				var infoWindow = new BMap.InfoWindow(content, { width: 300, title: '<span style="font-size: 18px; font-weight: bolder;">被点名人详情</span>' });
				marker.win = infoWindow;
				marker.addEventListener('click', function(){
					this.openInfoWindow(this.win);
				});
				this._map.addOverlay(marker);
				marker.openInfoWindow(infoWindow)
			}

			if(this._option.location2){
				pnt = new BMap.Point(this._option.location2.split(',')[0] * 1, this._option.location2.split(',')[1] * 1);
				arr.push(pnt);
				marker = new BMap.Marker(pnt, {
					icon: new BMap.Icon('assets/location2.png', new BMap.Size(32, 32), {
						anchor: BMap.Size(32, 16)
					})
				});
				var content = "";
				if(this._option.photo){
					content += '<p><img src="' + this._option.photo + '" style="width: 300px;height: 375px;"></img></p>';
				}
				content += '<p><span style="font-weight: bolder;">人员名称:</span><span>' + this._option.name2 + '</span></p>';
				content += '<p><span style="font-weight: bolder;">地址:</span><span>' + this._option.address2 + '</span></p>';
				content += '<p><span style="font-weight: bolder;">签到时间:</span><span>' + this._option.time2 + '</span></p>';

				var infoWindow = new BMap.InfoWindow(content, { width: 300, title: '<span style="font-size: 18px; font-weight: bolder;">点名人详情</span>' });
				marker.win = infoWindow
				marker.addEventListener('click', function(){
					this.openInfoWindow(this.win);
				});
				this._map.addOverlay(marker);
			}
			if(this._option.appointment){
				pnt = new BMap.Point(this._option.appointment.split(',')[0] * 1, this._option.appointment.split(',')[1] * 1);
				arr.push(pnt);
				// marker = new BMap.Circle(pnt, 50, {
				// 	strokeColor: '#ff0000',
				// 	fillColor: '#0000ff',
				// 	strokeOpacity: 0.8,
				// 	fillOpacity: 0.4,
				// 	strokeWeight: 1,
				// 	strokeStyle: 'dashed'
				// })
				marker = new BMap.Marker(pnt, {
					icon: new BMap.Icon('assets/location3.png', new BMap.Size(32, 32), {
						anchor: BMap.Size(32, 16)
					})
				});
				this._map.addOverlay(marker);
			}
			
			if(arr.length > 0){
				var view = this._map.getViewport(arr);
				this._map.centerAndZoom(view.center, view.zoom);
			}
		}
		this.show = function() {}
		this.initialize(map, option);
		return this;
	}

	function Track(map, option) {
		this.initialize = function(map, option) {
			this._map = map;
			this._option = option;
			var points = this._option.points;
			var start, end;

			var geo = [];
			for(var i = 0; i < points.length; i++){
				var pnt = new BMap.Point(points[i].x, points[i].y);
				geo.push(pnt);
				if(i == 0){
					start = pnt;
				}
				if(i == points.length - 1){
					end = pnt;
				}
			}

			var startMk = new BMap.Marker(start, {
				icon: new BMap.Icon('assets/start_point.png', new BMap.Size(32, 32), { anchor: BMap.Size(32, 16) }),
				title: '起点'
			})
			var endMk = new BMap.Marker(end, {
				icon: new BMap.Icon('assets/end_point.png', new BMap.Size(32, 32), { anchor: BMap.Size(32, 16) }),
				title: '终点'
			})
			var trackLine = new BMap.Polyline(geo, { strokeColor: '#84049b', strokeWeight: 4, strokeOpacity: 0.7 })
			this._map.addOverlay(startMk)
			this._map.addOverlay(endMk)
			this._map.addOverlay(trackLine)

			var view = this._map.getViewport(geo);
			this._map.centerAndZoom(view.center, view.zoom);

			this.timeIndex = 0;
			this.pointIndex = 0;
			this.marker = null;
		}
		this.play = function() {
			if(this.timeIndex){
				this.stop();
			}
			this.timeIndex = setInterval(this.pointMove.bind(this), 1000); 
		}.bind(this)
		this.pause = function() {
			if(this.timeIndex){
				clearInterval(this.timeIndex);
				this.timeIndex = null;
			}
		}.bind(this)
		this.stop = function() {
			if(this.timeIndex){
				clearInterval(this.timeIndex);
				this.timeIndex = null;
			}
			this.pointIndex = 0;
		}.bind(this)
		this.pointMove = function() {
			if(this.marker){
				this._map.removeOverlay(this.marker);
			}
			var pnt = new BMap.Point(this._option.points[this.pointIndex].x, this._option.points[this.pointIndex].y);
			this.marker = new BMap.Marker(pnt, {
				icon: new BMap.Icon('assets/track_via.png', new BMap.Size(64, 64), { anchor: BMap.Size(32, 32) }),
				title: this._option.points[this.pointIndex].time
			});
			this.marker.setLabel(new BMap.Label(this._option.points[this.pointIndex].time, {
				offset: new BMap.Size(-25, 70)
			}));
			this._map.addOverlay(this.marker);
			this._map.panTo(pnt);
			this.pointIndex++;
			if(this.pointIndex >= this._option.points.length){
				this.stop();
			}
		}.bind(this)
		this.initialize(map, option)
		return this;
	}

	function EFence(map, option) {
		this.initialize = (map, option) => {
			this._map = map;
			this._option = option;
		}

		this.refreshMap = (fences) => {
			this._map.clearOverlays()

			for(var i = 0; i < fences.length; i++) {
				var pointArr = []
				var points = fences[i].extent.split(';');
				for(var j = 0; j < points.length; j++) {
					pointArr.push(new BMap.Point(points[j].split(',')[0] * 1, points[j].split(',')[1] * 1));
				}

				var option = {}
				if(fences[i].type == '重点区域'){
					option.strokeColor = '#ff0000';
					option.fillColor = '#ff0000';
					option.strokeWeight = 2;
					option.strokeOpacity = 1;
					option.fillOpacity = 0.3;
				}
				else if(fences[i].type == '管控区域'){
					option.strokeColor = '#ffff00';
					option.fillColor = '#ffff00';
					option.strokeWeight = 2;
					option.strokeOpacity = 1;
					option.fillOpacity = 0.3;
				}
				var polygon = new BMap.Polygon(pointArr, option);
				polygon.fId = fences[i].id;
				this._map.addOverlay(polygon);
				var labelPnt = this._map.getViewport(pointArr).center;
				var label = new BMap.Label(fences[i].name, {
					position: labelPnt
				});
				this._map.addOverlay(label);
			}
		}

		this.highLight = (data) => {
			var overlays = this._map.getOverlays();
			for(var i = 0; i < overlays.length; i++) {
				if(overlays[i].fId == data.id){
					var points = overlays[i].getPath();
					var view = this._map.getViewport(points);
					this._map.centerAndZoom(view.center, view.zoom);
					break;
				}
			}
		}

		this.drawPolygon = (type, callback) => {
			this._map.drawTool.removeEventListener('overlaycomplete', this._drawEndCallback);
			this._callback = callback
            
            if(type == 'polygon'){
               this._map.drawTool.setDrawingMode(BMAP_DRAWING_POLYGON);
                this._map.drawTool.addEventListener('overlaycomplete', this._drawEndCallback);
                this._map.drawTool.open();
            }
		}

		this._drawEndCallback = (e) => {
			this._drawOverlay = e.overlay
			this._callback(e)
		}

		this.removeDrawOverlay = () => {
			if(this._drawOverlay) {
				this._map.removeOverlay(this._drawOverlay)
			}
		}

		this.initialize(map, option);
		return this;
	}
})();