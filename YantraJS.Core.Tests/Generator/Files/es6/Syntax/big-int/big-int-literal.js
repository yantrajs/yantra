const a = 34343n;

const b = BigInt("34343");

assert.strictEqual(a, b);

const c = BigInt("343431n");

assert.notStrictEqual(a, c);
