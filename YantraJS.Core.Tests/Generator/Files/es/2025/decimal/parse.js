let a = 0.2m;
let b = 0.1m;
assert.strictEqual(0.3m, a + b);

assert.strictEqual("decimal", typeof a);

let c = a * 10m;
assert.strictEqual(2m, c);