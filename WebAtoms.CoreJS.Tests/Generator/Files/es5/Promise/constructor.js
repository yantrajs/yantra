var p = new Promise((resolve, reject) => {

    setTimeout(() => {
        resolve(1);
    }, 100);

});

p.then((r) => {
    assert.strictEqual(1, r);
});