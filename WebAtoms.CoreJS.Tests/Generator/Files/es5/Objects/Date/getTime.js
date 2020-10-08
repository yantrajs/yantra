//let day = new Date('July 20, 69 00:20:18 GMT+00:00');
let day = new Date("1969-07-20T00:20:18.000Z");
//let day = new Date(-14254782000);
let m = day.getTime();
assert.strictEqual(m, -14254782000)
