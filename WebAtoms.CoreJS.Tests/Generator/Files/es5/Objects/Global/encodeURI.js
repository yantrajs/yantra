const uri = "https://mozilla.org/?x=шеллы";
const encoded = encodeURI(uri);

assert.strictEqual("https://mozilla.org/?x=%D1%88%D0%B5%D0%BB%D0%BB%D1%8B", encoded);

assert.strictEqual(uri, decodeURI(encoded));