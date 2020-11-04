var a = "akash";
assert.strictEqual(1, a.indexOf("k"));
assert.strictEqual(2, a.indexOf("a", 2));

assert.strictEqual(-1, a.indexOf(null));