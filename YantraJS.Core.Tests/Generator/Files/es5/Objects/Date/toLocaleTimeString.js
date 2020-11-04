// Depending on timezone, your results will vary
let event = new Date('August 19, 1975 23:15:30 GMT+00:00');

assert.strictEqual("4:45:30 AM", event.toLocaleTimeString('en-US'));
// expected output: 1:15:30 AM

assert.strictEqual("04:45:30", event.toLocaleTimeString('it-IT'));
// expected output: 01:15:30

