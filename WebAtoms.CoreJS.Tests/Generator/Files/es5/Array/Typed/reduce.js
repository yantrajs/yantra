let uint8 = new Int8Array([0, 1, 2, 3]);

function sum(previousValue, currentValue) {
    return previousValue + currentValue;
}

assert.strictEqual(6,uint8.reduce(sum));
// expected output: 6

//Assert.AreEqual(4, Evaluate("new Int8Array([1, 2, 3])
//.reduce(function (accum, value, index, array) { return accum + index; })"));

//Assert.AreEqual(6, Evaluate("new Int8Array([1, 2, 3])
//.reduce(function (accum, value, index, array) { return accum + array[index]; })"));

//Assert.AreEqual("1,2", Evaluate("indices = []; new Int8Array([1, 2, 3])
//.reduce(function (accum, value, index, array) { indices.push(index); }); indices.toString()"));

uint = new Int8Array([1, 2, 3]);
function val(accum, value, index, array) {
    return accum + value;
}
assert.strictEqual(6, uint.reduce(val));

function val5(accum, value, index, array) {
    return accum + index;
}

assert.strictEqual(4, uint.reduce(val5)); 

function val2(accum, value, index, array) {
    return accum + array[index];
}

assert.strictEqual(6, uint.reduce(val2));

let indices = [];
function val3(accum, value, index, array) {
    return indices.push(index);
}
uint.reduce(val3)
assert.strictEqual("1,2", indices.toString()); 


assert.throws(() => {
    uint8.reduce(true);
});

assert.throws(() => {
    uint8.reduce(1);
});


assert.throws(() => {
    uint8.reduce({});
});


let t = 0;

//'use strict'
function eval(a,b,c,d) {
    // "use strict";
    t = this;
}
//assert.strictEqual(undefined, uint.reduce(eval));