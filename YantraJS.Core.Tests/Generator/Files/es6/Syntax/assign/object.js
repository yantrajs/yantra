var a = { a: 1 };
var b = { b: 2 };
var c = { ...a, ...b };
assert.strictEqual('{"a":1,"b":2}', JSON.stringify(c));

a = [1, 2];
b = [3, 4];
c = [...a, ...b];
assert.strictEqual('1,2,3,4', c.toString());