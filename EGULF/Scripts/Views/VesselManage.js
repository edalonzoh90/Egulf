var VesselManage = {    

    vars: {
        valid: true,
        $formRegister: $('#VesselManage'),
        url: {
            manage: "/vessel/manage",
            edit: "/vessel/availabilityedit",
            transaction: "/vessel/availabilitytransaction/",
            list: "/vessel/availabilitylist",
        },
        tags: {
            OK: "Correcto",
            Saved: "Registrado correctamente",
            SomethingWrong: "Ha ocurrido un error",
            Warning: "Aviso",
            ErrorForm: "Verifica la información",
        },
    },

    Init: function () {
        VesselManage.List();
        $("#btnMngSave").click(function () { VesselManage.Transaction('ADD'); });
        $("#btnStatusSave").click(function () { VesselManage.Transaction('STATUS'); });
    },

    List: function(){

        let objForm = { VesselId: VesselManage.vars.model.VesselId }

        this.Ajax(this.vars.url.list, 'GET', objForm, true, function (data, ctx) {
            var templateYear = Handlebars.compile($("#tmpyear").html());
            var templateEvent = Handlebars.compile($("#tmpevent").html());
            
            var year = 0;
            var alt = "";
            for (var x = 0; x < data.length; x++) {                        
                let item = data[x];
                let eventid = "event"+item.AvailabilityVesselId;
                let nyear = moment(item.StartDate).format("YYYY");

                if(year != nyear){
                    year = nyear;
                    $(".timeline").append(templateYear({year: year}));
                }

                $(".timeline").append(templateEvent({
                    id: item.AvailabilityVesselId,
                    alt: alt,
                    eventid: eventid,
                    event: item.Reason,
                    day: moment(item.StartDate).format("DD-MMM-YYYY") + " / " + moment(item.EndDate).format("DD-MMM-YYYY"),
                    reason: item.ReasonDescription,

                    AvailabilityVesselId: item.AvailabilityVesselId,
                    VesselId: item.VesselId,
                    ReasonId: item.ReasonId,
                    ReasonDescription: item.ReasonDescription,
                    StartDate: item.StartDate,
                    EndDate: item.EndDate,
                }));
                
                alt = alt == "" ? "alt" : "";
                
                $("#"+eventid)
                .mouseout(function () { $("#"+eventid + " .album").hide(); })
                .mouseover(function () { $("#"+eventid + " .album").show(); });
            }
        });
    },

    ValidateTransaction: function(oper, id){
        if(oper == "DEL"){
            VesselManage.Modal.DeleteConfirm(function(val){VesselManage.Transaction(oper, id);});
        }
        else if(oper == "EDIT"){
            var data = VesselManage.UIToData($('#formevent'+id));
            VesselManage.DataToUI($("#VesselManage"), data);
            $("#btnAdd").click();
        }
    },

    Transaction: function(oper, id){
        let objForm = {};
        if(oper == "ADD"){
            this.vars.$formRegister.validator('validate');
            if (!this.vars.$formRegister[0].checkValidity())
                return;
            objForm = this.UIToData(this.vars.$formRegister);
        }
        else if(oper == "DEL"){
            objForm = {"AvailabilityVesselId": id}
        }
        else if(oper == "STATUS"){
            objForm = {
                "VesselId": VesselManage.vars.model.VesselId,
                "VesselEstatusId": $("#VesselEstatusId").val(),
            }
        }

        //***** AJAX *****
        this.Ajax(this.vars.url.transaction + oper, 'POST', objForm, true, function (data, ctx) {
            if (data != null && data.Status == 0) {
                ctx.Toast("success", "top rigth", ctx.vars.tags.OK, data.Message ? data.Message : ctx.vars.tags.Saved);
                window.location.reload();
            }
            else
                ctx.Toast("warning", "top rigth", ctx.vars.tags.Warning, data.Message ? data.Message : ctx.vars.tags.SomethingWrong);
        });
    }
}