let date1 = new Date('December 31, 1975, 23:15:30 GMT+11:00');

assert.strictEqual(15, date1.getUTCMinutes());
// expected output: 15

date1.setUTCMinutes(25);

assert.strictEqual(25, date1.getUTCMinutes());
// expected output: 25
