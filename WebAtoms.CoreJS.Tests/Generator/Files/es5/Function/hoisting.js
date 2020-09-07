function a(m) {

    var b = n();
    function n() {
        return 2;
    }
    return m + b;
};

assert(a(1) === 3);
