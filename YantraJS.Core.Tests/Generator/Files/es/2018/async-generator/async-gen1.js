function delay(n) {
    return new Promise((resolve) => {
        setTimeout(() => resolve(n), 10);
    });
}

function* g1() {
    console.log("start");
    yield delay(1);
    console.log("sent 1");
    yield delay(2);
    console.log("sent 2");
    yield delay(3);
    console.log("sent 3");
}

async function test() {
    const a = [];
    for (const x of g1()) {
        const i = await x;
        console.log(i);
        a.push(i);
    }
    // assert.equal("1,2", a.toString());
    return a.join(",");
}

//var r = await test();
//console.log(r);
//assert.equal("1,2", r);
test().then((r) => console.log(r));