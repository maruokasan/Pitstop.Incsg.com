// Class definition
//var KTSelect2 = function () {
//    // Private functions
//    var demos = function () {
//        $('#AccessTypeId').select2({
//            placeholder: "Select a value",
//            allowClear: true,
//            ajax: {
//                url: contentUrl + "UserMaster/AccessTypeDataList",
//                dataType: "JSON",
//                data: function (params) {
//                    return {
//                        searchTerm: params.term
//                    };
//                },
//                processResults: function (data, params) {
//                    return {
//                        results: data
//                    };
//                }
//            }
//        });
//    }
//    // Public functions
//    return {
//        init: function () {
//            demos();
//        }
//    };
//}();

// Initialization
jQuery(document).ready(function () {
    //KTSelect2.init();
    KTSelect3.init();
    KTSelect4.init();
});


var KTSelect4 = function () {
    // Private functions
    var demos = function () {

        // multi select
        $("#kt_select2_access_type_list").select2({
            placeholder: "Search Access Type",
            allowClear: true,
            ajax: {
                url: contentUrl + "UserMaster/AccessTypeDataList",
                dataType: 'json',
                delay: 250,
                data: function (params) {
                    return {
                        searchTerm: params.term,
                    };
                },
                processResults: function (data) {
                    return {
                        results: data
                    };
                },
                cache: true
            },
            escapeMarkup: function (markup) {
                return markup;
            },
            minimumInputLength: 0,
        });
    }
    // Public functions
    return {
        init: function () {
            demos();
        }
    };
}();