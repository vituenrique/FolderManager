var app = angular.module('App', ['ui.select', 'ngSanitize']);

app.config(['$locationProvider', function ($locationProvider) {
    $locationProvider.html5Mode({
        enabled: true,
        requireBase: false
    });
}]);

app.directive('repeatDone', function() {
    return function(scope, element, attrs) {
        element.bind('$destroy', function(event) {
            if (scope.$last) {
                scope.$eval(attrs.repeatDone);
            }
        });
    }
});

app.service('Global', function ($http) {

    this.SendNotification = function (msg, type) {
        var typeclass;
        switch (type) {
            case "warning":
                return alertify.warning(msg).delay(20);
            case "danger":
                return alertify.error(msg).delay(20);
            case "success":
                return alertify.success(msg).delay(20);
            default:
                return alertify.message(msg).delay(20);
        }
    }

    this.Requisition = function (requisition, data, callback) {
        var requisition = {
            method: 'POST',
            url: this.GetHost() + requisition,
            headers: {
                "Content-Type": "application/json"
            },
            dataType: 'json',
            data: data
        }
        $http(requisition).then(callback);
    }

    this.GetHost = function () {
        var string = "";
        if (!window.location.host.toLowerCase().includes("localhost")) string = "foldermanager.azurewebsites.net/"
        return string;
    }

    this.OpenModal = function (modalID) {
        console.log(modalID)
        $('#' + modalID).modal('show');
    }

});