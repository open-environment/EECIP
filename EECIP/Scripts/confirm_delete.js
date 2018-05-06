
$(function () {
    $("a.delete-link").click(function () {
        //DESCRIPTION ****************************************
        // Used in conjunction with toastr.js, this javascript performs 3 actions: 
        //    (1) prompts user with deletion confirmation
        //    (2) calls deletion action upon confirmation
        //    (3) displays deletion confirmation message via toastr popup

        //USAGE **********************************************
        // delete-link: class applied to hyperlink that is originally clicked for deletion
        // delete-confirm: class applied to sibling div that contains the additional attributes needed for deletion
        // data-delete-p: (attribute) URI path to action for deletion
        // data-delete-id: (attribute) key for record being deleted
        // data-delete-id2: (attribute) secondary id to optionally pass to the deletion action. This is sometimes used to redirect back to original page after deletion action finishes
        // data-delete-type: (attribute) helps indicate which parent html block should be removed upon deletion. If blank, assume a table row . If 'cells' then delete the row contents but keep row
        // data-success-url: (attribute) optional redirect to another page upon successful deletion

        var deleteLink = $(this);
        deleteLink.hide();
        var confirmButton = deleteLink.siblings(".delete-confirm");
        confirmButton.delay(100).fadeIn(700);

        var cancelDelete = function () {
            removeEvents();
            showDeleteLink();
        };


        //appPath needed to overcome difference in localhost path versus deployed in MVC envirionment 
        var appPath = (window.location.pathname.split("/")[1] == confirmButton.attr('data-delete-p').split("/")[1] ? '' : window.location.pathname.split("/")[1]);
        var delPath = window.location.protocol + "//" + window.location.host + "/" + appPath + confirmButton.attr('data-delete-p');
        
        var deleteItem = function () {
            removeEvents();
            confirmButton.fadeOut(700);

            $.post(
                delPath,
                AddAntiForgeryToken({ id: confirmButton.attr('data-delete-id'), id2: confirmButton.attr('data-delete-id2') }))
                .done(function (response) {
                    if (response == "Success") {

                        if (confirmButton.attr('data-success-url')!=null && confirmButton.attr('data-success-url').length > 0) {
                            var redirPath = window.location.protocol + "//" + window.location.host + "/" + appPath + confirmButton.attr('data-success-url');
                            console.log(redirPath);
                            window.location.replace(redirPath);
                        }

                        var idType = confirmButton.attr('data-delete-type');
                        var parentRow = deleteLink.parents("tr:first");

                        if (idType === 'team') //special handling for tree control
                            parentRow = deleteLink.parents(".treecont:first");

                        if (idType === 'cells')
                            parentRow = deleteLink.parents("td:first").next().nextAll();

                        parentRow.fadeOut('slow', function () {
                            parentRow.remove();
                        });
                    }
                    else {
                        toastr.warning(response);
                    }

                }).fail(function (data) {
                    toastr.warning("Record cannot be deleted.");
               });
            return false;
        };

        var removeEvents = function () {
            confirmButton.off("click", deleteItem);
            $(document).on("click", cancelDelete);
        };

        var showDeleteLink = function () {
            confirmButton.hide();
            deleteLink.fadeIn(700);
        };

        confirmButton.on("click", deleteItem);
        $(document).on("click", cancelDelete);

        return false;
    });

    AddAntiForgeryToken = function (data) {
        data.__RequestVerificationToken = $('input[name=__RequestVerificationToken]').val();
        return data;
    };
});