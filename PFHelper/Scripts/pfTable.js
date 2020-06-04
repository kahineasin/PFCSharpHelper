
/*
*分页组件，来自sapar项目
*/
jQuery.fn.pagination = function (maxentries, opts) {
    opts = jQuery.extend({
        items_per_page: 10, // 每页显示多少条记录
        current_page: 0,      //当前页码
        num_display_entries: 4, // 中间显示页码的个数
        num_edge_entries: 2, // 末尾显示页码的个数
        link_to: "javascript:;",         //页码点击后的链接
        prev_text: "&lt;&nbsp;上一页",   //上一页的文字
        next_text: "下一页&nbsp;&gt;",	   //下一页的文字
        ellipse_text: "...",  //页码之间的省略号
        display_msg: true, // 是否显示记录信息
        prev_show_always: true, //是否总是显示最前页
        next_show_always: true,//是否总是显示最后页
        setPageNo: false,//是否显示跳转第几页
        callback: function () {
            return false;
        } // 回调函数
    }, opts || {});

    return this.each(function () {
        // 总页数
        function numPages() {
            return Math.ceil(maxentries / opts.items_per_page);
        }
        /**
		 * 计算页码
		 */
        function getInterval() {
            var ne_half = Math.ceil(opts.num_display_entries / 2);
            var np = numPages();
            var upper_limit = np - opts.num_display_entries;
            var start = current_page > ne_half ? Math.max(Math.min(current_page
									- ne_half, upper_limit), 0) : 0;
            var end = current_page > ne_half ? Math.min(current_page + ne_half,
					np) : Math.min(opts.num_display_entries, np);
            return [start, end];
        }

        /**
		 * 点击事件
		 */
        function pageSelected(page_id, evt) {
            var page_id = parseInt(page_id);
            current_page = page_id;
            drawLinks();
            var continuePropagation = opts.callback(page_id, panel);
            if (!continuePropagation) {
                if (evt.stopPropagation) {
                    evt.stopPropagation();
                } else {
                    evt.cancelBubble = true;
                }
            }
            return continuePropagation;
        }

        /**
		 * 链接
		 */
        function drawLinks() {
            panel.empty();
            $pagin.empty();
            panel.append($pagin);
            var interval = getInterval();
            var np = numPages();
            var getClickHandler = function (page_id) {
                return function (evt) {
                    return pageSelected(page_id, evt);
                }
            }
            var appendItem = function (page_id, appendopts) {
                page_id = page_id < 0 ? 0 : (page_id < np ? page_id : np - 1);
                appendopts = jQuery.extend({
                    text: page_id + 1,
                    classes: ""
                }, appendopts || {});
                if (page_id == current_page) {
                    var lnk = $("<span class='current'>" + (appendopts.text)
							+ "</span>");
                } else {
                    var lnk = $("<a>" + (appendopts.text) + "</a>").bind(
							"click", getClickHandler(page_id)).attr('href',
							opts.link_to.replace(/__id__/, page_id));

                }
                if (appendopts.classes) {
                    lnk.addClass(appendopts.classes);
                }
                $pagin.append(lnk);
            }
            // 上一页
            if (opts.prev_text && (current_page > 0 || opts.prev_show_always)) {
                appendItem(current_page - 1, {
                    text: opts.prev_text,
                    classes: "prev"
                });
            }
            // 点点点
            if (interval[0] > 0 && opts.num_edge_entries > 0) {
                var end = Math.min(opts.num_edge_entries, interval[0]);
                for (var i = 0; i < end; i++) {
                    appendItem(i);
                }
                if (opts.num_edge_entries < interval[0] && opts.ellipse_text) {
                    jQuery("<span>" + opts.ellipse_text + "</span>")
							.appendTo($pagin);
                }
            }
            // 中间的页码
            for (var i = interval[0]; i < interval[1]; i++) {
                appendItem(i);
            }
            // 最后的页码
            if (interval[1] < np && opts.num_edge_entries > 0) {
                if (np - opts.num_edge_entries > interval[1]
						&& opts.ellipse_text) {
                    jQuery("<span>" + opts.ellipse_text + "</span>")
							.appendTo($pagin);
                }
                var begin = Math.max(np - opts.num_edge_entries, interval[1]);
                for (var i = begin; i < np; i++) {
                    appendItem(i);
                }

            }
            // 下一页
            if (opts.next_text
					&& (current_page < np - 1 || opts.next_show_always)) {
                appendItem(current_page + 1, {
                    text: opts.next_text,
                    classes: "next"
                });
            }
            // 记录显示
            if (opts.display_msg) {
                if (!maxentries) {
                    panel
						.append('<div class="pxofy">暂时无数据可以显示</div>');
                } else {
                    panel
                            .append('<div class="pxofy">显示第&nbsp;'
                                    + ((current_page * opts.items_per_page) + 1)
                                    + '&nbsp;条到&nbsp;'
                                    + (((current_page + 1) * opts.items_per_page) > maxentries
                                            ? maxentries
                                            : ((current_page + 1) * opts.items_per_page))
                                    + '&nbsp;条记录，总共&nbsp;' + maxentries + '&nbsp;条</div>');
                }
            }
            //设置跳到第几页
            if (opts.setPageNo) {
                $("<div class='goto'><span class='text'>转到第</span><input type='text'/><span class='page'>页</span><a href='javascript:;'>转</a></div>").insertBefore($pagin);
            }
        }

        // 当前页
        var current_page = opts.current_page;
        maxentries = (maxentries < 0) ? 0 : maxentries;
        opts.items_per_page = (!opts.items_per_page || opts.items_per_page < 0)
				? 1
				: opts.items_per_page;
        var panel = jQuery(this),
			$pagin = $('<div class="pagin-list"></div>');


        this.selectPage = function (page_id) {
            pageSelected(page_id);
        }
        this.prevPage = function () {
            if (current_page > 0) {
                pageSelected(current_page - 1);
                return true;
            } else {
                return false;
            }
        }
        this.nextPage = function () {
            if (current_page < numPages() - 1) {
                pageSelected(current_page + 1);
                return true;
            } else {
                return false;
            }
        }

        if (maxentries == 0) {
            panel.append('<span class="current prev">' + opts.prev_text + '</span><span class="current next">' + opts.next_text + '</span><div class="pxofy">暂时无数据可以显示</div>');
        } else {
            drawLinks();
        }
        $(this).find(".goto a").on("click", function (evt) {
            var setPageNo = $(this).parent().find("input").val();
            if (setPageNo != null && setPageNo != "" && setPageNo > 0 && setPageNo <= numPages()) {
                pageSelected(setPageNo - 1, evt);
            }
        });
    });
};//这里有了分号,否则有问题--wxj20180511



/*
* table组件扩展
* opts属性说明：
* button 自定义按钮，数组，如 [{text:,action:},'excel']
* selectMode:0不能选择 1单选 2多选,默认0,我增加的属性--benjamin
* rowDrag:bool 行可拖动排序--benjamin20190430
*/
jQuery.fn.pfTable = function (opts) {//设置DataTables组件的一些属性,要先引用相关资源--wxj20180515
    var me = this;
    var $pfTable = me;
    $pfTable.addClass('stripe');//当bAutoWidth为false时，有奇偶行样式(多了display这个class名)，但当为true时display这个class名没有加进入，原因未明.不过stripe也是DataTables自带样式，利用了--wxj20180830
    var result = {};
    result.tableApi = null;

    opts = opts || {};
    var hasSummary = false;
    var paginationOpts = null;
    var showPagination = opts.showPagination !== false;
    var selectMode = opts.selectMode || 0;
    var selectable = selectMode !== 0;
    var rowDrag = opts.rowDrag || false;//行可拖动排序,其实全部可拖也没什么影响--benjamin20190430
    var hasFixedColumns = opts.fixedColumns !== undefined && opts.fixedColumns !== false;//有锁定列--benjamin20200401
    //if (opts.pagination !== null && opts.pagination !== undefined) {
    //    paginationOpts = jQuery.extend({
    //        id: '',
    //        pageIndex: 0,
    //        pageSize: 10
    //    }, opts.pagination || {});
    //    delete opts.pagination;
    //    //showPagination =true;
    //}
    if (showPagination) {
        paginationOpts = jQuery.extend({
            id: $pfTable[0].id + '-pagin',
            pageIndex: 0,
            pageSize: 10
        }, opts.pagination || {});
        delete opts.pagination;
        //showPagination =true;
        if ($('#' + paginationOpts.id).length < 1) {
            $pfTable.after($('<div id="' + paginationOpts.id + '" class="pagination"></div>'));
        }
    }
    var _url = '';//调用loadUrl()时赋值(不包含pageIndex和pageSize参数)
    var _lastSort = '';//因为每次draw都触发order,所以自行判断
    var _lastFilterValue = '';
    var _hasGroupBtn = false;
    var _cacheKey = opts.cacheKey;
    var _dataTotal = -1;//总行数,为了判断超过100万行要分excel下载
    //var privateObj = {};//私有变量
    var userLoadOpts = {};//用户调用loadUrl方法后需要保存的参数--benjamin20190419

    function bindCellClick() {
        for (var i in opts.onCellClick) {
            if (opts.onCellClick.hasOwnProperty(i)) {
                $pf.bindTableColumnClick(me, i, opts.onCellClick[i]);
            }
        }
    }
    var $headTable = {};

    //function exchangeRow(row1, row2) {//交换行
    //    var data = result.tableApi.row(row1).data();
    //    //var nextRow = rows.parent().children()[idx - 1];
    //    //var nextIds = result.tableApi.row(row2).index();
    //    var nextData = result.tableApi.row(row2).data();

    //    result.tableApi.row(row2).data(data);
    //    result.tableApi.row(row1).data(nextData);
    //}

    function fixSelect(body, cur, isSelected) {
        var tr = body.find('tr:eq(' + cur + ')');
        if (isSelected) {
            body.find('tr input.pf-row-select:eq(' + cur + ')').prop("checked", true);
            if (!tr.hasClass('selected')) {
                tr.addClass('selected');
            }
        } else {
            if (tr.hasClass('selected')) {
                tr.removeClass('selected');
            }
        }
    }
    //orders,如:[3,1,2]
    result.orderRow = function (sOrders) {
        var body = $pfTable.find('tbody');
        var maxIdx = body.find('tr').length - 1;
        if (sOrders.length !== (maxIdx + 1)) { return; }

        var orders = [];
        //for (var i = 0; i < sOrders.length; i++) {//输入参数有可能是string[],但需要的是int[]
        //    orders.push(typeof sOrders[i] === 'string' ? parseInt(sOrders[i]) : sOrders[i]);
        //}

        var oldRows = [];//[{selected:true,}]只存Data
        var selected = [];
        for (var i = 0; i <= maxIdx; i++) {
            //debugger;
            orders.push(typeof sOrders[i] === 'string' ? parseInt(sOrders[i]) : sOrders[i]);//输入参数有可能是string[],但需要的是int[]

            //var oi = typeof orders[i] === 'string' ? parseInt(orders[i]) : orders[i];
            var row = body.find('tr:eq(' + i + ')');
            if (orders[i] !== i) {//有变动才赋值,增加性能
                oldRows.push(result.tableApi.row(row[0]).data());
            } else {
                oldRows.push({});
            }
            if (selectable) {
                selected.push(row.find('input.pf-row-select').is(':checked'));
            }
        }
        for (var i = 0; i <= maxIdx; i++) {
            if (orders[i] !== i) {//有变动才赋值,增加性能
                var row = body.find('tr:eq(' + i + ')');
                result.tableApi.row(row[0]).data(
                    oldRows[orders[i]]
                );
                fixSelect(body, i, selected[orders[i]]);
            }
        }
        //for (var i = 0; i <= maxIdx; i++) {
        //    fixSelect(body,i, selected[i]);
        //}
    };
    function insertRow(srcRowIdx, dstRowIdx) {//src插到dst后面(如果这个方法维护麻烦,可以简接调用result.orderRow)
        var body = $pfTable.find('tbody');
        var srcRow = body.find('tr:eq(' + srcRowIdx + ')');
        var srcData = result.tableApi.row(srcRow[0]).data();
        var cur = srcRowIdx;
        var maxIdx = body.find('tr').length - 1;
        if (maxIdx < srcRowIdx) { return; }
        if (maxIdx < dstRowIdx) { dstRowIdx = maxIdx; }

        var selected = [];
        if (selectable) {//调换行时,选择的checkbox会清掉
            while (cur !== dstRowIdx) {
                var next = cur + (srcRowIdx > dstRowIdx ? -1 : 1);

                selected.push(body.find('tr input.pf-row-select:eq(' + cur + ')').is(':checked'));
                cur = next;
            }
            //for (var i = srcRowIdx; i <= dstRowIdx; i++) {
            //    selected.push(body.find('tr input.pf-row-select:eq(' + i + ')').is(':checked'));
            //}
        }
        selected.push(body.find('tr input.pf-row-select:eq(' + dstRowIdx + ')').is(':checked'));

        //console.info(selected);
        var cur = srcRowIdx;
        var i = 0;
        while (cur !== dstRowIdx) {
            var next = cur + (srcRowIdx > dstRowIdx ? -1 : 1);
            //下一行的数据填到cur行;
            result.tableApi.row(body.find('tr:eq(' + cur + ')')[0]).data(
                result.tableApi.row(body.find('tr:eq(' + next + ')')[0]).data()
            );
            if (selectable) {
                fixSelect(body, cur, selected[i + 1]);
                i++;
            }
            //console.info(cur);
            cur = next;
        }
        ////src填到dst行
        result.tableApi.row(body.find('tr:eq(' + dstRowIdx + ')')[0]).data(
            srcData
        );
        if (selectable) {
            fixSelect(body, dstRowIdx, selected[0]);
        }
    }
    var defaultDrawCallback = function () {
        //if (selectMode ===0) {
        //    me.find("tbody tr:odd").css("backgroundColor", "#eff6fa");//奇偶行样式,直接写在dom上优先级太高,选择行的样式都不生效，改为复写odd的背景色好了--wxj20180830
        //    me.find("tbody tr:even").css("backgroundColor", "white");
        //}
        me.find('tbody tr td.text-right').css("text-align", "right");//网上建议createdRow fnRowCallback (https://bbs.csdn.net/topics/391050901)

        //修复一个bug,当无数据时再变为有数据,这里虽然加了tfoot但初始化后没有去掉no-footer这个类导致多了个border-bottom(最好以后看看api对象时的foot属性是不是在destroy后并没有重置)
        if (hasSummary) {
            var $wrapper = $('#' + $pfTable[0].id + '_wrapper');
            var headInner = $wrapper.find('div.dataTables_scrollHeadInner table.dataTable');
            if (headInner.hasClass('no-footer')) {
                headInner.removeClass('no-footer');
            }
            var footInner = $wrapper.find('div.dataTables_scrollFootInner table.dataTable');
            if (footInner.hasClass('no-footer')) {
                footInner.removeClass('no-footer');
            }
            var scrollBody = $wrapper.find('div.dataTables_scrollBody table.dataTable');
            if (scrollBody.hasClass('no-footer')) {
                scrollBody.removeClass('no-footer');
            }
        }
        bindCellClick();

        if (selectable) {
            $headTable = $pfTable.parent().parent().find('.dataTables_scrollHead table');
            if ($headTable.attr('selectAllBinded') !== 'true') {
                $headTable.on('click', 'thead tr input.pf-row-select-all', function (e) {
                    $headTable.attr('selectAllBinded', 'true');
                    //alert(1);
                    $pf.stopPropagation(e);
                    //debugger;
                    var b = $(this).is(':checked');
                    //if (b === true) {

                    //}
                    var cbs = $pfTable.find('tbody tr input.pf-row-select');
                    cbs.each(function (index, element) {
                        var cb = $(element);
                        if (cb.is(':checked') !== b) { cb.click(); }
                    });

                });
            }
        }
        //搜索栏改为后端
        var searchF = $pfTable.parents('.dataTables_wrapper').find('.dataTables_filter input[type=search]');
        //debugger;
        searchF.unbind();
        searchF.change(function () {
            //alert(this.value);
            _lastFilterValue = this.value;
            //var lastFilterValue = this.value;
            var url = $pf.setUrlParams(_url, { filterValue: this.value });
            //var tmpOpts = jQuery.extend({ needSetLastFilterValue: true }, userLoadOpts);
            result.sysLoadUrl(url, { needSetLastFilterValue: true });

            //不用下面ajax方式的原因是,如果后面无数据时返回了false,DataTable会报错
            //var url = result.tableApi.ajax.url($pf.setUrlParams(_url, { PageSize: opts.pagination.pageSize, PageIndex: 0, sort: _lastSort ,filterValue:this.value}));
            //url.load(function (data) {
            //    ////debugger;
            //    //if (selectMode === 2) {
            //    //    $headTable.find('thead tr input.pf-row-select-all').prop("checked", false);
            //    //}
            //    bindPaging(data.total, 0);
            //    //searchF.val(_lastFilterValue);
            //});
        });

        if (rowDrag) {
            function allowDrop(ev) {
                ev.preventDefault();
            }
            function drag(ev) {
                var idx = $(ev.target).parent().find('tr').index(ev.target);
                //ev.originalEvent.dataTransfer.setData("dragRowIdx", idx);
                ev.originalEvent.dataTransfer.setData("Text", idx.toString());//ie中,第一个参数必需为Text,第二个必需为String
            }
            function drop(ev) {
                ev.preventDefault();
                //var dragRowIdx = parseInt(ev.originalEvent.dataTransfer.getData("dragRowIdx"));
                var dragRowIdx = parseInt(ev.originalEvent.dataTransfer.getData("Text"));
                var dragRow = $pfTable.find('tbody tr:eq(' + dragRowIdx + ')');
                var targetRow = $(ev.target).parent('tr');
                var targetRowIdx = targetRow.parent().find('tr').index(targetRow);
                if (dragRowIdx === targetRowIdx) {
                    //console.info('相同');
                } else {
                    insertRow(dragRowIdx, targetRowIdx);
                    //exchangeRow(dragRow, targetRow);
                }
            }
            $pfTable.find('tbody tr').attr('draggable', true)
            .on('dragover', allowDrop)
            .on('dragstart', drag)
            .on('drop', drop);
        }
        //debugger;
        if (_hasGroupBtn) {
            $pf.post('GetGroupColumnCache', { cacheKey: _cacheKey }, function (data) {
                //var cbx = me.parent().parent().parent().find('.dt-buttons span').filter(function () { return $(this).text().indexOf("设置汇总列") > -1; })
                //    .find('input[type=checkbox]');
                var cbx = me.parents('.dataTables_wrapper').find('.dt-buttons span').filter(function () { return $(this).text().indexOf("设置汇总列") > -1; })
                    .find('input[type=checkbox]');//之前3个parent的方式不准确,在有锁定列时会多一层--benjamin20191009
                if (data.Result) {
                    cbx.prop("checked", true);
                } else {
                    cbx.prop("checked", false);
                }
            });
        }
    };
    //debugger;
    var drawCallback = null;
    if (typeof opts.drawCallback == 'function') {
        var userDrawCallback = opts.drawCallback;
        drawCallback = function () {
            var drawMe = this;
            defaultDrawCallback();
            userDrawCallback.call(drawMe);
        }
        delete opts.drawCallback;
    } else {
        drawCallback = defaultDrawCallback;
    }

    //var buttons = $pf.getTableToolbar(opts);
    var buttons = [];
    var excelBtn = {
        text: '导出Excel', action: function (event) {
            var url = _url;
            if (url[0] !== '/') {//如果是相对地址,要转为绝对地址
                url = window.location.pathname.replace(/[^/]*$/, url)
            }
            //var batchMax = 1000000;//excel每个sheet最多100万条.100万时,后台会报OutOfMemory错误(调试时)
            var batchMax = 500000;
            var isHugeData = _dataTotal > batchMax;
            if (isHugeData) {
                $pf.confirmPopups("数据量超过" + $pf.num2e(batchMax) + "行,由于excel限制,将分开多个excel下载,确定吗?", function () {
                    var batch = Math.ceil(_dataTotal / batchMax);
                    for (var i = 0; i < batch; i++) {

                        url = $pf.setUrlParams(url, { sort: _lastSort, filterValue: _lastFilterValue, PageSize: batchMax, PageIndex: i });
                        $pf.exporter({
                            title: null,
                            dataAction: url,
                            downloadBatch: i//有的浏览器,同名文件不能导出 
                        })
                        .download($(event.currentTarget).attr("suffix"))
                        ;
                    }
                });
            } else {

                url = $pf.setUrlParams(url, { sort: _lastSort, filterValue: _lastFilterValue });
                $pf.exporter({
                    title: null,
                    dataAction: url
                })
                .download($(event.currentTarget).attr("suffix"))
                ;
            }
        }
    };
    var addBtn = {
        text: '新增', alwayShow: true, action: function (event) {
            $pfTable.trigger('tbar.add');
        }
    };
    var editBtn = {
        text: '修改', action: function (event) {
            var rows = result.getSelectedRows();
            if (rows.length === 1) {
                $pfTable.trigger('tbar.edit', [result.tableApi.row(rows[0]).data()]);
            } else {
                $pf.warningPopups("请选择一行");
            }

            //var data = result.tableApi.row('.selected').data();
            //if (data === null || data === undefined){
            //    $pf.warningPopups("请选择一行"); return;
            //}
            //$pfTable.trigger('tbar.edit', [data]);
        }
    };
    var deleteBtn = {
        text: '删除', action: function (event) {
            var data = [];
            var rows = result.getSelectedRows();
            if (rows.length > 0) {
                for (var i = 0; i < rows.length; i++) {
                    data.push(result.tableApi.row(rows[i]).data());
                }
                $pf.confirmPopups('确定删除吗?' + (data.length > 1 ? '数量:' + (result.isRowSelectAll() ? '全部' : data.length) : ''), function () {
                    $pfTable.trigger('tbar.delete', [data]);//删除时应该是可以多选的,所以data是数组
                });
            } else {
                $pf.warningPopups("请选择需要删除的行");
            }

            //var data = result.tableApi.row('.selected').data();
            //if (data === null || data === undefined) {
            //    $pf.warningPopups("请选择一行"); return;
            //}
            //$pf.confirmPopups('确定删除吗?', function () {
            //    $pfTable.trigger('tbar.delete', [data]);
            //});
        }
    };
    var setVisibleColumnBtn = {
        text: '设置显示列', action: function (event) {
            var url = _url;
            if (url[0] !== '/') {//如果是相对地址,要转为绝对地址
                url = window.location.pathname.replace(/[^/]*$/, url)
            }
            $pf.setVisibleColumnPopups({ dataAction: url });
        }
    };
    var setGroupColumnBtn = {
        text: '设置汇总列<input type="checkbox" />', action: function (event) {
            var url = _url;
            if (url[0] !== '/') {//如果是相对地址,要转为绝对地址
                url = window.location.pathname.replace(/[^/]*$/, url)
            }
            $pf.setGroupColumnPopups({
                dataAction: url, cacheType: 'GroupColumn', cacheKey: _cacheKey,
                listeners: {
                    colSetted: function () {
                        result.sysLoadUrl(_url);
                    }
                }
            });
        }
    };
    //debugger;
    if (opts.buttons instanceof Array) {
        for (var i = 0; i < opts.buttons.length; i++) {
            switch (opts.buttons[i].Name) {
                case 'Export':
                case 'excel':
                    buttons.push(excelBtn);
                    break;
                case 'Add':
                    buttons.push(addBtn);
                    break;
                case 'Edit':
                    buttons.push(editBtn);
                    break;
                case 'Delete':
                    deleteBtn.text = opts.buttons[i].Text;
                    buttons.push(deleteBtn);
                    break;
                case 'SetVisibleColumn':
                    buttons.push(setVisibleColumnBtn);
                    break;
                case 'SetGroupColumn':
                    setGroupColumnBtn.alwayShow = setGroupColumnBtn.AlwayShow;
                    buttons.push(setGroupColumnBtn);
                    _hasGroupBtn = true;
                    break;
                default:
                    (function (name, text, alwayShow) {//注意action会有作用域的问题
                        buttons.push({
                            text: text,
                            alwayShow: alwayShow,
                            action: function (event) {
                                //debugger;
                                var data = [];
                                var rows = result.getSelectedRows();
                                if (rows.length > 0) {
                                    for (var i = 0; i < rows.length; i++) {
                                        data.push(result.tableApi.row(rows[i]).data());
                                    }
                                }
                                //var data = result.tableApi.row('.selected').data();
                                $pfTable.trigger('tbar.' + name, [data]);
                            }
                        });
                    }(opts.buttons[i].Name, opts.buttons[i].Text, opts.buttons[i].AlwayShow));
                    //var name = opts.buttons[i].Name;
                    //buttons.push({
                    //    text: opts.buttons[i].Text,
                    //    action: function (event) {
                    //        //debugger;
                    //        var data = [];
                    //        var rows = result.getSelectedRows();
                    //        if (rows.length > 0) {
                    //            for (var i = 0; i < rows.length; i++) {
                    //                data.push(result.tableApi.row(rows[i]).data());
                    //            }
                    //        }
                    //        //var data = result.tableApi.row('.selected').data();
                    //        $pfTable.trigger('tbar.' + name, [data]);
                    //    }
                    //});
                    break;
            }
        }
        //delete opts.buttons;
    } else {
        //buttons = [excelBtn];
        buttons = [];
    }
    delete opts.buttons;
    //debugger;
    var defaultOpts = jQuery.extend({
        "language": {
            search: "搜索:",
            sZeroRecords: "没有找到匹配的记录"
        },
        destroy: true,
        pagination: paginationOpts,
        paging: false,
        info: false,
        //scrollY: true,
        scrollX: true,
        order: [],
        drawCallback: drawCallback
    }, opts || {});
    if (buttons.length > 0) { defaultOpts.buttons = buttons; defaultOpts.dom = 'Bfrtip'; }

    function newOpts() {
        var r = jQuery.extend({}, defaultOpts);
        if (r.order instanceof Array) {
            r.order = r.order.slice(0);//数组型的成员要这样才不会因opts改变而改变
        }
        return r;
    }
    opts = newOpts();

    function bindPaging(total, current) {//绑定分页组件
        //if (showPagination) {
        paging = $('#' + opts.pagination.id).pagination(total, {
            items_per_page: opts.pagination.pageSize,
            current_page: current,
            callback: function (page) {
                opts.pagination.pageIndex = page;
                //var url = result.tableApi.ajax.url(_url + "&PageSize=" + opts.pagination.pageSize + "&PageIndex=" + page + "&sort=" + _lastSort);
                var url = result.tableApi.ajax.url($pf.setUrlParams(_url, { PageSize: opts.pagination.pageSize, PageIndex: page, sort: _lastSort, filterValue: _lastFilterValue }));
                url.load(function () {
                    //if (selectMode===2) {
                    //    $headTable.find('thead tr input.pf-row-select-all').prop("checked", false);
                    //}
                    bindPaging(total, page);
                });
            },
            display_msg: true,
            setPageNo: true
        });
        if (selectMode === 2) {
            $headTable.find('thead tr input.pf-row-select-all').prop("checked", false);

            if (opts.fixedColumns !== undefined && opts.fixedColumns !== false) {//如果有锁定列时
                $pfTable.parent().parent().parent().find('.DTFC_LeftWrapper .DTFC_LeftHeadWrapper table.dataTable tr th input.pf-row-select-all').prop("checked", false);
            }
        }
        if (!$pf.stringIsNullOrWhiteSpace(_lastFilterValue)) {
            var searchF = $pfTable.parents('.dataTables_wrapper').find('.dataTables_filter input[type=search]');
            searchF.val(_lastFilterValue);
        }
        //}
        //bindCellClick();
    }
    result.loadCurrentPage = function (loadOpts) {//刷新当前页(编辑数据后使用)
        var url = result.tableApi.ajax.url($pf.setUrlParams(_url, { PageSize: opts.pagination.pageSize, PageIndex: opts.pagination.pageIndex, sort: _lastSort, filterValue: _lastFilterValue, random: Math.random() }));
        url.load(function (data) {
            if (loadOpts !== null && loadOpts !== undefined && typeof loadOpts.success == 'function') {
                loadOpts.success(data);
            }
            bindPaging(data.total, opts.pagination.pageIndex);
        });
    };
    result.getSelectedRows = function () {
        return $pfTable.find('tbody tr.selected');
    };
    function getHeaderTR(lists, maxDepth, currentDepth) {
        if (currentDepth == null || currentDepth == undefined) { currentDepth = 1; }
        var writer = "";
        if (lists != null) {
            //显示下一层
            var next = [];
            //显示头
            for (var idx = 0; idx < lists.length; idx++) {
                var i = lists[idx];
                writer += getHeaderTD(i, maxDepth, currentDepth);

                if (i.Children != null) {//下一层数据
                    for (var j = 0; j < i.Children.length; j++) {
                        next.push(i.Children[j]);
                    }
                }
            }

            if (next.length > 0) {
                writer += "</tr>";
                writer += "<tr>";
                writer += getHeaderTR(next, maxDepth, ++currentDepth);
            }
        }
        return writer;
    }
    function getHeaderTD(item, maxDepth, currentDepth) {
        var writer = "";
        var title = item.data || item.title;//合并列用中文,其它列要对应data
        //if (title === null || title === undefined || title === '') { return; }
        item._columnSpan = 1;
        item._rowSpan = 1;
        var style = "";
        //debugger;
        //if (item.Children != null && item.Children != undefined && item.Children.length > 1)
        if (item.Children != null && item.Children != undefined && item.Children.length > 0)//大于0才是对的--wxj20180906
        {
            item._columnSpan = $pf.getAllLeafCount(item);
        } else {
            style += "border-bottom: 1px solid #111;";//最后一个th加下加框--wxj20180716
            if (maxDepth > currentDepth) {
                item._rowSpan = maxDepth - currentDepth + 1;
            }
        }
        if (!$pf.stringIsNullOrWhiteSpace(item.width)) { style += "width=" + item.width + ";"; }
        if (!$pf.stringIsNullOrWhiteSpace(style)) { style = " style='" + style + "' "; }
        //if (!Visible) { style = "style=' display:none' "; }
        var attrs = "";
        if (item.data !== null && item.data !== undefined) { attrs += " field-name='" + item.data + "' "; }

        if (item.isSelectAllCol) {
            if (selectMode === 1) { title = "选择"; }
            else if (selectMode === 2) { title = "<input type='checkbox' class='pf-row-select-all' />全选"; }
        }
        writer += $pf.formatString("<th {1} {2} {3} {4}>{0}</th>", title, item._columnSpan > 1 ? ("colspan=" + item._columnSpan) : "", item._rowSpan > 1 ? ("rowspan=" + item._rowSpan) : "", style, attrs);

        return writer;
    }
    function setLastFilterValue() {
        //这里必需重新找组件,因为loadUrl时重新生成了search组件
        $pfTable.parents('.dataTables_wrapper').find('.dataTables_filter input[type=search]')
            .val(_lastFilterValue);//因为loadUrl是页面共用的,理应把过滤器的值清掉,所以这里要重新赋值
    }
    result._loadTip = function (tip) {
        if (result.tableApi !== null) {
            result.tableApi.destroy();
            $(me).empty();
        }
        var message = tip;//"没有数据";
        //if (typeof data === 'string') { message = data; }
        //else if (data && data.Message) { message = data.Message; }
        $(me).html('<thead><tr><th>提示</th></tr></thead><tbody><tr onclick="$pf.stopPropagation(arguments[0])"><td  onclick="$pf.stopPropagation(arguments[0])">' + message + '</td></tr></tbody>');
        var alwayBtns = [];
        //debugger;
        for (var i = 0; i < buttons.length; i++) {
            if (buttons[i].alwayShow === true) {
                alwayBtns.push(buttons[i]);
            }
        }
        //debugger;
        var nullOpts = { paging: false, info: false, searching: false, bSort: false };
        if (alwayBtns.length > 0) { nullOpts.buttons = alwayBtns; nullOpts.dom = 'Bfrtip'; }
        result.tableApi = me.DataTable(nullOpts);
        if (showPagination) { $('#' + opts.pagination.id).hide(); }
    };
    result._loadUrl = function (url, loadOpts) {
        var mescope = this;
        //if ($('body').width() == 0) {//当在页面a中用异步tab加页面b的iframe时，如果页面b中有pfTable,parent的宽度是0，结果是表头和body都不是100%，表头靠左，body水平居中--wxj20181218
        //debugger;
        if ($('body').width() == 0 || $pfTable.parent().width() < 11) {//以前的版本在"批量增加用户"功能(使用了TabContainer)仍然不能自动设置autoWidth--ben20190529
            //console.info('wait');
            setTimeout(function () {
                result._loadUrl.call(mescope, url, loadOpts);
            }, 100);
            return;
        }
        loadOpts = loadOpts || {};
        //var toUrl = url + "&PageIndex=0";        
        var toUrlParams = { PageIndex: 0 };
        if (showPagination) {
            //toUrl += "&PageSize=" + opts.pagination.pageSize;
            toUrlParams.PageSize = opts.pagination.pageSize;
        }
        toUrl = $pf.setUrlParams(url, toUrlParams);
        var layerIdx = layer.load('正在查询请稍候');
        $pf.post(toUrl, null, function (data) {
            //debugger;
            $pfTable = me;
            if (data && data.Result !== false && typeof data !== 'string') {//MvcMenuFilter拦截后直接返回了文字--wxj20181127
                if (result.tableApi !== null && data.columns !== null) {
                    //alert('destroy');
                    result.tableApi.destroy();
                    $(me).empty();
                }
                //debugger;
                //opts.pagination.url = url;
                _url = url;
                if (data.columns !== null || result.tableApi === null) {//列数据有更新时
                    _dataTotal = data.total;
                    opts = newOpts();
                    opts.data = data.data;
                    if (data.columns !== null) {
                        var treeColumn = { Children: data.columns };
                        //为了生成多表头,需要拼接<thead>
                        var thead = "<thead><tr>";
                        if (selectable) {
                            data.columns.splice(0, 0, {
                                isSelectAllCol: true,
                                orderable: false,
                                width: '15px',
                                Children: [],
                                render: function () {
                                    return '<input type="checkbox" class="pf-row-select"  />';
                                }
                            });
                        }

                        thead += getHeaderTR(data.columns, $pf.getDepth(treeColumn) - 1);
                        thead += "</tr></thead>";
                        //debugger;
                        $(thead).appendTo($pfTable);
                        //汇总信息
                        var foot = '';
                        hasSummary = false;

                        var columns = [];

                        $pf.eachLeaf(treeColumn, function (i) {
                            if (i.dataType === 'decimal' || i.dataType === 'int' || i.dataType === 'long' || i.dataType === 'double') {
                                if (i.className === null || i.className === undefined) { i.className = ''; }
                                i.className += ' text-right';
                                i.render = function (data, type, row) {
                                    return $pf.thousandth(data);
                                }
                            }
                            if (i.dataType === 'percent') {
                                //debugger;
                                if (i.className === null || i.className === undefined) { i.className = ''; }
                                i.className += ' text-right';
                                i.render = function (data, type, row) {
                                    return $pf.stringIsNullOrWhiteSpace(data) ? '' : ($pf.thousandth($pf.toFixed(data)) + ' %');
                                }
                            }
                            if (i.dataType === 'DateTime') {
                                i.render = function (data, type, row) {
                                    return $pf.formatTime(data, 'yyyy-MM-dd hh:mm:ss');
                                }
                            }
                            if (i.dataType === 'date') {
                                i.render = function (data, type, row) {
                                    return $pf.formatDate(data);
                                }
                            }
                            if (i.dataType === 'bool') {
                                i.render = function (data, type, row) {
                                    return data === true || data === 1 ? "是" : "否";
                                }
                            }

                            if (i.summary !== null && i.summary !== undefined) {
                                if (!hasSummary) {
                                    foot = foot.replace(/<th><\/th>$/, '<th style="text-align:right">Total:</th>');
                                }
                                hasSummary = true;
                                var h = typeof i.render === 'function' ? i.render(i.summary) : i.summary;
                                foot += '<th style="text-align:right">' + h + '</th>';
                            } else {
                                foot += '<th></th>';
                            }
                            //if (i.width !== null && i.width !== undefined) {//后台返回42px,到table时,内部只有36--benjamin20190716
                            //    i.width = (parseFloat(i.width.replace('px')) + 6).toString() + 'px';
                            //}

                            if (i.data === 'rownumber') {//后台sql分页的情况下,行号不应该用于排序--benjamin20191122
                                i.orderable = false;
                            }

                            columns.push(i);
                        });
                        opts.columns = columns;

                        if (hasSummary) {
                            $($('<tfoot>' +
                                    '<tr>' + foot +
                                    '</tr>' +
                                '</tfoot>')).appendTo($pfTable);

                        }
                        //bindEvent();
                        //alert('create with column');
                    }
                    else {
                        //alert('create without column');
                    }
                    result.tableApi = $pfTable.DataTable(opts);
                    //console.info($pfTable.width());
                    //console.info($pfTable.parent().width());
                    if ($pfTable.width() < $pfTable.parent().width()) {//默认自动调列宽。当table的宽度小于容器和分页器时，如果用自动，会变得很窄，所以设置为不自动，然后利用css设置宽度为100%

                        opts.bAutoWidth = false;
                        result.tableApi = $pfTable.DataTable(opts);

                    }
                    //console.info("bAutoWidth = " + opts.bAutoWidth);
                    //console.info($pfTable.width());
                    //console.info($pfTable.parent().width());
                    //console.info($('body').width());
                    //console.info('----------------');
                } else {//列数据无更新时
                    result.tableApi.clear();
                    for (var i = 0; i < data.data.length; i++) {
                        result.tableApi.row.add(data.data[i]);
                    }
                    //alert('reload rows');
                    result.tableApi.draw();
                }
                if (showPagination) {
                    opts.pagination.pageIndex = 0;
                    bindPaging(data.total, 0);//如果不重新绑定,总页数不会改变,当前页也不会置为0
                    $('#' + opts.pagination.id).show();
                }

                if (typeof loadOpts.success == 'function') {//complete回调一般后续使用data更新其它dom
                    loadOpts.success(data);
                    //privateObj.userLoadSuccess = loadOpts.success;
                }
                if (loadOpts.needSetLastFilterValue) {
                    setLastFilterValue();
                }
                //if (typeof loadOpts.sysLoadSuccess == 'function') {
                //    loadOpts.sysLoadSuccess(data);
                //}
                ////$('#' + opts.pagination.id).show();
            } else {
                var message = "没有数据";
                if (typeof data === 'string') { message = data; }
                else if (data && data.Message) { message = data.Message; }
                result._loadTip(message);

                //if (result.tableApi !== null) {
                //    result.tableApi.destroy();
                //    $(me).empty();
                //}
                //var message = "没有数据";
                //if (typeof data === 'string') { message = data; }
                //else if (data && data.Message) { message = data.Message; }
                //$(me).html('<thead><tr><th>提示</th></tr></thead><tbody><tr onclick="$pf.stopPropagation(arguments[0])"><td  onclick="$pf.stopPropagation(arguments[0])">' + message + '</td></tr></tbody>');
                //var alwayBtns = [];
                ////debugger;
                //for (var i = 0; i < buttons.length; i++) {
                //    if (buttons[i].alwayShow === true) {
                //        alwayBtns.push(buttons[i]);
                //    }
                //}
                ////debugger;
                //var nullOpts = { paging: false, info: false, searching: false };
                //if (alwayBtns.length > 0) { nullOpts.buttons = alwayBtns; nullOpts.dom = 'Bfrtip'; }
                //result.tableApi = me.DataTable(nullOpts);
                //if (showPagination) { $('#' + opts.pagination.id).hide(); }

            }
            if (layerIdx) { layer.close(layerIdx); }
            if (typeof loadOpts.complete == 'function') {//complete回调一般用于mask的状态控制,重新更新列宽等
                loadOpts.complete();
            }
        });
    };
    result.loadUrl = function (url, loadOpts) {//为了便于导出,url请用完整地址.load时更新分页组件的属性.不要想把批量上传的文件放这里，因为如果每次都上传文件会很慢的,也不要把hybhs拼接到url上(url太长也会报错),只能后台用session保存,然后传进来;loadOpts不应改为系统变量
        _lastFilterValue = '';
        _lastSort = '';
        userLoadOpts = loadOpts;
        result._loadUrl(url, loadOpts);
    };
    result.sysLoadUrl = function (url, loadOpts) {//Table内部使用
        loadOpts = jQuery.extend(loadOpts, userLoadOpts);
        result._loadUrl(url, loadOpts);
    };
    result.readyToSearch = function () {
        result._loadTip("点击查询后显示数据");
    };
    if (selectMode == 1 || selectMode == 2) {
        result.selectRow = function (tr) {//这里的tr可以是列表列里的
            if (tr.hasClass('selected')) {
                if (selectMode == 2 && result.isRowSelectAll()) {
                    result.uncheckRowSelectAll();
                }
                tr.removeClass('selected');
                tr.find('input.pf-row-select').prop("checked", false);
            }
            else {
                if (selectMode == 1) {
                    //var trs = result.tableApi.$('tr.selected');
                    //var cbs = trs.find('input.pf-row-select');
                    //cbs.prop("checked", false);
                    //trs.removeClass('selected');

                    var tmpTb = tr.parents('table:eq(0)');//如果tr是锁定列的table的,这里的tmpTb就是锁定列的table--benjamin20200401
                    var trs = tmpTb.find('tr.selected');
                    var cbs = trs.find('input.pf-row-select');
                    cbs.prop("checked", false);
                    trs.removeClass('selected');

                }
                tr.addClass('selected');
                tr.find('input.pf-row-select').prop("checked", true);
            }
        };
        result.unselectAllRows = function () {
            $pfTable.find('tbody tr.selected').removeClass('selected')
                .find('input.pf-row-select').prop("checked", false);

        };
        result.selectRows = function (dataMatch) {
            //debugger;
            var rows = $pfTable.find('tbody tr');
            if (rows.length > 0) {
                for (var i = 0; i < rows.length; i++) {
                    var data = result.tableApi.row(rows[i]).data();
                    if (dataMatch(data) === true) {
                        result.selectRow(rows.eq(i));
                    }
                }
            }
        };
        $pfTable.on('click', 'tbody tr input.pf-row-select', function (e) {
            $pf.stopPropagation(e);
            //debugger;
            var tr = $(this).parent().parent();
            var tempTb = tr.parent().parent();
            if (tempTb.attr('id') !== $pfTable.attr('id')) {//如果有锁定列时,点击锁定列时进入这里
                var idx = tr.parent().find('tr').index(tr);//行序号
                $pfTable.find('tbody tr:nth-child(' + (idx + 1) + ')').click();
                return;
            }

            if (opts.fixedColumns !== undefined && opts.fixedColumns !== false) {//如果有锁定列时,点击普通列进入这里
                var idx = tr.parent().find('tr').index(tr);//行序号
                //$pfTable.parent().parent().parent().find('.DTFC_LeftWrapper table.dataTable tr:nth-child(' + (idx + 1) + ') input.pf-row-select').prop("checked", false);
                result.selectRow($pfTable.parent().parent().parent().find('.DTFC_LeftWrapper .DTFC_LeftBodyWrapper table.dataTable tr:nth-child(' + (idx + 1) + ') '));
            }

            result.selectRow(tr);
            //if (tr.hasClass('selected')) {
            //    tr.removeClass('selected');
            //}
            //else {
            //    if (selectMode == 1) {
            //        var trs = result.tableApi.$('tr.selected');
            //        var cbs = trs.find('input.pf-row-select');
            //        cbs.prop("checked", false);
            //        trs.removeClass('selected');
            //    }
            //    tr.addClass('selected');
            //}
        });
        $pfTable.on('click', 'tbody tr', function (e) {
            $pf.stopPropagation(e);
            $(this).find('input.pf-row-select').click();
            //var cbs =$(this).find('input.pf-row-select');
            ////cbs.prop("checked", true);
            //cbs.click();
        });
    }

    var _isOrdering = false;
    $pfTable.on('order.dt', function (e) {//注意，这里的datatables原生排序事件不仅在点表头时会触发，draw后也会触发
        // This will show: "Ordering on column 1 (asc)", for example
        if (result.tableApi) {
            //debugger;
            $pf.stopPropagation(e);
            var order = result.tableApi.order();
            if (order.length > 0) {
                var sort = result.tableApi.column(order[0][0]).dataSrc() + "%20" + order[0][1];
                if (_lastSort === sort) { return; }
                if (_isOrdering === true) { return; }
                _isOrdering = true;
                //var url = _url + "&sort=" + sort;//当_url没有?号时直接加&会使地址错误--benjamin20190430
                var url = $pf.setUrlParams(_url, { sort: sort });

                if (showPagination) { url += "&PageSize=" + opts.pagination.pageSize + "&PageIndex=" + opts.pagination.pageIndex; }
                $pf.post(url, null, function (data) {
                    //$pf.post(getPagingUrl(), null, function (data) {
                    if (data && data.Result !== false && data.data !== undefined) {
                        result.tableApi.clear();
                        for (var i = 0; i < data.data.length; i++) {
                            result.tableApi.row.add(data.data[i]);
                        }
                        opts.order = [[order[0][0], order[0][1]]];
                        result.tableApi.draw();
                        setLastFilterValue();//benjamin20190419
                        //result.tableApi.order([order[0], order[1]]);
                    }
                    _isOrdering = false;
                    _lastSort = sort;
                });
                //var param = $("#f1").serialize();
                //tableApi.loadUrl("GetData?" + param + "&sort=" + tableApi.tableApi.column(order[0]).dataSrc() + "%20" + order[0][1]);
            }
            //alert('Ordering on column ' + order[0][0] + ' (' + order[0][1] + ')');
            return false;
        }
        //$('#orderInfo').html('Ordering on column ' + order[0][0] + ' (' + order[0][1] + ')');
    });
    result.isRowSelectAll = function () {
        return $headTable.find('thead tr input.pf-row-select-all').is(':checked');
    };
    result.uncheckRowSelectAll = function () {
        $headTable.find('thead tr input.pf-row-select-all').prop("checked", false);
    };
    //result.rowMoveUp = function () {//已经实现拖放移动--benjamin20190429
    //    var rows = result.getSelectedRows();
    //    if (rows.length === 1) {
    //        //table.data().length
    //        var idx=result.tableApi.row(rows[0]).index();
    //        if (idx === 0) {
    //            $pf.warningPopups("第一行不能上移");
    //        }
    //        exchangeRow(rows[0], rows.parent().children()[idx - 1]);
    //        rows.find('input.pf-row-select').prop("checked", true);

    //        //var data = result.tableApi.row(rows[0]).data();
    //        //var nextRow = rows.parent().children()[idx - 1];
    //        //var nextIds = result.tableApi.row(nextRow).index();
    //        //var nextData = result.tableApi.row(nextRow).data();

    //        //result.tableApi.row(nextRow).data(data);
    //        //result.tableApi.row(rows[0]).data(nextData);
    //        ////alert(idx);
    //        ////alert(nextIds);
    //        ////$pfTable.trigger('tbar.edit', [result.tableApi.row(rows[0]).data()]);
    //    } else {
    //        $pf.warningPopups("请选择一行");
    //    }
    //};
    return result;
};

/*
*fixedColumns:int 锁定列
*/
jQuery.fn.pfTreeTable = function (opts) {
    var me = this;
    var $pfTable = me;
    //var $headTable = {};
    //var selector = me.selector;
    me.addClass('pfTreeGrid');
    var _url = '';//调用loadUrl()时赋值(不包含pageIndex和pageSize参数)
    var _colIdxs = [];//保存多表头的列信息,为了不用多次循环生成,有改变时更新
    _colIdxs = $pf.getTableColumnIdxs(me);

    opts = opts || {};
    //var hasSummary = false;
    //var paginationOpts = null;
    //var showPagination = opts.showPagination !== false;
    var selectMode = opts.selectMode || 0;
    var selectable = selectMode !== 0;
    var closeTree = opts.closeTree || false;

    function bindCellClick() {
        for (var i in opts.onCellClick) {
            if (opts.onCellClick.hasOwnProperty(i)) {
                //$pf.bindTableColumnClick(me.selector, i, opts.onCellClick[i]);
                var idx = _colIdxs.indexOf(i);
                //debugger;
                if (idx !== null && idx !== undefined) {
                    me.on('click', 'tbody tr td:nth-child(' + (idx + 1) + ')', function (e) {
                        //debugger;
                        //alert('x');
                        if (!($(e.target).hasClass('hitarea'))) {
                            opts.onCellClick[i].call(this, e);
                            //opts.onCellClick[i](e);
                        }
                    });
                    //var $cell = me.find(' tbody tr td:nth-child(' + (idx + 1) + ')');
                    //$cell.unbind('click', opts.onCellClick[i]);
                    //$cell.bind('click', opts.onCellClick[i]);
                    //$cell.css('textDecoration', 'underline');
                    //$cell.css('cursor', 'pointer');
                }
            }
        }
    }
    function bindCellClickCss() {
        for (var i in opts.onCellClick) {
            if (opts.onCellClick.hasOwnProperty(i)) {
                var idx = _colIdxs.indexOf(i);
                if (idx !== null && idx !== undefined) {
                    var $cell = me.find(' tbody tr td:nth-child(' + (idx + 1) + ')');
                    //$cell.unbind('click', opts.onCellClick[i]);
                    //$cell.bind('click', opts.onCellClick[i]);
                    $cell.css('textDecoration', 'underline');
                    $cell.css('cursor', 'pointer');
                }
            }
        }
    }
    bindCellClick();
    function setOddEvenRowCss(tbody) {
        //tbody.find("tr:odd").css("backgroundColor", "#eff6fa");//奇偶行样式
        //tbody.find("tr:even").css("backgroundColor", "white");
        $pf.setOddEvenRowCss(tbody);
        //var odd = true;
        //tbody.find('tr').each(function () {
        //    var tr = $(this);
        //    if (odd) {
        //        tr.removeClass('even').addClass('odd');
        //    } else {
        //        tr.removeClass('odd').addClass('even');
        //    }
        //    odd != odd;
        //});
    }
    //function resetFixedColumnWidth() {
    //    if (opts.fixedColumns !== undefined && opts.fixedColumns !== false) {
    //        var thead = me.find('thead');

    //        //取消注释--benjamin 
    //        $pf.copyTableHeadWidth(thead, $('#'+me[0].id +'-fixedColumn thead'), true);
    //        $pf.copyTableHeadWidth(thead, $('#'+me[0].id +'-fixedCorner thead'), true);
    //        $pf.copyTableHeadWidth(thead, $('#'+me[0].id +'-fixedhead thead'));
    //    }

    //}
    function resetFixedColumnSize() {
        if (opts.fixedColumns !== undefined && opts.fixedColumns !== false) {
            var thead = me.find('thead');

            //取消注释--benjamin 
            $pf.copyTableHeadSize(thead, $('#' + me[0].id + '-fixedColumn thead'), true);
            $pf.copyTableHeadSize(thead, $('#' + me[0].id + '-fixedCorner thead'), true);
            $pf.copyTableHeadSize(thead, $('#' + me[0].id + '-fixedhead thead'));
        }

    }
    var defaultDrawCallback = function () {
        bindCellClickCss();

        ////fixedColumns: {
        ////        leftColumns: 2
        ////}
        ////alert(1);
        if (opts.fixedColumns !== undefined && opts.fixedColumns !== false//如果有锁定列时
            ) {
            $('#' + me[0].id + '-fixedColumn').remove();
            $('#' + me[0].id + '-fixedhead').remove();
            $('#' + me[0].id + '-fixedCorner').remove();

            if (me.find('thead tr').length < 1//当显示"无相关数据"时
                ) {
                return;
            }

            //取消display:none --benjamin 
            var fixedColumnTable = $('<table id="' + me[0].id + '-fixedColumn" class="pfTreeColumnGrid" style="position:absolute;top:0px;left:0px;"></table>');
            var fixedHeadTable = $('<table id="' + me[0].id + '-fixedhead" style="position:absolute;top:0px;left:0px;"></table>');
            var fixedCornerTable = $('<table id="' + me[0].id + '-fixedCorner" style="position:absolute;top:0px;left:0px;"></table>');
            function setFixedPosition() {
                var l = me.parent()[0].scrollLeft + 'px';
                var t = me.parent()[0].scrollTop + 'px';
                //console.info(l);
                //console.info(t);
                fixedColumnTable.css('left', l);
                fixedHeadTable.css('top', t);
                fixedCornerTable.css('left', l);
                fixedCornerTable.css('top', t);
            }

            var fixedColumns = opts.fixedColumns && typeof opts.fixedColumns == 'number' ? opts.fixedColumns : 1;
            if (selectable) { fixedColumns += 1; }
            //fixedColumns = 2;
            var fixedHeader = $('<thead></thead>');
            var fixedBody = $('<tbody></tbody>');
            var headTr = me.find('thead tr:eq(0)');//现时没有考虑锁定列是多表头的情况,如果以后有了,可以增加 curRowNum:[2,1,0]解决,每个int代表当前列是到第几行了
            //var headTr = me.find('thead tr');
            //var width = 0;
            //var headRowSpan = headRowSpan.find('tr').length;
            for (var i = 0; i < headTr.length; i++) {
                var tmpTr = $('<tr class="pfGridHead"></tr>');
                for (var j = 0; j < fixedColumns; j++) {
                    var tmpTh = headTr.find('th:eq(' + j + ')');
                    //tmpTh.attr('rowSpan', headRowSpan);
                    //width = tmpTh.width();
                    var cloneTh = tmpTh.clone(true);
                    var rect = tmpTh[0].getBoundingClientRect();
                    //cloneTh.css('lineHeight', (
                    //    rect.height
                    //    - 1
                    //    - parseInt(tmpTh.css('paddingTop').replace('px'))
                    //    - parseInt(tmpTh.css('paddingBottom').replace('px'))
                    //    ) + 'px');//不要设置lineHeight,换行时太宽--benjamin20190711
                    cloneTh.css('height', (
                        rect.height
                        - 1
                        - parseInt(tmpTh.css('paddingTop').replace('px'))
                        - parseInt(tmpTh.css('paddingBottom').replace('px'))
                        ) + 'px');//
                    cloneTh.css('padding', '10px 18px');
                    cloneTh.css('background', 'none');
                    cloneTh.css('backgroundColor', '#E8F1F7');
                    //debugger;
                    $pf.copyDomBorder(tmpTh, cloneTh);
                    //cloneTh.css('borderRight', tmpTh.css('borderRight'));
                    tmpTr.append(cloneTh);

                    (function (idx, jdx) {//注意action会有作用域的问题
                        cloneTh.click(function (e) {
                            $pf.stopPropagation(e);
                            //debugger;

                            if ($(e.target).hasClass('pf-row-select-all')) {
                                me.find('thead tr th:nth-child(' + (jdx + 1) + ') .pf-row-select-all').eq(idx).trigger('click');

                                $pf.stopPropagation(e);
                                return;
                            } else {
                                me.find('thead tr th:nth-child(' + (jdx + 1) + ')').eq(idx).trigger('click');
                            }
                        });
                    }(i, j));
                }
                tmpTr.appendTo(fixedHeader);
            }
            //fixedHeader.find('tr td .pf-row-select-all').click(function () {

            //});

            function setFixedColumnWidth() {
                var thead = me.find('thead');

                //取消注释--benjamin 
                $pf.copyTableHeadWidth(thead, fixedHeader, true);
                $pf.copyTableHeadWidth(thead, fixedCornerTable.find('thead'), true);
                $pf.copyTableHeadWidth(thead, fixedHeadTable.find('thead'));

            }

            var bodyTr = me.find('tbody tr');
            for (var i = 0; i < bodyTr.length; i++) {
                var tmpTr = $('<tr></tr>');
                var bodyTrI = bodyTr.eq(i);
                tmpTr.css('display', bodyTrI.css('display'));
                if (bodyTrI.attr('expanded') === 'expanded') { tmpTr.attr('expanded', 'expanded'); }
                if (bodyTrI.hasClass('selected')) { tmpTr.addClass('selected'); }

                for (var j = 0; j < fixedColumns; j++) {
                    var srcTh = bodyTrI.find('td:eq(' + j + ')');
                    var tmpTh = srcTh.clone(true);
                    //debugger;
                    tmpTh.css('textDecoration', srcTh.css('textDecoration'));
                    tmpTh.css('cursor', srcTh.css('cursor'));

                    tmpTr.append(tmpTh);
                    (function (idx, jdx) {//注意action会有作用域的问题
                        tmpTh.click(function (e) {
                            $pf.stopPropagation(e);
                            //debugger;
                            if ($(e.target).hasClass('hitarea')) {
                                //alert(1);
                                //debugger;
                                $pf.expandTree($(e.target).parent());

                                //me.find('tbody tr td:nth-child(1) .hitarea').eq(idx).trigger('click');
                                //me.find('tbody tr td:nth-child(' + (selectable ? 2 : 1) + ') .hitarea').eq(idx).trigger('click');
                                me.find('tbody tr td:nth-child(' + (jdx + 1) + ') .hitarea').eq(idx).trigger('click');

                                $pf.stopPropagation(e);
                                //$pf.expandTree(me.find('tbody tr td:nth-child(' + 1 + (selectable ? 1 : 0) + ') .hitarea').eq(idx).parent());
                                //var hitarea = me.find('tbody tr td:nth-child(' + (jdx + 1) + ') .hitarea');
                                //$pf.expandTree(hitarea.eq(idx).parent());
                                //if (typeof opts.onRowExpand == 'function') {
                                //    //opts.onRowExpand();
                                //    opts.onRowExpand.call(hitarea);
                                //}

                                setFixedColumnWidth();
                                setFixedPosition();
                                //defaultDrawCallback();
                                return;
                            } else if ($(e.target).hasClass('pf-row-select')) {
                                me.find('tbody tr td:nth-child(' + (jdx + 1) + ') .pf-row-select').eq(idx).trigger('click');

                                $pf.stopPropagation(e);
                                return;
                            } else {
                                //me.find('tbody tr td:nth-child(1)').eq(idx).trigger('click');
                                //me.find('tbody tr td:nth-child(' + 1 + (selectable ? 1 : 0) + ')').eq(idx).trigger('click');
                                me.find('tbody tr td:nth-child(' + (jdx + 1) + ')').eq(idx).trigger('click');
                            }
                        });
                    }(i, j));

                    tmpTr.attr('level', bodyTrI.attr('level'));
                    tmpTr.attr('expanded', bodyTrI.attr('expanded'));
                }
                tmpTr.appendTo(fixedBody);
            }
            fixedColumnTable.append(fixedHeader);
            fixedColumnTable.append(fixedBody);
            //fixedColumnTable.width(width);
            fixedColumnTable.css('backgroundColor', 'white');
            me.parent().append(fixedColumnTable);

            setOddEvenRowCss(fixedBody);


            //锁表头
            var fixedHeadHeader = me.find('thead').clone(true);
            var fixedHeadBody = $('<tbody></tbody>');


            fixedHeadTable.append(fixedHeadHeader);
            fixedHeadTable.append(fixedHeadBody);
            me.parent().append(fixedHeadTable);

            //锁左上角
            var fixedCornerHeader = fixedHeader.clone(true);
            var fixedCornerBody = $('<tbody></tbody>');

            fixedCornerTable.append(fixedCornerHeader);
            fixedCornerTable.append(fixedCornerBody);
            me.parent().append(fixedCornerTable);

            //////fixedColumnTable[0]
            me.parent().unbind('scroll', setFixedPosition).bind('scroll', setFixedPosition);
            setFixedColumnWidth();
            setFixedPosition();//异步tableApi.loadChildNode(),是靠这一句来刷新top的


        }

        //if (selectable) {
        //    //$headTable = $pfTable.parent().parent().find('.dataTables_scrollHead table');
        //    //if ($headTable.attr('selectAllBinded') !== 'true') {
        //    //    $headTable.on('click', 'thead tr input.pf-row-select-all', function (e) {
        //    //        $headTable.attr('selectAllBinded', 'true');
        //    //        //alert(1);
        //    //        $pf.stopPropagation(e);
        //    //        //debugger;
        //    //        var b = $(this).is(':checked');
        //    //        //if (b === true) {

        //    //        //}
        //    //        var cbs = $pfTable.find('tbody tr input.pf-row-select');
        //    //        cbs.each(function (index, element) {
        //    //            var cb = $(element);
        //    //            if (cb.is(':checked') !== b) { cb.click(); }
        //    //        });

        //    //    });
        //    //}
        //}
    };
    //debugger;
    var drawCallback = null;
    if (typeof opts.drawCallback == 'function') {
        var userDrawCallback = opts.drawCallback;
        drawCallback = function () {
            defaultDrawCallback();
            userDrawCallback();
        }
        delete opts.drawCallback;
    } else {
        drawCallback = defaultDrawCallback;
    }
    var defaultOpts = jQuery.extend({

    }, opts || {});
    var opts = jQuery.extend({}, defaultOpts);
    var result = {};
    function createTr(list, pId, depth) {
        if (depth === undefined) { depth = 0; }
        var rowId = 1;
        if (pId !== undefined) { rowId = pId + 1; }
        var result = [];
        for (var i = 0; i < list.length; i++) {
            var row = { id: rowId, pId: pId, data: list[i].Data, depth: depth };
            result.push(row);
            if (list[i].Children) {
                var c = createTr(list[i].Children, rowId, depth + 1);
                for (var j = 0; j < c.length; j++) {
                    result.push(c[j]);
                }
                rowId = rowId + c.length;
            }
            rowId++;
        }
        return result;
    }
    function createTd(column, value) {
        var style = '';
        if ((column.dataType === 'decimal' || column.dataType === 'int') && value !== null && value !== undefined) {
            value = $pf.thousandth(value);
            style += 'text-align:right;padding-right: 9px;';
            //style = 'style="text-align:right;padding-right: 9px"';
        }
        if (column.visible === false) { style += 'display:none;'; }
        if (style.length > 0) { style = 'style="' + style + '"'; }
        if (value === null) { return '<td ' + style + '></td>'; }
        return '<td ' + style + '>' + value + '</td>';
    }
    //function createTrByTreeData(data, columns, tr) {//data为后台返回的TreeStore
    //    if (tr === undefined) {
    //        tr = $('<tr></tr>');
    //    }
    //    //var isFirstFloor = tr === undefined || tr === null;
    //    var isFirstFloor = tr.find('.linearea').length<1;
    //    var level = isFirstFloor ? 0 : tr.attr('level');

    //    var h = '';
    //    var iLevel = null;
    //    if (level !== null && level !== undefined) { iLevel = parseInt(level) + 1; }
    //    else { iLevel = 0;}

    //    for (var i = 0; i < data.length; i++) {
    //        var trData = data[i].Data;
    //        var ij = 0;
    //        h += '<tr class="pfGridItem" level="' + iLevel + '" >';

    //        //debugger;
    //        if (selectable) {
    //            h += "<td><input type=\"checkbox\" class=\"pf-row-select\"></td>";
    //        }
    //        //就根据后台的columns顺序来生成好了,因为打印方法要使用,所以后台反正要写columns的,以后台为准
    //        $pf.eachLeaf({ Children: columns }, function (c) {
    //            var txt = trData[c.data];
    //            if (txt === undefined) { txt = ''; }
    //            if (ij === 0) {
    //                var linearea = '';
    //                for (var k = 0; k < iLevel; k++) {
    //                    //异步也可以划线--benjamin20190712
    //                    var lineClass = '';
    //                    if (!isFirstFloor) {//为了共用createTrByTreeData方法,tr可以传入null
    //                        if (k < iLevel - 1
    //                            ) {//当不是最右边的格
    //                            var pLine = tr.find('.linearea').eq(k);
    //                            if (pLine.hasClass('linearea-ud') || pLine.hasClass('linearea-urd')) {
    //                                lineClass = 'linearea-ud';
    //                            } 
    //                        } else {
    //                            if (i < data.length - 1) {//当不是最后一行
    //                                lineClass = 'linearea-urd';
    //                            } else {
    //                                lineClass = 'linearea-ur';
    //                            }
    //                        }

    //                        linearea += '<div class="linearea '+lineClass+'"></div>';
    //                    }
    //                }
    //                h += '<td>' + linearea + '<div class="hitarea hitarea-closed"></div>' + txt + '</td>';
    //            } else {
    //                h += createTd(c, txt);
    //            }
    //            ij++;
    //        });

    //        h += '</tr>';
    //    }
    //    if (data.Children.length > 0) {
    //        tr
    //        h += createTrByTreeData(data.Children, columns)
    //    }
    //    if (h !== '') { $(tr).after(h); }
    //    //return h;
    //    return tr;
    //}
    result.loadData = function (data, loadOpts) {//当json的深度和行数非常大时,不能通过post来实现,只能用iframe放入子页面的方法
        loadOpts = loadOpts || {};
        //debugger;
        //if (data !== null && data !== undefined && typeof data === 'string') {//很复杂的JSON时,后端手动序列化为str后返回
        //    data = JSON.parse(data);
        //}
        var $body = me.find('tbody');
        if ($body.length === 0) { me.append('<tbody></tbody>'); $body = me.find('tbody'); }
        $body.html('');
        var $head = me.find('thead');
        if ($head.length === 0) { me.prepend('<thead></thead>'); $head = me.find('thead'); }
        $head.html('');
        if (data && data.Result !== false) {
            if (data.data) {
                var h = '';
                //h += createTrByTreeData(data);
                var trs = createTr(data.data);
                var treeMatrix = new TreeMatrix();
                treeMatrix.initByTreeList(data.data);
                //console.info(treeMatrix.getNetLine(0, 1));

                for (var i = 0; i < trs.length; i++) {
                    var tr = trs[i];

                    ////h += '<tr class="treegrid-' + tr.id + ' '
                    ////    + (tr.pId == null ? '' : ('treegrid-parent-' + tr.pId))
                    ////    + '">';//jquery treegrid的写法

                    //h += '<tr class="treegrid-' +tr.id + ' '
                    //    +(tr.pId == null ? '': ('treegrid-parent-' +tr.pId))
                    //    + ' pfGridItem" level="' +iLevel + '">';
                    ////h += '<tr class="pfGridItem" level="' +iLevel + '" >';

                    var iLevel = tr.depth;
                    h += $pf.formatString("<tr class='pfGridItem {0}' level='{1}' {2} {3}>",
                        '',//_itemClass,
                        iLevel,
                        closeTree ? "" : "expanded='expanded'",
                        closeTree && iLevel > 0 ? "style='display:none'" : ""
                    );

                    //debugger;
                    //if (selectable) {//测试后可启用
                    //    h += "<td><input type=\"checkbox\" class=\"pf-row-select\"></td>";
                    //}

                    if (data.columns) {
                        for (var j = 0; j < data.columns.length; j++) {
                            var value = tr.data[data.columns[j].data];

                            if (j === 0) {
                                //var iLevel = tr.depth;
                                var css = closeTree ? "hitarea hitarea-closed" : "hitarea hitarea-expanded";
                                var linearea = '';
                                //debugger;
                                for (var k = 0; k < iLevel; k++) {
                                    //异步也可以划线--benjamin20190712
                                    var lineClass = '';
                                    //if (k < iLevel - 1) {//当不是最右边的格
                                    //    var pLine = tr.find('.linearea').eq(k);
                                    //    if (pLine.hasClass('linearea-ud') || pLine.hasClass('linearea-urd')) {
                                    //        lineClass = 'linearea-ud';
                                    //    } 
                                    //} else {
                                    //    if (i < data.data.length - 1) {//当不是最后一行
                                    //        lineClass = 'linearea-urd';
                                    //    } else {
                                    //        lineClass = 'linearea-ur';
                                    //    }
                                    //}
                                    switch (treeMatrix.getNetLine(j, i)) {
                                        case '┝':
                                            lineClass = 'linearea-urd';
                                            break;
                                        case '│':
                                            lineClass = 'linearea-ud';
                                            break;
                                        case '┕':
                                            lineClass = 'linearea-ur';
                                            break;
                                        default:
                                            break;
                                    }
                                    //line += string.Format("<div class='{0} {1}'></div>", "linearea ", GetClassByTreeMatrixNetLine(matrix.GetNetLine(j, rowIdx)));
                                    linearea += '<div class="linearea ' + lineClass + '"></div>';
                                }
                                h += '<td>' + linearea + '<div class="' + css + '"></div>' + value + '</td>';
                            } else {
                                h += createTd(data.columns[j], value);
                            }

                            //var style = '';
                            //if ((data.columns[j].dataType === 'decimal' || data.columns[j].dataType === 'int') && value !== null && value !== undefined) {
                            //    value = $pf.thousandth(value);
                            //    style = 'style="text-align:right;padding-right: 9px"';
                            //}
                            //h += '<td ' + style + '>' + value + '</td>';
                        }
                    } else {
                        for (var j in tr.data) {
                            if (tr.data.hasOwnProperty(j)) {
                                h += '<td>' + tr.data[j] + '</td>';
                            }
                        }
                    }
                    h += '</tr>';
                }
                $body.append(h);
            }
            if (data.columns) {
                var h = '<tr>';
                for (var j = 0; j < data.columns.length; j++) {
                    h += '<th>' + (data.columns[j].title || data.columns[j].data) + '</th>';
                }
                h += '</tr>';
                $head.append(h);
            }
            if (typeof loadOpts.success == 'function') {
                loadOpts.success(data);
            }
        } else {
            $head.append('<tr><th>提示</th></tr>');
            var message = "没有数据";
            if (data && data.Message) { message = data.Message; }
            $body.append('<tr><td>' + message + '</td></tr>');
        }

        //me.treegrid(opts);//使用了jquery的树型类

        setOddEvenRowCss($body);

        if (typeof loadOpts.complete == 'function') {
            loadOpts.complete(data);
        }
    };
    result.loadUrl = function (url, loadOpts) {
        loadOpts = loadOpts || {};
        $.post(url, null, function (data) {
            result.loadData(data, loadOpts);
            _url = url;
        });
    };
    var layerIdx = null;
    result.loadChildNode = function (td, url, loadOpts) {//单层加载(pfGrid).为便于使用,td可以传tr
        //debugger;
        //if (td.find) {
        //} else {
        //    td = $(td);
        //}
        td = $pf.getJQ(td);

        if (td[0].nodeName === 'TR') {
            //debugger;
            td = td.find('td .hitarea').parent();
        }
        //var td = $(td);
        var tr = td.parent('tr');
        var level = tr.attr('level');
        var expanded = tr.attr('expanded');
        var loaded = tr.attr('loaded');
        //var me = this;
        loadOpts = loadOpts || {};
        //debugger;
        //$pf.expandTree(td);
        if (//expanded !== 'expanded' &&
            loaded !== 'loaded') {//pfUtils.js里展开了
            //debugger;
            layerIdx = layer.load('正在查询请稍候');
            $.post(url, null, function (data) {
                if (data && data.Result !== false) {
                    if (data.data) {
                        //var h = createTrByTreeData(data,tr);
                        //这部分代码暂不要共用了,因为只是单层的
                        var h = '';
                        var iLevel = null;
                        if (level !== null && level !== undefined) { iLevel = parseInt(level) + 1; }

                        for (var i = 0; i < data.data.length; i++) {
                            var trData = data.data[i].Data;
                            var ij = 0;
                            h += '<tr class="pfGridItem" level="' + iLevel + '" >';

                            //debugger;
                            if (selectable) {
                                h += "<td><input type=\"checkbox\" class=\"pf-row-select\"></td>";
                            }
                            //就根据后台的columns顺序来生成好了,因为打印方法要使用,所以后台反正要写columns的,以后台为准
                            $pf.eachLeaf({ Children: data.columns }, function (c) {
                                var txt = trData[c.data];
                                if (txt === undefined) { txt = ''; }
                                if (ij === 0) {
                                    var linearea = '';
                                    for (var k = 0; k < iLevel; k++) {
                                        //异步也可以划线--benjamin20190712
                                        var lineClass = '';
                                        if (k < iLevel - 1) {//当不是最右边的格
                                            var pLine = tr.find('.linearea').eq(k);
                                            if (pLine.hasClass('linearea-ud') || pLine.hasClass('linearea-urd')) {
                                                lineClass = 'linearea-ud';
                                            }
                                        } else {
                                            if (i < data.data.length - 1) {//当不是最后一行
                                                lineClass = 'linearea-urd';
                                            } else {
                                                lineClass = 'linearea-ur';
                                            }
                                        }

                                        linearea += '<div class="linearea ' + lineClass + '"></div>';
                                    }
                                    h += '<td>' + linearea + '<div class="hitarea hitarea-closed"></div>' + txt + '</td>';
                                } else {
                                    h += createTd(c, txt);
                                }
                                ij++;
                            });

                            h += '</tr>';
                        }
                        if (h !== '') { $(tr).after(h); }

                        //$pf.expandTree(td);
                        //var hitarea = td.find('.hitarea');
                        //tr.attr('expanded', 'expanded');
                        //hitarea.removeClass('hitarea-closed');
                        //hitarea.addClass('hitarea-expanded');
                        drawCallback();
                    }
                }
                tr.attr('loaded', 'loaded');
                if (layerIdx) { layer.close(layerIdx); }
                //debugger;
                setOddEvenRowCss(me.find('tbody'));
                //me.find('tbody').find("tr:odd").css("backgroundColor", "#eff6fa");//奇偶行样式
                //me.find('tbody').find("tr:even").css("backgroundColor", "white");
                if (typeof loadOpts.complete == 'function') {
                    loadOpts.complete(data);
                }
                //result.loadData(data, loadOpts);
                //_url = url;
            });
        }
        //else {
        //    setFixedColumnWidth();
        //    //$pf.expandTree(td);
        //}
    };
    result.setUrl = function (url) {
        _url = url;

    };
    //result.initTable = function () {//用页面上已生成tr节点的完整Table来初始化
    //    $(selector).treegrid(opts);
    //    //$(selector + ' tbody tr td:nth-child(1)').click(function (e) { $pf.expandTree(this); });//展开事件
    //    $(selector).on('click', 'tbody tr td:nth-child(1)', function (e) {//注意索引是从1开始的
    //        $pf.expandTree(this);
    //    });
    //};
    result.exportExcel = function (event) {
        //alert(1);
        //debugger;
        var url = _url;
        if (url[0] !== '/') {//如果是相对地址,要转为绝对地址
            url = window.location.pathname.replace(/[^/]*$/, url)
        }
        //debugger;
        $pf.exporter({
            title: null,
            dataAction: url
        })
        .download($(event.currentTarget).attr("suffix"))
        ;
    };
    //工具栏
    //debugger;
    var buttons = [];
    var excelBtn = {
        text: '导出Excel', action: result.exportExcel
    };
    if (opts.buttons instanceof Array) {
        for (var i = 0; i < opts.buttons.length; i++) {
            switch (opts.buttons[i].Name) {
                case 'Export':
                case 'excel':
                    buttons.push(excelBtn);
                    break;
                default:
                    //buttons.push({ text: opts.buttons[i].Text });
                    (function (name, text, alwayShow) {//注意action会有作用域的问题
                        buttons.push({
                            text: text,
                            alwayShow: alwayShow,
                            action: function (event) {
                                //debugger;
                                var data = [];
                                var rows = result.getSelectedRows();
                                if (rows.length > 0) {
                                    for (var i = 0; i < rows.length; i++) {
                                        data.push(result.tableApi.row(rows[i]).data());
                                    }
                                }
                                //var data = result.tableApi.row('.selected').data();
                                $pfTable.trigger('tbar.' + name, [data]);
                            }
                        });
                    }(opts.buttons[i].Name, opts.buttons[i].Text, opts.buttons[i].AlwayShow));
                    break;
            }
            //if (typeof opts.buttons[i] === 'string') {//字符串型(系统按钮)
            //    switch (opts.buttons[i]) {
            //        case 'Export':
            //        case 'excel':
            //            buttons.push(excelBtn);
            //            break;
            //        default:
            //            //buttons.push(opts.buttons[i]);
            //            break;
            //    }
            //} else {
            //    buttons.push(opts.buttons[i]);//对象型(自定义按钮)
            //}
        }
    } else {
        //buttons = [excelBtn];
        buttons = [];
    }
    if (buttons.length > 0) {
        var $operate = $('<div class="table-operate ue-clear"></div>');// style="display:none;"
        //var buttons = $pf.getTableToolbar(opts);
        for (var i = 0; i < buttons.length; i++) {
            var button = $('<a href="javascript:;" class="' + (buttons[i].icon || 'add') + '" id="' + me.id + '-exportExcel">' + buttons[i].text + '</a>');
            button.on('click', buttons[i].action);
            button.appendTo($operate);
        }
        //me.before($operate);
        me.parent().before($operate);//放在table-box外面才不会影响锁定列(原版也是放在box外)--ben20190627
    }
    drawCallback();

    result.tableApi = {
        row: function (rowDom) {
            return {
                data: function () {
                    //var cols = 
                    //var colIdxs = $pf.getTableColumnIdxs(selector);
                    //var colIdxs = $pf.getTableColumnIdxs(selector);

                    //me.find('thead tr td').each(function (i, ele) {
                    //    var td = $(ele);
                    //    var t = td.attr('field-name') || td.text();
                    //    cols.push(t);
                    //});
                    var row = $(rowDom);
                    var r = {};
                    for (var i = 0; i < _colIdxs.length; i++) {
                        r[_colIdxs[i]] = row.children()[i].innerText;
                    }
                    return r;
                }
            }
        }
    };
    result.isRowSelectAll = function () {
        //return $headTable.find('thead tr input.pf-row-select-all').is(':checked');
        return $pfTable.find('thead tr input.pf-row-select-all').is(':checked');
    };
    result.getSelectedRows = function () {
        return $pfTable.find('tbody tr.selected');
    };

    me.on('click', 'tbody tr td:nth-child(' + (selectable ? 2 : 1) + ') .hitarea', function (e) {//注意索引是从1开始的
        //alert(1);
        $pf.stopPropagation(e);
        var hitarea = $(this);
        var td = hitarea.parent();
        $pf.expandTree(td);

        if (typeof opts.onRowExpand == 'function') {
            var tr = td.parent('tr');
            //var rowData = result.tableApi.row(tr).data();
            ////var rowData = {};
            ////for (var i = 0; i < _colIdxs.length; i++) {
            ////    rowData[_colIdxs[i]] = tr.find('td:nth-child(' + (i + 1) + ')').text();
            ////}
            //////opts.onRowExpand();
            opts.onRowExpand.call(hitarea, tr);
        }
        //defaultDrawCallback();
        resetFixedColumnSize();//重设宽度也不够,要重设高度
    });
    if (selectable) {
        result.selectRow = function (tr) {
            if (tr.hasClass('selected')) {
                tr.removeClass('selected');
                tr.find('input.pf-row-select').prop("checked", false);
            }
            else {
                if (selectMode == 1) {
                    var trs = result.tableApi.$('tr.selected');
                    var cbs = trs.find('input.pf-row-select');
                    cbs.prop("checked", false);
                    trs.removeClass('selected');
                }
                tr.addClass('selected');
                tr.find('input.pf-row-select').prop("checked", true);
            }
        };
        $pfTable.on('click', 'thead tr input.pf-row-select-all', function (e) {
            $pf.stopPropagation(e);
            var b = $(this).is(':checked');

            var cbs = $pfTable.find('tbody tr input.pf-row-select');
            cbs.each(function (index, element) {
                var cb = $(element);
                if (cb.is(':checked') !== b) {
                    cb.click();
                    //cb.trigger('click');
                }
            });

        });
        //顺序:
        //1.全选->行checkbox点击->result.selectRow()
        //2.tr点击->行checkbox点击->result.selectRow()
        $pfTable.on('click', 'tbody tr input.pf-row-select', function (e) {
            $pf.stopPropagation(e);
            var tr = $(this).parent().parent();

            //这段是参照pfTable的,但忘记当时为什么这样写了
            //var tempTb = tr.parent().parent();
            //if (tempTb.attr('id') !== $pfTable.attr('id')) {//如果有锁定列时,点击锁定列时进入这里
            //    var idx = tr.parent().find('tr').index(tr);//行序号
            //    $pfTable.find('tbody tr:nth-child(' + (idx + 1) + ')').click();
            //    return;
            //}

            if (opts.fixedColumns !== undefined && opts.fixedColumns !== false) {//如果有锁定列时,点击普通列进入这里
                var idx = tr.parent().find('tr').index(tr);//行序号
                result.selectRow($('#' + me[0].id + '-fixedColumn tbody tr:nth-child(' + (idx + 1) + ') '));
            }

            result.selectRow(tr);
        });
        $pfTable.on('click', 'tbody tr', function (e) {
            $pf.stopPropagation(e);
            $(this).find('input.pf-row-select').click();

        });

    }
    return result;
};

/*
*统计图表(需要引用jqwidgets)
*/
jQuery.fn.pfPivotTable = function (opts) {
    var $pfTable = this;
    var me = $pfTable;
    $pfTable.css('width', '800px');
    $pfTable.css('height', '400px');
    $pfTable.css('backgroundColor', 'white');

    var result = {};
    var pfOpts = {};
    var errorOpts = ['buttons'];//这些属性放到jqxPivotGrid里会报错--benjamin20191220
    for (var i = 0; i < errorOpts.length; i++) {
        if (opts[errorOpts[i]] !== undefined) {
            pfOpts[errorOpts[i]] = opts[errorOpts[i]];
            delete opts[errorOpts[i]];
        }
    }

    var defaultDrawCallback = function () {
        //工具栏按钮
        var buttons = [];
        var excelBtn = {
            text: '导出Excel', action: result.exportExcel
        };
        if (pfOpts.buttons instanceof Array) {
            for (var i = 0; i < pfOpts.buttons.length; i++) {
                switch (pfOpts.buttons[i].Name) {
                    case 'Export':
                    case 'excel':
                        buttons.push(excelBtn);
                        break;
                    default:
                        //buttons.push({ text: pfOpts.buttons[i].Text });
                        (function (name, text, alwayShow) {//注意action会有作用域的问题
                            buttons.push({
                                text: text,
                                alwayShow: alwayShow,
                                action: function (event) {
                                    //debugger;
                                    var data = [];
                                    var rows = result.getSelectedRows();
                                    if (rows.length > 0) {
                                        for (var i = 0; i < rows.length; i++) {
                                            data.push(result.tableApi.row(rows[i]).data());
                                        }
                                    }
                                    //var data = result.tableApi.row('.selected').data();
                                    $pfTable.trigger('tbar.' + name, [data]);
                                }
                            });
                        }(pfOpts.buttons[i].Name, pfOpts.buttons[i].Text, pfOpts.buttons[i].AlwayShow));
                        break;
                }
                //if (typeof pfOpts.buttons[i] === 'string') {//字符串型(系统按钮)
                //    switch (pfOpts.buttons[i]) {
                //        case 'Export':
                //        case 'excel':
                //            buttons.push(excelBtn);
                //            break;
                //        default:
                //            //buttons.push(pfOpts.buttons[i]);
                //            break;
                //    }
                //} else {
                //    buttons.push(pfOpts.buttons[i]);//对象型(自定义按钮)
                //}
            }
        } else {
            //buttons = [excelBtn];
            buttons = [];
        }
        if (buttons.length > 0) {
            me.parent().parent().find('.table-operate').remove();
            var $operate = $('<div class="table-operate ue-clear"></div>');// style="display:none;"
            //var buttons = $pf.getTableToolbar(opts);
            for (var i = 0; i < buttons.length; i++) {
                var button = $('<a href="javascript:;" class="' + (buttons[i].icon || 'add') + '" id="' + me.id + '-exportExcel">' + buttons[i].text + '</a>');
                button.on('click', buttons[i].action);
                button.appendTo($operate);
            }
            //me.before($operate);
            me.parent().before($operate);//放在table-box外面才不会影响锁定列(原版也是放在box外)--ben20190627
            $operate.width(me.width() - 15);//15是toobar的padding-left
        }
    };

    var drawCallback = null;
    if (typeof opts.drawCallback == 'function') {
        var userDrawCallback = opts.drawCallback;
        drawCallback = function () {
            defaultDrawCallback();
            userDrawCallback();
        }
        delete opts.drawCallback;
    } else {
        drawCallback = defaultDrawCallback;
    }
    var defaultOpts = jQuery.extend({
        //theme: 'classic',
        //source: pivotAdapter,
        treeStyleRows: true,
        autoResize: false,
        multipleSelectionEnabled: true
    }, opts || {});

    function newOpts() {
        var r = jQuery.extend({}, defaultOpts);
        //if (r.order instanceof Array) {
        //    r.order = r.order.slice(0);//数组型的成员要这样才不会因opts改变而改变
        //}
        return r;
    }
    opts = newOpts();

    var _url = '';

    var $errorTableContainer = $('<div class="table"><div class="table-box"><table></table></div></div>')
    //var $errorTable = $('<table></table>');
    var $errorTable = $errorTableContainer.find('table');
    $pfTable.after($errorTableContainer);
    result.loadUrl = function (url, loadOpts) {
        loadOpts = loadOpts || {};
        var layerIdx = layer.load('正在查询请稍候');
        $.post(url, null, function (data) {
            if (data && data.Result !== false) {
                $errorTable.hide();
                $pfTable.show();
                _url = url;

                //var data = new Array();

                //var firstNames =
                //[
                //    "Andrew", "Nancy", "Shelley", "Regina", "Yoshi", "Antoni", "Mayumi", "Ian", "Peter", "Lars", "Petra", "Martin", "Sven", "Elio", "Beate", "Cheryl", "Michael", "Guylene"
                //];

                //var lastNames =
                //[
                //    "Fuller", "Davolio", "Burke", "Murphy", "Nagase", "Saavedra", "Ohno", "Devling", "Wilson", "Peterson", "Winkler", "Bein", "Petersen", "Rossi", "Vileid", "Saylor", "Bjorn", "Nodier"
                //];

                //var productNames =
                //[
                //    "Black Tea", "Green Tea", "Caffe Espresso"
                //];

                //var priceValues =
                //[
                //    "2.25", "1.5", "3.0", "3.3", "4.5", "3.6", "3.8", "2.5", "5.0", "1.75", "3.25", "4.0"
                //];

                //for (var i = 0; i < 500; i++) {
                //    var row = {};
                //    var productindex = Math.floor(Math.random() * productNames.length);
                //    var price = parseFloat(priceValues[productindex]);
                //    var quantity = 1 + Math.round(Math.random() * 10);

                //    row["firstname"] = firstNames[Math.floor(Math.random() * firstNames.length)];
                //    row["lastname"] = lastNames[Math.floor(Math.random() * lastNames.length)];
                //    row["productname"] = productNames[productindex];
                //    row["price"] = price;
                //    row["quantity"] = quantity;
                //    row["total"] = price * quantity;

                //    data[i] = row;
                //}

                var datafields = [];
                for (var i = 0; i < data.columns.length; i++) {
                    datafields.push({
                        name: data.columns[i].data,
                        type: data.columns[i].dataType
                    });
                }

                // create a data source and data adapter
                var source =
                {
                    //localdata: data,
                    localdata: data.data,
                    datatype: "array",
                    datafields: datafields
                    //[
                    //    { name: 'firstname', type: 'string' },
                    //    { name: 'lastname', type: 'string' },
                    //    { name: 'productname', type: 'string' },
                    //    { name: 'quantity', type: 'number' },
                    //    { name: 'price', type: 'number' },
                    //    { name: 'total', type: 'number' }
                    //]
                };

                var dataAdapter = new $.jqx.dataAdapter(source);
                dataAdapter.dataBind();

                opts = newOpts();
                // create a pivot adapter from the dataAdapter
                var pivotAdapter = new $.jqx.pivot(
                    dataAdapter,
                    {
                        pivotValuesOnRows: false,
                        totals: { rows: { subtotals: true, grandtotals: true }, columns: { subtotals: false, grandtotals: true } },
                        //rows: [{ dataField: 'firstname' }, { dataField: 'lastname' }],
                        //columns: [{ dataField: 'productname' }],
                        //values: [
                        //    { dataField: 'price', 'function': 'sum', text: 'sum', width: NamedNodeMap, formatSettings: { prefix: '$', decimalPlaces: 2 } },
                        //    { dataField: 'price', 'function': 'count', text: 'count' },
                        //    { dataField: 'price', 'function': 'average', text: 'average', formatSettings: { prefix: '$', decimalPlaces: 2 } }
                        //]

                        //rows: [{ dataField: '店主姓名' }, { dataField: '年龄段' }],//YJLevel功能的配置
                        //columns: [{ dataField: '当月级别' }],
                        //values: [
                        //    {
                        //        dataField: '人数', 'function': 'sum', text: '人数', width: NamedNodeMap
                        //        //, formatSettings: { prefix: '$', decimalPlaces: 0 } 
                        //    },
                        //]
                        rows: opts.rows,
                        columns: opts.columns,
                        values: opts.values
                    });
                delete opts.rows;
                delete opts.columns;
                delete opts.values;
                opts.source = pivotAdapter;
                if (typeof getJqxPivotLocalization == 'function') {
                    opts.localization = getJqxPivotLocalization('chs');
                }

                var aaa = $pfTable.jqxPivotGrid(opts
                    //{
                    //    theme: 'classic',
                    //    source: pivotAdapter,
                    //    treeStyleRows: true,
                    //    autoResize: false,
                    //    multipleSelectionEnabled: true
                    //}
                    );
                //debugger;
                drawCallback();
            } else {
                var message = "没有数据";
                if (data && data.Message) { message = data.Message; }
                //$pf.warningPopups(message);
                $errorTable.html('<thead><tr><th style="border:1px solid #c1d3de;background-color:#E8F1F7;">提示</th></tr></thead><tbody><tr onclick="$pf.stopPropagation(arguments[0])"><td  onclick="$pf.stopPropagation(arguments[0])"  style="border:1px solid rgb(17, 17, 17);vertical-align:top;height:auto;">' + message + '</td></tr></tbody>');
                $pfTable.hide();
                $errorTable.show();
            }
            if (layerIdx) { layer.close(layerIdx); }
        });
    };

    result.exportExcel = function (event) {
        //alert(1);
        //debugger;
        var url = _url;
        if (url[0] !== '/') {//如果是相对地址,要转为绝对地址
            url = window.location.pathname.replace(/[^/]*$/, url)
        }
        ////debugger;
        //url = $pf.setUrlParams(url, {
        //    pivotLeft: $pf.listSelect(me.jqxPivotGrid('source').rows,'dataField'),
        //    pivotTop: $pf.listSelect(me.jqxPivotGrid('source').columns, 'dataField'),
        //    isPivotTable:true
        //});
        //debugger;
        $pf.exporter({
            title: null,
            dataAction: url,
            action: $pf.setUrlParams(window.location.pathname.replace(/^(\/[^\/]+\/[^\/]+).*$/, '$1' + '/DownloadPivotTable'), {
                pivotLeft: $pf.listSelect(me.jqxPivotGrid('source').rows, 'dataField'),
                pivotTop: $pf.listSelect(me.jqxPivotGrid('source').columns, 'dataField'),
                pivotValue: $pf.listSelect(me.jqxPivotGrid('source').values, 'dataField'),
                isPivotTable: true
            })
        })
        .download($(event.currentTarget).attr("suffix"))
        ;
    };
    return result;
};



