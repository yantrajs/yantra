let event = new Date(Date.UTC(2012, 11, 20, 3, 0, 0));

let options = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };

assert.strictEqual("Donnerstag, 20. Dezember 2012", event.toLocaleDateString('de-DE'));
// expected output: Donnerstag, 20. Dezember 2012

assert.throws(() => {
    assert.strictEqual("Donnerstag, 20. Dezember 2012", event.toLocaleDateString('de-DE', options));
});

//assert.strictEqual("الخميس، ٢٠ ديسمبر، ", event.toLocaleDateString('ar-EG', options));
// expected output: الخميس، ٢٠ ديسمبر، ٢٠١٢

// assert.strictEqual("Thursday, December 20, 2012", event.toLocaleDateString(undefined, options));
// expected output: Thursday, December 20, 2012 (varies according to default locale)