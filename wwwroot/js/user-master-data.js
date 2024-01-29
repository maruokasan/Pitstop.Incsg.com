function CreateUpdateUser(submitType, submitConfirmButtonText) {
    Swal.fire({
        title: 'Do you want to ' + submitConfirmButtonText + ' the changes?',
        showCancelButton: true,
        icon: "info",
        confirmButtonText: submitConfirmButtonText
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {

            var formData = new FormData();

            formData.append("param.UserId", $('#UserId').val());
            formData.append("param.FullName", $('#FullName').val());
            formData.append("param.UserName", $('#UserName').val());
            formData.append("param.EmailId", $('#EmailId').val());
            formData.append("param.MobileNumber", $('#MobileNumber').val());
            //formData.append("param.AccessTypeId", $('#AccessTypeId').val());
            formData.append("param.IsActive", $('#IsActive').is(':checked'));

            var appsValues = [];
            $.each($("input[name='ApplicationNames']:checked"), function () {
                appsValues.push($(this).val());
            });

            formData.append("param.ApplicationNames", appsValues);

            var jobcodeTypeValues = $('#kt_select2_jobcode_list').val();
var accessTypeValues = $('#kt_select2_access_type_list').val();

formData.append("jobcodeTypeValues", jobcodeTypeValues);
formData.append("accessTypeValues", accessTypeValues);


            formData.append("SubmitType", submitType);

            $.ajax({
                type: "POST",
                url: contentUrl + "UserMaster/CreateUpdateUser",
                dataType: "JSON",
                contentType: false, // Set content type to false for FormData
                processData: false, // Set processData to false for FormData
                data: formData, // Use FormData directly
                success: function (result) {
                    Swal.fire({
                        icon: 'success',
                        title: result.message,
                        showConfirmButton: true
                    }).then(function () {
                        $('.custom-loader-wrapper').css('display', 'flex');
                        window.location = contentUrl + "UserMaster/UserMaster";
                    });
                },
                error: function (result) {
                    Swal.fire({
                        icon: 'error',
                        title: result.responseJSON.message,
                        showConfirmButton: true
                    });
                }
            });
        } else if (result.isDenied) {
            Swal.fire('Changes are not saved', '', 'info')
        }
    })
}


function setSelect2Value(value, name, id) {
    var $option = $("<option selected></option>").val(value).text(name);
    $(id).append($option).trigger('change');
}

$(document).on("click", ".modal-user-master-data-create", function () {

    $('#UserId').val('');
    $('#FullName').val('');
    $('#UserName').val('');
    $('#EmailId').val('');
    $('#MobileNumber').val('');
    $('#IsActive').prop('checked', false);
    //$("#AccessTypeId").val(null).trigger("change");

    $('#IsActiveDiv').hide();
    $('#ResetPasswordDiv').hide();

    $('#exampleModalLabel').text("Create User");

    var buttonSaveUpdate = $('#btn-save-update-user');
    buttonSaveUpdate.text("Create User");
    buttonSaveUpdate.on('click', function () {
        CreateUpdateUser('Save', 'Save');
    });

    $.each($("input[name='ApplicationNames']"), function () {
        $(this).prop('checked', false);
    });

    $('#kt_select2_jobcode_list').empty();
    $('#kt_select2_access_type_list').empty();
});



function ResetPassword() {
    var userId = $('#UserId').val();
    Swal.fire({
        title: 'Do you want to reset this user password?',
        showCancelButton: true,
        icon: "warning",
        confirmButtonText: 'Reset'
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {
            $.ajax({
                url: contentUrl + "UserMaster/ResetUserPassword",
                contentType: "application/json;charset=utf-8",
                dataType: "JSON",
                data: { userId: userId },
                success: function (result) {
                    Swal.fire({
                        icon: 'success',
                        title: result.message,
                        showConfirmButton: true
                    }).then(function () {
                        $('.custom-loader-wrapper').css('display', 'flex');
                        window.location = contentUrl + "UserMaster/UserMaster";
                    });
                },
                error: function (result) {
                    Swal.fire({
                        icon: 'error',
                        title: result.responseJSON.message,
                        showConfirmButton: true
                    });
                }
            });
        } else if (result.isDenied) {
            Swal.fire('Changes are not saved', '', 'info')
        }
    })
}