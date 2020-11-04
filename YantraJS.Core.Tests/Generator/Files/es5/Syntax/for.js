function a(n) {
    var s = "";
    for (var i = 0; i < n; i++) {
        s += i;
    }
    return s;
}

assert.strictEqual("012", a(3));

// let...
(function () {

    var b = [];
    for (var i = 0; i < 3; i++) {
        b.push(() => i);
    }
    b = b.map((n) => n());
    assert.strictEqual("3,3,3", b.toString());
    b = [];
    for (let i = 0; i < 3; i++) {
        b.push(() => i);
    }
    b = b.map((n) => n());
    assert.strictEqual("0,1,2", b.toString());
})();
var b = [];
for (let i = 0; i < 3; i++) {
    b.push(() => i);
}