var Vessel = {    
    
    vars: {
        valid: true,
        $formRegister: $('#CreateVessel'),
        $formRegister2: $('#CreateVesselSpecificInfo'),
        $formRegister3: $('#CreateVesselExtraInfo'),
        $formRegister4: $('#CreateVesselCost'),
        url: {
            index: "/vessel/index",
            create: "/vessel/create",
            edit: "/vessel/edit",
            manage: "/vessel/manage",
            add: "/vessel/transaction/add",
            list: "/vessel/list",
        },
        tags: {
            OK: "Correcto",
            Saved: "Registrado correctamente",
            SomethingWrong: "Ha ocurrido un error",
            Warning: "Aviso",
            ErrorForm: "Verifica la información",
        },
        status: [null, "No disponible", "Disponible", "Fuera de servicio"],
        statusColor: [null, "badge-info", "badge-success", "badge-danger"]
    },

    ChangeWizard: function (oper) {
        var step = parseInt($("#wizard-vertical-ul").find(".current")[0].id.substring(19));
        var nstep = oper == "PREV" ? (step - 1)
            : oper == "NEXT" ? (step + 1)
                : oper;

        if (step == 0 && nstep > step) {
            this.vars.$formRegister.validator('validate');
            if (!this.vars.$formRegister[0].checkValidity())
                return;
        }
        else if (step == 1 && nstep > step) {
            this.vars.$formRegister2.validator('validate');
            if (!this.vars.$formRegister2[0].checkValidity())
                return;
        }
        else if (step == 2 && nstep > step) {
            this.vars.$formRegister3.validator('validate');
            if (!this.vars.$formRegister3[0].checkValidity())
                return;
        }

        if (oper == "END") {
            if (!this.vars.$formRegister4[0].checkValidity())
                return;
            this.Transaction();
        }
        else if ($("#wizard-vertical-li-" + step).hasClass("first") && oper == "PREV")
            return;
        else if ($("#wizard-vertical-li-" + step).hasClass("last") && oper == "NEXT")
            return;

        $("#wizard-vertical-li-" + step)
            .addClass("disabled")
            .removeClass("current")
            .attr("aria-selected", "false")
            .attr("aria-disabled", "true");
        $("#wizard-vertical-p-" + step)
            .removeClass("current")
            .css("display", "none");
        
        $("#wizard-vertical-li-" + nstep)
            .removeClass("disabled")
            .addClass("current")
            .attr("aria-selected", "true")
            .attr("aria-disabled", "false");
        $("#wizard-vertical-p-" + nstep)
            .addClass("current")
            .css("display", "block");

        $(".btn-pagination").removeClass("disabled");
        $("#btnNext").css("display", "block");
        $("#btnFinish").css("display", "none");
        if ($("#wizard-vertical-li-" + nstep).hasClass("first")) {
            $("#btnPrev").addClass("disabled");
        }
        else if ($("#wizard-vertical-li-" + nstep).hasClass("last")) {
            $("#btnNext").css("display", "none");
            $("#btnFinish").css("display", "block");
        }
    },

    LoadTable: function () {
        this.vars.table = this.CreateTable('#tblVessels',
            this.vars.url.list,
            this.tableContent.columns,
            this.tableContent.columnsDef,
            this.tableContent.fnServerData,
            this.tableCallbacks.onDraw, this.tableCallbacks.onDrawRow,
            () => {
                $('.item-delete').on('click', this, this.OnDeleteRowConfirm);

                var lengthBtn = this.vars.table.buttons().length
                var lengthCol = this.vars.table.columns()[0].length

                for (var x = 3; x >= 0; x--) 
                    this.vars.table.buttons(x).remove();    //Borra los botones de descarga
                
                if (!this.permissions.canCreate)
                    this.vars.table.buttons(lengthBtn - 1).remove();

                this.vars.table.buttons().enable()
                $(".input-filter").remove();
                $('.grid').unblock(); 
            });
    },

    tableContent: {
        columns: [
            { data: 'Status', orderName: 'Status' },
            { data: 'Barco', orderName: 'Barco' },
            { data: 'VesselType.Name', orderName: 'VesselType' },
            { data: null }
        ],
        columnsDef: [
            {
                targets: 0,
                render: function (data, type, row, meta) {
                    return '<div style="text-align: center; margin: auto"><span class="badge ' + Vessel.vars.statusColor[data] + '">' + Vessel.vars.status[data] + '</span></div>';
                }
            },
            {
                targets: 1,
                render: function (data, type, row, meta) {
                    let data_dom = '<span>' + row.Imo + ' - ' + row.Name + '</span>';
                    return data_dom;
                }
            },
            {
                targets: 3,
                render: function (data, type, row, meta) {
                    let ctx = Vessel;
                    let urlEdit = Vessel.vars.url.edit + "/" + row.VesselId;
                    let urlManage = Vessel.vars.url.manage + "/" + row.VesselId;
                    let data_dom = '<div class="btns-ctrl text-center">';
                    if (ctx.permissions.canEdit)
                        data_dom += '<a title="Editar" href="' + urlEdit + '" class="badge badge-warning ml-1"><i class="fa fa-edit"></i></a>'

                    data_dom += '<a title="Administrar" href="' + urlManage + '" class="badge badge-warning ml-1"><i class="fa fa-gear"></i></a>'
                    data_dom += '</div>';
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
        onDraw: function () { },
        onDrawRow: function (row, data, index) { }
    },

    InitCreate: function () {

        //***** Tab1 *****
        $('.select2-multiple').select2({ width: '100%', theme: "bootstrap" });

        $("#Suitability").change(function () {
            var vals = $('#Suitability').val();

            $(".cDinamyc").hide();
            $("#NumberPassenger").removeAttr("required");
            $("#SubtypeId").removeAttr("required");
            $("#BHP").attr("required");
            $("#CargoCapacity").removeAttr("required");

            var isEmpty = true;
            if (vals.indexOf("1") > -1 || vals.indexOf("2") > -1) {
                $(".cTowage").show();
                isEmpty = false;

                $("#SubtypeId").attr("required", "");
                $("#BHP").attr("required", "");
            }
            if (vals.indexOf("3") > -1) {
                $(".cPersonnelTransportation").show();
                isEmpty = false;

                $("#NumberPassenger").attr("required", "");
            }
            if (vals.indexOf("5") > -1) {
                $(".cPersonnelTransportation").show();
                $(".cFlotel").show();
                isEmpty = false;

                $("#NumberPassenger").attr("required", "");
            }
            if (vals.indexOf("4") > -1) {
                $(".cMaterialTransportation").show();
                isEmpty = false;

                $("#CargoCapacity").attr("required", "");
            }
            if (isEmpty) 
                $(".cEmpty").show();

            Vessel.vars.$formRegister3.validator('update');
        });

        $("#fileImage").change(function (e) {
            var File = e.target.files[0];
            if (File.type.match('image/jpeg') || File.type.match('image/jpg') || File.type.match('image/png')) {
                var reader = new FileReader();
                reader.onload = function (ev) {
                    $("#image").attr({ "src": ev.target.result });
                };
                reader.readAsDataURL(File);
            }
            else {
                Vessel.Toast("warning", "top right", Vessel.vars.tags.Warning, Vessel.vars.tags.BadExtension);
            }  
        });

        //***** Tab2 *****
        

        //***** Tab3 *****
        $("#SubtypeId").change(function () {
            $(".cBollardPull").hide();
            $(".cBollardPullAhead").hide();
            $(".cBollardPullAstern").hide();

            $("#BollardPull").removeAttr("required");
            $("#BollardPullAhead").removeAttr("required");
            $("#BollardPullAstern").removeAttr("required");

            if (this.value == 1 || this.value == 3 || this.value == 4) {
                $(".cBollardPull").show();

                $("#BollardPull").attr("required", "");
            }
            else if (this.value == 2) {
                $(".cBollardPullAhead").show();
                $(".cBollardPullAstern").show();

                $("#BollardPullAhead").attr("required", "");
                $("#BollardPullAstern").attr("required", "");
            }

            Vessel.vars.$formRegister3.validator('update');

        });
    },

    InitEdit: function () {
        //***** Tab1 *****
        var lst = [];
        this.vars.model.Suitability.forEach(function (entry) {
            lst.push(entry.ProjectTypeId);
        });
        $("#ClassValidity").val(moment(this.vars.model.ClassValidity).format("YYYY-MM-DD"));
        $("#Suitability").val(lst).change();

        //***** Tab2 *****

        //***** Tab3 *****
        $("#SubtypeId").change();
        if (this.vars.extraInfo.PemexCheck)
            $("#PemexCheck").attr("checked", "")
        if (this.vars.extraInfo.AirCondition)
            $("#AirCondition").attr("checked", "")
        if (this.vars.extraInfo.MessRoom)
            $("#MessRoom").attr("checked", "")
        if (this.vars.extraInfo.ControlRoom)
            $("#ControlRoom").attr("checked", "")
        if (this.vars.extraInfo.ConferenceRoom)
            $("#ConferenceRoom").attr("checked", "")
        if (this.vars.extraInfo.Gymnasium)
            $("#Gymnasium").attr("checked", "")
        if (this.vars.extraInfo.SwimingPool)
            $("#SwimingPool").attr("checked", "")
        if (this.vars.extraInfo.Office)
            $("#Office").attr("checked", "")
        if (this.vars.extraInfo.Hospital)
            $("#Hospital").attr("checked", "")
    },

    Transaction: function () {

        this.BlockUI();

        //**** RULES *****
        var vals = $('#Suitability').val();
        if (vals.indexOf("1") == -1 && vals.indexOf("2") == -1) {   //Si NO es de tipo Towage, limpiamos sus campos
            $(".cTowage :input").each(function (index, element) {
                $(element).val("");
            });
        }
        if (vals.indexOf("3") == -1 && vals.indexOf("5") == -1) {
            $(".cPersonnelTransportation :input").each(function (index, element) {  //Si NO es de tipo PersonnelTransportation, limpiamos sus campos
                if ($(element).is(':checkbox'))
                    $(element).removeAttr("checked");
                else
                    $(element).val("");
            });
        }
        if (vals.indexOf("5") == -1) {
            $(".cFlotel :input").each(function (index, element) {  //Si NO es de tipo Flotel, limpiamos sus campos
                $(element).val("");
            });
        }
        if (vals.indexOf("4") == -1) {
            $(".cMaterialTransportation :input").each(function (index, element) {  //Si MaterialTransportation es de tipo Flotel, limpiamos sus campos
                $(element).val("");
            });
        }

        vals = $('#SubtypeId').val();
        if (vals != 1 && vals != 3 && vals != 4) {
            $("#BollardPull").val("");
        }
        if (vals != 2) {
            $("#BollardPullAhead").val("");
            $("#BollardPullAstern").val("");
        }

        //***** Tab1 *****
        let data = this.UIToData(this.vars.$formRegister);

        var lst = $('#Suitability').val();
        data.SuitabilityIds = $('#Suitability').val().toString();
        data.Suitability = [];
        lst.forEach(function (entry) {
            data.Suitability.push({ ProjectTypeId: parseInt(entry, 10) });
        });

        var objForm = new FormData();
        var fileImage = $("#fileImage").get(0);
        if (fileImage.files.length > 0)
            objForm.append(fileImage.files[0].name, fileImage.files[0]);

        for (var prop in data) {
            if (prop.toString() != "Suitability")
                objForm.append("Vessel."+prop.toString(), data[prop]);
        }

        //***** Tab2 *****
        let data2 = this.UIToData(this.vars.$formRegister2);
        for (var prop in data2) {
            objForm.append("VesselSpecificInfo."+prop.toString(), data2[prop]);
        }

        //***** Tab3 *****
        let data3 = this.UIToData(this.vars.$formRegister3);
        data3.PemexCheck = $("#PemexCheck").is(":checked");
        for (var prop in data3) {
            objForm.append("SpecificInfo." + prop.toString(), data3[prop]);
        }

        //***** Tab4 *****
        let data4 = this.UIToData(this.vars.$formRegister4);

        for (var prop in data4) {
            objForm.append("VesselCost." + prop.toString(), data4[prop]);
        }

        //***** AJAX *****
        this.AjaxFile(this.vars.url.add, objForm, function (data, ctx) {
            if (data != null && data.Status == 0) {
                ctx.Toast("success", "top rigth", ctx.vars.tags.OK, data.Message ? data.Message : ctx.vars.tags.Saved);
                location.href = ctx.vars.url.index;
            }
            else
                ctx.Toast("warning", "top rigth", ctx.vars.tags.Warning, data.Message ? data.Message : ctx.vars.tags.SomethingWrong);
        });
    }

}