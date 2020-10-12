const isBelowThreshold = (currentValue) => currentValue < 10;

const array1 = [1, 30, 39, 29, 10, 13];

assert.strictEqual(array1.some(isBelowThreshold), true);