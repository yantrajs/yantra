var error = null;
try {
    console.log("a");
} finally {
    error = e;
}
assert(error, error);