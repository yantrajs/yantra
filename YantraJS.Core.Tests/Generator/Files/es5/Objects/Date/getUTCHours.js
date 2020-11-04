let date1 = new Date('December 31, 1975, 23:15:30 GMT+11:00');
let date2 = new Date('December 31, 1975, 23:15:30 GMT-11:00');

assert.strictEqual(12, date1.getUTCHours());
// expected output: 12

assert.strictEqual(10, date2.getUTCHours());
// expected output: 10