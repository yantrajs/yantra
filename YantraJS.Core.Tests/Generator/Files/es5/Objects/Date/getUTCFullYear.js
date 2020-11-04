let date1 = new Date('December 31, 1975, 23:15:30 GMT+11:00');
let date2 = new Date('December 31, 1975, 23:15:30 GMT-11:00');

assert.strictEqual(1975, date1.getUTCFullYear());
// expected output: 1975

assert.strictEqual(1976, date2.getUTCFullYear());
// expected output: 1976