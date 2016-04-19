!function(){"use strict";function t(t,n){var s=this;s.tripName=t.tripName,s.stops=[],s.errorMessage="",s.isBusy=!0,s.newStop={};var a="/api/trips/"+s.tripName+"/stops";n.get(a).then(function(t){angular.copy(t.data,s.stops),o(s.stops)},function(t){s.errorMessage="Failed to load stops"})["finally"](function(){s.isBusy=!1}),s.addStop=function(){s.isBusy=!0,n.post(a,s.newStop).then(function(t){s.stops.push(t.data),o(s.stops),s.newStop={}},function(t){s.errorMessage="Failed to add new stop"})["finally"](function(){s.isBusy=!1})}}function o(t){if(t&&t.length>0){var o=_.map(t,function(t){return{lat:t.latitude,"long":t.longitude,info:t.name}});travelMap.createMap({stops:o,selector:"#map",currentStop:1,initialZoom:3})}}angular.module("app-trips").controller("tripEditorController",t)}();