var a = { a: 1, b: 2 };
var r = [];

function* m() {
    yield 1;
    if (a) {
        for (var i in a) {
            if (a.hasOwnProperty(i)) {
                if (i === 'a')
                    continue;
                try {
                    r.push(i);
                } catch (e) {
                    console.log(e);
                }
            }
        }
    }
    yield 2;
}
assert.strictEqual(Array.from(m()).toString(), '1,2');