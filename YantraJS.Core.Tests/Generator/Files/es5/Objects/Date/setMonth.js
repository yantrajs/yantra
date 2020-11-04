let event = new Date('August 19, 1975 23:15:30');

event.setMonth(3);

assert.strictEqual(3,event.getMonth());
// expected output: 3
//console.log(event);
assert.strictEqual(167161530000,event.getTime());
// Sat Apr 19 1975 23:15:30 GMT+0100 (CET)
// (note: your timezone may vary)
//Sat Apr 19 1975 23:15:30 GMT+0530 (India Standard Time)