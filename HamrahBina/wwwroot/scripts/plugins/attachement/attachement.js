var Attachment = (function () {
    var public = {};
    public.initPlugins = function () {
        if ($.fn.DataTable.isDataTable(".dataTables-js")) {
            $('.dataTables-js').DataTable().clear().destroy();
        }
        $('.dataTables-js').DataTable({
            responsive: true,
            "language": {
                "url": "/Content/Persian.json"
            },
            dom: 'Tgt',
            buttons: [
                { extend: 'copy' },
                { extend: 'csv' },
                { extend: 'excel', title: '@ViewData["Title"]' },
                { extend: 'pdf', title: '@ViewData["Title"]' },
                {
                    extend: 'print', customize: function (win) {
                        $(win.document.body).addClass('white-bg');
                        $(win.document.body).css('font-size', '10px');
                        $(win.document.body).find('table')
                            .addClass('compact')
                            .css('font-size', 'inherit');
                    }
                }
            ]
        });
        $(":file")
               .filestyle({
                   buttonText: "انتخاب فایل",
                   buttonBefore: "true",
                   placeholder: "فایل مورد نظر را انتخاب نمایید...",
                   size: "sm"
               });
    }
    public.DeleteAttachement = function(fileType, fileTypeId, deletedId) {
        var url = "/DocumentCenters/Delete?id=" + deletedId + '&type=' + fileType + '&fileId=' + fileTypeId;
        swal({
            title: 'حذف فایل پیوست',
                text: "آیا مطمئن به حذف می باشید؟",
                type: 'warning',
                showCancelButton: true,
                confirmButtonColor: '#3085d6',
                cancelButtonColor: '#d33',
                confirmButtonText: 'حذف',
                cancelButtonText: 'لغو',
                confirmButtonClass: 'btn btn-danger swalBtn',
                cancelButtonClass: 'btn btn-success swalBtn',
                buttonsStyling: true,
                onOpen: function() {
                    $(":input:not(.swalBtn)").prop("disabled", true);

                },
                onClose: function() {
                    $(":input:not(.swalBtn)").prop("disabled", false);

                }
            })
            .then(function() {
                    $("body").css("opacity", "0.8");
                    $(".cssload-loader").css("display", "block");
                    $.ajax(
                        {
                            type: "post",
                            url: url
                        })
                        .done(function(res) {
                            $(".cssload-loader").css("display", "none");
                            $("body").css("opacity", "1");
                            $(":input").prop("disabled", false);
                            if (res.success) {
                                $("#Attachment").html(res.message);
                                public.initPlugins();
                                toastr.success("با موفقیت حذف شد",
                                    'حذف',
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
                                toastr.warning(res.message,
                                    'حذف',
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


                        })
                        .error(function() {
                            toastr.warning("خطایی رخ داده است",
                                'حذف',
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

                        });
                },
                function(dismiss) {
                    // dismiss can be 'cancel', 'overlay',
                    // 'close', and 'timer'
                    //if (dismiss === 'cancel') {

                    //}
                });
    }
    public.UploadFile = function (fileType, fileTypeId, pageType) {
        var fileVal = $('#FileUpload').val();
        if (fileVal === '' || fileVal === null || fileVal === undefined) {
            $("#FileUploadError").text("فایلی انتخاب نشده است ");
            return;
        }
        var file = document.getElementById("FileUpload");
        var formData = new FormData();
        formData.append("UploadedImage", file.files[0]);
        var sizeOfFile = $("#FileUpload")[0].files[0].size;
        if (sizeOfFile > 20000000) {
            $("#FileUploadError").text("فایل بزرگتر از 20 مگابایت است ، فایل کوچکتری انتخاب کنید ");
            return;
        }
        var url = "/DocumentCenters/UploadFiles?type=" + fileType + '&fileId=' + fileTypeId + '&pageType=' + pageType;
        $("body").css("opacity", "0.8");
        $(".cssload-loader").css("display", "block");
        $.ajax({
            url: url,
            type: "Post",
            data: formData,
            //dataType: 'json',
            contentType: false,
            processData: false

        }).done(function (res) {
            $("body").css("opacity", "1");
            $(".cssload-loader").css("display", "none");
            if (res.success) {
                $("#Attachment").html(res.message);
                public.initPlugins();
                toastr.success("بار گذاری با موفقیت انجام گرفت",
                    'آپلود',
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

                toastr.warning(res.message,
               'آپلود',
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
    };
    return public;
})();