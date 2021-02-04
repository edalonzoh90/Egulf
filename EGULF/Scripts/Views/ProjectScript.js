
$(document).ready(function () {
    Base.call(Project);
    Project.Init();
});

var Project = {
    vars: {
        urlCntrl: _UrlCntrl,
        urlCntrlEdit: _UrlCntrlEdit,
        urlCntrlManage: _UrlCntrlManage,
        urlCntrlDelete: _UrlCntrlDelete,
        url: {
            create: "/project/create"
        },
        companyId: _CompanyId,
        emptySubject: _EmptySubject,
        empryMessage: _EmptyMessage
    },
    controls: {

    },
    ui: {
        emptySection: $("#EmptySection"),
        mainSection: $("#MainSection")
    },

    Init: function () {     
        Project.ValidateCompany();
    },

    ValidateCompany: function(){
        if (Project.vars.companyId > 0) {
            Project.BlockTarget($(".grid"));
            Project.LoadTable();        
            Project.ui.emptySection.hide();
            Project.ui.mainSection.show();
        }
        else {
            Project.ui.emptySection.show();
            Project.ui.mainSection.hide();
            Project.Toast("warning", "top right", Project.vars.emptySubject, Project.vars.empryMessage);
        }
    },

    LoadTable: function () {
        this.url.create = this.vars.url.create;
        this.vars.table = Project.CreateTable("#gridProjects",
                                 Project.vars.urlCntrl + 'Get',
                                 Project.tableContent.columns,
                                 Project.tableContent.columnsDef,
                                 Project.tableContent.fnServerData,
                                 this.tableCallbacks.onDraw,
                                 this.tableCallbacks.onDrawRow,
                                 () => {
                                     var lengthBtn = this.vars.table.buttons().lengthBtn;
                                     for (var x = 3; x >= 0; x--)
                                         this.vars.table.buttons(x).remove();

                                     $(".input-filter").remove();
                                     $('.grid').unblock();
                                 }).order([[0,'desc']]);
    },

    tableContent: {
        columns: [
            { data: null, orderName: 'Status', searchable: false },
            { data: 'Folio', orderName: 'ProjectId', searchable: false },   
            {data:'ProjectType',orderName:'ProjectType',searchable:false},
            {data:'StartDate',orderName:'StartDate',searchable:false},
            { data: 'Duration', orderName: 'Duration', searchable: false },
            {data:null}
        ],
        columnsDef: [
            {
                targets: 0, render: function (row, meta)
                {
                    var color = '';
                    switch (row["Status"])
                    {
                        case 1:
                            color = 'badge-success';
                            break;
                        case 2:
                            color = 'badge-danger';
                            break;
                        case 3:
                            color = 'badge-info';
                            break;
                        case 4:
                            color = 'badge-primary'
                    }
                    return '<div style="text-align: center; margin: auto"><span class="badge ' + color + '">' + row["StatusDescription"] + '</span></div>';
                }
            },
            {
                targets: 3, data: 'StartDate', render: function (data, type, row, meta)
                {return moment(data).format("DD/MM/YYYY");}
            },
            {targets:5,render:function(row,meta)
            {
                var urlEdit = Project.vars.urlCntrlEdit;
                var urlManage = Project.vars.urlCntrlManage;
                var urlDelete = Project.vars.urlCntrlDelete;
                var cid = row["ProjectId"];
                var html = '<div class="text-center">';
                html += '<a href="' + urlEdit.replace("ID", cid) + '" class="badge badge-warning ml-1"><i class="fa fa-edit"></i></a>';
                html += '<a href="' + urlDelete.replace("ID", cid) + '" class="badge badge-danger ml-1"><i class="fa fa-trash"></i></a>';
                html += '</div>';
                return html;
            }
            }
        ],
        fnServerData: function (sSource, aoData, fnCallback) {
            aoData.push({ "name": "sSortColumn", "value": this.fnSettings().aoColumns[this.fnSettings().aaSorting[0][0]].orderName });
            $.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
            });
        },
    },

    tableCallbacks: {
        onDraw: function () {
        },
        onDrawRow: function (row, data, index) {
        }
    },


}