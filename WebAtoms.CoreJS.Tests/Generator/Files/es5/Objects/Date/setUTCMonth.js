let event = new Date('December 31, 1975 23:15:30 GMT-3:00');

//console.log(event.toUTCString());
// Thu, 01 Jan 1976 02:15:30 GMT

assert.strictEqual(0, event.getUTCMonth());
// expected output: 0

let n = event.setUTCMonth(11);

assert.strictEqual(218254530000, n);
//console.log(event.toUTCString());
// expected output: Wed, 01 Dec 1976 02:15:30 GMT