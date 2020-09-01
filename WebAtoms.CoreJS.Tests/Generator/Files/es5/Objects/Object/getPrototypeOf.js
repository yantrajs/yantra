var a = [1];
var p = Object.getPrototypeOf(a);
assert(p === Array.prototype, p);
