let date1 = new Date('August 19, 1975 23:15:30 GMT+11:00');
let date2 = new Date('August 19, 1975 23:15:30 GMT-11:00');

// Tuesday
assert.strictEqual(2, date1.getUTCDay());
// expected output: 2

// Wednesday
assert.strictEqual(3, date2.getUTCDay());
// expected output: 3