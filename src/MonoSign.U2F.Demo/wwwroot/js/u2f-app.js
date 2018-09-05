// We will keep this state for closed modal if someone close it before register and then tap the key.
var waitingResponse = false;
// Register Device
$("#register_device").on("click",
    function (event) {
        event.preventDefault();
        $.ajax({
            url: 'RegisterDeviceRequest',
            method: 'POST',
            error: function (result) {
                if (result.responseJSON) {
                    checkResult(result.responseJSON);
                }
            },
            success: function (result) {

                if (!result || result.error) {
                    showMessage(result.error, 3);
                    return;
                }

                var request = {"challenge": result.challenge, "version": "U2F_V2", "appId": result.appId};
                var registeredKeys = [];

                if (result.registeredKeys) {
                    $.each(result.registeredKeys,
                        function () {
                            registeredKeys.push({keyHandle: this, version: "U2F_V2"});
                        });
                }

                $("#device_name").val('');
                waitingResponse = true;
                try {
                    u2f.register(result.appId,
                        [request],
                        registeredKeys,
                        function (data) {
                            if (!waitingResponse)
                                return;

                            waitingResponse = false;
                            console.log("Register callback", data);
                            if (data.errorCode) {
                                onError(data.errorCode, true);
                                $("#register_modal").modal("hide");
                            } else {
                                var json = JSON.stringify(data);

                                var req = {
                                    appId: result.appId,
                                    challenge: result.challenge,
                                    rawRegisterResponse: json,
                                    deviceName: $("#device_name").val()
                                };

                                $.ajax({
                                    url: 'RegisterDevice',
                                    method: 'POST',
                                    data: req,
                                    json: true,
                                    success: function (result) {
                                        checkResult(result);
                                    },
                                    error: function (result) {
                                        if (result.responseJSON) {
                                            checkResult(result.responseJSON);
                                        }
                                    },
                                    complete: function () {
                                        $("#register_modal").modal("hide");
                                    }
                                });
                            }
                        });

                    $("#register_modal").modal();
                } catch (e) {
                    console.error(e);
                    waitingResponse = false;
                }
            }
        });
    });

$('#register_modal').on('hidden.bs.modal', function () {
    waitingResponse = false;
});

$('#authenticate_modal').on('hidden.bs.modal', function () {
    waitingResponse = false;
});

// Authenticate Device
$('[data-js="authenticate-device"]').on("click", function (event) {

    event.preventDefault();
    var keyHandle = $(this).data("id");
    var deviceName = $(this).data("name");
    $.ajax({
        url: 'AuthenticateDeviceRequest',
        method: 'POST',
        data: {
            keyHandle: keyHandle
        },
        json: true,
        error: function (result) {
            if (result.responseJSON) {
                checkResult(result.responseJSON);
            }
        },
        success: function (result) {

            if (!result || result.error) {
                showMessage(result.error, 3);
                return;
            }

            waitingResponse = true;
            try {
                var request = {"challenge": result.challenge, "keyHandle": result.keyHandle, "version": "U2F_V2", "appId": result.appId};
                console.log(request);

                u2f.sign(result.appId, result.challenge, [request], function (data) {
                    if (!waitingResponse)
                        return;

                    waitingResponse = false;
                    console.log("Authenticate callback", data);
                    if (data.errorCode) {
                        onError(data.errorCode);
                        $("#authenticate_modal").modal("hide");
                    } else {
                        var json = JSON.stringify(data);

                        var req = {
                            appId: result.appId,
                            challenge: result.challenge,
                            rawAuthenticateResponse: json,
                            keyHandle: request.keyHandle
                        };

                        $.ajax({
                            url: 'AuthenticateDevice',
                            method: 'POST',
                            data: req,
                            json: true,
                            success: function (result) {
                                checkResult(result);
                            },
                            error: function (result) {
                                if (result.responseJSON) {
                                    checkResult(result.responseJSON);
                                }
                            },
                            complete: function () {
                                $("#authenticate_modal").modal("hide");
                            }
                        });
                    }
                });
                $("#user_device_name").text(deviceName);
                $("#authenticate_modal").modal();
            } catch (e) {
                console.error(e);
                waitingResponse = false;
            }
        }
    });


});

// Remove Device
$("[data-js='remove-device']").on("click",
    function (event) {
        event.preventDefault();
        var identifier = $(this).data("id");

        $.ajax({
            url: 'RemoveDevice',
            method: 'POST',
            data: {
                identifier: identifier
            },
            json: true,
            success: function (result) {
                checkResult(result);
            },
            complete: function () {
                $("#register_modal").modal("hide");
            }
        });
    });

function checkResult(result) {
    if (!result) {
        showMessage("No return value from server.", 3);
        return false;
    }
    if (result.error) {
        showMessage(result.error, 3);
        return false;
    }

    if (result.message) {
        showMessage(result.message, 1);
    }

    if (result.redirect) {
        // Waiting for 1 sec to show success message then redirect.
        setTimeout(function () {
                location.href = result.redirect;
            },
            1500);
    }

    return true;
}

function onError(code, enrolling) {
    switch (code) {
        case u2f.ErrorCodes.OTHER_ERROR:
            showMessage('sign error (other)', 2);
            break;
        case u2f.ErrorCodes.BAD_REQUEST:
            showMessage('bad request', 2);
            break;
        case u2f.ErrorCodes.CONFIGURATION_UNSUPPORTED:
            showMessage('configuration unsupported', 3);
            break;
        case u2f.ErrorCodes.DEVICE_INELIGIBLE:
            if (enrolling)
                showMessage('U2F token is already registered', 2);
            else
                showMessage('U2F token is not registered', 3);
            break;
        case u2f.ErrorCodes.TIMEOUT:
            showMessage('timeout', 2);
            break;
        default:
            showMessage('unknown error code=' + code, 3);
            break;
    }
}

function showMessage(message, type) {
    var cssClass = 'alert-info';
    if (type === 1) {
        cssClass = "alert-success";
    } else if (type === 2) {
        cssClass = "alert-warning";
    } else if (type === 3) {
        cssClass = "alert-danger";
    }
    var element = $("<div class='alert " + cssClass + "'></div>");
    element.append($('<button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button>'));
    element.append(message);
    $("#messages").append(element);
}