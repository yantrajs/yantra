var a = [1,2,3];
var n = [];
for (let i of a) {
    try {
        (undefined).name();
    } finally {
        n.push(i);
        // continue;
    }
}
assert.strictEqual(n.toString(), "1,2,3");
console.log("This test is pending !!")