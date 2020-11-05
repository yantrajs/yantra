var ts;
(function (ts) {
    ts.a = 1;
})(ts || (ts = {}));
var ts;
(function (ts) {
    ts.b = 2;
})(ts || (ts = {}));
assert.strictEqual(1, ts.a);
assert.strictEqual(2, ts.b);