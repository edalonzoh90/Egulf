$(document).ready(function () {
    Base.call(Company);
});

var Company = {
    resources: {
        txt_mje_cancel_save: "¿Desea cancelar el registro?",
        txt_mje_empty: "No tienes empresa asignada",
        txt_aviso: "Aviso",
        txt_correcto: "Correcto",
        txt_company_saved: "Compañía registrada con éxito",
        txt_company_error_saved: "Error al registrar la compañía",
        txt_company_error_form: null,
        txt_company_updated: null,
        txt_company_error_updated: 'Error al editar la compañía',
        txt_succes_del: null,

        txt_success_invite: "Se enviaron las invitaciones exitosamente",
        txt_error_invite: "Se produjo un error al enviar las invitaciones",
        txt_error_confirm: "No se pudo unir a la empresa",
        txt_remove_invitation: "Se removera a este usuario de la empresas",
        txt_remove_invitation_success: null,
    },
    vars: {
        $formRegister: $('#companyRegister'),
        $formEdit: $('#companyEdit'),
        $txtInvite: $("#txtInvite"),
        $modalInvite: $("inviteModal"),
        url: {
            index: "/company/index",
            create: "/company/create",
            edit: "/company/edit",
            delete: "/company/delete",
            invite: "/company/invitate",
            decline: "/company/decline",
            confirmInvite: "/company/confirm",
            removeInvite: "/company/deleteInvivtation",
            logoff: "/login/logout"
        },
        table: null,
        messageUISweet: null
    },

    Init: function () {
        this.permissions.canCreate = false;
        this.permissions.canCancelInvite = true;
        this.Modal.ButtonClick("#btnCancelSave", this.vars.url.index, 'warning', this.resources.txt_mje_cancel_save);
        this.Modal.ButtonClick("#btnCancelEdit", this.vars.url.index, 'warning');
        this.OnSaveCompany();
        this.OnEditCompany();
        $('#btnDeleteCompany').on('click', this, this.OnDeleteConfirm);
        $('#btn_invite').on('click', this, this.onInvite);
        //$('#btnOpenInvitation').on('click', this, this.LoadInvitationTable);
        this.LoadInvitationTable();
    },

    EmptyCompany: function () {
        this.Toast("warning", "top rigth", this.resources.txt_aviso, this.resources.txt_mje_empty);
    },

    OnSaveCompany: function () {
        this.FormValidate(this.vars.$formRegister, e => {
            let ctx = e.data
            ctx.BlockUI();
            e.preventDefault();
            let data = ctx.vars.$formRegister.serialize();
            ctx.Ajax(ctx.vars.url.create, 'POST', data, true, function (data, ctx) {
                if (data != null && data.Status == 0) {
                    ctx.Toast("success", "top rigth", ctx.resources.txt_correcto, data.Message ? data.Message : ctx.resources.txt_company_saved);
                    ctx.ClearForm(ctx.vars.$formRegister);

                    var message = Company.vars.messageUISweet[data.Status];
                    Company.Modal.Generic(message.type,
                                           message.subject,
                                           message.message,
                                           message.buttonText,
                                           "",
                                           function () {
                                               location.href = Company.vars.url.logoff;
                                           },
                                           Company.GetSwalStyle.btn_success,
                                           false);
                    //window.location.href = ctx.vars.url.index;
                }

                else
                    ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.txt_company_error_saved);
            }, function (xhr, textStatus, ctx) {

            });
        }, invalidCallback = e => {
            e.data.Toast('warning', "top rigth", this.resources.txt_aviso, this.resources.txt_company_error_form)
        })
    },

    OnEditCompany: function () {
        this.FormValidate(this.vars.$formEdit, e => {
            let ctx = e.data
            ctx.BlockUI();
            e.preventDefault();
            let data = ctx.vars.$formEdit.serialize();
            ctx.Ajax(ctx.vars.url.edit, 'POST', data, true, function (data, ctx) {
                if (data != null && data.Status == 0) {
                    ctx.Toast("success", "top rigth", ctx.resources.txt_correcto, data.Message ? data.Message : ctx.resources.txt_company_updated);
                    ctx.ClearForm(ctx.vars.$formEdit);
                    window.location.href = ctx.vars.url.index;
                }

                else
                    ctx.Toast("Warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.txt_company_error_updated);
            }, function (xhr, textStatus, ctx) {

            });
        }, invalidCallback = e => {
            e.data.Toast('error', "top rigth", ctx.resources.txt_aviso, ctx.resources.txt_company_error_form)
        })
    },

    OnDeleteConfirm: function (ctx) {
        ctx.data.delete_id = $(this).data('id');
        ctx.data.Modal.DeleteConfirm(ctx => {
            ctx.BlockUI();
            ctx.Ajax(ctx.vars.url.delete + "/" + ctx.delete_id, 'POST', ctx.delete_id, true,
		        function (data, ctx) {
		            if (data.Status == 0) {
		                ctx.Toast("success", "top rigth", ctx.resources.txt_correcto, ctx.resources.txt_succes_del);//Debe ir realmente en el succuess del ajax

		                var message = Company.vars.messageUISweet[data.Status];
		                Company.Modal.Generic(message.type,
		                                       message.subject,
		                                       message.message,
		                                       message.buttonText,
		                                       "",
		                                       function () {
		                                           location.href = Company.vars.url.logoff;
		                                       },
		                                       Company.GetSwalStyle.btn_success,
		                                       false);

		                //window.location.href = ctx.vars.url.index;
		            } else
		                ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.txt_error_invite);
		        });


        }, ctx.data);
    },

    onInvite: function (e) {
        let ctx = e.data;
        data = ctx.vars.$txtInvite.val();
        ctx.Ajax(ctx.vars.url.invite, 'POST', { mails: data }, true, function (data, ctx) {
            if (data != null && data.Status == 0) {
                ctx.Toast("success", "top rigth", ctx.resources.txt_correcto, data.Message ? data.Message : ctx.resources.txt_success_invite);
                ctx.vars.$txtInvite.val("");
                $("#txtInvite").tagsinput('removeAll');
                ctx.vars.table.ajax.reload(ctx.tableCallbacks.onInitComplete);
                //ctx.vars.$modalInvite.modal('hide');
            }

            else {
                ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.txt_error_invite);
                ctx.vars.table.ajax.reload(ctx.tableCallbacks.onInitComplete);
            }
                
        }, function (xhr, textStatus, ctx) {

        });
    },

    LoadInvitationTable: function () {
        let ctx = this;
        if (ctx.vars.table != null)
            return;
        ctx.vars.table = ctx.CreateTable('#dtInvitations',
            '/Company/GetInvitation',
            ctx.tableInvitationContent.columns,
            ctx.tableInvitationContent.columnsDef,
            ctx.tableInvitationContent.fnServerData,
            ctx.tableCallbacks.onDraw, ctx.tableCallbacks.onDrawRow, ctx.tableCallbacks.onInitComplete).order([[3, 'desc']]);
    },

    tableInvitationContent: {
        columns: [
            { data: 'CompanyInvitationId', orderName: 'CompanyInvitationId', searchable: false, visible:false },
            { data: 'Email', orderName: 'Email' },
            { data: 'FullName', orderName: 'Name' },
            { data: 'Date', orderName: 'Date' },
            { data: 'Status', orderName: 'Status' },

        ],
        columnsDef: [
            { "orderSequence": ["desc"], "targets": [0] },
            {
                targets: 3,
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
                        status = data == 1 ? 'Aceptado' : data == 0 ? 'Pendiente' : 'Rechazado';
                        style = data == 1 ? 'badge-success' : data == 0 ? 'badge-warning' : 'badge-danger';
                    }
                    let _d = '<div><span class="badge ' + style + '">' + status + '</span>';

                    let ctx = Company;
                    let data_dom = '<span>' + _d + '<div class="btns-ctrl" style="display: inline-block; float: right; visibility: hidden; z-index: 9999">';
                    if (ctx.permissions.canCancelInvite)
                        data_dom += '<div title="Desinvitar" href="#" class="badge badge-danger ml-1 item-delete" data-id="' + row.CompanyInvitationId + '" data-email="' + row.Email + '"><i class="fa fa-window-close-o"></i></div>';
                    data_dom += '</div></span></div>';
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
        onDraw: function () {//Cuando se pinta la tabla
            //this.vars.idsToDelete.length = 0;   
        },
        onDrawRow: function (row, data, index) {//Cuando se pinta cada fila
            $('.item-delete').on('click', ctx, ctx.OnDeleteRowConfirm);
        },
        onInitComplete: function () {
            $('.item-delete').on('click', ctx, ctx.OnDeleteRowConfirm);

            var lengthBtn = ctx.vars.table.buttons().length
            var lengthCol = ctx.vars.table.columns()[0].length
            if (!ctx.permissions.canCreate && lengthBtn > 0)
                ctx.vars.table.buttons(lengthBtn - 1).remove();
            if (lengthBtn > 0)
                for (var x = 3; x >= 0; x--)
                    ctx.vars.table.buttons(x).remove();    //Borra los botones de descarga


            ctx.vars.table.buttons().enable()
        }

    },

    OnConfirmInvitation: function (data) {
        let ctx = this;
        ctx.Ajax(ctx.vars.url.confirmInvite, 'POST', { companyId: data }, true, function (data, ctx) {
            if (data != null && data.Status == 0) {

                var message = Company.vars.messageUISweet[data.Status];
                Company.Modal.Generic(message.type,
                    message.subject,
                    message.message,
                    message.buttonText,
                    "",
                    function () {
                        location.href = Company.vars.url.logoff;
                    },
                    Company.GetSwalStyle.btn_success,
                    false);

                //window.location.href = ctx.vars.url.index;
            }

            else
                ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.txt_error_invite);
        }, function (xhr, textStatus, ctx) {

        });
    },

    OnDeclineInvitation: function (data) {
        let ctx = this;
        ctx.Ajax(ctx.vars.url.decline, 'POST', { companyId: data }, true, function (data, ctx) {
            if (data != null && data.Status != 0)
                ctx.Toast("warning", "top rigth", ctx.resources.txt_aviso, data.Message ? data.Message : ctx.resources.txt_error_invite);
        }, function (xhr, textStatus, ctx) {

        });
    },

    OnDeleteRowConfirm: function (ctx) {
        ctx.data.delete_id = { id: $(this).data('id'), email: $(this).data('email') };
        ctx.data.Modal.DeleteConfirm(ctx => {
            ctx.BlockUI();
            ctx.Ajax(ctx.vars.url.removeInvite + "/" + ctx.delete_id, 'POST', ctx.delete_id, true,
		        function (data, ctx) {
		            if (data.Status == 0)
		                ctx.Toast("success", "top rigth", ctx.resources.txt_correcto, ctx.resources.txt_succes_del);//Debe ir realmente en el succuess del ajax
                    ctx.vars.table.ajax.reload(ctx.tableCallbacks.onInitComplete);
                    ctx.UnblockUI();
		            //window.location.href = ctx.vars.url.index;
		        });


        }, ctx.data, ctx.data.resources.txt_remove_invitation);
    },


}