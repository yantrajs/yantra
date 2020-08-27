(function () {
    var a = 1;
    var b = "";
    while (a < 10) {
        a++;
        b += a;
    }
    assert(b === "", b);
})()