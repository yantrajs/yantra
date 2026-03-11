var a = 1;
for (; ;) {
    a++;
    if (a === 10) {
        break;
    }
}
assert.strictEqual(10, a);

class A {

    init() {
        this.a = 10;
    }

    check() {
        this.a++ < 10;
    }

    run() {

        for (this.init(); this.check();) {
            this.nothing();
        }
        assert.strictEqual(this.a, 11);
    }

    nothing() {

    }

}

var a1 = new A();
a1.run();
