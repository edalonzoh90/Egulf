var Utils = new function () {
    return {

     
        UserInterfaceToData: function UserInterfaceToData(form) {
            var obj = new Object();
            $.each(form.serializeArray(), function (i, field) {
                try {
                    //field.name = field.name.replace(".", "\\.");
                    try {
                        obj[field.name] = field.value.trim();
                    } catch (ex) {
                        obj[field.name] = field.value;
                    }
                    var $input = $(form).find('input[name="' + field.name + '"]');
                    if ($input.hasClass("date") && field.value != "") {
                        if ($input.data('datepicker')) {
                            var d = field.value.split("/");
                            obj[field.name] = d[2] + "-" + d[1] + "-" + d[0];
                        }
                        else
                            if ($input.data('DateTimePicker')) {
                                obj[field.name] = $input.data("DateTimePicker").date().startOf('day').toISOString();
                            }
                    }

                } catch (ex) {
                    console.log(ex);
                }
            })
            return obj;
        },
    }
}();