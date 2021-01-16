var a = Function("return this")();
assert.strictEqual(a, undefined);