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

//Tue Oct 13 2020 15:58:20 GMT+0530 (India Standard Time)
event = new Date(1602584900669);
event.setHours(24);
assert.strictEqual(1602617300669, event.getTime());

event = new Date(1602584900669);
event.setHours(0);
assert.strictEqual(1602530900669, event.getTime());


event = new Date(1602584900669);
event.setHours(-1);
assert.strictEqual(1602527300669, event.getTime());


//Tue Oct 13 2020 15:58:20 GMT+0530 (India Standard Time)
event = new Date(1602584900669);
event.setHours(30, 61);
assert.strictEqual(1602639080669, event.getTime());

//Tue Oct 13 2020 15:58:20 GMT+0530 (India Standard Time)
event = new Date(1602584900669);
event.setHours(-30, -70);
assert.strictEqual(1602415220669, event.getTime());

event = new Date(1602584900669);
event.setHours(30, 70, 1000,10000);
assert.strictEqual(1602640610000, event.getTime());

event = new Date(1602584900669);
event.setHours(-35, -80, -999, -11100);
assert.strictEqual(1602395589900, event.getTime());