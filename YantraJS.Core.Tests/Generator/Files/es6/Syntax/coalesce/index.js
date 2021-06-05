var a = null;

var b = 1;

function add() {
    b++;
    return b;
}
var n = a?.[add()];
assert.strictEqual(undefined, n);
assert.strictEqual(1, b);