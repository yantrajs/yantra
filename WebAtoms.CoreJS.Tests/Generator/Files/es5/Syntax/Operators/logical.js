assert(false === false);

assert((false && {}) === false);

assert((0 && {}) === 0);

assert.strictEqual(typeof (0 || {}), "object");

assert.strictEqual(typeof (1 && {}), "object");

assert.strictEqual((1 && null), null);

assert.strictEqual((1 || null), 1);