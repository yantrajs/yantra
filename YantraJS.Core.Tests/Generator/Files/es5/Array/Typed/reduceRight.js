

let uint = new Int8Array([1, 2, 3]);
//function val(accum, value, index, array) {
//    return accum + value;
//}
//assert.strictEqual(6, uint.reduceRight(val));

//function val5(accum, value, index, array) {
//    return accum + index;
//}

//assert.strictEqual(4, uint.reduceRight(val5));

//function val2(accum, value, index, array) {
//    return accum + array[index];
//}

//assert.strictEqual(6, uint.reduceRight(val2));

let indices = [];
function val3(accum, value, index, array) {
    return indices.push(index);
}
uint.reduceRight(val3)
assert.strictEqual("1,0", indices.toString());


//assert.throws(() => {
//    uint8.reduceRight(true);
//});

//assert.throws(() => {
//    uint8.reduceRight(1);
//});


//assert.throws(() => {
//    uint8.reduceRight({});
//});

