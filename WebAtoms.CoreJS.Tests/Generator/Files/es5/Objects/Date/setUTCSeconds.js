let date1 = new Date('December 31, 1975, 23:15:30 GMT+11:00');

assert.strictEqual(30, date1.getUTCSeconds());
// expected output: 30

date1.setUTCSeconds(39);

assert.strictEqual(39,date1.getUTCSeconds());
// expected output: 39