﻿$(document).ready(function () {
    var userId = getParameterByName('id');

    $("#divGrid").jqGrid({
        ajaxGridOptions: { contentType: 'application/json; charset=utf-8' },
        prmNames: {
            rows: "numRows",
            page: "pageNumber"
        },
        postData: {userId: userId },
        datatype: function (postdata) {
                var dataUrl = 'UploaderService.svc/GetAllFiles'
            $.ajax({
                url: dataUrl,
                type: "POST",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                data: JSON.stringify(postdata),
                success: function (data, st) {
                    if (st == "success" && JSON.parse(data.d.indexOf("_Error_") != 0)) {
                        var grid = $("#divGrid")[0];
                        grid.addJSONData(JSON.parse(data.d));
                    }
                },
                error: function () {
                    alert("Error while processing your request");
                }
            });
        },
        colNames: ['Id', 'File Name', 'Uploaded By', 'Uploaded On', 'Size(Kb)', 'Status'], //define column names
        colModel: [
        {
            name: 'id', index: 'id', key: true, width: 100, sortable: true, searchoptions: {
                searchOperators: true,
                sopt: ['eq', 'ne', 'lt', 'le', 'gt', 'ge']
            }
        },
        {
            name: 'fileName', index: 'fileName', width: 350, formatter: 'showlink',
            formatoptions: { target: "_new", baseLinkUrl: 'DownloadFile.aspx' }, sorttype: 'text', sortable: true,
            searchoptions: {
                searchOperators: true,
                sopt: ['cn', 'nc']
            }
        },
        {
            name: 'uploadedBy', index: 'uploadedBy', width: 200, sorttype: 'text', sortable: true, searchoptions: {
                searchOperators: true,
                sopt: ['cn', 'nc']
            }
        },
        { name: 'uploadedOn', index: 'uploadedOn', width: 200, sortable: true, search:false},
        { name: 'size', index: 'size', width: 100, sortable: true, search: false },
        {
            name: 'status', index: 'status', width: 100, sortable: true, searchoptions: {
                searchOperators: true,
                sopt: ['eq', 'ne']
            }
        }
        ], //define column models
        pager: '#divPaging',
        sortname: 'uploadedOn', //the column according to which data is to be sorted; optional
        viewrecords: true, //if true, displays the total number of records, etc. as: "View X to Y out of Z” optional
        sortorder: "desc", //sort order; optional
        caption: "Files",
        multiselect: true,
        rowNum: 20,
        loadonce: false,
        autowidth: true,
        shrinkToFit: true,
        height: '100%',
        rowList: [10, 20, 30, 50, 100],
        sortable: true,
        beforeSelectRow: function (rowid, e) {
            var $link = $('a', e.target);
            if (e.target.tagName.toUpperCase() === "A" || $link.length > 0) {
                // link exist in the item which is clicked
                return false;
            }
            return true;
        }
    }).navGrid("#divPaging", { search: true, edit: false, add: false, del: false }, {}, {}, {}, { multipleSearch: false });
});$(window).resize(function () {
    var myGrid = $('#divGrid');
    var contentWidth = $('#divMain').width()
    myGrid.setGridWidth(contentWidth);
});
function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}