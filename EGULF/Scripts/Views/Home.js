$(document).ready(function () {
    Base.call(Home);
    Home.Init();
    Home.OnSaveHome();
    Home.permissions.canCreate = true;
    var table = $('#datatable-buttons').DataTable({
            lengthChange: false,
            //buttons: ['copy', 'excel', 'pdf']
        });

        table.buttons().container()
                .appendTo('#datatable-buttons_wrapper .col-md-6:eq(0)');
} );


var Home = {
    vars: {
        idsToDelete: 0,
        itemsIds: [],
        table : null,
        name: "Omar",
        form: {
            name: $('#name')
        },
        $form: $('form#homeform')

    },
    Init: function () {
        $(".select2").select2();
        $("#btn1").on('click',this,function (context) {
            console.log($(this).text());
            context.data.Modal.Oops("By passing a parameter, you can execute something else for 'Cancel'.");
        });

        this.Modal.ButtonClick("#btn2", "/Home#",'success');

        $("#btn3").click(function () {
            Home.Modal.Generic("error", "Correcto!", "Operación demo ejecutada exitosamente", "Aceptar", "Cerrar");
        })

        $("#toast1").click(function(){
            Home.Toast("success","top rigth","Correto","implementación del toas!, se puede cambiar los colores solo con la propiedad type, y la colocacion con la propiedad position",{timeOut: 3000, hideDuration: 0});
        });
        $("#ajax1").on('click',this,this.GetDataPerson);

        $("#ajax2").on('click',this,this.GetDataError);

        this.LoadTable();
        $('#form2').parsley();
    },

    GetDataPerson: function(context){
        context.data.BlockUI();
        let f = context.data.vars.form;
        console.log(f.name.val());
        context.data.Ajax("https://swapi.co/api/people/1/", 'GET', null, true, function(data,ctx){
            console.log(data);
            $('#contentApi').html(data.name);
        }, function(xhr,textStatus,ctx){
            console.log("EndAjax");
            ctx.Toast("success","top rigth","Correto","Carga correcta "+ctx.vars.form.name.val());
        })
    },

    GetDataError: function(context){
        context.data.BlockUI();
        Home.Ajax("https://swapi.co/api/peopl/", 'GET', null, true, function(data,ctx){
            console.log(data);
            $('#contentApi').html(data.name);
        }, function(xhr, txtStatus,ctx){
            console.log("EndAjax");
            if(txtStatus == 'success')
                ctx.Toast("success","top rigth","Correto","Carga correcta");
        })
    },

    LoadTable: function(){
        this.vars.table = this.CreateTable('#datatable1',
            '/Example/Get',
            this.tableContent.columns,
            this.tableContent.columnsDef,
            this.tableContent.fnServerData,
            this.tableCallbacks.onDraw, this.tableCallbacks.onDrawRow, ()=> {
                $('.item-delete').on('click', this, this.OnDeleteRowConfirm);

                var lengthBtn = this.vars.table.buttons().length
                var lengthCol = this.vars.table.columns()[0].length
                if (!this.permissions.canCreate)
                    this.vars.table.buttons(lengthBtn - 1).remove();

                this.vars.table.buttons().enable()
            }).order([[0,'desc']]);

        
        
    },

    tableContent: {
        columns: [
            { data: 'NotificationId', orderName: 'NotificationId', searchable: false},
            { data: 'Type', orderName: 'Type' },
            { data: 'Date', orderName: 'Date' },
            { data: 'Description', orderName: 'Description' },
            { data: 'Status', orderName: 'Status' },
            { data: 'SourceId', orderName: 'SourceId', searchable: false }
        ],
        columnsDef: [
            { "orderSequence": [ "desc" ], "targets": [ 0 ] },
            {
                targets: 2,
                data: 'Date',
                render: function (data, type, row, meta) {
                    return moment(data).format("DD/MM/YYYY");
                }
            },
            {
                targets: 4,
                data: 'Status',
                render: function (data, type, row, meta) {
                    var status = 'indefinido';
                    var style = 'label-warning';
                    if (data !== null && data !== '') {
                        status = data ? 'visto' : 'no visto';
                        style = data ? 'badge-success' : 'badge-danger';
                    }
                    return '<div style="text-align: center; margin: auto"><span class="badge ' + style + '">' + status + '</span></div>';
                }
            },
            {
                targets: 5,
                data: 'SourceId',
                render: function (data, type, row, meta) {
                    let ctx = Home;
                    var url = '#';
                    if (data !== null && data !== '')
                        url = '/Example';
                    let data_dom = '<span>' + data + '<div class="btns-ctrl" style="display: inline-block; float: right; visibility: hidden; z-index: 9999">';
                    if (ctx.permissions.canEdit)
                        data_dom += '<a title="Editar" href="' + url + '" class="badge badge-warning ml-1"><i class="fa fa-edit"></i></a>'
                    if (ctx.permissions.canDelete)
                        data_dom += '<div title="Eliminar" href="#" class="badge badge-danger ml-1 item-delete" data-id="' + row.NotificationId + '"><i class="fa fa-window-close-o"></i></div>';
                    data_dom += '</div></span>';
                    return data_dom;
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
        onDraw: function() {//Cuando se pinta la tabla
            //this.vars.idsToDelete.length = 0;   
        },
        onDrawRow: function(row,data,index) {//Cuando se pinta cada fila
            /*let $row = $(row);
            this.vars.itemsIds[index] = data.id;*/

        },
        
    },

    OnSaveHome : function(){
        this.FormValidate(this.vars.$form,e =>{
            let ctx = e.data
            ctx.BlockUI();
            e.preventDefault();//La accion por default del evento no se ejecuta.
            let data = ctx.vars.$form.serialize();
        }, invalidCallback = ()=>{})
    },

    OnDeleteRowConfirm: function (ctx) {
        ctx.data.delete_id = $(this).data('id');
        ctx.data.Modal.DeleteConfirm(ctx.data.Delete,ctx.data);
    },

    Delete: function (ctx) {
        //AJAX PARA ELIMINAR REGISTRO        
        /*ctx.Ajax('/business/rows_delete', 'get', { rows: idsArray }, true,
		function (data) {
			
			console.log(data);
		});*/
        ctx.vars.table.ajax.reload();
        ctx.Toast("success", "top rigth", "Correto", "Eliminación exitosa", { timeOut: 3000, hideDuration: 0 });//Debe ir realmente en el succuess del ajax
        
    },

}