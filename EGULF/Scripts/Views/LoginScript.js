class Login {

    constructor() {
        url: urlLogin;
    }

    Recover() {
        $("#LoginForm").hide();
        $("#RecoverAccountForm").show();
    }

    CancelRecover() {
        $("#RecoverAccountForm").hide();
        $("#LoginForm").show();     
    }

    MonitorRecover() {
        $("#btnReset.RecoverAccount").on('click',function () {
            $("#RecoverAccountForm").validator('validate');
            var isValid = $("#RecoverAccountForm")[0].checkValidity();
            if (isValid == true) {
                login.RecoverAccout();
            }
            else
                return false;
        });
    }

    RecoverAccout() {
        login.BlockUI();
        var objParameters = new Object();
        objParameters.Email = Crypto.Encrypt($("#Email.RecoverAccount").val());
        $.ajax({
            url: urlLogin + "Recover/",
            type: "POST",
            dataType: "JSON",
            contentType: "application/json",
            data: JSON.stringify(objParameters),
            beforeSend: function () { },
            complete: function () { login.UnblockUI(); },
            success: function (result) {       
                if (result.Data.Status == 0) {
                    login.Modal.Generic('success',
                                       'Cuenta restablecida',
                                       result.Data.Message,
                                       'Ok',
                                        "",
                                        function () {
                                            $("#Email.RecoverAccount").val("");                               
                                            location.href = urlHome + "Index";
                                        },
                                        login.GetSwalStyle.btn_success,
                                        false);
                }
                else {
                    var Type;
                    var Subject;
                    if (result.Data.Status == 2) {
                        Type = 'Warning';
                        Subject = "Datos Incorrectos";
                    }
                    else {
                        Type = 'Error';
                        Subject = 'Error';
                    }
                      
                    login.Toast(Type, "top right", Subject, result.Data.Message);
                }
            }
        });
    }

    Transaction() {
        $("#lblError").addClass("hidden");
        var obj = Utils.UserInterfaceToData($("#LoginForm"));
        obj.Password = Crypto.Encrypt($("#Password").val());

        $.ajax({
            url: urlLogin + "Login/",
            type: "POST",
            dataType: "JSON",
            contentType: "application/json",
            data: JSON.stringify(obj),
            beforeSend: function () {  },
            complete: function () {  },
            success: function (result) {
                if (result.status == 0 && result.data == 0) {
                    //Toast redirect
                    location.href = urlHome + "Index";
                }
                else {
                    login.Toast('Warning', "top right", result.data["subject"], result.data["message"]);
                }
            }
        });
    }

    MonitorHelp() {
        $("#btnHelp.Help").on('click', function () {
            $("#HelpForm").validator('validate');
            var isValid = $("#HelpForm")[0].checkValidity();
            if (isValid == true) {
                login.SendHelpRequest();
            }
            else
                return false;
        });
    }

    SendHelpRequest() {
        login.BlockUI();
        var objParameters = new Object();
        objParameters.Email = Crypto.Encrypt($("#helpEmail.Help").val());
        objParameters.Message = Crypto.Encrypt($("#helpMessage.Help").val());
        $.ajax({
            url: urlSupport + "Help/",
            type: "POST",
            dataType: "JSON",
            contentType: "application/json",
            data: JSON.stringify(objParameters),
            beforeSend: function () { },
            complete: function () { login.UnblockUI(); },
            success: function (result) {
                if (result.Data.Status == 0) {
                    login.Modal.Generic('success',
                                       'Mensaje Enviado',
                                       result.Data.Message,
                                       'Ok',
                                        "",
                                        function () {
                                            login.CleanHelp();
                                            $('#help-modal').modal('toggle');
                                        },
                                        login.GetSwalStyle.btn_success,
                                        false);
                }
                else {
                    login.Toast('Warning', "top right", "Lo sentimos", result.Data.Message);
                }
            }
        });   
    }

    CleanHelp() {
        $("#helpEmail.Help").val("");
        $("#helpMessage.Help").val("");
    }




};

var login = new Login();

$(document).ready(function () {
    Base.call(login);
    $("#btnLogin").click(function () {
        login.Transaction();
    });
    login.MonitorRecover();
    login.MonitorHelp();
});


