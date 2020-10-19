let event = new Date('August 19, 1975 23:15:30');

assert.strictEqual("23:15:30 GMT+0530 (India Standard Time)", event.toTimeString());
// expected output: 23:15:30 GMT+0200 (CEST)
// (note: your timezone may vary)
event = new Date(NaN).toTimeString();
assert.strictEqual("Invalid Date", event);