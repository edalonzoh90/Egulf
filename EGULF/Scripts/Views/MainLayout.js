var sessionTimeout = null;
var chat;

//#HANDLE SESSION#
function startSessionTimer() {
    sessionTimeout = window.setTimeout(function () {
        $.ajax({
            url: "/login/sessiontimeout", type: "POST", dataType: "JSON", contentType: "application/json",
            success: function (resp) {
                if (resp == true) {
                    window.location.href = '/';
                }
                else {
                    clearTimeout(sessionTimeout);
                    startSessionTimer();
                }
            },
        });
    }, timeout);
}

var Main = {

    Init: function () {
       
         
        //#SIGNALR#
        if (parseInt($("#alertTotal").text(), 10) == 0)
            $("#alertTotal").hide();

        chat = $.connection.alertHub;

        chat.client.broadcastMessage = function (message) {
            Main.Toast("success", "top rigth", "SignalR", message, { timeOut: 3000, hideDuration: 0 });
        };

        chat.client.newAlert = function (alert) {
            console.log(alert);
            if (alert.AlertTemplateId  === 8) {
                let obj = JSON.parse(alert.Extra);
                try { MapEgulf.RefreshOffer(obj.Id); } catch (ex) { }
            }
            else if (alert.AlertTemplateId === 9) {
                let obj = JSON.parse(alert.Extra);
                try { MapEgulf.RefreshChat(obj.Id); } catch (ex) { }
            }
            else if (alert.AlertTemplateId === 10) {
                let obj = JSON.parse(alert.Extra);
                try { MapEgulf.RefreshAvailablyOffers(obj.Id); } catch (ex) { }
                try { Match.LoadOfferts(); } catch (ex) { }

            }
            else if (alert.AlertTemplateId === 11) {
                let obj = JSON.parse(alert.Extra);
                try { Match.LoadOfferts(); } catch (ex) { }
            }
            else {
                //No es un error, algunas alertas cumplen con doble función, alertar y disparar una acción
                if (alert.AlertTemplateId === 4) {
                    try { Match.LoadOfferts(); } catch (ex) { }
                }

                var alertTotal = parseInt($("#alertTotal").text(), 10) + 1;
                $("#alertTotal").html(alertTotal.toString()).show();

                var newAlert = `<a href='javascript:Main.MarkAsReaded(${alert.AlertId});' id='alert_${alert.AlertId}' class='dropdown-item notify-item alert-item'>`
                    + `<div class='notify-icon bg-info'><i class='mdi ${alert.Icon}'></i></div>`
                    + `<p class='notify-details' style='white-space: normal'><small>${alert.Body}</small><small class='text-muted'>${alert.TimeAgo}</small></p></a>`;
                $("#alerts").prepend(newAlert);
                if (alertTotal > 8)
                    $("#alerts .alert-item").last().remove();

                $("#alert").effect("shake");
            }
            //if (message.tags == "XXXX") {
            //    //Do something
            //}
        };

        chat.client.newAction = function (alert) {

            
        };

        chat.client.newMessage = function (message) {
            try {
                if ($('#btnChatOffer[data-OfferId="' + message.ReferenceId + '"]').length > 0) {
                    $('#btnChatOffer[data-OfferId="' + message.ReferenceId + '"]').parent().addClass("btn-warning");
                }
                Chat.AddMsg(message);
            } catch (ex) {
                
                //$("#" + message.ReferenceId).show();
                //No había instancia de chat, la pantalla estaba cerrada 
            }
        };

        $.connection.hub.start().done(function () {
            console.log("SignalR OK");
        }).fail(function (reason) {
            console.log("SignalR Error - " + reason);
        });
    },

    MarkAsReaded: function (AlertId) {
        Main.Ajax(urlBase + "Alert/MarkAsReaded", 'POST', { AlertId: AlertId }, true, function (data, ctx) {
            $("#alert_" + AlertId).remove();
            if (data.AlertId != null) {
                var alert = data;
                var newAlert = `<a href='javascript:Main.MarkAsReaded(${alert.AlertId});' id='alert_${alert.AlertId}' class='dropdown-item notify-item alert-item'>`
                    + `<div class='notify-icon bg-info'><i class='mdi ${alert.Icon}'></i></div>`
                    + `<p class='notify-details'>${alert.Body}<small class='text-muted'>${alert.TimeAgo}</small></p></a>`;

                $("#alerts .alert-item").last().after(newAlert);

                //$("#alerts").find(".notify-all").parent().prepend(newAlert);
            }

            var alertTotal = parseInt($("#alertTotal").text(), 10) - 1;
            $("#alertTotal").html(alertTotal.toString());
            if (alertTotal == 0)
                $("#alertTotal").hide();
        });
    },

    MonitorHelp() {
        $("#btnHelp.Help").on('click', function () {
        $("#HelpForm").validator('validate');
        var isValid = $("#HelpForm")[0].checkValidity();
        if (isValid == true) {
            Main.SendHelpRequest();
        }
        else
            return false;
        });
    },

    SendHelpRequest() {
        Main.BlockUI();
        var objParameters = new Object();
        objParameters.Message = Crypto.Encrypt($("#helpMessage.Help").val());
        $.ajax({
            url: urlSupport + "HelpSession/",
            type: "POST",
            dataType: "JSON",
            contentType: "application/json",
            data: JSON.stringify(objParameters),
            beforeSend: function () { },
            complete: function () { Main.UnblockUI(); },
            success: function (result) {
                if (result.Data.Status == 0) {
                    Main.Toast("success", "top right", "Mensaje Enviado", result.Data.Message,10000);
                    $("#helpMessage.Help").val("");
                    $('#help-modal').modal('toggle');
                }
                else {
                    Main.Toast('warning', "top right", "Lo sentimos", result.Data.Message);
                }
            }
        });   
    }


}
//#READY#
$(document).ready(function () {
    Base.call(Main);
    Main.Init();
    startSessionTimer();
    Main.MonitorHelp();
});