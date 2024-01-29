
function Save() {
    Swal.fire({
        title: 'Do you want to save the changes?',
        showCancelButton: true,
        icon: "info",
        confirmButtonText: 'Save'
    }).then((result) => {
        /* Read more about isConfirmed, isDenied below */
        if (result.isConfirmed) {

            var formData = new FormData();

            formData.append("param.NumberOfDaysDeactiveAccount", $('#NumberOfDaysDeactiveAccount').val());

            $.ajax({
                type: "POST",
                url: contentUrl + "UserMaster/AccountSettings",
                dataType: "JSON",
                contentType: false,
                data: formData,
                processData: false,
                success: function (result) {
                    Swal.fire({
                        position: 'top-end',
                        icon: 'success',
                        title: result.message,
                        showConfirmButton: false,
                        timer: 1500
                    }).then(function () {
                        $('.custom-loader-wrapper').css('display', 'flex');
                        window.location = contentUrl + "UserMaster/AccountSettings";
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