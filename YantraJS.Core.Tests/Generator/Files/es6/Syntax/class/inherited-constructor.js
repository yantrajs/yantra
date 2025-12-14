class A {

    aa = 4;

    constructor(a) {
        this.a = a;
    }
}

class B extends A {

}

class C extends A {
    constructor(a, c) {
        super(a);
        this.c = c;
    }
}

var b = new B(1);

assert.strictEqual(b.a, 1);

var c = new C(1, 2);
assert.strictEqual(c.a, 1);
assert.strictEqual(c.c, 2);


assert.strictEqual(c.aa, 4);
assert.strictEqual(b.aa, 4);
