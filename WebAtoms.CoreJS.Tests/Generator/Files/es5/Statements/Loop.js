(function () {
    var a = 1;
    var b = "";
    while (a < 10) {
        a++;
        b += a;
    }
    assert.strictEqual("2345678910", b);
})();
(function () {
    var a = 1;
    var b = "";
    while (a <= 10) {
        b += a;
        a++;
    }
    assert.strictEqual("12345678910", b);
})()