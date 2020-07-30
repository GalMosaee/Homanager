// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

    var c = document.getElementById('c'),
        ctx = c.getContext('2d'),
        cw = c.width = window.innerWidth,
        ch = c.height = window.innerHeight / 4,
        points = [],
        tick = 0,
            opt = {
        count: 5,
                range: {
        x: 20,
    y: 80
},
                duration: {
        min: 20,
    max: 40
},
thickness: 10,
strokeColor: '#444',
level: .35,
curved: true
},
            rand = function (min, max) {
                return Math.floor((Math.random() * (max - min + 1)) + min);
},
            ease = function (t, b, c, d) {
                if ((t /= d / 2) < 1) return c / 2 * t * t + b;
    return -c / 2 * ((--t) * (t - 2) - 1) + b;
};

ctx.lineJoin = 'round';
ctx.lineWidth = opt.thickness;
ctx.strokeStyle = opt.strokeColor;

        var Point = function (config) {
        this.anchorX = config.x;
    this.anchorY = config.y;
    this.x = config.x;
    this.y = config.y;
    this.setTarget();
};

        Point.prototype.setTarget = function () {
        this.initialX = this.x;
    this.initialY = this.y;
    this.targetX = this.anchorX + rand(0, opt.range.x * 2) - opt.range.x;
    this.targetY = this.anchorY + rand(0, opt.range.y * 2) - opt.range.y;
    this.tick = 0;
    this.duration = rand(opt.duration.min, opt.duration.max);
}

        Point.prototype.update = function () {
            var dx = this.targetX - this.x;
    var dy = this.targetY - this.y;
    var dist = Math.sqrt(dx * dx + dy * dy);

            if (Math.abs(dist) <= 0) {
        this.setTarget();
    } else {
                var t = this.tick;
    var b = this.initialY;
    var c = this.targetY - this.initialY;
    var d = this.duration;
    this.y = ease(t, b, c, d);

    b = this.initialX;
    c = this.targetX - this.initialX;
    d = this.duration;
    this.x = ease(t, b, c, d);

    this.tick++;
}
};

        Point.prototype.render = function () {
        ctx.beginPath();
    ctx.arc(this.x, this.y, 3, 0, Math.PI * 2, false);
    ctx.fillStyle = '#000';
    ctx.fill();
};

        var updatePoints = function () {
            var i = points.length;
            while (i--) {
        points[i].update();
    }
};

        var renderPoints = function () {
            var i = points.length;
            while (i--) {
        points[i].render();
    }
};

        var renderShape = function () {
        ctx.beginPath();
    var pointCount = points.length;
    ctx.moveTo(points[0].x, points[0].y);
    var i;
            for (i = 0; i < pointCount - 1; i++) {
                var c = (points[i].x + points[i + 1].x) / 2;
    var d = (points[i].y + points[i + 1].y) / 2;
    ctx.quadraticCurveTo(points[i].x, points[i].y, c, d);
}
ctx.lineTo(-opt.range.x - opt.thickness, ch + opt.thickness);
ctx.lineTo(cw + opt.range.x + opt.thickness, ch + opt.thickness);
ctx.closePath();
ctx.fillStyle = 'hsl(' + (tick / 2) + ', 80%, 60%)';
ctx.fill();
ctx.stroke();
};

        var clear = function () {
        ctx.clearRect(0, 0, cw, ch);
    };

        var loop = function () {
        window.requestAnimFrame(loop, c);
    tick++;
    clear();
    updatePoints();
    renderShape();
    //renderPoints();
};

var i = opt.count + 2;
var spacing = (cw + (opt.range.x * 2)) / (opt.count - 1);
        while (i--) {
        points.push(new Point({
            x: (spacing * (i - 1)) - opt.range.x,
            y: ch - (ch * opt.level)
        }));
    }

        window.requestAnimFrame = function () { return window.requestAnimationFrame || window.webkitRequestAnimationFrame || window.mozRequestAnimationFrame || window.oRequestAnimationFrame || window.msRequestAnimationFrame || function (a) {window.setTimeout(a, 1E3 / 60)} }();

loop();


var _lat = null;
var _long = null;
function gettingJSON() {
    if (!navigator.geolocation) {
        getDefaultWeather();
        return;
    }
    function success(position) {
        _lat = position.coords.latitude;
        _long = position.coords.longitude;
        $.getJSON("https://api.openweathermap.org/data/2.5/weather?lat=" + _lat + "&lon=" + _long + "&APPID=fad61ab6ac8274b09f695e38255e9c20&units=metric", function (data) {
            var city = data.name.toString();
            var country = data.sys.country.toString();
            var temp = parseInt(data.main.temp.toString());
            var status = data.weather[0].main.toString();
            var icon = data.weather[0].icon.toString();
            $("#weatherIcon").html("<img src='http://openweathermap.org/img/w/" + icon + ".png' alt='Icon depicting current weather.'>");
            document.getElementById("weather").innerHTML = city + "," + country + "<br>" + temp + '&#8451' + ", " + status;
        });
    }
    function error() {
        getDefaultWeather();
    }
    navigator.geolocation.getCurrentPosition(success, error);
}

function getDefaultWeather() {
    $.getJSON("http://api.openweathermap.org/data/2.5/weather?q=Tel Aviv,IL&APPID=fad61ab6ac8274b09f695e38255e9c20&units=metric", function (data) {
        var city = data.name.toString();
        var country = data.sys.country.toString();
        var temp = parseInt(data.main.temp.toString());
        var status = data.weather[0].main.toString();
        var icon = data.weather[0].icon.toString();
        _lat = data.coord.lat;
        _long = data.coord.lon;
        $("#weatherIcon").html("<img src='http://openweathermap.org/img/w/" + icon + ".png' alt='Icon depicting current weather.'>");
        document.getElementById("weather").innerHTML = city + "," + country + "<br>" + temp + '&#8451' + ", " + status;
    });
}

//function initAutocomplete() {
    // This example requires the Places library. Include the libraries=places
    // parameter when you first load the API. For example:
    // <script src="https://maps.googleapis.com/maps/api/js?key=YOUR_API_KEY&libraries=places">

var map;
var infowindow;

function initAutocomplete1() {
    if (_long == null)
        _long = 34.84;
    if (_lat == null)
        _lat = 32.08;
    var israel = { lat: _lat, lng: _long };

    map = new google.maps.Map(document.getElementById('map'), {
        center: israel,
        zoom: 12.2
    });

    infowindow = new google.maps.InfoWindow();
    var service = new google.maps.places.PlacesService(map);
    service.nearbySearch({
        location: israel,
        radius: 9000,
        type: ['supermarket']
    }, callback);
}

function initAutocomplete() {
    if (_long == null)
        _long = 34.84;
    if (_lat == null)
        _lat = 32.08;
    var israel = { lat: _lat, lng: _long };

    map = new google.maps.Map(document.getElementById('map'), {
        center: israel,
        zoom: 12.2
    });

    infowindow = new google.maps.InfoWindow();
    console.log("ourData");
    console.log(ourData);
    var markets = ourData.map(supermarket => {
        return new google.maps.Marker({
            position: { lat: supermarket.latitude, lng: supermarket.longitude },
            map: map,
            icon: { url: "http://maps.google.com/mapfiles/ms/icons/blue-dot.png" },
            title: supermarket.title
        });
    });
};

function callback(results, status) {
    if (status === google.maps.places.PlacesServiceStatus.OK) {
        console.log(results)
        for (var i = 0; i < results.length; i++) {
            createMarker(results[i]);
        }
    }
}

function createMarker(place) {
    var placeLoc = place.geometry.location;
    var marker = new google.maps.Marker({
        map: map,
        position: place.geometry.location
    });

    google.maps.event.addListener(marker, 'click', function () {
        infowindow.setContent(place.name);
        infowindow.open(map, this);
    });
}

function fb_publish() {
    FB.ui(
        {
            method: 'stream.publish',
            message: 'Message here.',
            attachment: {
                name: 'Hey',
                caption: 'Homemanger Post.',
                description: (
                    'description here'
                ),
                href: 'Homanager'
            },
            action_links: [
                { text: 'Code', href: 'action url here' }
            ],
            user_prompt_message: 'Personal message here'
        },
        function (response) {
            if (response && response.post_id) {
                alert('Post was published.');
            } else {
                alert('Post was not published.');
            }
        }
    );
}
