
(function ($) {
    /**
     * @name Dialog
     * @class 弹出框,可拖拽，设置宽和高，有无遮罩，
     */
    $.uedWidget('ued.Dialog', {
        options: /**@lends Dialog# */
        {
            /**
             *传入的标题
             * @type string
             * @default null
             * @example
             * $("#dialog").uedDialog({ title :"string"}})
             */
            title: null,
            /**
             *下端按钮，如果有数值，则添加，默认没有按钮
             * @type object
             * @default null
             * @example
             *  $("#dialog").uedDialog({buttons:["false":function(){
             *     alert(2)
             * },"true":function(){
             *     alert(3)
             * }]})
             */
            buttons: null,
            /**
             *是否有遮罩，添加在body下,默认有
             * @type boolean
             * @default true
             * @example
             *  $("#dialog").uedDialog({modal:false})
             */
            modal: false,
            /**
             *定位,如果没有传入默认居中,以百分比为单位
             * @type object
             * @default null
             * @example
             * $("#dialog").uedDialog({position:[50,10]})
             */
            position: null,
            /**
             *dialog的宽度
             * @type int
             * @default 300
             * @example
             *  $("#dialog").uedDialog({width:500})
             */
            width: 300,
            /**
             *dialog的高度
             * @type int
             * @default 200
             * @example
             * $("#dialog").uedDialog({height:500})
             */
            height: 200,
            /**
             *是否可以拖拽
             * @type boolean
             * @default true
             * @example
             *  $("#dialog").uedDialog({isDrag:false})
             */
            isDrag: true,
            /**
             *默认打开
             * @type boolean
             * @default false
             * @example
             *  $("#dialog").uedDialog({autoOpen:false})
             */
            autoOpen: false,
            /**
             *提示类型"success","error","confirm","warning"
             * @type string
             * @default null
             * @example
             * $("#dialog").uedDialog({type:"success"})
             */
            type: null,
            /**
             *提示信息
             * @type string
             * @default null
             * @example
             * $("#dialog").uedDialog({message:"tishixingxing"})
             */
            message: null,
            /*
			 * type:confirm时,点击确定按钮的回调
			 */
            confirmCallback: null,
            /*
			 * 弹窗关闭后的回调,默认为null
			 */
            closeCallback: null,
            /*
             * 弹窗的定位方式，默认为true  
             */
            fixed: true
        },

        _create: function () {
            var mescope = this;
            if ($('body').width() == 0) {//当在页面a中用异步tab加页面b的iframe时，如果页面b中有pfTable,parent的宽度是0，结果是表头和body都不是100%，表头靠左，body水平居中--wxj20181218
                //console.info('wait');
                setTimeout(function () {
                    mescope._create.call(mescope);
                }, 100);
                return;
            }
            var $el = this.element, options = this.options, self = this;
            //debugger;
            //当弹窗宽高过大时(由于桌面分辨率太小所致),自动缩小弹窗--wxj20180719
            if (typeof options.width === 'string') { options.width = parseInt(options.width.replace('px', '')); }
            if (typeof options.height === 'string') { options.height = parseInt(options.height.replace('px', '')); }
            var ww = $(window).width() - 100;
            var wh = $(window).height() - 100;
            if (options.width > ww) { options.width = ww; }
            if (options.height > wh) { options.height = wh; }

            var _define = this._define = {};
            //创建弹窗节点
            this._createDialogDom($el, options, _define);
            //设置弹窗属性
            this._calDialogAttr($el, options, _define);
            //绑定弹窗事件
            this._bindEvent($el, options, _define);
        },

        _createDialogDom: function ($el, options, _define) {
            var $farther = _define.$farther = $el.parent(), buttons = options.buttons;
            var $dialog = _define.$dialog = $('<div class="ui-dialog-panel"><div class="ui-dialog"></div><div class="ui-dialog-leftYY"></div><div class="ui-dialog-rightYY"></div><div class="ui-dialog-bottomLeft"></div><div class="ui-dialog-bottomRight"></div><div class="ui-dialog-bottomCenter"></div></div>');
            //创建弹窗头部区域
            var $dialogHd = _define.$dialogHd = $('<div class="ui-dialog-hd"><h6></h6><a href="javascript:;" class="ui-dialog-close ue-state-default"><i></i></a></div>');
            //创建弹窗内容区域
            var $dialogContent = _define.$dialogContent = $el.addClass('ui-dialog-content');
            //由于ui-dialog-bottomCenter 有bottom: -13px; 导致其和 ui-dialog 之间有空隙,能看穿--wxj20180930
            $dialog.css('backgroundColor', 'white');
            if (options.type) {
                //创建提示型弹窗提示区域
                this._createMessageDom($el, $dialogContent, options);
            } else {
                //普通型弹窗内为iframe时，拖动卡顿
                if ($el.children().is('iframe')) {
                    var $contentMask = _define.$contentMask = $('<div style="position:absolute;top:45px;left:0;right:0;bottom:0;display:none"></div>');
                    $dialogContent.append($contentMask);
                } else {
                    $dialogContent.css('overflow', 'auto');//当内容过高时，父中的内容穿透上来了，倒不如让里面滚动--wxj20180724
                }
            }
            $el.css('display', 'block');//显示$el内容

            $dialog.children('.ui-dialog').append($dialogHd).append($dialogContent);
            //创建弹窗底部区域
            var $dialogFt = _define.$dialogFt = $('<div class="ui-dialog-ft"></div>');
            if (buttons) {
                var buttonArray = [];
                for (var i = 0, len = buttons.length; i < len; i++) {
                    var button = buttons[i], length = this._returnLetterLength(button.label);
                    length = length < 3 ? 3 : length;
                    if (button.recommend) {
                        buttonArray.push('<button type="button" class="ue-button-recommend ue-state-default long' + length + '">' + button.label + '</button>');
                    } else {
                        buttonArray.push('<button type="button" class="ue-button ue-state-default long' + length + '">' + button.label + '</button>');
                    }
                }
                $dialogFt.append(buttonArray.join(''));
                //$dialog.children().append($dialogFt);
                $dialog.children('.ui-dialog').append($dialogFt);
            }

            //判断是否需要生成遮罩
            //if(!options.modal){
            if (options.modal) {//原版的意思是当modal为true时不创建dragMask,在open时用$.mask.open(),但实测没效果,于是改为open时直接显示dragMask--wxj20180525
                //var $dragMask = _define.$dragMask = $('<div style="position: fixed;z-index:9999;top:0;left:0;right:0;bottom:0;display:none"></div>');
                var $dragMask = _define.$dragMask = $('<div class="diagMask" style="position: fixed;z-index:9999;top:0;left:0;right:0;bottom:0;display:none;width:100%;height:100%;background-color:black;opacity: 0.3;"></div>');//原版没背景色的mask没起作用
                $('body').append($dragMask);
            }

            //插入到$farther中去
            $farther.append($dialog.hide());
        },
        /*
		 * 创建提示型弹窗提示区域
		 */
        _createMessageDom: function ($el, $dialogContent, options) {
            var type = options.type, _define = this._define;

            _define.$dialog.addClass('ui-dialog-' + type);
            //创建节点
            if (options.type === 'confirm') {
                //加confirm的确认取消按钮--wxj20180524
                var str =
                '<div class="ui-dialog-icon"></div><div class="ui-dialog-text">' +
                    '<p class="dialog-content"><span class="dialog-text"></span></p>' +
                    '<p class="tips">如果是请点击“确定”，否则点“取消”</p>' +
                    '<br />' +
                    '<div class="buttons">' +
                        '<input type="button" class="button long2 ok" value="确定" />&nbsp;' +
                        '<input type="button" class="button long2 normal" value="取消" />' +
                    '</div>' +
			    '</div>';
                $dialogContent.empty().append(str);
            } else {
                $dialogContent.empty().append('<div class="ui-dialog-icon"></div><div class="ui-dialog-text"><span class="patch"></span><span class="dialog-text"></span></div>');//原版
            }

            var $dialogMessage = _define.$dialogMessage = $dialogContent.find('.dialog-text');
        },

        _calDialogAttr: function ($el, options, _define) {

            this.setTitle(options.title);
            if (!options.type) {
                //普通弹窗
                this.setWidth(options.width);
                this.setHeight(options.height);
            } else {
                //提示型弹窗
                this.setWidth(options.width);
                this.setHeight(options.height);
                //this.setWidth(355);
                //this.setHeight(145);
                //this.setWidth(400 > options.width?options.width:400);//原本355的宽度放不下,导致ui-dialog-text换行了.option.width是根据iframe的宽度调整过的--wxj20180523
                //this.setHeight(145 > options.height ? options.height : 145);
                if (400 > options.width) {
                    $el.find('div.ui-dialog-text').css('width', (options.width - 136) + 'px');//当窗口宽度小于400时,文字会换行,所以要缩小text宽度(text如果不设置宽度,直接就换行了).136是左边logo的宽度
                }
                this.setMessage(options.message);
            }
            //设置弹窗的位置
            this.setPosition(options.position);

            if (options.autoOpen) {
                this.open();
            }
        },

        _calLeft: function (_left, _define) {
            if (_left == 'left') {
                _define.left = 0;
            } else {
                //_define.left = ($(window).width() - _define.width) / 2;
                _define.left = ($(window).width() - _define.$dialog.width()) / 2;//原版的_define.width是undefined--wxj20180525
                _define.left = _define.left < 0 ? 0 : _define.left;
            }
        },

        _calTop: function (_top, _define) {
            if (_top == 'top') {
                _define.top = 0;
            } else {
                //_define.top = ($(window).height() - _define.height) / 2;
                _define.top = ($(window).height() - _define.$dialog.height()) / 2;//原版的_define.height是undefined--wxj20180525
                _define.top = _define.top < 0 ? 0 : _define.top;
            }
        },

        _bindEvent: function ($el, options, _define) {
            var self = this;
            //绑定关闭按钮事件
            _define.$dialogHd.children('.ui-dialog-close').click(function () {
                self.close();
            });

            //绑定弹窗拖拽事件
            if (options.isDrag) {
                self._dialogDrag(self, _define);
            }
            //绑定按钮点击事件
            var buttons = options.buttons;
            if (buttons) {
                _define.$dialogFt.bind('click', function (e) {
                    var $target = $(e.target), button = buttons[$target.index()];
                    if ($target.is('button')) {
                        if (!button.setFunc) {
                            button.callBack && button.callBack(self.element, $('#__mask', $('body')));
                        } else {
                            button.callBack && button.callBack(button.param);
                            self.close();
                        }
                    }
                });
            }
            if (options.type === 'confirm') {
                //debugger;
                _define.$dialogContent.find('input.ok').bind('click', function (e) {
                    options.confirmCallback.call(self);
                    self.close();
                });
                //_define.$dialogContent.find('input.ok').click(function (e) {
                //    options.confirmCallback.call(self.element);
                //    //debugger;
                //    //options.confirmCallback();
                //    self.close();
                //});
                _define.$dialogContent.find('input.normal').bind('click', function (e) {
                    self.close();
                });
            }
        },
        /*
		 * 弹窗拖动函数
		 * @param {Object} self 弹窗对象
		 * @param {Object} _define组件内部自定义对象。用来存放临时值 
		 */
        _dialogDrag: function (self, _define) {
            var $dragMask = _define.$dragMask, $contentMask = _define.$contentMask;
            var $dialogHd = _define.$dialogHd, $dialog = _define.$dialog, winWidth, winHeight;
            var left, top, dialogWidth = _define.width, dialogHeight = _define.height;
            var tempX, tempY;
            $dialogHd.css('cursor', 'move');
            $dialogHd.bind('mousedown', function (e) {
                dialogHeight = _define.height;//重新获得高--benjamin20190428
                //e.preventDefault();
                //获取最新值
                left = _define.left, top = _define.top, tempX = e.clientX, tempY = e.clientY, winWidth = $(window).width(), winHeight = $(window).height();

                if (!self.options.fixed) {//定位为absolute
                    winHeight = $(document).outerHeight();
                }
                $contentMask && $contentMask.show();
                //$dragMask && $dragMask.show();
                $dragMask && $dragMask.css('opacity', 0.4);//open时已打开,这里改为修改透明度--wxj20180525
                this.setCapture && this.setCapture();
                $(document).bind('mousemove', mouseMove).bind('mouseup', mouseUp);
            });

            var mouseMove = function (e) {
                _define.left = e.clientX - tempX + left;
                _define.top = e.clientY - tempY + top;
                _define.left = _define.left < 0 ? 0 : _define.left > (winWidth - dialogWidth) ? (winWidth - dialogWidth) : _define.left;
                _define.top = _define.top < 0 ? 0 : _define.top > (winHeight - dialogHeight) ? (winHeight - dialogHeight) : _define.top;
                $dialog.css({ 'left': _define.left, 'top': _define.top });
            }
            var mouseUp = function (e) {
                this.releaseCapture && this.releaseCapture();
                $contentMask && $contentMask.hide();
                //$dragMask && $dragMask.hide();
                $dragMask && $dragMask.css('opacity', 0.3);
                $(document).unbind('mousemove', mouseMove).unbind('mouseup', mouseUp);
            }
        },

        /*
		 * 给弹窗设置标题。
		 * @param {String or Object} _title 弹窗标题
		 */
        setTitle: function (_title) {
            var _define = this._define, $dialogHd = _define.$dialogHd;
            if (_title) {
                $dialogHd.find('h6').empty().append('<span class="dialog-hd-lc"></span>' + _title + '<span class="dialog-hd-rc"></span>');
            }
        },

        /*
		 * 给弹窗设置宽度
		 * @param {int} _width 弹窗宽度
		 */
        setWidth: function (_width) {
            var _define = this._define, $dialogContent = _define.$dialogContent;
            $dialogContent.width(_width);
            //存储弹窗宽度，以便后期计算位置
            _define.width = $dialogContent.outerWidth() + 10;
        },

        /*
		 * 给弹窗设置高度
		 * @param {int} _height 弹窗高度
		 */
        setHeight: function (_height) {
            var _define = this._define, $dialogContent = _define.$dialogContent;
            $dialogContent.height(_height);
            //存储弹窗高度，以便后期计算位置
            _define.height = $dialogContent.outerHeight() + 50;
        },
        /*
		 * 给提示型弹窗设置提示信息
		 * @param {String or Object} _message 提示弹窗的提示信息
		 */
        setMessage: function (_message) {
            if (!_message) return;
            var _define = this._define, $dialogMessage = _define.$dialogMessage;
            $dialogMessage.empty().append(_message);
        },
        /*
		 * 设置弹窗的位置
		 */
        setPosition: function (_position) {
            if (!_position) {
                _position = ['center', 'center'];
            }
            var _define = this._define, left = _position[0], top = _position[1], options = this.options;
            _define.left = left, _define.top = top;
            if (typeof left != 'number') {
                //计算弹窗的left值
                this._calLeft(left, _define);
            }

            if (typeof top != 'number') {
                //计算弹窗的left值
                this._calTop(top, _define);
            }

            //赋值给弹窗
            if (options.fixed) {
                _define.$dialog.css({ 'position': 'fixed', 'left': _define.left, 'top': _define.top });
            } else {
                _define.$dialog.css({ 'position': 'absolute', 'left': _define.left, 'top': _define.top });
            }
        },

        open: function () {
            var _define = this._define, $dialog = _define.$dialog, options = this.options, isMask = options.modal;
            //isMask && $.mask.open('ued-dialog');
            isMask && _define.$dragMask.show();//上句报错--wxj20180525
            if (!options.fixed && options.position[1] == 'center') {
                //如果弹窗不是fiexd定位，且top为center时，打开前需要+scrollTop
                _define.top += $(window).scrollTop();
                $dialog.css('top', _define.top);
            }
            $dialog.show();
        },

        close: function () {
            var _define = this._define, $dialog = _define.$dialog, closeCallback = this.options.closeCallback, isMask = this.options.modal;
            //isMask && $.mask.close('ued-dialog');
            isMask && _define.$dragMask.hide();//上句报错--wxj20180525
            $dialog.hide();
            closeCallback && closeCallback.call(this.element, this.element, $('#__mask', $('body')));
            if ($pf !== undefined && typeof $pf.clearIframe == 'function') { $pf.clearIframe(_define.$dialogContent.children('iframe')); }//如果不清除iframe,下次打开时会显示旧内容--wxj20181023
            this.element.trigger('popupsClosed');
            //alert('popupsClosed');
        },
        _init: function () {
            // var self = this, options = this.options, $el = this.element;
        },
        _returnLetterLength: function (_str) {
            return _str.replace(/[a-z0-9]{2}/ig, 'a').length;
        },
        remove: function () {
            this._define.$dialog.remove();
            $.mask.close('ued-dialog');
            this._define = null;
        },
        /*
         * 用于修改第一个button的默认函数 addBy xzjiang,update rbai
         * @param {Object} _func 函数体
         * @param {string} _param
		 */
        setFunc: function (_func, _param) {

            var self = this, options = this.options, firstButton = options.buttons[0];
            firstButton.callBack = _func;
            firstButton.param = _param;
            firstButton.setFunc = true;
        }
    });
})(jQuery);


(function ($) {
    /**
	 * select 下拉选择框渲染组件
	 * author: yswang
	 * version: 1.0
	 */
    $.fn.iSelect = function () {
        return this.each(function () {
            var _iSel = new $.iSelect(this);
            _iSel = null;
        });
    };

    $.iSelect = function (select) {
        if (!select || select.multiple) {
            return false;
        }

        var isel_id = select.id;
        if (!isel_id || isel_id === "" || typeof isel_id == "undefined") {
            isel_id = "iSel-" + Math.round(Math.random() * 10000);
            select.id = isel_id;
        }
        if (document.getElementById("_iSelWrap_" + isel_id)) {
            return false;
        }

        var $select = $(select), sel_w = select.offsetWidth;
        $select.addClass("iselect");
        var _onchange = select.onchange;

        if (sel_w >= 500) {
            sel_w = 500;
        }

        var editable = $select.attr('editable') === 'editable';

        var isel_wrap = '<div id="_iSelWrap_' + isel_id + '" class="iselect-wrapper"></div>';
        var isel_wrapin = editable ?
             '<input  id="_iSelVal_' + isel_id + '" class="iselwrap-val" type="text" style="margin:0px;padding:0px 10px;border:0px;z-index:3;position:absolute;left:0px" />' :
            '<span id="_iSelVal_' + isel_id + '" class="iselwrap-val"></span>';

        // 包裹select
        $select.wrap(isel_wrap);
        $("#_iSelWrap_" + isel_id).append(isel_wrapin);

        var iSel_val = $("#_iSelVal_" + isel_id);

        function setISelectValText(text) {
            if (editable) {
                iSel_val.val(text);
            } else {
                iSel_val.text(text);
            }
        }

        select.onchange = null;
        $select.change(function () {
            //console.info(1);
            var _val = this.options[this.selectedIndex].text;
            //iSel_val.text(_val);
            setISelectValText(_val);

            // 执行可能存在的原始onchange事件
            if (_onchange && typeof _onchange != "undefined") {
                _onchange.apply(this);
            }
        });

        // 初始下显示select默认值
        var sel_option = $select.find("option:selected").first();
        if (sel_option.length <= 0) {
            sel_option = $select.find("option:first");
        }

        iSel_val.text(sel_option.text());

        var $isel_wrap = $("#_iSelWrap_" + isel_id);
        var w = $select.width();//根据select的宽度设置外层宽度,参照原框架原则:E:\svn\businessSys2018\yjquery.Web\Content\static\system\my_info.html 选地址的宽度
        if (w !== null && w !== undefined) {
            //$isel_wrap.width(w - 27);
            $isel_wrap.width(w - 20);//当页面控件的宽度设为auto时,发现原本减的27太大了,会换行.20为.iselect-wrapper上的padding-right--wxj20181017
        }

        if (editable) {
            iSel_val.width(w - 35);
            iSel_val.height($isel_wrap.height());

            iSel_val.attr('name', $select.attr('name'));
            $select.attr('name', $select.attr('name') + '_select');
        }

        //在原生对象中加入此方法来改变select的选项--wxj20180817
        select.selectOption = function (value, text) {
            var _text = '';
            if (value !== null && value !== undefined) {
                $select.val(value);
                _text = $select.find("option[value=" + value + "]").text();
            } else {
                var _val = $select.find("option").filter(function () { return $(this).text() === text; }).val();
                $select.val(_val);
                _text = text;
            }
            iSel_val.text(_text);

            ////当使用了 时,要把隐藏字段的值也设置,但未测试先不启用--benjamin todo
            //var hideFId = $select[0].getAttribute('id') + '_hidden';
            //var hideF = document.getElementById(hideFId);
            //if (hideF === null || hideF === undefined) {
            //} else {
            //    hideF.value = $select[0].value;
            //}
        };
        //替换原生的empty方法--benjamin20190927
        select.pfEmpty = function (emptyText) {
            $select.empty();
            $("#_iSelVal_" + isel_id).text(emptyText || '');
        };
    };

})(jQuery);
function setSelectVal(selectId, val) {
    $("#_iSelVal_" + selectId).text(val);
}