var a = {};
var n = 1;

Object.defineProperties(a, {
    b: {
        value: 1
    },
    c: {
        set: function (v) {
            n = v;
        },
        get: function () {
            return n;
        }
    }
});
assert(a.b === 1);


assert(a.c === 1);

n = 2;

assert(a.c === 2);
a.c = 4;
assert(n === 4);
assert(a.c === 4);
