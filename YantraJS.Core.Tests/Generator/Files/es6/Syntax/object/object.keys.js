var result = Object.keys({ 100: 'a', 2: 'b', 7: 'c' });
assert.strictEqual(result.toString(), "2,7,100");
