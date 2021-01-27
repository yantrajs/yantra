var obj = {
    a: 1,
    b: 2
};
var a = [];
for (var i in obj) {
    a.push(i);
}
assert.strictEqual("a,b", a.toString());