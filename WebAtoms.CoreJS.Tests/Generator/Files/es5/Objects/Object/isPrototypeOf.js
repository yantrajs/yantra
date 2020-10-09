function Foo() {
};

function Bar() {
};

function Baz() {
};

Bar.prototype = Object.create(Foo.prototype);
Baz.prototype = Object.create(Bar.prototype);

var baz = new Baz();

assert(Baz.prototype.isPrototypeOf(baz));
assert(Bar.prototype.isPrototypeOf(baz));
assert(Foo.prototype.isPrototypeOf(baz));
assert(Object.prototype.isPrototypeOf(baz));