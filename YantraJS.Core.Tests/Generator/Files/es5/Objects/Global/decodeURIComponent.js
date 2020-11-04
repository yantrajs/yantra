function containsEncodedComponents(x) {
    // ie ?,=,&,/ etc
    return (decodeURI(x) !== decodeURIComponent(x));
}

assert.notStrictEqual(decodeURI('%3Fx%3Dtest'), decodeURIComponent('%3Fx%3Dtest'));

assert(containsEncodedComponents('%3Fx%3Dtest'));
assert(!containsEncodedComponents('%D1%88%D0%B5%D0%BB%D0%BB%D1%8B'));