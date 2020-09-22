var error = null;
try {
    (undefined).name();
} catch (e) {
    error = e;
}
asserts(error.name, error.name);