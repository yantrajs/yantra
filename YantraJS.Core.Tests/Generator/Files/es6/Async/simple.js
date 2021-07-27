function delay(n) {
    return new Promise((r) => {
        setTimeout(() => r(), n);
    });
}

let start = Date.now();

// let DateTime = clr.getClass('System.DateTime');

async function fill(m) {
    let a = [];
    console.log('started ' + start);
    for (var i = 0; i < m; i++) {
        await delay(100);
        a.push(Date.now());
        await delay(10);
    }
    console.log(a.toString());
    console.log('end');
    return a;
}

console.log('invoking fill');

fill(4).then((r) => {
    console.log('done');
    assert.strictEqual(4, r.length);
    let s = start;
    for (var i = 0; i < 4; i++) {
        const n = r[i];
        console.log(n);
        assert(s < n);
        s += 100;
    }
});