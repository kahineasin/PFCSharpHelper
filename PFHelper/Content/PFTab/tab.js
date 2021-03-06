﻿
if (jQuery !== null && jQuery !== undefined) {
    jQuery.fn.pfTab = function () {
        var me = this;
        var result = {};

        me.find('.nav-tabs li a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
            //setTimeout(function () {
            $.fn.dataTable.tables({ visible: true, api: true }).columns.adjust();
            //}, 500);
        });

        function addTab(tabId, tabName, opts) {
            var existTab = me.find('.nav-tabs li a[href="#' + tabId + '"]');
            if (existTab.length > 0) {
                existTab.click();
                return;
            }
            var newTab = $('<li><a  href="#' + tabId + '" data-toggle="tab">' + tabName + '<button class="close closeTab" type="button">×</button></a></li>');
            var newContent = null;
            if (opts.url !== undefined) {
                newContent = $('<div class="tab-pane fade" id="' + tabId + '"><iframe src="' + opts.url + '" width="100%" /></div>');
            }
            //debugger;
            //me.find('.nav-tabs').append($('<li><a  href="#' + tabId + '" data-toggle="tab">' + tabName + '</a><span class="fa fa-times-circle"></span></li>'));
            me.find('.nav-tabs').append(newTab);
            if (newContent !== null) {
                var $tabContent = me.find('.tab-content');
                var $iframe = newContent.find('iframe');
                //debugger;
                $tabContent.append(newContent);

                //newContent.width($tabContent.width());
                $iframe.height(me.height() - 40);
                //$iframe.height(me.height()-100);
            }
            newTab.find('button.closeTab').click(function () {

                var prevTab = newTab.prev();
                var curActive = newTab.hasClass('active');
                newTab.remove();
                if (newContent !== null) {
                    newContent.remove();
                }

                if (curActive && prevTab.length > 0) {
                    prevTab.find('a[data-toggle=tab]').tab('show');
                    //prevTab.find('a[data-toggle=tab]').click();
                }
            });
            newTab.find('a[data-toggle=tab]').tab('show');
            //newTab.find('a[data-toggle=tab]').click();
        }
        function onTabChange(callback) {
            me.find('a[data-toggle=tab]').on('show.bs.tab', callback);
        }
        //function getCurrentTabText() {//shown.bs.tab事件触发时,active的类名还没更新,shown事件才适合这么用,但loadGrid还是在show时合适
        //    return me.find('.nav-tabs li.active a').text();
        //}
        result.addTab = addTab;
        result.onTabChange = onTabChange;
        //result.getCurrentTabText = getCurrentTabText;
        return result;
    };
}