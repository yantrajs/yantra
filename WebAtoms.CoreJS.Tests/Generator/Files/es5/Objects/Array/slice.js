let fruits = ['Banana', 'Orange', 'Lemon', 'Apple', 'Mango']
let citrus = fruits.slice(1, 3);

assert.strictEqual(citrus.toString(), "Orange,Lemon");

function list(a1) {
    return Array.prototype.slice.call(a1)
}

var args = {
    0: 1,
    1: 2,
    2: 3,
    length: 3
};

let list1 = list(args);

assert.strictEqual(list1.toString(), "1,2,3");

function list2() {
    return Array.prototype.slice.call(arguments);
}

list1 = list2(1,2,3);

assert.strictEqual(list1.toString(), "1,2,3");