var animals = ['pigs', 'goats', 'sheep'];
var pop = animals.pop();

assert.strictEqual(pop, 'sheep');

animals = {
    0: 'pigs',
    1: 'goats',
    2: 'sheep',
    length: 3
};

pop = Array.prototype.pop.call(animals);
assert.strictEqual(pop, 'sheep');
