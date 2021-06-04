var a = null;
assert.strictEqual(undefined, a?.b);
assert.strictEqual(undefined, a?.b?.c);
a = {};
a.b = null;

assert.strictEqual(null, a?.b);
assert.strictEqual(undefined, a?.b?.c);