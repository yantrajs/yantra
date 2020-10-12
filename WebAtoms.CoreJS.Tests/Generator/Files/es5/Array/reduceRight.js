const array1 = [[0, 1], [2, 3], [4, 5]].reduceRight(
    (accumulator, currentValue) => accumulator.concat(currentValue)
);
assert.strictEqual(array1.toString(), "4,5,2,3,0,1");