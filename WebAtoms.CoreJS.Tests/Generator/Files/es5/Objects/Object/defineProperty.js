var a = {};
Object.defineProperty(a, "b", {
    value: 1
});
assert(a.b === 1);

var n = 1;

// property..
Object.defineProperty(a, "c", {
    get: function () {
        return n;
    }
});
assert(a.c === 1);

n = 2;

assert(a.c === 2);
Object.defineProperty(a, "c", {
    set: function (v) {
        n = v;
    },
    get: function () {
        return n;
    }
});

a.c = 4;
assert(n === 4);
assert(a.c === 4);
