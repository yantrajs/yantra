let event = new Date('December 31, 1975 23:15:30 GMT-3:00');

assert.strictEqual(1976, event.getUTCFullYear());
// expected output: 1976

// console.log(event.toUTCString());
// expected output: Thu, 01 Jan 1976 02:15:30 GMT
let n = event.setUTCFullYear(1975);
//console.log(event);
assert.strictEqual(157774530000,n );

// console.log(event.toUTCString());
// expected output: Wed, 01 Jan 1975 02:15:30 GMT