
$(document).ready(function () {
    Base.call(CreateProject);
    CreateProject.Init();
});

var CreateProject = {
    vars: {
        projectId: _ProjectId,
        urlCntrlBase: _UrlCntrlBase,
        urlCntrlSave: _UrlCntrlBase + "SaveProjectInfo",
        messageUI: _UIMessages,
        btnTextOk: _BtnTextOk,
        projectCategoryType: _ProjectCategoryType,
        //cabinSpecificationType: JSON.parse(_CabinSpecificationType)
    },
    controls: {
        projectId: $("#ProjectId"),
        projectTypeId: $("#ProjectTypeId"),
        startDate: $("#StartDate"),
        duration: $("#Duration"),
        isExtension: $("#IsExtension"),
        isExtensionNo: $("#IsExtensionNo"),
        isExtensionYes: $("#IsExtensionYes"),
        extension: $("#Extension"),
        regionId: $("#RegionId"),
        budget: $("#Budget"),
        maxRateBudget: $("#MaxRateBudget"),
        budgetBO: $("#BudgetBO"),
        budgetMDR: $("#BudgetMDR"),
        budgetLumbsum: $("#BudgetLumbsum"),

        freeDeckArea: $("#FreeDeckArea"),
        mudCapacity: $("#MudCapacity"),
        cementTankCapacity: $("#CementTankCapacity"),
        oilRecoveryCapacity: $("#OilRecoveryCapacity"),
        dpsRequired: $("#DPSRequired"),
        dpsNoRequired: $("#DPSNoRequired"),
        pemexCheckRequired: $("#PemexCheckRequired"),
        pemexCheckNoRequired: $("#PemexCheckNoRequired"),

        conventionalTug: $("#ConventionalTug"),
        sternDriveTug: $("#SternDriveTug"),
        bollardPull: $("#BollardPull"),
        bollardPullAhead: $("#BollardPullAhead"),
        bollardPullAstern: $("#BollardPullAstern"),
        numberPassenger: $("#NumberPassenger"),
        singleBerth: $("#SingleBerth"),
        doubleBerth: $("#DoubleBerth"),
        fourBerth: $("#FourBerth"),
        airCondition: $("#AirCondition"),
        messRoom: $("#MessRoom"),
        controlRoom: $("#ControlRoom"),
        conferenceRoom: $("#ConferenceRoom"),
        gymnasium: $("#Gymnasium"),
        swimingPool: $("#SwimingPool"),
        office: $("#Office"),
        hospital: $("#Hospital"),
        cargoCapacity: $("#CargoCapacity"),
        deckStrenght: $("#DeckStrenght"),
        tankCapacity: $("#TankCapacity"),
        dischargeRate: $("#DischargeRate"),

        btnCancel: $("#btnCancel"),
        btnBefore: $("#btnBefore"),
        btnNext: $("#btnNext"),
        btnFinish: $("#btnFinish"),

        $formProjectBasicInfo: $("#ProjectBasicInfo"),
        $formProjectSpecificInfo: $("#ProjectSpecificInfo"),
        $formProjectCategorySuitability: $("#ProjectCategorySuitability")
    },
    ui: {
        btnStep1CP: $("#tabCreateProject1"),
        btnStep2CP: $("#tabCreateProject2"),
        btnStep3CP: $("#tabCreateProject3"),
        tab1CP: $("#tab1CP"),
        tab2CP: $("#tab2CP"),
        tab3CP: $("#tab3CP"),
        tab1ContentCP: $("#tabContentCreateProject1"),
        tab2ContentCP: $("#tabContentCreateProject2"),
        tab3ContentCP: $("#tabContentCreateProject3"),
        dpOptions: $("#DPOptions"),

        isTowageContainer: $("#IsTowageContainer"),
        isPersonnelTransportationContainer: $("#IsPersonnelTransportationContainer"),
        isMaterialTransportationContainer: $("#IsMaterialTransportationContainer"),
        isBunkeringContainer: $("#IsBunkeringContainer")
    },

    Init: function () {
        CreateProject.RestoreForm();
        CreateProject.LoadProject(CreateProject.vars.projectId);

        $(CreateProject.controls.isExtensionYes).on('change', this, function () {
            if (CreateProject.controls.isExtensionYes.prop('checked')) {
                CreateProject.controls.extension.removeAttr("disabled");
                CreateProject.controls.extension.attr("required","required");
            }            
        });
        $(CreateProject.controls.isExtensionNo).on('change', this, function () {
            if (CreateProject.controls.isExtensionNo.prop('checked')) {
                CreateProject.controls.extension.attr("disabled", "disabled");
                CreateProject.controls.extension.removeAttr("required");
                CreateProject.controls.extension.val("");
            }
        });
        $(CreateProject.controls.dpsRequired).on('change', this, function () {
            if (CreateProject.controls.dpsRequired.prop('checked')) {
                CreateProject.ui.dpOptions.show();            
                $("input[name='DP']").attr("required", "required");
            }
        });
        $(CreateProject.controls.dpsNoRequired).on('change', this, function () {
            if (CreateProject.controls.dpsNoRequired.prop('checked'))
            {
                CreateProject.ui.dpOptions.hide();
                $("input[name='DynamicPositionSystem']").removeAttr("required");
            }
        });
        $(CreateProject.controls.projectTypeId).on('change', this, function () {
            CreateProject.PrepareControlsCategorySuitability();
        });

        CreateProject.controls.budgetBO.on('change', this, function () {
            if (CreateProject.controls.budgetBO.prop('checked')){
                CreateProject.controls.maxRateBudget.val("");
                CreateProject.controls.maxRateBudget.removeAttr("required");
                CreateProject.controls.maxRateBudget.attr("disabled","disabled");
            }
        });
        CreateProject.controls.budgetMDR.on('change', this, function () {
            CreateProject.controls.maxRateBudget.attr("required","required");
            CreateProject.controls.maxRateBudget.removeAttr("disabled");
        });
    },

   
    LoadStep: function (Step) {
        if (CreateProject.ui.tab1CP.hasClass("current")) {
            if (this.controls.$formProjectBasicInfo.validator('validate')) {
                var isValid = CreateProject.controls.$formProjectBasicInfo[0].checkValidity();
                if (isValid == true) {
                    switch (Step) {
                        case 1:
                            CreateProject.LoadStep1();
                            break;
                        case 2:
                            CreateProject.LoadStep2();
                            break;
                        case 3:
                            CreateProject.LoadStep3();
                            break;
                    }
                }
                else
                    return false;
            }
        }
        else if (CreateProject.ui.tab2CP.hasClass("current")) {
            if (this.controls.$formProjectSpecificInfo.validator('validate')) {
                var isValid = CreateProject.controls.$formProjectSpecificInfo[0].checkValidity();
                if (isValid == true) {
                    switch (Step) {
                        case 1:
                            CreateProject.LoadStep1();
                            break;
                        case 2:
                            CreateProject.LoadStep2();
                            break;
                        case 3:
                            CreateProject.LoadStep3();
                            break;
                    }
                }
                else
                    return false;
            }
        }
        else {
            switch (Step) {
                case 1:
                    CreateProject.LoadStep1();
                    break;
                case 2:
                    CreateProject.LoadStep2();
                    break;
                case 3:
                    CreateProject.LoadStep3();
                    break;
            }
        }
    },


    Next: function () {
        if (CreateProject.ui.tab1CP.hasClass("current")) {
            if (this.controls.$formProjectBasicInfo.validator('validate')) {
                var isValid = CreateProject.controls.$formProjectBasicInfo[0].checkValidity();
                if (isValid == true) {
                    CreateProject.LoadStep2();
                }
                else 
                    return false;
            }
        }
        else if (CreateProject.ui.tab2CP.hasClass("current")) {
            if (this.controls.$formProjectSpecificInfo.validator('validate'))
            {
                var isValid = CreateProject.controls.$formProjectSpecificInfo[0].checkValidity();
                if (isValid == true) {
                    CreateProject.LoadStep3();
                }
                else
                    return false;
            }        
        }
    },

    Previous: function () {
        if (CreateProject.ui.tab1CP.hasClass("current")) {
        }
        else if (CreateProject.ui.tab2CP.hasClass("current")) {
            CreateProject.LoadStep1();
        }
        else if (CreateProject.ui.tab3CP.hasClass("current")) {
            CreateProject.LoadStep2();
        }
    },

    //ValidateProjectBasicInfo: function(){
    //let Prom = new Promise((resolve, reject) => {
    //    CreateProject.ValidateSaveProjectBasicInfo();
    //    resolve();
    //});
    //Prom.then((resolve) => {
    //});
    //},

    ValidateAndSave: function(){
        if (this.controls.$formProjectCategorySuitability.validator('validate')) {
            var isValid = CreateProject.controls.$formProjectCategorySuitability[0].checkValidity();
            if (isValid == true) {
                CreateProject.SaveProject();
            }
            else 
                return false;
        }
    },

    LoadProject: function (ProjectId) {
        if (ProjectId != null && ProjectId > 0)
            CreateProject.GetProjectInfo(ProjectId);
    },

    SaveProject: function () {
        var ObjData = new Object();
        ObjData["ProjectId"] = this.vars.projectId;
        ObjData["ProjectTypeId"] = this.controls.projectTypeId.val();
        ObjData["StartDate"] = this.controls.startDate.val();
        ObjData["Duration"] = this.controls.duration.val();
        ObjData["Extension"] = this.controls.extension.val();
        ObjData["RegionId"] = this.controls.regionId.val();

        var Budget = $("input[name='radioBudget']:checked").val();
        ObjData["Budget"] = Budget;
        ObjData["MaxRateBudget"] = this.controls.maxRateBudget.val();
 
        ObjData["FreeDeckArea"] = this.controls.freeDeckArea.val();
        ObjData["MudCapacity"] = this.controls.mudCapacity.val();
        ObjData["CementTankCapacity"] = this.controls.cementTankCapacity.val();
        ObjData["OilRecoveryCapacity"] = this.controls.oilRecoveryCapacity.val();
        
        var DPS = (CreateProject.controls.dpsRequired.prop('checked')) ? $("input[name='DynamicPositionSystem']:checked").val() : 0;
        ObjData["DynamicPositionSystem"] = DPS;
        var PemexChecked = $("input[name='PemexCheck']:checked").val();
        ObjData["PemexCheck"] = (PemexChecked == 1) ? true: false;

        ObjData["SubtypeId"] = $("input[name='TugType']:checked").val();
        ObjData["BollardPull"] = CreateProject.controls.bollardPull.val();
        ObjData["BollardPullAhead"] = CreateProject.controls.bollardPullAhead.val();
        ObjData["BollardPullAstern"] = CreateProject.controls.bollardPullAstern.val();
        ObjData["NumberPassenger"] = CreateProject.controls.numberPassenger.val();
        
        //var CabinSpecificationData = new Array();
        //for(var i = 0; i < CreateProject.vars.cabinSpecificationType.length;i++)
        //{
        //    var c = CreateProject.vars.cabinSpecificationType[i];
        //    var cabinControlId = "#" + c["CabinName"];
        //    c["CabinQuantity"] = $(cabinControlId).val();
        //    CabinSpecificationData.push(c);
        //}
        //ObjData["CabinSpecification"] = CabinSpecificationData;

        ObjData["AirCondition"] = (CreateProject.controls.airCondition.prop('checked')) ? true : false;
        ObjData["MessRoom"] = (CreateProject.controls.messRoom.prop('checked')) ? true : false;
        ObjData["ControlRoom"] = (CreateProject.controls.controlRoom.prop('checked')) ? true : false;
        ObjData["ConferenceRoom"] = (CreateProject.controls.conferenceRoom.prop('checked')) ? true : false;
        ObjData["Gymnasium"] = (CreateProject.controls.gymnasium.prop('checked')) ? true : false;
        ObjData["SwimingPool"] = (CreateProject.controls.swimingPool.prop('checked')) ? true : false;
        ObjData["Office"] = (CreateProject.controls.office.prop('checked')) ? true : false;
        ObjData["Hospital"] = (CreateProject.controls.hospital.prop('checked')) ? true : false;
        ObjData["CargoCapacity"] = CreateProject.controls.cargoCapacity.val();
        ObjData["DeckStrenght"] = CreateProject.controls.deckStrenght.val();
        ObjData["TankCapacity"] = CreateProject.controls.tankCapacity.val();
        ObjData["DischargeRate"] = CreateProject.controls.dischargeRate.val();

        var Data = JSON.stringify(ObjData);
        var Url = CreateProject.vars.urlCntrlSave;
        $.ajax({
            url: Url,
            type: "POST",
            dataType: "JSON",
            contentType: "application/json",
            data: Data,
            beforeSend: function () {
                CreateProject.BlockUI();
            },
            complete: function () {
                CreateProject.UnblockUI();
            },
            error: function (xhr, textStatus, errorThrown) {
                CreateProject.Modal.Oops(errorThrown);
            },
            success: function (result) {
                var messages = CreateProject.vars.messageUI;
                if (result) {
                    var message = messages[result.Status];
                    if (result.Status == 0)
                    {
                        CreateProject.Toast(message.type, "top right", message.subject, message.message);
                        window.location.href = CreateProject.vars.urlCntrlBase;
                        //CreateProject.Modal.Generic(message.type,
                        //                      message.subject,
                        //                      message.message,
                        //                      CreateProject.vars.btnTextOk,
                        //                      "",
                        //                      function () {
                        //                          window.location.href = CreateProject.vars.urlCntrlBase;
                        //                      },
                        //                      CreateProject.GetSwalStyle.btn_success,
                        //                      false);                
                    }                        
                    else 
                        CreateProject.Toast(message.type, "top right", message.subject, result.Message);
                }
            }
        });
    },

    GetProjectInfo: function(ProjectId){
        var Url = CreateProject.vars.urlCntrlBase + "GetProjectInfo";
        $.ajax({
            url: Url,
            type: "GET",
            dataType: "JSON",
            contentType: "application/json",
            data: {projectId: ProjectId},
            beforeSend:function(){
                CreateProject.BlockUI();
            },
            complete:function(){
                CreateProject.UnblockUI();
            },
            error: function (xhr, textStatus, errorThrown) {
                CreateProject.Modal.Oops(errorThrown);
            },
            success:function(result){
                var messages = CreateProject.vars.messageUI;
                if (result) {
                    var message = messages[result.Status];
                    if (result.Status == 0)
                    {
                        if (result.Data)
                            CreateProject.FillData(result.Data);
                    }
                    else
                        CreateProject.Toast(message.type, "top right", message.subject, result.Message);
                }
            }
        });
    },

    FillData: function (ObjData) {
        var input = CreateProject.controls;
        input.projectId.val(ObjData["ProjectId"]);
        input.projectTypeId.val(ObjData["ProjectTypeId"]);
        input.startDate.val(CreateProject.EpocToStringDate(ObjData["StartDate"]));
        input.duration.val(ObjData["Duration"]);
        if (ObjData["Extension"] != null)
        {
            input.isExtensionYes.prop('checked', true);
            input.extension.removeAttr('disabled');
        }      
        input.extension.val(ObjData["Extension"]);
        input.regionId.val(ObjData["RegionId"]);
        $('input[name=radioBudget][value=' + ObjData["Budget"] + ']').prop("checked", true).trigger("change");
        input.maxRateBudget.val(ObjData["MaxRateBudget"]);

        input.freeDeckArea.val(ObjData["FreeDeckArea"]);
        input.mudCapacity.val(ObjData["MudCapacity"]);
        input.cementTankCapacity.val(ObjData["CementTankCapacity"]);
        input.oilRecoveryCapacity.val(ObjData["OilRecoveryCapacity"]);
        if (ObjData["DynamicPositionSystem"] != null && ObjData["DynamicPositionSystem"] > 0) {
            input.dpsRequired.prop('checked', true);
            CreateProject.ui.dpOptions.show();
            $('input[name=DynamicPositionSystem][value=' + ObjData["DynamicPositionSystem"] + ']').prop('checked', true);
        }
        else {
            input.dpsNoRequired.prop('checked');
        }

        if (ObjData["PemexCheck"] != null && ObjData["PemexCheck"] > 0)
            input.pemexCheckRequired.prop('checked', true);
        else 
            input.pemexCheckNoRequired.prop('checked',true);

        var SelectedProjectType = CreateProject.vars.projectCategoryType.find(x => x.ProjectTypeId == ObjData["ProjectTypeId"]);
        var SelectedCategory = "Is" + SelectedProjectType["Category"] + "Container";
        CreateProject.PrepareControlsCategorySuitability();
        switch (SelectedCategory) {
            case CreateProject.ui.isTowageContainer.attr('id'):
                $('input[name=TugType][value='+ ObjData["SubtypeId"] +']').prop('checked', true).trigger('change');
                input.bollardPull.val(ObjData["BollardPull"]);
                input.bollardPullAhead.val(ObjData["BollardPullAhead"]);
                input.bollardPullAstern.val(ObjData["BollardPullAstern"]);
                break;
            case CreateProject.ui.isPersonnelTransportationContainer.attr('id'):
                input.numberPassenger.val(ObjData["NumberPassenger"]);

                //for (var i=0; i < ObjData["CabinSpecification"].length; i++)
                //{
                //    var cCabin = ObjData["CabinSpecification"][i];
                //    var CabinInput = CreateProject.vars.cabinSpecificationType.find(x => x.CabinType == cCabin["CabinType"]);
                //    $("#" + CabinInput["CabinName"]).val(cCabin["CabinQuantity"]);
                //}
                
                (ObjData["AirCondition"] == true) ? input.airCondition.prop('checked', true) : input.airCondition.prop('checked', false);
                (ObjData["MessRoom"] == true) ? input.messRoom.prop('checked', true) : input.messRoom.prop('checked', false);
                (ObjData["ControlRoom"] == true) ? input.controlRoom.prop('checked', true) : input.controlRoom.prop('checked', false);
                (ObjData["ConferenceRoom"] == true) ? input.conferenceRoom.prop('checked', true) : input.conferenceRoom.prop('checked', false);
                (ObjData["Gymnasium"] == true) ? input.gymnasium.prop('checked', true) : input.gymnasium.prop('checked', false);
                (ObjData["SwimingPool"] == true) ? input.swimingPool.prop('checked', true) : input.swimingPool.prop('checked', false);
                (ObjData["Office"] == true) ? input.office.prop('checked', true) : input.office.prop('checked', false);
                (ObjData["Hospital"] == true) ? input.hospital.prop('checked', true) : input.hospital.prop('checked', false);
                break;
            case CreateProject.ui.isMaterialTransportationContainer.attr('id'):
                input.cargoCapacity.val(ObjData["CargoCapacity"]);
                input.deckStrenght.val(ObjData["DeckStrenght"]);
                break;
            case CreateProject.ui.isBunkeringContainer.attr('id'):
                input.tankCapacity.val(ObjData["TankCapacity"]);
                input.dischargeRate.val(ObjData["DischargeRate"]);
                break;
        }
    },

    RestoreForm:function(){
        var input = CreateProject.controls;
        input.projectId.val(0);
        input.projectTypeId.val("");
        input.startDate.val("");
        input.duration.val("");

        input.extension.val("");     
        input.extension.attr('disabled','disabled'); 
        input.extension.removeAttr('required');
        input.isExtensionNo.prop('checked', true);  

        input.regionId.val("");
        input.budgetBO.prop('checked', true);
        input.maxRateBudget.val("");

        input.freeDeckArea.val("");
        input.mudCapacity.val("");
        input.cementTankCapacity.val("");
        input.oilRecoveryCapacity.val("");
        input.dpsNoRequired.prop('checked', true);
        input.pemexCheckNoRequired.prop('checked',true);

        //restore is not necessary for thirth tab, 
        //this is cleaned and prepared each time tab is loaded
    },

    Cancel:function(){
        window.location.href = CreateProject.vars.urlCntrlBase;
    },

    MonitorTabs: function () {
        $(CreateProject.ui.btnStep1CP).on('click', this, function () {
            CreateProject.LoadStep1();
        });
        $(CreateProject.ui.btnStep2CP).on('click', this, function () {
            CreateProject.LoadStep2();
        });
        $(CreateProject.ui.btnStep3CP).on('click', this, function () {
            CreateProject.LoadStep3();
        });
    },

    LoadStep1: function()
    {
        $(this.ui.tab1CP).removeClass("disabled");
        $(this.ui.tab1CP).addClass("current");
        $(this.ui.tab1CP).attr("aria-disabled", "false");
        $(this.ui.tab1CP).attr("aria-selected", "true");

        $(this.ui.tab1ContentCP).show();
        $(this.ui.tab1ContentCP).addClass("current");
        $(this.ui.tab1ContentCP).attr("aria-hidden", "false");

        $(this.ui.tab2CP).removeClass("current");
        $(this.ui.tab2CP).addClass("disabled");
        $(this.ui.tab2CP).attr("aria-disabled", "true");
        $(this.ui.tab2CP).attr("aria-selected", "false");

        $(this.ui.tab3CP).removeClass("current");
        $(this.ui.tab3CP).addClass("disabled");
        $(this.ui.tab3CP).attr("aria-disabled", "true");
        $(this.ui.tab3CP).attr("aria-selected", "false");

        $(this.ui.tab2ContentCP).hide();
        $(this.ui.tab2ContentCP).removeClass("current");
        $(this.ui.tab2ContentCP).attr("aria-hidden", "true");

        $(this.ui.tab3ContentCP).hide();
        $(this.ui.tab3ContentCP).removeClass("current");
        $(this.ui.tab3ContentCP).attr("aria-hidden", "true");

        this.controls.btnBefore.hide();
        this.controls.btnNext.show();
        this.controls.btnFinish.hide();
    },

    LoadStep2: function () {
        $(this.ui.tab2CP).removeClass("disabled");
        $(this.ui.tab2CP).addClass("current");
        $(this.ui.tab2CP).attr("aria-disabled", "false");
        $(this.ui.tab2CP).attr("aria-selected", "true");

        $(this.ui.tab2ContentCP).show();
        $(this.ui.tab2ContentCP).addClass("current");
        $(this.ui.tab2ContentCP).attr("aria-hidden", "false");

        $(this.ui.tab1CP).removeClass("current");
        $(this.ui.tab1CP).addClass("disabled");
        $(this.ui.tab1CP).attr("aria-disabled", "true");
        $(this.ui.tab1CP).attr("aria-selected", "false");

        $(this.ui.tab3CP).removeClass("current");
        $(this.ui.tab3CP).addClass("disabled");
        $(this.ui.tab3CP).attr("aria-disabled", "true");
        $(this.ui.tab3CP).attr("aria-selected", "false");

        $(this.ui.tab1ContentCP).hide();
        $(this.ui.tab1ContentCP).removeClass("current");
        $(this.ui.tab1ContentCP).attr("aria-hidden", "true");

        $(this.ui.tab3ContentCP).hide();
        $(this.ui.tab3ContentCP).removeClass("current");
        $(this.ui.tab3ContentCP).attr("aria-hidden", "true");

        this.controls.btnBefore.show();
        this.controls.btnNext.show();
        this.controls.btnFinish.hide();
    },

    LoadStep3: function () {
        $(this.ui.tab3CP).removeClass("disabled");
        $(this.ui.tab3CP).addClass("current");
        $(this.ui.tab3CP).attr("aria-disabled", "false");
        $(this.ui.tab3CP).attr("aria-selected", "true");

        $(this.ui.tab3ContentCP).show();
        $(this.ui.tab3ContentCP).addClass("current");
        $(this.ui.tab3ContentCP).attr("aria-hidden", "false");

        $(this.ui.tab2CP).removeClass("current");
        $(this.ui.tab2CP).addClass("disabled");
        $(this.ui.tab2CP).attr("aria-disabled", "true");
        $(this.ui.tab2CP).attr("aria-selected", "false");

        $(this.ui.tab1CP).removeClass("current");
        $(this.ui.tab1CP).addClass("disabled");
        $(this.ui.tab1CP).attr("aria-disabled", "true");
        $(this.ui.tab1CP).attr("aria-selected", "false");

        $(this.ui.tab2ContentCP).hide();
        $(this.ui.tab2ContentCP).removeClass("current");
        $(this.ui.tab2ContentCP).attr("aria-hidden", "true");

        $(this.ui.tab1ContentCP).hide();
        $(this.ui.tab1ContentCP).removeClass("current");
        $(this.ui.tab1ContentCP).attr("aria-hidden", "true");

        this.controls.btnBefore.show();
        this.controls.btnNext.hide();
        this.controls.btnFinish.show();
    },

    PrepareControlsCategorySuitability: function () {
        CreateProject.ui.isTowageContainer.hide();
        CreateProject.ui.isPersonnelTransportationContainer.hide();
        CreateProject.ui.isMaterialTransportationContainer.hide();
        CreateProject.ui.isBunkeringContainer.hide();

        var SelectedCategory = CreateProject.controls.projectTypeId.val();
        if (SelectedCategory != null && SelectedCategory > 0) {
            var SelectedType = CreateProject.vars.projectCategoryType.find(x => x.ProjectTypeId == SelectedCategory);
            var SelectedTypeContainer = "Is" + SelectedType["Category"] + "Container";
            $("#" + SelectedTypeContainer).show();
        }

        CreateProject.RestoreCategorySuitabilityControls();
        switch (SelectedTypeContainer) {
            case CreateProject.ui.isTowageContainer.attr('id'):
                $("input[name='TugType']").attr("required", "required");
                CreateProject.MonitorBollard();
                break;
            case CreateProject.ui.isPersonnelTransportationContainer.attr('id'):
                CreateProject.controls.numberPassenger.attr("required", "required");
                break;
            case CreateProject.ui.isMaterialTransportationContainer.attr('id'):
                CreateProject.controls.cargoCapacity.attr("required", "required");
                break;
            case CreateProject.ui.isBunkeringContainer.attr('id'):
                CreateProject.controls.tankCapacity.attr("required", "required");
                break;
        };
    },

    RestoreCategorySuitabilityControls: function () {
        $("input[name='TugType']").removeAttr("required");
        CreateProject.controls.numberPassenger.removeAttr("required");
        CreateProject.controls.cargoCapacity.removeAttr("required");
        CreateProject.controls.tankCapacity.removeAttr("required");

        CreateProject.controls.bollardPull.attr("disabled", "disabled");
        CreateProject.controls.bollardPullAhead.attr("disabled", "disabled");
        CreateProject.controls.bollardPullAstern.attr("disabled", "disabled");

        $("input[name='TugType']").prop("checked", false);
        CreateProject.controls.bollardPull.val("");
        CreateProject.controls.bollardPullAhead.val("");
        CreateProject.controls.bollardPullAstern.val("");
        CreateProject.controls.numberPassenger.val("");

        //for (var i = 0; i < CreateProject.vars.cabinSpecificationType.length; i++) {
        //    var c = CreateProject.vars.cabinSpecificationType[i];
        //    var cabinControlId = "#" + c["CabinName"];
        //    $(cabinControlId).val("");
        //}

        CreateProject.controls.airCondition.prop('checked', false);
        CreateProject.controls.messRoom.prop('checked', false);
        CreateProject.controls.controlRoom.prop('checked', false);
        CreateProject.controls.conferenceRoom.prop('checked', false);
        CreateProject.controls.gymnasium.prop('checked', false);
        CreateProject.controls.swimingPool.prop('checked', false);
        CreateProject.controls.office.prop('checked', false);
        CreateProject.controls.hospital.prop('checked', false);
        CreateProject.controls.cargoCapacity.val("");
        CreateProject.controls.deckStrenght.val("");
        CreateProject.controls.tankCapacity.val("");
        CreateProject.controls.dischargeRate.val("");
    },

    MonitorBollard: function () {
        CreateProject.controls.conventionalTug.on('change', this, function () {
            if (CreateProject.controls.conventionalTug.prop('checked')) {
                CreateProject.controls.bollardPull.attr("required","required");
                CreateProject.controls.bollardPull.removeAttr("disabled");
                CreateProject.controls.bollardPullAhead.attr("disabled", "disabled");
                CreateProject.controls.bollardPullAstern.attr("disabled", "disabled");
                CreateProject.controls.bollardPullAhead.removeAttr("required");
                CreateProject.controls.bollardPullAstern.removeAttr("required");
                CreateProject.controls.bollardPullAhead.val("");
                CreateProject.controls.bollardPullAstern.val("");
            }
        });
        CreateProject.controls.sternDriveTug.on('change', this, function () {
            if (CreateProject.controls.sternDriveTug.prop('checked')) {
                CreateProject.controls.bollardPull.attr("disabled", "disabled");
                CreateProject.controls.bollardPull.removeAttr("required");
                CreateProject.controls.bollardPull.val("");
                CreateProject.controls.bollardPullAhead.removeAttr("disabled");
                CreateProject.controls.bollardPullAstern.removeAttr("disabled");
                CreateProject.controls.bollardPullAhead.attr("required","required");
                CreateProject.controls.bollardPullAstern.attr("required","required");
            }
        });
    }





}