$(document).ready(function () {
    Base.call(Notification);

    

    //fileService.listSharesSegmented(null, function (error, result) {
    //    if (error) {
    //        // List shares error
    //    } else {
    //        for (var i = 0, share; share = results.entries[i]; i++) {
    //            console.log("x");
    //            console.log(share);
    //        }
    //    }
    //});

});



var Notification = {

    Alert: function () {
        Notification.Ajax(urlBase + "Alert/Send", 'POST', null, true, function (data, ctx) {
            console.log("=)");
        });
    },

    Transaction: function () {
        Notification.Ajax(urlBase + "Example/Transaction", 'POST', null, true, function (data, ctx) {
            console.log("=)");
        });
    },

    TransactionOffer: function (oper) {
        Notification.Ajax(urlBase + "Example/TransactionOffers/" + oper, 'POST', null, true, function (data, ctx) {
            
        });
    },

    Upload: function () {
        var data = new FormData();
        jQuery.each(jQuery('#File')[0].files, function (i, file) {
            data.append('file-' + i, file);
        });

        jQuery.ajax({
            url: urlBase + 'Example/Transaction',
            data: data,
            cache: false,
            contentType: false,
            processData: false,
            method: 'POST',
            type: 'POST', 
            success: function (data) {
                console.log("=)");
            }
        });
    }
    
}