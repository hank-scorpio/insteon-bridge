'use strict';
/* App Module */

var LuxApp = angular.module('LuxApp', ['ngResource', 'ngMaterial']);

LuxApp.factory('LuxApi', ['$resource',
    function ($resource)
    {
        return $resource('./api/lights/:id/:level', {},
        {
            GetLights: { method: 'GET', isArray: true },
            GetLight: { method: 'GET', params: { id: '@id' }, isArray: false },
            SetLevel: { method: 'GET', params: { id: '@id', level: '@level' }, isArray: false }
        });
    }
]);

LuxApp.controller('HouseController',
    function ($scope, LuxApi)
    {
        $scope.Lights = {};

        $scope.Update = function ()
        {
            LuxApi.GetLights({}, function (lights, resp)
            {
                for (let l of lights) $scope.Lights[l.Id] = l;
            });
        };
        /*
        $scope.GetLightById = function (id)
        {
            for (var i = 0; i < $scope.Lights.length; i++)
            {
                if ($scope.Lights[i].Id === id)
                {
                    return $scope.Lights[i];
                }
            }
        };
        */
        $scope.SetLevel = function(id, level) 
        {

            $scope.Lights[id].Loading = true;
            
            LuxApi.SetLevel({ id: id, level: '~' + level }, function (data, resp)
            {
                var l = $scope.Lights[id];
                l.Level = data.Level;
                l.Loading = false;
            });
        };
        $scope.Update();
    }
)
.config(function($mdThemingProvider) {
    $mdThemingProvider.theme('dark-grey').backgroundPalette('grey').dark();
    $mdThemingProvider.theme('dark-orange').backgroundPalette('orange').dark();
    $mdThemingProvider.theme('dark-purple').backgroundPalette('deep-purple').dark();
    $mdThemingProvider.theme('dark-blue').backgroundPalette('blue').dark();
});