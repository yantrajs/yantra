class BaseClass {
    m() {
        return 'base';
    }
}

class ChildClass extends BaseClass {

    constructor() {
        super();

        this.n = this.m();
    }
}

var c = new ChildClass();

assert.strictEqual(c.n, 'base');

assert.strictEqual(Object.getPrototypeOf(ChildClass), BaseClass);

class Shape {

    constructor(n) {
        this.name = n;
    }

}

class Circle extends Shape {
    constructor() {
        super(...arguments);
    }
}

class RoundedCircle extends Circle {
    constructor() {
        super(...arguments);
    }
}

let c = new Circle('Circle');
assert.strictEqual('Circle', c.name);

let rc = new RoundedCircle('RoundedCircle');
assert.strictEqual('RoundedCircle', rc.name);
