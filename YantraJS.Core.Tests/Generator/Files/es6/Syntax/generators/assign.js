//function* g1() {
//    yield 0;
//    yield 1;
//    yield 2;
//}

//let g = g1();
//let c = g.next();
//assert.strictEqual(0, c.value);
//assert.strictEqual(false, c.done);
//c = g.next();
//assert.strictEqual(1, c.value);
//assert.strictEqual(false, c.done);
//c = g.next();
//assert.strictEqual(2, c.value);
//assert.strictEqual(false, c.done);

//c = g.next();
//assert.strictEqual(undefined, c.value);
//assert.strictEqual(true, c.done);
let a = 4;
let m;
class A {
    *g1(n) {
        m = a = yield n;
    }
}

var r = new A();

var ra = Array.from(r.g1(5));
assert.strictEqual(ra.toString(), '5');