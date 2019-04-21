var Apply = (function () {
    var pub = {};

    pub.assign = function (a, b,c) {

        $("#AssignSel")
                   .select2({
                       language: "fa",
                       dropdownAutoWidth: true,
                       dir: "rtl",
                       allowClear: true,
                       dropdownParent: $("#myModal"),
                       placeholder: "نام شخص را وارد نمایید...",
                       id: function (repo) { return repo.Id; }, /* <-- ADDED FUNCTION */
                       ajax: {
                           url:'../../RoleAccessGroups/GetPersons',
                           dataType: 'json',
                           delay: 250,
                           data: function (params) {
                               return {
                                   id: params.term // search term
                                   //page: params.page
                               };
                           },
                           processResults: function (data) {
                               return {
                                   results: $.map(data.items,
                                       function (it) {
                                           return {
                                               text: it.FullName,
                                               id: it.Id
                                           }
                                       })
                               };
                           },
                           cache: true
                       },
                       escapeMarkup: function (m) { return m; }, // let our custom formatter work
                       //minimumInputLength: 1,
                       templateResult: formatRepo, // omitted for brevity, see the source of this page
                       templateSelection: formatRepoSelection // omitted for brevity, see the source of this page
                   });
        //Assin triger
        $("#AssignBtn")
            .click(function () {
                var person = $('#AssignSel').val();
                $.ajax({
                    type: "GET",
                    url:'../../RoleAccessGroups/Assign',
                    beforeSend: function (xhr) {
                        $("body").css("opacity", "0.8");
                        $(".cssload-loader").css("display", "block");
                        $('#myModal').modal('hide');
                    },
                    data: { id: a, personId: person, modelName: b, ctrName: c }
                })
                    .done(function (data) {
                        $("#AssignSel").empty().trigger('change');
                        $(".cssload-loader").css("display", "none");
                        $("body").css("opacity", "1");
                        if (data.success) {
                            toastr.success(data.message,
                            '',
                            {
                                closeButton: true,
                                progressBar: true,
                                positionClass: 'toast-top-right',
                                timeOut: 5000,
                                extendedTimeOut: 0,
                                hideDuration: 250,
                                showDuration: 250,
                                rtl: true
                            });
                        } else {
                            toastr.warning(data.message,
                                '',
                                {
                                    closeButton: true,
                                    progressBar: true,
                                    positionClass: 'toast-top-right',
                                    timeOut: 5000,
                                    extendedTimeOut: 0,
                                    hideDuration: 250,
                                    showDuration: 250,
                                    rtl: true
                                });
                        }
                    });
            });


    }
    function formatRepo(repo) {
        return repo.text;
    }

    function formatRepoSelection(repo) {
        return repo.text;
    }

    $('#AssignSel')
        .on('select2:select',
            function (evt) {
                // Do something
                var $el = $(this);
                // NOTE: trigger('change') is needed to make custom controls (such as Select2)
                // aware of the value change
                $('#AssignSel').val($el.val()).trigger('change');
            });
    


    return pub;

})();