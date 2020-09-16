var animals = ['pigs', 'goats', 'sheep'];
var count = animals.push("cows");

assert(count === 4, count);

assert(animals[3] === "cows");

count = animals.push('chickens', 'cats', 'dogs');

assert(count === 7, count);

assert(animals[5] === "cats");

var a = {};

Array.prototype.push.call(a, 1);

assert(a.length === 1);