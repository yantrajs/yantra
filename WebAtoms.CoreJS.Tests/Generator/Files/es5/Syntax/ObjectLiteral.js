var a = {
    b: 3
};

assert(a.b === 3);

var c = {
    get c(n) {
        return "c";
    }
};
assert(c.c === "c", "c.c is " + a.c);