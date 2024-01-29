'use strict';
// Class definition

var KTDatatableDataLocalDemo = function () {
    // Private functions

    // demo initializer
    var demo = function () {
        var datatable = $('#kt_datatable').KTDatatable({
            // datasource definition
            data: {
                type: 'remote',

                source: {
                    read: {
                        url: contentUrl + 'UserMaster/GetDataUserMaster',
                    },
                },
                pageSize: 15, // display 15 records per page
                serverPaging: true,
                serverFiltering: true,
                serverSorting: true,
            },

            // layout definition
            layout: {
                scroll: false, // enable/disable datatable scroll both horizontal and vertical when needed.
                // height: 450, // datatable's body's fixed height
                footer: false, // display/hide footer
            },

            // column sorting
            sortable: true,

            pagination: true,

            search: {
                input: $('#kt_datatable_search_query'),
                delay: 400,
                key: 'generalSearch',
            },

            // columns definition
            columns: [{
                field: 'Name',
                title: 'Name',
                sortable: 'asc',
                selector: false,
                textAlign: 'left',
                template: function (data) {
                    //return '<a  href="' + contentUrl + 'User/Detail/' + data.userId + '">' + data.name + '</a >';
                    return '<a class="modal-user-master-data" data-toggle="modal" data-target="" data-id="' + data.userId +'" href="#exampleModal">' + data.name + '</a >';
                }
            }, {
                field: 'Email',
                title: 'Email',
                width:200,
                sortable: 'asc',
                selector: false,
                textAlign: 'left',
                template: function (data) {
                    return data.email;
                },
            }, {
                field: 'Status',
                title: 'Status',
                sortable: 'asc',
                selector: false,
                textAlign: 'left',
                template: function (data) {
                    return data.status;
                }
            }, {
                field: 'Access',
                title: 'Access',
                sortable: 'asc',
                selector: false,
                textAlign: 'left',
                template: function (data) {
                    return data.access;
                }
            }, {
                field: 'Role',
                title: 'Role',
                sortable: 'asc',
                selector: false,
                textAlign: 'left',
                template: function (data) {
                    return data.role;
                },
            }, {
                field: 'CreatedBy',
                title: 'Created By',
                sortable: 'asc',
                selector: false,
                textAlign: 'left',
                template: function (data) {
                    return data.createdBy;
                    },
            }, {
                field: 'CreatedDate',
                title: 'Created Date',
                sortable: 'asc',
                selector: false,
                textAlign: 'left',
                template: function (data) {
                    return data.createdDate;
                }
            }],
        });


        $(document).on("click", ".modal-user-master-data", function () {

            $('#UserId').val('');
            $('#FullName').val('');
            $('#EmailId').val('');
            $('#MobileNumber').val('');
            $('#IsActive').prop('checked', false);
            //$("#AccessTypeId").val(null).trigger("change");

            $('#IsActiveDiv').show();
            $('#ResetPasswordDiv').show();

            $('#exampleModalLabel').text("Update User");

            $.each($("input[name='ApplicationNames']"), function () {
                $(this).prop('checked', false);
            });

            $('#kt_select2_jobcode_list').empty();
            $('#kt_select2_access_type_list').empty();

            var userId = $(this).data('id');
            $.ajax({
                url: contentUrl + "UserMaster/GetUserData",
                contentType: "application/json;charset=utf-8",
                dataType: "JSON",
                data: {
                    userId: userId
                },
                success: function (result) {
                    $('#UserName').val(result.data.userName);
                    $('#UserId').val(result.data.userId);
                    $('#FullName').val(result.data.fullName);
                    $('#EmailId').val(result.data.emailId);
                    $('#MobileNumber').val(result.data.mobileNumber);
                    $('#IsActive').prop('checked', result.data.isActive);

                    //var value = result.data.accessTypeId;
                    //var name = result.data.accessTypeName;
                    //setSelect2Value(value, name, '#AccessTypeId');

                    var buttonSaveUpdate = $('#btn-save-update-user');
                    buttonSaveUpdate.text("Update");
                    buttonSaveUpdate.on('click', function () {
                        CreateUpdateUser('Update', 'Update');
                    });

                    var buttonResetPassword = $('#btn-reset-password');
                    buttonResetPassword.val("Reset Password");
                    buttonResetPassword.on('click', function () {
                        ResetPassword();
                    });

                    $.each($("input[name='ApplicationNames']"), function () {
                        var chk = $(this);
                        if (jQuery.inArray(chk.val(), result.data.userSystemApps) > -1) {
                            chk.prop('checked', true);
                        }
                    });
                    
                    result.data.jobcodeTypeList.forEach((item) => {
                        setSelect2Value(item.id, item.code, "#kt_select2_jobcode_list");
                    });
                              
                    result.data.accessTypeList.forEach((item) => {
                        setSelect2Value(item.id, item.name, "#kt_select2_access_type_list");
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

        });

        function setSelect2Value(value, name, id) {
            var $option = $("<option selected></option>").val(value).text(name);
            $(id).append($option).trigger('change');
        }

        $('#kt_datatable_search_status').on('change', function () {
            datatable.search($(this).val(), 'Status');
        });

        $('#kt_datatable_search_role').on('change', function () {
            datatable.search($(this).val(), 'Role');
        });
    };

    return {
        // Public functions
        init: function () {
            // init dmeo
            demo();
        },
    };
}();

jQuery(document).ready(function () {
    KTDatatableDataLocalDemo.init();
});
