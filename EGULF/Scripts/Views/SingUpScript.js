
$(document).ready(function () {
    Base.call(Singup);
    Singup.Init();
});

var Singup = {
    vars: {
        urlSingUp: urlCntrlSingUp,
        urlLogin: urlCntrlLogin,
        btnTextRegisterConfirmed: BtnTextRegisterConfirmed,
        form: {
            firstName: $("#FirstName"),
            lastName: $("#LastName"),
            email: $("#Email"),
            phoneNumber: $("#PhoneNumber"),
            skype: $("#Skype"),
            username: $("#Username"),
            password: $("#Password"),
            repeatPassword: $("#RepeatPassword")
        },
        $form: $("#SingUpForm"),
        uiMessages: UIMessages
    },
    controls:{
        btnRegister: $("#btnRegister")
    },


    Init: function () {
        //this.FormValidate(this.vars.$form, e => {
        //   // $(Singup.vars.$form).submit(function (e, context) {
        //        e.preventDefault();
        //        var isValid = Singup.vars.$form[0].checkValidity();
        //        if (isValid == true) {
        //            if (Singup.vars.form.password.val() == Singup.vars.form.repeatPassword.val())
        //            {   
        //                var valid = $(Singup.vars.$form).validator('validate');
        //                Singup.Register();
        //            }
        //            else
        //                return false;
        //        }
        //        else {
        //            return false;
        //        }
           
        //    //});
        //});
        Singup.controls.btnRegister.on('click', this, function () {
            if (Singup.vars.$form.validator('validate')) {
                var isValid = Singup.vars.$form[0].checkValidity();
                if (isValid == true) {
                    if (Singup.vars.form.password.val() == Singup.vars.form.repeatPassword.val()) {
                        Singup.Register();
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
        });
    },


    Register: function (context) {
        var objData = new Object();
        objData.FirstName = this.vars.form.firstName.val();
        objData.LastName = this.vars.form.lastName.val();
        objData.PhoneNumber = this.vars.form.phoneNumber.val();
        objData.Email = this.vars.form.email.val();
        objData.Skype = this.vars.form.skype.val();
        objData.UserName = this.vars.form.username.val();
        objData.Password = Crypto.Encrypt(this.vars.form.password.val());
        var url = this.vars.urlSingUp;

        Singup.BlockUI();
        $.ajax({
            url: url,
            type: "POST",
            dataType: "JSON",
            contentType: "application/json",
            data: JSON.stringify(objData),
            beforeSend: function () {
                Singup.vars.form.email.removeAttr("data-remote");
                Singup.vars.form.email.removeAttr("data-remote-error");
                Singup.vars.form.email.removeAttr("remote");
                Singup.vars.form.username.removeAttr("data-remote");
                Singup.vars.form.username.removeAttr("data-remote-error");
                Singup.vars.form.username.removeAttr("remote");
            },
            complete: function () {
                Singup.UnblockUI();
            },
            success: function (result) {
                var messages = Singup.vars.uiMessages;
                if (result) {                  
                    var message = messages[result.Status];
                    if (result.Status == 0) {
                        
                     
                        Singup.Modal.Generic(message.type,
                                             message.subject,
                                             result.Message,
                                             Singup.vars.btnTextRegisterConfirmed,
                                             "",
                                             function () {
                                                 location.href = Singup.vars.urlLogin;
                                             },
                                             Singup.GetSwalStyle.btn_success,
                                             false);
                    }
                    else {
                        Singup.Toast(message.type, "top right", message.subject, result.Message);
                    }
                }
                else {
                    var message = messages[1];
                    Singup.Toast(message.type, "top right", message.subject, message.message);
                }
            }
        });
    },



}

