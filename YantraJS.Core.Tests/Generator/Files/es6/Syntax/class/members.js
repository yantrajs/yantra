// members...
class A {
    a = 1;
}

assert.strictEqual(1, (new A()).a);

// members with a constructor
class B {
    a = 1;
    constructor() {
        this.b = 1;
    }
}

assert.strictEqual(1, (new B()).a);


// override in constructor...
class C {
    a = 1;
    constructor() {
        this.a = 2;
    }
}

assert.strictEqual(2, (new C()).a);

// with super...j

class AA extends A {
    a = 2;
}

assert.strictEqual(2, (new AA()).a);


// members with a constructor
class BB extends B {
    a = 2;
    constructor() {
        super();
        this.b = 2;
    }
}

assert.strictEqual(2, (new BB()).a);


// override in constructor...
class CC extends C {
    a = 1;
    constructor() {
        super();
        this.a = 3;
    }
}

assert.strictEqual(3, (new CC()).a);

class D {
    a = () => {
        this.b = 2;
    }
}

let d = new D();
d.a();

assert.strictEqual(2, d.b);