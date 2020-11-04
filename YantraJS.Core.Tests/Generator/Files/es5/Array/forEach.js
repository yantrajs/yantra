var a = [1, 2, 3];

var b = [];

a.forEach((item, i) => b.push(item * i));

assert.strictEqual("0,2,6", b.join());