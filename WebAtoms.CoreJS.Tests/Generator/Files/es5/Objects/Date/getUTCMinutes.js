let date1 = new Date('1 January 2000 03:15:30 GMT+07:00');
let date2 = new Date('1 January 2000 03:15:30 GMT+03:30');

assert.strictEqual(15, date1.getUTCMinutes()); // 31 Dec 1999 20:15:30 GMT
// expected output: 15

assert.strictEqual(45, date2.getUTCMinutes()); // 31 Dec 1999 23:45:30 GMT
// expected output: 45