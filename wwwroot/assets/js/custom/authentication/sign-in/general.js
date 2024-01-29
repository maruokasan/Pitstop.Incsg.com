"use strict";

var KTSigninGeneral = function() {
    var _signin;
    var _form;
    var _submitButton;
    var _validation;
    

    var _initForm = function() {
        _form = document.querySelector('#kt_sign_in_form');
        _submitButton = document.querySelector('#kt_sign_in_submit');

        _validation = FormValidation.formValidation(
            _form, {
                fields: {
                    username: {
                        validators: {
                            regexp: {
                                regexp: /^[^\s@]+@[^\s@]+\.[^\s@]+$/,
                                message: "The value is not a valid email address"
                            },
                            notEmpty: {
                                message: "Email address is required"
                            }
                        }
                    },
                    password: {
                        validators: {
                            notEmpty: {
                                message: "The password is required"
                            }
                        }
                    }
                },
                plugins: {
                    trigger: new FormValidation.plugins.Trigger(),
                    bootstrap: new FormValidation.plugins.Bootstrap5({
                        rowSelector: ".fv-row",
                        eleInvalidClass: "",
                        eleValidClass: ""
                    })
                }
            }
        );

        _submitButton.addEventListener('click', function(e) {
            e.preventDefault();
        
            _validation.validate().then(function(status) {
                if (status == 'Valid') {
                    _submitButton.setAttribute('data-kt-indicator', 'on');
                    _submitButton.disabled = true;
        
                    var formData = new FormData(_form);
                    formData.append("param.Username", document.querySelector('#username').value);
                    formData.append("param.Password", document.querySelector('#password').value);
                    formData.append("param.RememberMe", document.querySelector('#remember').checked); // Assuming it's a checkbox
                    formData.append("param.DateNow", new Date().toISOString());
        
                    axios.post('/Auth/Login', formData)
                        .then(function(response) {
                            // Assuming your server responds with a URL to redirect to on successful login
                            if (response.data.redirectUrl) {
                                window.location.href = response.data.redirectUrl;
                            } else {
                                // If no redirect URL is provided, default to a hard-coded path
                                window.location.href = '/Dashboard/Index';
                            }
                            _handleSignInSuccess(response);
                        })
                        .catch(function(error) {
                            // Handle error
                            var errorMessage = error.response && error.response.data ? error.response.data.message : "An error occurred. Please try again.";
                            Swal.fire({
                                text: errorMessage,
                                icon: "error",
                                buttonsStyling: false,
                                confirmButtonText: "Ok, got it!",
                                customClass: {
                                    confirmButton: "btn font-weight-bold btn-light-primary"
                                }
                            });
                            _handleSignInError(error);
                        })
                        .then(function() {
                            // Always executed
                            _submitButton.removeAttribute('data-kt-indicator');
                            _submitButton.disabled = false;
                        });
                    }
                    else{
                        _handleValidationError();
                }
            });
        });
    }
    
        

    var _handleSignInSuccess = function(response) {
        Swal.fire({
            text: "You have successfully logged in!",
            icon: "success",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-light-primary"
            }
        }).then(function() {
            var redirectUrl = _form.getAttribute('data-kt-redirect-url');
            if (redirectUrl) {
                window.location.href = redirectUrl;
            }
        });
    };

    var _handleSignInError = function(error) {
        var errorMessage = "Incorrect details. Please try again."; // Default error message
        if (error.response && error.response.data && error.response.data.message) {
            errorMessage = error.response.data.message; // Use the error message from the response
        }
    
        Swal.fire({
            text: errorMessage,
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-light-primary"
            }
        });
    };
    

    var _handleValidationError = function() {
        console.log("Reached _handleValidationError");
        Swal.fire({
            text: "Sorry, looks like there are some errors detected, please try again.",
            icon: "error",
            buttonsStyling: false,
            confirmButtonText: "Ok, got it!",
            customClass: {
                confirmButton: "btn font-weight-bold btn-light-primary"
            }
        });
    };
    

    // Public Functions
    return {
        init: function() {
            _signin = $('#kt_sign_in');
            _initForm();
        }
    };
}();

jQuery(document).ready(function() {
    KTSigninGeneral.init();
});
