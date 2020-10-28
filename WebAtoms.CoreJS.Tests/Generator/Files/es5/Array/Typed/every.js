function isNegative(element, index, array) {
    return element < 0;
}

let int8 = new Int8Array([-10, -20, -30, -40, -50]);

//console.log(int8.every(isNegative));
assert.strictEqual(true,int8.every(isNegative));
// expected output: true