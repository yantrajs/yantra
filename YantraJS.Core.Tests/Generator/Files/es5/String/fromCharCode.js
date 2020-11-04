var a = String.fromCharCode(189, 43, 190, 61);
assert.strictEqual(a, "½+¾=");

assert.strictEqual(String.fromCharCode(0xD83C, 0xDF03), "🌃");