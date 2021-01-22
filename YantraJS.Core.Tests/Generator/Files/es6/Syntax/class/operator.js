class A {

    constructor() {
        this.ids = 1;
    }

    process() {
        return this.ids++;
    }
}

var a1 = new A();

assert.strictEqual(a1.process(), 1);
assert.strictEqual(a1.process(), 2);
assert.strictEqual(a1.ids, 3);
