var a = {
    a: 1, get b() { return this.a; } };
var c = { ...a, a: 2 };
assert.strictEqual('{"a":2,"b":1}', JSON.stringify(c));
a.a = 2;
assert.strictEqual('{"a":2,"b":2}', JSON.stringify(a));

const symbol = Symbol("aa");

a = { [symbol]: 1 };
c = { ...a };
assert.strictEqual(c[symbol], a[symbol]);