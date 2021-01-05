class M {

    static m() {
        a.factory = M.m;
        function a() { }
        return a;
    }
}

var n = M.m();