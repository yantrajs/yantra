class Shape {

    constructor() {
        this.a = 1;
    }

};

let a = new Shape();

assert(a instanceof Shape);
assert.strictEqual(1, a.a);

class Circle extends Shape {

    constructor() {
        super();
        this.b = 1;
    }

}

let b = new Circle();
assert(b instanceof Circle);
assert(b instanceof Shape);
assert.strictEqual(1, b.a);
assert.strictEqual(1, b.b);