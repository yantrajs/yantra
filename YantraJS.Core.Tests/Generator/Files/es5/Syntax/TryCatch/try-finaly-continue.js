var a = [1,2,3];
var n = [];
for (let i of a) {
    try {
        (undefined).name();
    } catch (e) {
        n.push(i);
    } finally {
        // continue;
    }
}
console.log(n.toString());
assert.strictEqual(n.toString(), '1,2,3');
console.log('This test is pending !!')