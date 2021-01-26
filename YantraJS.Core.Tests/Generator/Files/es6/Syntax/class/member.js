class SP {

    constructor() {
        this.instances = {};
    }

    put(key, value) {
        var v = this.instances[key.id];
        if (!v) {
            this.instances[key.id] = value;
        } else {
            console.log(`v is ${v}`);
        }
    }

    get(key) {
        return this.instances[key.id];
    }

}

let s = new SP();

s.put({ id: "a1" }, "a1");

let a1 = s.get({ id: "a1" });

assert.strictEqual(a1, "a1");
assert.strictEqual(s.instances["a1"], "a1");