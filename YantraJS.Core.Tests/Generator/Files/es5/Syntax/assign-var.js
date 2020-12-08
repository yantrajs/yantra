a = {};
var c = a.b = function () {
    return 5;
}

assert.strictEqual(5, a.b());