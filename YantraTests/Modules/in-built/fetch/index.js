async function test() {
    const rs = await fetch("https://httpbin.org/get");
    const json = await rs.json();

    assert.strictEqual(json.headers.Host, "httpbin.org");
}

await test();
