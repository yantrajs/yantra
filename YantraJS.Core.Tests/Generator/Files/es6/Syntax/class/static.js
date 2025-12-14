// hoisting of class is pending !!

class Shape {

    static a = 1;

    static get shapeName() {
        return Shape._name;
    }
    static set shapeName(v) {
        Shape._name = v;
    }

}

Shape.shapeName = "shape";
assert.strictEqual("shape", Shape.shapeName);

assert.strictEqual(1, Shape.a);