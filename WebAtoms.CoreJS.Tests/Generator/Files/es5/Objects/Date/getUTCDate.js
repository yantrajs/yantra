let date1 = new Date('August 19, 1975 23:15:30 GMT+11:00');
let date2 = new Date('August 19, 1975 23:15:30 GMT-11:00');

assert.strictEqual(19, date1.getUTCDate());
// expected output: 19

assert.strictEqual(20, date2.getUTCDate());// expected output: 20