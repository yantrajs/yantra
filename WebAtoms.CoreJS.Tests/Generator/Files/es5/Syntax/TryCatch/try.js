var error = null;
try {
    (undefined).name();
} catch (e) {
    error = e;
}
assert(error, error);