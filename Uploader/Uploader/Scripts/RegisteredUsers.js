﻿$(document).ready(function () {

   
    var myGrid = $("#dvUsersGrid");
    myGrid.jqGrid({
        ajaxGridOptions: { contentType: 'application/json; charset=utf-8' },
        prmNames: {
            rows: "numRows",
            page: "pageNumber"
        },
        datatype: function (postdata) {
                var dataUrl = 'UploaderService.svc/GetAllUsersDetail'
            $.ajax({
                url: dataUrl,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(postdata),
                success: function (data, st) {
                    if (st == "success" && JSON.parse(data.d.indexOf("_Error_") != 0)) {
                        var grid = $("#dvUsersGrid")[0];
                        grid.addJSONData(JSON.parse(data.d));
                    }
                },
                error: function () {
                    alert("Error while processing your request");
                }
            });
        },
        colNames: ['User Id','User Name', 'Allocated Space(in MB)', 'Used Space(in MB)', 'Free Space(in MB)', 'Total Files(Active/Inactive)', 'Last login'], //define column names
        colModel: [
        {name: 'UserId', key: true, index: 'UserId', hidden:true},
        {
            name: 'UserName', index: 'UserName', width: 130, sorttype: 'text', sortable: true, formatter: 'showlink',
            formatoptions: { baseLinkUrl: 'Admin.aspx' },
            searchoptions: {
                searchOperators: true,
                sopt: ['cn', 'nc']
            }
        },
        { name: 'AllocatedSpace', index: 'AllocatedSpace', width: 100, sorttype: 'text', sortable: true, editable: true, search:false },
        { name: 'UsedSpace', index: 'UsedSpace', width: 100, sortable: true, search: false },
        { name: 'FreeSpace', index: 'FreeSpace', width: 100, sortable: true, search: false },
        { name: 'TotalFiles', index: 'TotalFiles', width: 130, sortable: true, search: false },
        { name: 'LastLogin', index: 'LastLogin', width: 90, sortable: true, search: false }
        ], //define column models
        pager: '#dvPaging',
        sortname: 'LastLogin', //the column according to which data is to be sorted; optional
        viewrecords: true, //if true, displays the total number of records, etc. as: "View X to Y out of Z” optional
        sortorder: "desc", //sort order; optional
        caption: "User Details",
        multiselect: true,
        rowNum: 20,
        loadonce: false,
        autowidth: true,
        shrinkToFit: true,
        height: '100%',
        rowList: [10, 20, 30, 50, 100],
        sortable: true,
        cellEdit: true,
        cellsubmit: 'clientArray',
        beforeSubmitCell: function (rowid, cellname, value, iRow, iCol) {

            //Get the selected row values
            var selected_row = myGrid.jqGrid('getGridParam', 'selrow');
            var rowdata = myGrid.getRowData(selected_row);
            var userId = rowdata.UserId;

            $.ajax({
                url: 'UploaderService.svc/UpdateTotalSpace',
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: '{"userId":"' + userId + '","colValue":"' + value + '","colName":"' + cellname + '"}',
                success: function (msg) {
                    if (msg.d == true) {
                        myGrid.trigger("reloadGrid");
                    }
                },
                error: function () {
                    alert("Error while processing your request");
                }
            });
        }
    }).navGrid("#dvPaging", { search: true, edit: false, add: false, del: false }, {}, {}, {}, { multipleSearch: false });
});$(window).resize(function () {
    var myGrid = $('#dvUsersGrid');
    var contentWidth = $('#dvRegisteredUsers').width()
    myGrid.setGridWidth(contentWidth);
});
