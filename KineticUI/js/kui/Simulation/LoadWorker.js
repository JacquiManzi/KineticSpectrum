self.onmessage = function (e) {
    var data = e.data;
    var url = getUrl(data);
    getRange(data.start, data.end, url);
};
function getRange(start, end, url) {
    var xhr = {};
    try {
        xhr = new XMLHttpRequest();
        var params = "?start=" + (start * 1000) + "&end=" + (end * 1000);
        xhr.open('GET', url+params, true);
        xhr.setRequestHeader('Content-Type', 'application/x-www-form-urlencoded');
        xhr.onreadystatechange = function () {
            if (xhr.readyState == 4 && xhr.status == 200) {
                var result = JSON.parse(xhr.responseText);
                formatData(result);
                self.postMessage({done:true});
                self.close();
            }
        };
        xhr.send();
    } catch (e) {
        throw Error( 'Error occured in XMLHttpRequest: ' + xhr.statusText + '  ReadyState: ' + xhr.readyState + ' Status:' + xhr.status + ' E: ' + e + ' Msg:' + e.message );
    }
}

function getUrl(data) {
    var page = data.url;
    var index = page.indexOf('/', 9);

    return page.substring(0, index + 1) + "PatternService.svc/GetRange";
}

function formatData(lightStates) {
    var time;
    var times = [];
    var states = [];
    for (var i = 0; i < lightStates.length; i++) {
        var lights = lightStates[i];
        time = lights[0] / 1000.0;
        var state = [];
        for (var j = 1; j < lights.length;) {
            state.push({
                address: {
                    fixtureNo: lights[j],
                    portNo: lights[j + 1],
                    lightNo: lights[j + 2]
                },
                color: lights[j + 3]
            });
            j = j + 4;
        }
        self.postMessage({ time: time, state: state });
    }
}