let event = new Date('August 19, 1975 23:15:30');

event.setSeconds(42);

assert.strictEqual(42, event.getSeconds());
// expected output: 42

assert.strictEqual(177702342000, event.getTime());
// Tue Aug 19 1975 23:15:42 GMT+0530 (India Standard Time)
// (note: your timezone may vary)