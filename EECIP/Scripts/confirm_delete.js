
$(function () {
    $("a.delete-link").click(function () {
        //USAGE **********************************************
        // delete-link: class applied to hyperlink that is originally clicked for deletion
        // delete-confirm: class applied to sibling div that contains the additional attributes needed for deletion
        // data-delete-p: (attribute) URI path to action for deletion
        // data-delete-id: (attribute) key for record being deleted
        // data-delete-id2: (attribute) secondary id to optionally pass to the deletion action. This is sometimes used to redirect back to original page after deletion action finishes
        // data-delete-type: (attribute) helps indicate which parent html block should be removed upon deletion. If blank, assume a table row 

        var deleteLink = $(this);
        deleteLink.hide();
        var confirmButton = deleteLink.siblings(".delete-confirm");
        //confirmButton.show("slide", { direction: "left" }, 1500);
        confirmButton.delay(100).fadeIn(700);

        var cancelDelete = function () {
            removeEvents();
            showDeleteLink();
        };


        //appPath needed to overcome difference in localhost path versus deployed in MVC envirionment 
        var appPath = (window.location.pathname.split("/")[1] == confirmButton.attr('data-delete-p').split("/")[1] ? '' : window.location.pathname.split("/")[1]);
        var delPath = window.location.protocol + "//" + window.location.host + "/" + appPath + confirmButton.attr('data-delete-p');
        console.log(delPath);
        var deleteItem = function () {
            removeEvents();
            confirmButton.fadeOut(700);

            $.post(
                delPath,
                AddAntiForgeryToken({ id: confirmButton.attr('data-delete-id'), id2: confirmButton.attr('data-delete-id2') }))
               .done(function () {

                   var idType = confirmButton.attr('data-delete-type');
                   var parentRow = deleteLink.parents("tr:first");

                   if (idType === 'team') //special handling for team control
                       parentRow = deleteLink.parents(".treecont:first");

                   parentRow.fadeOut('slow', function () {
                       parentRow.remove();
                   });

                }).fail(function (data) {
                   alert("Record cannot be deleted.");
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