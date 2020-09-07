function a(n) {
    var s = "";
    for (var i = 0; i < n; i++) {
        s += i;
    }
    return s;
}

assert(a(3) === "012", a(3));