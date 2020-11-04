let date1 = new Date('2018-01-24T12:38:29.069Z');

assert.strictEqual(69, date1.getUTCMilliseconds());
// expected output: 69

date1.setUTCMilliseconds(420);

assert.strictEqual(420, date1.getUTCMilliseconds());
// expected output: 420

let n = date1.setUTCMilliseconds(1005);
assert.strictEqual(1516797510005, n);
