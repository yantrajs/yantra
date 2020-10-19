let a = [1,2,3];

let b = [];
for(let n of a) {
    b.push(n);
}

assert.strictEqual("1,2,3", b.toString());