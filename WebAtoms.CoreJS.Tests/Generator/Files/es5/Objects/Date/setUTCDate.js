let event = new Date('August 19, 1975 23:15:30 GMT-3:00');

assert.strictEqual(20, event.getUTCDate());
// expected output: 20

let n = event.setUTCDate(19);
assert.strictEqual(177646530000, n);
assert.strictEqual(19, event.getUTCDate());
// expected output: 19

n = event.setUTCDate(35);
assert.strictEqual(179028930000, n);
n = event.valueOf();
assert.strictEqual(179028930000, n);