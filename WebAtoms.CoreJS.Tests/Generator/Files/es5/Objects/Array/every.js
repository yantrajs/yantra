const isBelowThreshold = (currentValue) => currentValue < 40;

const array1 = [1, 30, 39, 29, 10, 13];

assert.strictEqual(array1.every(isBelowThreshold), true);