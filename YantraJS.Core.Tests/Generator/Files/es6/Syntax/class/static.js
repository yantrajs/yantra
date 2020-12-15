// hoisting of class is pending !!

class Shape {

    static get shapeName() {
        return Shape._name;
    }
    static set shapeName(v) {
        Shape._name = v;
    }

}

Shape.shapeName = "shape";
assert.strictEqual("shape", Shape.shapeName);