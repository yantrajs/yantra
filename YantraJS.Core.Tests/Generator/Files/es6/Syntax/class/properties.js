class Shape {

    constructor(name) {
        this.n = name;
    }

    get name() {
        return this.n;
    }
    set name(v) {
        this.n = v;
    }
}

let s = new Shape('shape');

assert.strictEqual('shape', s.n);
assert.strictEqual('shape', s.name);

class Circle extends Shape {

    get name() {
        return super.name + '$' + super.name;
    }

}

s = new Circle('circle');
assert.strictEqual('circle', s.n);
assert.strictEqual('circle$circle', s.name);
