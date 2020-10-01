let a = Date.UTC(96, 1, 2, 3, 4, 5);

assert.strictEqual(a, 823230245000);

a = Date.UTC(1996, 1, 2, 3, 4, 5);

assert.strictEqual(a, 823230245000);
