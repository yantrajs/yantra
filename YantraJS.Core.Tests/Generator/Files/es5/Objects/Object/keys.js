var a = [1, , 3];

var k = Object.keys(a).join();

assert.strictEqual(k, "0,2");