let event = new Date('August 19, 1975 23:15:30');

event.setMinutes(45);

assert.strictEqual(45, event.getMinutes());
// expected output: 45

//console.log(event);
assert.strictEqual(177704130000, event.getTime());
// expected output: Tue Aug 19 1975 23:45:30 GMT+0200 (CEST)
// (note: your timezone may vary)
//for us it is - Tue Aug 19 1975 23:45:30 GMT+0530 (India Standard Time) {}