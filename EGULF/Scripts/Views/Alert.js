var Alert = {
    vars: {
        table: null,
        tags: {}
    },

    Init: function(){
        this.LoadTable();
    },

    MarkAsReaded: function (AlertId) {
        Main.MarkAsReaded(AlertId);
        this.vars.table.draw('page');
    },

    LoadTable: function(){
        this.vars.table = this.CreateTable('#tblNotifications',
            '/Alert/GetTable',
            this.tableContent.columns,
            this.tableContent.columnsDef,
            this.tableContent.fnServerData,
            this.tableCallbacks.onDraw, this.tableCallbacks.onDrawRow,
            () => {
                $('.item-delete').on('click', this, this.OnDeleteRowConfirm);

                for (var x = 4; x >= 0; x--)
                    this.vars.table.buttons(x).remove();    //Borra los botones de descarga

                var lengthBtn = this.vars.table.buttons().length
                var lengthCol = this.vars.table.columns()[0].length
                //if (!this.permissions.canCreate)
                //    this.vars.table.buttons(lengthBtn - 1).remove();

                this.vars.table.buttons().enable()
            })
            .order([[1, "desc"]]);
    },

    tableContent: {
        columns: [
            { data: 'Status', orderName: 'Status' },
            { data: 'TimeAgo', orderName: 'CreatedAt'},
            { data: 'Body', orderName: 'Body' },
        ],
        columnsDef: [
            {
                targets: 0,
                render: function (data, type, row, meta) {
                    var status = data == 2 ? Alert.vars.tags.Nuevo : Alert.vars.tags.Visto;
                    var style = data == 2 ? "badge-warning" : "badge-success";
                    return '<div style="text-align: center; margin: auto"><span class="badge ' + style + '">' + status + '</span></div>';
                }
            },
            {
                targets: 2,
                render: function (data, type, row, meta) {
                    var html = '<span>' + data + '</span>';
                    if(row.Status == 1)
                        html = '<span>' + data + '<div class="btns-ctrl" style="display: inline-block; float: right; visibility: hidden; z-index: 9999">' +
                            '<a title="' + Alert.vars.tags.MarkAsReaded + '" href="javascript:Alert.MarkAsReaded(' + row.AlertId + ')" class="badge badge-success ml-1"><i class="fa fa-check"></i></a>' +
                            '</div></span>'
                        return html;
                }
            },
        ],
        fnServerData: function (sSource, aoData, fnCallback) {
            aoData.push({ "name": "sSortColumn", "value": this.fnSettings().aoColumns[this.fnSettings().aaSorting[0][0]].orderName });
            $.getJSON(sSource, aoData, function (json) {
                fnCallback(json);
            });
        },
    },
    tableCallbacks: {
        onDraw: function() {},
        onDrawRow: function(row,data,index) {}
    },

};