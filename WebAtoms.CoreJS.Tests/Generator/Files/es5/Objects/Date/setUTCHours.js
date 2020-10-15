let event = new Date('August 19, 1975 23:15:30 GMT-3:00');

//console.log(event.toUTCString());
// expected output: Wed, 20 Aug 1975 02:15:30 GMT

assert.strictEqual(2,event.getUTCHours());
// expected output: 2

let n = event.setUTCHours(23);

assert.strictEqual(177808530000,n);
// expected output: Wed, 20 Aug 1975 23:15:30 GMT

// Tue May 25 2010 10:59:57 GMT+0530 (India Standard Time)
event = new Date(1274765397000);
n = event.setUTCHours(34);
assert.strictEqual(1274869797000, n);


event = new Date('Tue May 25 2010 05:04:03 GMT+0530');
n = event.setUTCHours(5, 4, 3, 2);
assert.strictEqual(1274677443002, n);

