let event = new Date('August 19, 1975 23:15:30');

assert.strictEqual(0, event.getMilliseconds());
// expected output: 0

event.setMilliseconds(456);

assert.strictEqual(456, event.getMilliseconds());
// expected output: 456
