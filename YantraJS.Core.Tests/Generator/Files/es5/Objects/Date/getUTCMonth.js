let date1 = new Date('December 31, 1975, 23:15:30 GMT+11:00');
let date2 = new Date('December 31, 1975, 23:15:30 GMT-11:00');

// December
assert.strictEqual(11, date1.getUTCMonth());
// expected output: 11

// January
assert.strictEqual(0, date2.getUTCMonth());
// expected output: 0