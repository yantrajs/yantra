//moonLanding = new Date('July 20, 69 00:20:18');
let moonLanding = new Date(-14274582000);
//let millis = moonLanding.setMilliseconds(123);
//assert.strictEqual(123,millis)

//new Date('24 Apr 2010 23:59:57')
moonLanding = new Date(1272133797000);
let millis = moonLanding.getMilliseconds();
assert.strictEqual(0, millis);


assert.strictEqual(0, moonLanding.getMilliseconds.length);

//Thu Oct 08 2020 17:22:48 GMT+0530 (India Standard Time)
moonLanding = new Date(1602157968701)
millis = moonLanding.getMilliseconds();
assert.strictEqual(701, millis);