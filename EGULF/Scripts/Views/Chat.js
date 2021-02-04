var Chat = {

    vars: {
        valid: true,
        url: {
            transaction: "/chat/transaction",
        },
        tags: {
            OK: "Correcto",
            Saved: "Registrado correctamente",
            SomethingWrong: "Ha ocurrido un error",
            Warning: "Aviso",
            ErrorForm: "Verifica la información",
        },
    },

    AddMsg: function (msg) {

        if (msg.ReferenceId !== ReferenceId)
            return;
        var id = "#chat" + msg.MessageId;
        var odd = SessionPersonId === msg.From ? "odd" : "";
        var alias = SessionPersonId === msg.From ? "" : msg.Alias === "PROJECT_OWNER" ? "Admin Proyecto" : "Admin Barco"; 
        var img = msg.Alias === "VESSEL_OWNER" ? "/Content/Images/Barco-md.png" : msg.Alias === "PROJECT_OWNER" ? "/Content/Images/box.png" : "/Content/Images/avatar.png"; 

        $("#msg0").clone().attr("id", id.substring(1)).removeAttr("style").addClass(odd).appendTo("#lst-msg");
        $(id + " .name").text(alias);
        $(id + " .created-at").text(moment(msg.CreatedAt).format("HH:mm"));
        $(id + " .message").text(msg.Message);
        $(id).find("img").attr("src", img);

        var objDiv = document.getElementById("lst-msg");
        objDiv.scrollTop = objDiv.scrollHeight;
    },

    InitCreate: function () {
        $("#btnSend").click(function () {
            Chat.Transaction();
        });

        $("#Message").on('keypress', function (e) {
            if (e.which === 13) {
                Chat.Transaction();
            }
        });

        
    },

    Transaction: function () {
        let objForm = {
            From: From,
            To: To,
            ReferenceId: ReferenceId,
            Alias: Alias,
            Message: $("#Message").val()
        };

        //**** RULES *****
        if ($("#Message").val().trim().length === 0)
            return;

        //***** AJAX *****
        this.Ajax(this.vars.url.transaction, 'POST', objForm, true, function (data, ctx) {
            if (data !== null && data.Status === 0) {
                Chat.AddMsg(data.Data);
                $("#Message").val("");
            }
            else
                ctx.Toast("warning", "top rigth", ctx.vars.tags.Warning, data.Message ? data.Message : ctx.vars.tags.SomethingWrong);
        });
    }
}