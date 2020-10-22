function a(m) {

    var b = n();
    function n() {
        return 2;
    }
    return m + b;
};

assert.strictEqual(a(1),3);

// top level hoisting
//var b = {
//    c: 4
//};
//assert.strictEqual(cx(), 4);
//function cx() {
//    return b.c;
//}

//// internal hoisting
//function inner() {

//    var n = {
//        m: 3
//    };
//    return r();
//    function r() {
//        return n.m;
//    }

//}

//assert(inner(), 3);


//var t = function (t) {

//    return t + 1;
//};

//assert.strictEqual(t(2),3);
//function n() {
//    var t = function (t) {

//        return t + 2;
//    };
//    return t(3);
//}

//assert.strictEqual(n(), 5);