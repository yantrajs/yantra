var a = [1];
var p = Object.getPrototypeOf(a);
assert.strictEqual(p, Array.prototype);
