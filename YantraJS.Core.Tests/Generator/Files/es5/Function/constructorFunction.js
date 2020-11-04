var fx = new Function("a", "b", "return a + b;");
assert.strictEqual(fx(1, 2), 3);
