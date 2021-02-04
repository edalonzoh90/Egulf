$(document).ready(function () {
    Base.call(DeleteProject);
    DeleteProject.Init();
});

var DeleteProject = {
    vars: {
        projectId: _ProjectId,
        urlDeleteProject: _UrlDeleteProject,
        urlBase: _UrlBase,
        uimessages: _UIMessages,
        deleteProjectTextConfirm: _DeleteProjectTextConfirm,
        deleteProjectTitleConfirm: _DeleteProjectTitleConfirm
    },
    controls: {
        cancelationComment: $("#CancelationComment"),
        $DeleteProjectform: $("#DeleteProjectForm")
    },

    Init: function () {

    },

    Delete: function () {
        if (this.controls.$DeleteProjectform.validator('validate'))
        {
            var isValid = this.controls.$DeleteProjectform[0].checkValidity();
            if (isValid == true) {
                DeleteProject.DeleteProjectConfirm();
            }
            else
                return false;
        }
    },

    DeleteProjectConfirm: function(){
        this.Modal.DeleteConfirm(
            function () {
                DeleteProject.DeleteProject();
            },
            this,
            this.vars.deleteProjectTextConfirm,
            this.vars.deleteProjectTitleConfirm
        );
    },

    DeleteProject: function () {
        ObjData = new Object();
        ObjData["ProjectId"] = this.vars.projectId;
        ObjData["CancelationComment"] = this.controls.cancelationComment.val();

        var Url = DeleteProject.vars.urlDeleteProject;
        this.BlockUI();
        this.Ajax(
                Url,
                "POST",
                ObjData,
                true,
                function (data, ctx) {
                    var messages = ctx.vars.uimessages;
                    var message = messages[data.Status];
                    ctx.Toast(message.type, "top right", message.subject, message.message);
                    window.location.href = ctx.vars.urlBase;
                },
                function (xhr, textStatus, ctx)
                {
                    ctx.UnblockUI();
                }
            );
    }
}