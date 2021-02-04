var Base = function(){

    this.delete_id = null
    this.permissions = {
        canCreate: true,
        canDelete: true,
        canEdit: true,
    }

    swalStyle = {
            btn_primary: '#1ab394',
            btn_danger: '#ed5565',
            btn_warning: '#f8ac59',
            btn_success: '#1c84c6',
            btn_info: '#23c6c8'
    };

    this.GetSwalStyle = function(){
        return swalStyle;
    }

    this.Loader = {
        hide: function(cssClass = '.with-loader'){
            //$(cssClass).removeClass('sk-loading');
        },
        show: function (cssClass = '.with-loader') {
            //$(cssClass).addClass('sk-loading');
        }
    };
    
    this.host = window.location.origin,
    
    this.url= {
        logoPath: '../src/logos/',
        dt_language: '../../Content/Json/language_dt.json',
        create : "#",
    };

    this.CreateTable = function(tableId, ajaxUrl, columns, columnsDefinition, serverData, onDrawCallback, onDrawRowCallback, onInitComplete = () => {}) {
        ctx = this;
        var table = $(tableId).DataTable({
            pageLength: 10,
            responsive: true,
            select: true,
            dom: '< <"input-filter"f>< <"html5buttons"B> ><l><t><"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>> >',
            //dom: '< <"row"<"col-sm-12 col-md-4"l><"col-sm-12 col-md-8"<"pull-right"f><"pull-right"B>>><t><"row"<"col-sm-12 col-md-6"i><"col-sm-12 col-md-6"p>> >',
            //dom: '< <"row"<"col-sm-12 col-md-4"l><"col-sm-12 col-md-4"B><"col-sm-12 col-md-4"f>><t><"row"<"col-sm-12 col-md-5"i><"col-sm-12 col-md-7"p>> >',
            buttons: [
                {extend: 'copy',  },
                {extend: 'csv'},
                {extend: 'excel', title: 'Data', class:"x"},
                {extend: 'pdf', title: 'Data'},
                {extend: 'print',
                    customize: function (win){
                        $(win.document.body).addClass('white-bg');
                        $(win.document.body).css('font-size', '10px');
                        $(win.document.body).find('table')
                                .addClass('compact')
                                .css('font-size', 'inherit');
                    }
                },
                {
                    enabled: true,
                    text: 'Nuevo <span></span>',
                    className: 'btn btn-primary waves-effect waves-light m-b-5',
                    titleAttr: 'Nueva Elemento',
                    action: function (e, dt, node, config) {
                        window.location.href = ctx.url.create;
                    }
                },
            ],
            serverSide: true,
            language: {
                url: '/Content/Json/language_dt.json'
            },
            sAjaxSource: ajaxUrl,
            columns: columns,
            fnServerData: serverData,
            columnDefs: columnsDefinition,
            fnDrawCallback: function( oSettings ) {
                onDrawCallback(oSettings);
            },
            fnRowCallback: function(row,data,index) {
                onDrawRowCallback(row,data,index);
            },
            initComplete: function(settings, json) { 
                onInitComplete(settings, json);
            },
        });
		
        table.on('select', this, function (e, dt, type, indexes) {
            let ctx = e.data;
            if (type === 'row') {
                ctx.vars.table[type](indexes).nodes().to$().addClass('table-info');
                ctx.vars.table[type](indexes).nodes().to$().siblings().removeClass('table-info').find('.btns-ctrl').css('visibility', 'hidden');
                ctx.vars.table[type](indexes).nodes().to$().find('.btns-ctrl').css('visibility', 'visible')
                    // do something with the ID of the selected items
            }
        });
		
        return table;
    };

    this.TableEvents = function(tableId,event,selector,callback) {
        $(tableId).on(event,selector, function(e) {
            e.preventDefault();
            callback($(this));
        });
    };

    this.Ajax = function(url, method, _data, async, onsuccess, oncomplete, onerror=null) {
        var oncomplete = oncomplete || function() {};
        $.ajax({
            url: url,
            type: method,
            dataType: 'json',
            content: "application/json; charset=utf-8",
            data: _data,
            context: this,
            async: true,
            success: function(data, textStatus, xhr) {
                onsuccess(data,this);
            },
            complete: function(xhr, textStatus) {
                this.UnblockUI;
                //#HANDLE SESSION#
                clearTimeout(sessionTimeout);
                startSessionTimer();
                //#HANDLE SESSION#
                oncomplete(xhr, textStatus ,this);
            },
            error: function(xhr, textStatus, errorThrown) {
                if(xhr.status == 440)
                    location.href = "/";
                this.UnblockUI;
                if(onerror != null)
                {
                    let error = onerror(xhr);
                    if ( error === undefined ) 
                        return;
                }
                this.Modal.Oops(errorThrown);
                console.log(errorThrown);
            }
        });
    };

    this.AjaxFile = function (url, _data, onsuccess, oncomplete, onerror = null) {
        var oncomplete = oncomplete || function () { };
        $.ajax({
            url: url,
            type: "POST",
            dataType: 'json',
            contentType: false,
            processData: false,
            data: _data,
            context: this,
            async: true,
            success: function (data, textStatus, xhr) {
                onsuccess(data, this);
            },
            complete: function (xhr, textStatus) {
                this.UnblockUI;
                //#HANDLE SESSION#
                clearTimeout(sessionTimeout);
                startSessionTimer();
                //#HANDLE SESSION#
                oncomplete(xhr, textStatus, this);
            },
            error: function (xhr, textStatus, errorThrown) {
                if (xhr.status == 440)
                    location.href = "/";
                this.UnblockUI;
                if (onerror != null) {
                    let error = onerror(xhr);
                    if (error === undefined)
                        return;
                }
                this.Modal.Oops(errorThrown);
                console.log(errorThrown);
            }
        });
    };

    this.Modal = {
        Oops : function (detail = '', title = 'Opss', message = 'Ha ocurrido un error inesperado') {
            if(detail!= ''){
                detail = '<span class="view-detail" data-toggle="collapse" data-target="#demo">ver detalle...</span>'+
              '<div id="demo" class="collapse detail-error">'+
                detail+
              '</div>'
            }
            swal({
                title: title,
                text: message+". "+detail,
                type: 'error',
                showCancelButton: false,
                confirmButtonColor: swalStyle.btn_danger,
                confirmButtonText: 'Continuar',
                cancelButtonText: 'Cancelar',
                closeOnConfirm: true,
                closeOnCancel: true
            },
            function(){
                return;
            });	
        },
        ButtonClick : function(buttonId,urlToRedirect,type_modal = 'warning',title = 'Desea cancelar la edición?'
                , message = 'No se guardarán los cambios hechos',showCancel = true
                ,confirmText = 'Si', cancelText = 'No', buttonColor = swalStyle.btn_warning) {
            $(buttonId).click(function(e) {
                e.preventDefault();
                swal({
                    title: title,
                    text: message,
                    type: type_modal,
                    showCancelButton: showCancel,
                    confirmButtonColor: buttonColor,//btn-primary
                    confirmButtonText: confirmText,
                    cancelButtonText: cancelText,
                    closeOnConfirm: true,
                    closeOnCancel: true
                },
                function(){
                    window.location.href = urlToRedirect; 
                });
            });
        },
        Generic: function(type,title,message,txtbtnConfirm,txtbtnCancel,onconfirmCallback
            ,buttonColor = swalStyle.btn_primary, showCancel = true) {
            var onconfirmCallback = onconfirmCallback || function() {};
            swal({
                title: title,
                text: message,
                type: type,
                showCancelButton: showCancel,
                confirmButtonColor: buttonColor,//btn-primary
                confirmButtonText: txtbtnConfirm,
                cancelButtonText: txtbtnCancel,
                closeOnConfirm: true,
                closeOnCancel: true
            },
            function(isConfirm) {
                if (isConfirm) {
                    onconfirmCallback(this); 
                } else {
                    
                }
            });
        },
        DeleteConfirm: function(okcallback,ctx,text = 'El registro se eliminará permanentemente',title='¿Desea Continuar?') {
            swal({
                title: title,
                text: text,
                type: 'error',
                showCancelButton: true,
                confirmButtonColor: swalStyle.btn_warning,//btn-primary
                confirmButtonText: 'Continuar',
                cancelButtonText: 'Cancelar',
                closeOnConfirm: true,
                closeOnCancel: true
            },
                function (e) {
                    try {
                        e ? okcallback(ctx) : ctx.delete_id = null;
                    } catch (ex) { }
            });
        }

    };
        
    this.Toast = function(type,position,title,text, options={timeOut: 6000, hideDuration: 1000}){
        $.Notification.autoHideNotify(type, position, title, text, options)
    };

    this.ToastConfirm = function(type, position,title,text,callbackYes = function() {
        //show button text
        alert($(this).text() + " clicked!");
        //hide notification
        $(this).trigger('notify-hide');
    },callbackNo = function() {
        //programmatically trigger propogating hide event
        $(this).trigger('notify-hide');
    },yes_text="Yes", no_text="No" ){
        $.Notification.confirm(type,position, title,text,yes_text,no_text,callbackNo,callbackYes);
    };
    
    this.FormValidate = function($form, validCallback = ()=>{}, invalidCallback = ()=>{}){
        $form.validator().on('submit',this,function(e){
            if(e.isDefaultPrevented()){
                invalidCallback(e)
            }
            else{
                validCallback(e);
            }
        })
    };

    this.BlockTarget = function ($target) {
        $target.block({
            overlayCSS: {
                backgroundColor: '#fff',
                opacity: 1.0,
                cursor: 'wait',
            },
            message: $('#EgulfLoader'),
            css: {
                border: 'none',
                padding: '20px',
                backgroundColor: '#fff',
                '-webkit-border-radius': '10px',
                '-moz-border-radius': '10px',
                opacity: 0.1,
                color: '#fff'
            },
        });
    };

    this.BlockUI = function (target) {
        if (target === undefined) {
            $.blockUI({
                message: $('#EgulfLoader'),
                overlayCSS: {
                    backgroundColor: '#fff',
                    opacity: 0.5,
                    cursor: 'wait',
                },
                css: {
                    border: 'none',
                    padding: '20px',
                    backgroundColor: '#fff',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .8,
                    color: '#fff'
                },
            });
        }
        else {
            $(target).block({
                message: $('#EgulfLoader'),
                overlayCSS: {
                    backgroundColor: '#fff',
                    opacity: 0.5,
                    cursor: 'wait',
                },
                css: {
                    border: 'none',
                    padding: '20px',
                    backgroundColor: '#fff',
                    '-webkit-border-radius': '10px',
                    '-moz-border-radius': '10px',
                    opacity: .8,
                    color: '#fff'
                },
            }); 
        }
    };

    this.UnblockUI = function (target) {
        if (target === undefined)
            $.unblockUI();
        else 
            $(target).unblock(); 
    };

    this.ClearForm = function ($form) {
        $form.find('input').each(function (index, element) {
            $(element).val("");
        });
    };
           
    this.UIToData = function UIToData(form) {
        var obj = new Object();
        $.each(form.serializeArray(), function (i, field) {
            try {
                try {
                    obj[field.name] = field.value.trim();
                } catch (ex) {
                    obj[field.name] = field.value;
                }

            } catch (ex) {
                console.log(ex);
            }
        })
        return obj;
    },

    this.DataToUI = function DataToUI(form, data) {
        $.each(form.serializeArray(), function (i, field) {
            try {
                let type = $("#" + form[0].id + " [name=" + field.name + "]").attr("type");
                if (type == "date")
                    $("#" + form[0].id + " [name=" + field.name + "]").val(moment(data[field.name]).format("YYYY-MM-DD"));
                else
                    $("#" + form[0].id + " [name=" + field.name + "]").val(data[field.name]);

            } catch (ex) {
                console.log(ex);
            }
        })
    },

    this.EpocToStringDate = function(EpocData)
    {
        //Example input:/Date(1224043200000)/ output:2019-03-13
        var jsDate = new Date(parseInt(EpocData.substr(6)));
        var momentConverted = moment(jsDate);
        return StringDate = momentConverted.format("YYYY-MM-DD");
    }
};