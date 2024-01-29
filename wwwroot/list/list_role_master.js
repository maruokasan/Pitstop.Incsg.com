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
                        url: 'GetDataRoleMaster',
                    },
                },
                pageSize: 15, // display 5 records per page
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
                    return data.name;
                }
            }, {
                field: 'Action',
                title: 'Action',
                sortable: 'asc',
                selector: false,
                textAlign: 'left',
                template: function (data) {
                    //return '<button href="#">Edit</button> | <button href="#">Delete</button>';
                    let id = data.roleId;
                    return '<a href="/UserMaster/RoleAssign/' + id + '" class="btn-success btn btn-light-success font-weight-bold py-2 px-5 ml-2">Assign</a>\
                    <a href="javascript:deleteRecord(\'' + id + '\');" class="btn-delete btn btn-light-primary font-weight-bold py-2 px-5 ml-2">Delete</a>';               
                }
            }],
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
