﻿$(document).ready(function () {
//var srcName = $("#sig").attr("src");
//function refreshSig() {
//    $("#sig").attr("src", srcName + '?' + Math.random());
//    console.log("refreshing");
//}

function refreshSig() {
    $("#sig").attr("src", $("#sig").attr("src") + '?' + Math.random());
    console.log("refreshing");
}

var intervalID = setInterval(refreshSig, 1000);

$("#connectButton").click(function () {
   
});

$("#disconnectButton").click(function () {
    clearInterval(intervalID);
    });
});