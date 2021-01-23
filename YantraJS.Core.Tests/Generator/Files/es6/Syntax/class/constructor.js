class A {

    constructor(a) {
        this.a = a;
    }
}

var a = new A(1);

assert.strictEqual(a.a, 1);