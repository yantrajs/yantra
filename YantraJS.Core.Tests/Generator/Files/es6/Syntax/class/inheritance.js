class BaseClass {
    m() {
        return "base";
    }
}

class ChildClass extends BaseClass {

    constructor() {
        super();

        this.n = this.m();
    }
}

var c = new ChildClass();

assert.strictEqual(c.n, "base");

assert.strictEqual(Object.getPrototypeOf(ChildClass), BaseClass);