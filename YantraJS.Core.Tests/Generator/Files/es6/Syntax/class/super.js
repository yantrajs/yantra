class A {
    a() {
        return "a";
    }
}

class B extends A {
    a() {
        return "b" + super.a();
    }
}

class C extends B {
    a() {
        return "c" + super.a();
    }
}

let c = new C();
assert.strictEqual("cba", c.a());

class D extends C {
    a() {
        this.a = super.a;
        return "";
    }
    b() {
        delete this.a;
    }
}

let d = new D();
assert.strictEqual("", d.a());
assert.strictEqual("cba", d.a());
d.b();
assert.strictEqual("", d.a());
