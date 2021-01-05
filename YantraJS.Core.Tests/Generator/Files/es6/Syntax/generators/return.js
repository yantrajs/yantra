function* g() {
    yield 1;
    yield 2;
    return yield 3;
}

var a = Array.from(g());
