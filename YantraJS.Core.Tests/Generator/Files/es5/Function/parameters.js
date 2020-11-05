function a(a1, a2, a3, a4, a5, a6, a7) {
    return a1 + a2 + a3 + a4 + a5 + a6 + a7;
}

assert(isNaN(a(1)));
assert(isNaN(a(1, 2)));
assert(isNaN(a(1, 2, 3)));
assert(isNaN(a(1, 2, 3, 4)));
assert(isNaN(a(1, 2, 3, 4, 5)));
assert(isNaN(a(1, 2, 3, 4, 5, 6)));
// assert(isNaN(a(1, 2, 3, 4, 5, 6, 7)));
// assert(isNaN(a(1, 2, 3, 4, 5, 6, 7, 8)));
