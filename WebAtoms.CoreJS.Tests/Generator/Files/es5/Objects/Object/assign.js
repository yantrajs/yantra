var target = { a: 1, b: 2 };
var source = { b: 4, c: 5 };

var returnedTarget = Object.assign(target, source);

assert.strictEqual(returnedTarget.a, 1);
assert.strictEqual(returnedTarget.b, 4);
assert.strictEqual(returnedTarget.c, 5);
