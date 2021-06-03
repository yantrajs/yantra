function isNegative(element, index, array) {
    return element < 0;
}

let int8 = new Int8Array([10, 0, -10, 20, -30, 40, -50]);

assert.strictEqual(-10,int8.find(isNegative));
// expected output: -10

int8 = new Int8Array([1, 2, 3]);
assert.throws(() => {
    int8.find(true);
});
// assert.strictEqual("TypeError", int8.filter(true));
int8 = new Int8Array([1, 2, 3]);
assert.throws(() => {
    int8.find(1);
});

int8 = new Int8Array([1, 2, 3]);
assert.throws(() => {
    int8.find({});
});
(function () {
 var output = 5;
    function isPresent(element, index, array) {
        output = this;
        return false;

    }
  var output = 5; //-> If we give it here its and issue, var output is declared above 
    let int8 = new Int8Array([1, 2, 3]);
    int8.find(isPresent, 10);
    assert.strictEqual(output, 10);
})();

