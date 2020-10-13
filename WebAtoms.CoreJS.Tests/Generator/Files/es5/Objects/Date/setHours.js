let event = new Date('August 19, 1975 23:15:30');
event.setHours(20);
assert.strictEqual(177691530000, event.getTime());
//console.log(event);
// expected output: Tue Aug 19 1975 20:15:30 GMT+0200 (CEST)
// (note: your timezone may vary)

event.setHours(20, 21, 22);

//console.log(event);
assert.strictEqual(177691882000, event.getTime());
// expected output: Tue Aug 19 1975 20:21:22 GMT+0200 (CEST)