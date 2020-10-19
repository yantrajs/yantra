let event = new Date(1993, 6, 28, 14, 39, 7);

assert.strictEqual("Wed Jul 28 1993 14:39:07 GMT+0530 (India Standard Time)", event.toString());
// expected output: Wed Jul 28 1993 14:39:07 GMT+0200 (CEST)
// (note: your timezone may vary)
// assert.strictEqual("Wed Jul 28 1993 14:39:07 GMT+0530", event.toString())
assert.strictEqual("Wed Jul 28 1993", event.toDateString());
// expected output: Wed Jul 28 1993
