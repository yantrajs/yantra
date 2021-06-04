var a = null;

assert.strictEqual(null, a ?? null);
assert.strictEqual(1, a ?? 1);

a = "";
assert.strictEqual("", a ?? "a");