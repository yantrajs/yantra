var animals = ['pigs', 'goats', 'sheep'];
var count = animals.push("cows");

assert(count === 4, count);

assert(animals[3] === "cows");

count = animals.push('chickens', 'cats', 'dogs');

assert(count === 7, count);

assert(animals[5] === "cats");