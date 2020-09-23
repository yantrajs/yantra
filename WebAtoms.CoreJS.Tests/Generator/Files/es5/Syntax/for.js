function a(n) {
    var s = "";
    for (var i = 0; i < n; i++) {
        s += i;
    }
    return s;
}

assert.strictEqual("012", a(3));