var animals = ['pigs', 'goats', 'sheep'];
var count = animals.push("cows");

assert.strictEqual(count, 4);

assert.strictEqual(animals[3], "cows");

count = animals.push('chickens', 'cats', 'dogs');

assert.strictEqual(count, 7);

assert.strictEqual(animals[5], "cats");

var a = {};

Array.prototype.push.call(a, 1);

assert.strictEqual(a.length, 1);
assert.strictEqual(a[0], 1);