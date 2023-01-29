function do1 () {

}

function start() {
    var a;

    return {
        execute: function () {
            a = class a { static get name() { return "a" } };
        },
        get: function () {
            return a;
        }
    };
}

const a = start();
a.execute();
assert.strictEqual("a", a.get().name);
