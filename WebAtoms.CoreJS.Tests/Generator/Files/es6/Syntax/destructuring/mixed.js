var a = [1, 2, { aa: "a", bb: "b" } , 3, 4];

var [a1, a2, { aa, bb }, ...all] = a;

assert.strictEqual(1, a1);
assert.strictEqual(2, a2);
assert.strictEqual("a", aa);
assert.strictEqual("b", bb);
assert.strictEqual("3,4", all.toString());

a = {
    aa: [1, 2],
    bb: {
        c: "c"
    }
};

var { aa: [a11, b11], bb: { c: cc } } = a;

assert.strictEqual(1, a11);
assert.strictEqual(2, b11);
assert.strictEqual("c", cc);