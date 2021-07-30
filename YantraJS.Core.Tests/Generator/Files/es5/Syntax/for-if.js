var a = { a: 1, b: 2 };
var r = [];
if (a) {
    for (var i in a) {
        if (a.hasOwnProperty(i)) {
            if (i === "a")
                continue;
            try {
                r.push(i);
            } catch (e) {
                console.log(e);
            }
        }
    }
}
assert.strictEqual(r.toString(), "b");