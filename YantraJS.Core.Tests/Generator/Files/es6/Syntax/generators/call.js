function* m(n, r) {
    // var r1 = yield n();
    r(yield n());
}

function nn() {
    return 1;
}

var r2 = [];

function rr(a) {
    console.log(a);
    r2.push(a);
}

var r1 = m(nn, rr);

var r3 = r1.next(2);
assert.strictEqual(r3.value, 1);
r3 = r1.next(2);
assert.strictEqual(r3.done, true);

assert.strictEqual(r2[0], 2);
