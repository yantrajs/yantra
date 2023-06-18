const a = 34343n;

const b = BigInt("34343");

assert.strictEqual(a, b);

const c = BigInt("343431n");

assert.notStrictEqual(a, c);

assert.strictEqual(0n, BigInt.asIntN(1, 2n));
assert.strictEqual(-2n, BigInt.asIntN(2, 2n));
assert.strictEqual(2n, BigInt.asIntN(3, 2n));

assert.strictEqual(-8n, BigInt.asIntN(4, 8n));

assert.strictEqual(-1n, BigInt.asIntN(4, 255n));

assert.strictEqual(-1n, BigInt.asIntN(9, 65535n));

assert.strictEqual(-1n, BigInt.asIntN(15, 65535n));

assert.strictEqual(65535n, BigInt.asIntN(17, 65535n));

assert.strictEqual(-6n, BigInt.asIntN(9, 65530n));

assert.strictEqual(-6n, BigInt.asIntN(8, 65530n));

assert.strictEqual(6n, BigInt.asIntN(8, -65530n));

assert.strictEqual(250n, BigInt.asUintN(8, 65530n));

assert.strictEqual(0n, BigInt.asUintN(1, 2n));
assert.strictEqual(2n, BigInt.asUintN(2, 2n));
assert.strictEqual(2n, BigInt.asUintN(3, 2n));

assert.strictEqual(8n, BigInt.asUintN(4, 8n));

assert.strictEqual(15n, BigInt.asUintN(4, 255n));

assert.strictEqual(511n, BigInt.asUintN(9, 65535n));

assert.strictEqual(32767n, BigInt.asUintN(15, 65535n));

assert.strictEqual(65535n, BigInt.asUintN(17, 65535n));

assert.strictEqual(506n, BigInt.asUintN(9, 65530n));

assert.strictEqual(250n, BigInt.asUintN(8, 65530n));

assert.strictEqual(typeof 250n, "bigint");

assert.strictEqual(typeof BigInt("250n"), "bigint");


var n = 24n;
n = -n;

assert.strictEqual(-24n, n);