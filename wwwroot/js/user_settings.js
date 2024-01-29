
function Submit(submitType) {
    Swal.fire({
        title: 'Do you want to save the changes?',
        text: "You can change anytime later on!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonText: 'Yes, change it!'
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {

            var formData = new FormData();

            formData.append("param.ChangePassword.Current", $('#Current').val());
            formData.append("param.ChangePassword.NewPassword", $('#NewPassword').val());
            formData.append("param.ChangePassword.ConfirmNewPassword", $('#ConfirmNewPassword').val());
            formData.append("param.ChangeMobileNumber.MobileNumber", $('#MobileNumber').val());

            formData.append("SubmitType", submitType);

            $.ajax({
                type: "POST",
                url: contentUrl + "Home/Settings",
                dataType: "JSON",
                contentType: false,
                data: formData,
                processData: false,
                success: function (result) {
                    Swal.fire({
                        position: 'center',
                        icon: 'success',
                        title: result.message,
                        showConfirmButton: false,
                        timer: 1500
                    }).then(function () {
                        $('.custom-loader-wrapper').css('display', 'flex');
                        window.location = contentUrl + "Home/Settings";
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