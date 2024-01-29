$(function () {
    function moveItems(origin, dest) {
        $(origin).find(':selected').appendTo(dest);
    }

    function moveAllItems(origin, dest) {
        $(origin).children().appendTo(dest);
    }

    $('#left').click(function () {
        moveItems('#multiple-right', '#multiple-left');
    });

    $('#right').on('click', function () {
        moveItems('#multiple-left', '#multiple-right');
    });

    $('#leftall').on('click', function () {
        moveAllItems('#multiple-right', '#multiple-left');
    });

    $('#rightall').on('click', function () {
        moveAllItems('#multiple-left', '#multiple-right');
    }); 

    $("#save-assign").click(function () {
        $("select > option").prop("selected", "selected");
        var valdata = $("#kt_assign_form").serialize();

        var formData = new FormData();

        var appsValues = [];
        $("#multiple-right").val().forEach((res) => {
            appsValues.push(res);
        })

        formData.append("param.Id", $("#Id").val());
        formData.append("param.SystemPermissionIds", appsValues);

        $.ajax({
            url: contentUrl + "UserMaster/AssignRole",
            type: "POST",
            data: formData,
            dataType: "JSON",
            contentType: false,
            data: formData,
            processData: false,
            success: function (result) {
                Swal.fire({
                    icon: 'success',
                    title: result.message,
                    showConfirmButton: true
                }).then(function () {
                    $('.custom-loader-wrapper').css('display', 'flex');
                    window.location = contentUrl + "UserMaster/RoleAssign/" + $("#Id").val();
                });
            },
            error: function (request, status, error) {
                Swal.fire({
                    icon: 'error',
                    title: result.responseJSON.message,
                    showConfirmButton: true
                });
            }
        });
    });
});