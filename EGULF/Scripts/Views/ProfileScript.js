
$(document).ready(function () {
    Base.call(Profile);
    Profile.Init();
});


var Profile = {
    vars: {
        userId: UserId,
        sessionUserId: SessionUserId,
        urlCntrlProfile: _urlCntrlProfile,
        profile: {
            personId: null,
            userId:null,
            firstName: null,
            lastName: null,
            companyName: null,
            username: null,
            phoneNumber: null,
            email: null,
            skype: null,
            companyId: null,
            fileReferenceId: null
        },
        uiMessages: UIMessages,
        uiConfirmationSweet: UIConfirmationSweet
    },
    controls:{
        input: {
            firstName: $("#FirstName"),
            lastName: $("#LastName"),
            companyName: $("#CompanyName"),
            username: $("#Username"),
            phoneNumber: $("#PhoneNumber"),
            email: $("#Email"),
            skype: $("#Skype"),
            password: $("#Password"),
            newPassword: $("#NewPassword"),
            repeatPassword: $("#RepeatPassword"),
            profileImageFile: $("#ProfileImageFile")
        },
        buttons: {
            editProfile: $("#btnEditProfile"),
            cancelProfile: $("#btnCancelProfile"),
            saveProfile: $("#btnSaveProfile"),
            changePassword: $("#btnChangePassword"),
            cancelPassword: $("#btnCancelPassword"),
            savePassword: $("#btnSavePassword"),
            removeProfileImage: $("#btnRemoveProfileImage")
        },
        $formProfile: $("#ProfileForm"),
        //$formControlsProfile: $("#ProfileForm .form-control"),
        $formPassword: $("#PasswordForm"),
        //$formControlsPassword: $("#PasswordForm .form-control"),
        editButtonsSection: $("#EditButtonsSection"),
        editPasswordSection: $("#EditPasswordSection"),
        profileImage: $("#ProfileImage"),
        profileImageOptions: $("#ProfileImageOptions")
    },

    Init: function () {
        Profile.GetProfile();

        $(Profile.controls.buttons.editProfile).on('click', this, function (context) {
            Profile.PrepareProfileForm();           
        });
        
        $(Profile.controls.buttons.cancelProfile).on('click', this, function (context) {
            Profile.RestoreProfileForm();          
        });

        $(Profile.controls.buttons.changePassword).on('click', this, function (context) {
            Profile.PreparePasswordForm();
        });

        $(Profile.controls.buttons.cancelPassword).on('click', this, function (context) {
            Profile.RestorePasswordForm();
        });

        $(Profile.controls.buttons.removeProfileImage).on('click', this, function (context) {
            Profile.DeleteUserImage();
        });

        $(Profile.controls.buttons.saveProfile).on('click', this, function (context) {
            //$(Profile.controls.$formProfile).submit(function (e, context) {
            Profile.FormValidate(Profile.controls.$formProfile, e => {
                e.stopImmediatePropagation();
                e.preventDefault();
                var isValid = Profile.controls.$formProfile[0].checkValidity();
                if (isValid == true) {
                    let PromRefresProfile = new Promise((resolve, reject) => {
                        Profile.SaveProfile();
                        resolve();
                    });
                    
                    PromRefresProfile.then((resolve) => {
                        window.location.href = Profile.vars.urlCntrlProfile;
                    });                  
                }
                else {
                    return false;
                }
            });
            //});
        });

        $(Profile.controls.buttons.savePassword).on('click', this, function (context) {
            $(Profile.controls.$formPassword).submit(function (e, context) {
                e.stopImmediatePropagation();
                e.preventDefault();
                var isValid = Profile.controls.$formPassword[0].checkValidity();
                if (isValid == true) {
                    if (Profile.controls.input.newPassword.val() == Profile.controls.input.repeatPassword.val())
                    {
                        Profile.SavePassword();                
                    }                       
                    else
                        return false;
                }
                else {
                    return false;
                }
            });
        });


        $(Profile.controls.input.profileImageFile).change(function (e) {
            var File = e.target.files[0];
            if (File.type.match('image/jpeg') || File.type.match('image/jpg') || File.type.match('image/png')) {
                var reader = new FileReader();
                reader.onload = function (ev) {           
                    Profile.controls.profileImage.attr({ "src": ev.target.result });       
                };
                reader.readAsDataURL(File);
            }
            else {
                var message = Profile.vars.uiMessages[2];
                Profile.Toast(message.type, "top right", message.subject, message.message);
            }    
        });
    },


    GetUserImage:function(){
        var UrlImage = Profile.vars.urlCntrlProfile + "GetUserImage?userId=" + Profile.vars.userId;
        Profile.controls.profileImage.attr({ "src": UrlImage, height: "auto", width: "100%" });
    },

    GetProfile: function () {
        let GetProfileDataProm = new Promise((resolve, reject) => {
            Profile.GetUserImage();
            resolve();
        });

        GetProfileDataProm.then((resolve) => {
            var Url = Profile.vars.urlCntrlProfile + "GetUserProfile?userId=" + Profile.vars.userId;
            $.ajax({
                url: Url,
                type: "GET",
                beforeSend: function () {
                    Profile.BlockUI();
                },
                complete: function () {
                    Profile.UnblockUI();
                },
                error: function (request, status, error) {
                    var message = Profile.vars.uiMessages[1];
                    Profile.Toast(message.type, "top right", message.subject, error);
                },
                success: function (result) {
                    var messages = Profile.vars.uiMessages;
                    if (result) {
                        var message = messages[result.Status];
                        if (result.Status == 0) {
                            Profile.LoadData(result);
                            Profile.LoadProfile();
                        }
                        else {
                            Profile.Toast(message.type, "top right", message.subject, result.Message);
                        }
                    }
                    else {
                        var message = messages[1];
                        Profile.Toast(message.type, "top right", message.subject, message.message);
                    }
                }
            });
        });         
    },

    LoadData: function (data) {
        Profile.vars.profile.personId = data.Data["PersonId"];
        Profile.vars.profile.userId = data.Data["UserId"];
        Profile.vars.profile.firstName = data.Data["FirstName"];
        Profile.vars.profile.lastName = data.Data["LastName"];
        Profile.vars.profile.companyName = data.Data["CompanyName"];
        Profile.vars.profile.username = data.Data["Username"];
        Profile.vars.profile.phoneNumber = data.Data["PhoneNumber"];
        Profile.vars.profile.email = data.Data["Email"];
        Profile.vars.profile.skype = data.Data["Skype"];
        Profile.vars.profile.companyId = data.Data["CompanyId"];
        Profile.vars.profile.fileReferenceId = data.Data["FileReferenceId"];
    },

    LoadProfile: function () {
        Profile.controls.input.firstName.val(Profile.vars.profile.firstName);
        Profile.controls.input.lastName.val(Profile.vars.profile.lastName);
        Profile.controls.input.companyName.val(Profile.vars.profile.companyName);
        Profile.controls.input.username.val(Profile.vars.profile.username);
        Profile.controls.input.phoneNumber.val(Profile.vars.profile.phoneNumber);
        Profile.controls.input.email.val(Profile.vars.profile.email);
        Profile.controls.input.skype.val(Profile.vars.profile.skype);
    },

    RestorePasswordForm: function () {
        $(Profile.controls.$formPassword)[0].reset();
        Profile.controls.input.password.attr("disabled", "disabled");
        Profile.controls.input.newPassword.attr("disabled", "disabled");
        Profile.controls.input.repeatPassword.attr("disabled", "disabled");
        Profile.controls.input.password.val("");
        Profile.controls.input.newPassword.val("");
        Profile.controls.input.repeatPassword.val("");
        Profile.controls.editPasswordSection.hide();
        Profile.controls.buttons.changePassword.show();     
    },

    PreparePasswordForm: function(){
        Profile.controls.input.password.removeAttr("disabled");
        Profile.controls.input.newPassword.removeAttr("disabled");
        Profile.controls.input.repeatPassword.removeAttr("disabled");
        Profile.controls.editPasswordSection.show();
        Profile.controls.buttons.changePassword.hide();
    },

    RestoreProfileForm: function () {
        $(Profile.controls.$formProfile)[0].reset();
        Profile.GetProfile();
        Profile.controls.input.firstName.attr("readonly", "readonly");
        Profile.controls.input.lastName.attr("readonly", "readonly");
        Profile.controls.input.email.attr("readonly", "readonly");
        Profile.controls.input.phoneNumber.attr("readonly", "readonly");
        Profile.controls.input.skype.attr("readonly", "readonly");
        Profile.controls.editButtonsSection.hide();
        Profile.controls.buttons.editProfile.show();
        Profile.controls.profileImageOptions.hide();   
    },

    PrepareProfileForm:function(){
        Profile.controls.input.firstName.removeAttr("readonly");
        Profile.controls.input.lastName.removeAttr("readonly");
        Profile.controls.input.email.removeAttr("readonly");
        Profile.controls.input.phoneNumber.removeAttr("readonly");
        Profile.controls.input.skype.removeAttr("readonly");
        Profile.controls.editButtonsSection.show();
        Profile.controls.buttons.editProfile.hide();
        Profile.controls.profileImageOptions.show();
    },

    SaveProfile: function () {
        var objData = new Object();
        objData.PersonId = this.vars.profile.personId;
        objData.FirstName = this.controls.input.firstName.val();
        objData.LastName = this.controls.input.lastName.val();
        objData.PhoneNumber = this.controls.input.phoneNumber.val();
        objData.Email = this.controls.input.email.val();
        objData.Skype = this.controls.input.skype.val();
        objData.FileReferenceId = this.vars.profile.fileReferenceId;
        objData.CompanyId = this.vars.profile.companyId;

        var objForm = new FormData();
        var FileImage = Profile.controls.input.profileImageFile.get(0);
        if (FileImage.files.length > 0)
            objForm.append(FileImage.files[0].name, FileImage.files[0]);

        for (var prop in objData) {
            objForm.append(prop.toString(), objData[prop]);
        }

        var url = this.vars.urlCntrlProfile + "SaveProfile";
        $.ajax({
            url: url,
            type: "POST",
            dataType: "JSON",
            contentType: false,
            processData: false,
            data: objForm,
            beforeSend: function () {
                Profile.BlockUI();
            },
            complete: function () {
                Profile.UnblockUI();
            },
            success: function (result) {
                var messages = Profile.vars.uiMessages;
                if (result) {
                    var message = messages[result.Status];
                    if (result.Status == 0) {                       
                        Profile.Toast(message.type, "top right", message.subject, message.message);
                        //Profile.RestoreProfileForm();
                    }
                    else {
                        Profile.Toast(message.type, "top right", message.subject, result.Message);
                    }
                }
                else {
                    var message = messages[1];
                    Profile.Toast(message.type, "top right", message.subject, message.message);
                }
            }
        });
    },

    SavePassword: function (context) {
        var objData = new Object();
        //objData.UserId = this.vars.profile.userId; //it is set on backend server
        objData.UserName = this.vars.profile.username;
        objData.Password = Crypto.Encrypt(this.controls.input.password.val());
        objData.NewPassword = Crypto.Encrypt(this.controls.input.newPassword.val());

        var url = this.vars.urlCntrlProfile + "ChangePassword";       
        $.ajax({
            url: url,
            type: "POST",
            dataType: "JSON",
            contentType: "application/json",
            data: JSON.stringify(objData),
            beforeSend: function () {
                Profile.BlockUI();
            },
            complete: function () {
                Profile.UnblockUI();
            },
            success: function (result) {
                var messages = Profile.vars.uiMessages;
                if (result) {             
                    var message = messages[result.Status];
                    if (result.Status == 0) {
                        Profile.Toast(message.type, "top right", message.subject, message.message);
                        Profile.RestorePasswordForm();
                    }
                    else {
                        Profile.Toast(message.type, "top right", message.subject, result.Message);
                    }
                }
                else {
                    var message = messages[1];
                    Profile.Toast(message.type, "top right", message.subject, message.message);
                }
            }
        });
    },


    DeleteUserImage: function (context)
    {
        var Url = Profile.vars.urlCntrlProfile + "DeleteUserImage";
        $.ajax({
            url: Url,
            type: "GET",
            beforeSend: function () {
                Profile.BlockUI();
            },
            complete: function () {
                Profile.UnblockUI();
            },
            error: function (request, status, error) {
                var message = Profile.vars.uiMessages[1];
                Profile.Toast(message.type, "top right", message.subject, error);
            },
            success: function (result) {
                var messages = Profile.vars.uiMessages;
                if (result) {
                    var message = messages[result.Status];
                    if (result.Status == 0) {
                        Profile.Toast(message.type, "top right", message.subject, message.message);
                    }
                    else {
                        Profile.Toast(message.type, "top right", message.subject, result.Message);
                    }
                }
                else {
                    var message = messages[1];
                    Profile.Toast(message.type, "top right", message.subject, message.message);
                }
            }
        });
        Profile.RestoreProfileForm();
    }


}