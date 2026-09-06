function delay(n) {
    return new Promise((resolve) => {
        setTimeout(() => resolve(n), 10);
    });
}

async function* g1() {
    yield await delay(1);
    yield delay(2);
    yield delay(3);
}

const a = [];
for await (const i of g1()) {
    a.push(i);
}

assert.equal("1,2,3", a.toString());
