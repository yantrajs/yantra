//var d1 = new Date(2010, 4, 24)
//var d2 = new Date('24 Apr 2010 23:59:57').toLocaleString();
//assert.strictEqual(d1, d2);

let event = new Date(Date.UTC(2012, 11, 20, 3, 0, 0));

// British English uses day-month-year order and 24-hour time without AM/PM
//assert.strictEqual("20/12/2012, 03:00:00", event.toLocaleString('en-GB', { timeZone: 'UTC' }));
assert.strictEqual("Thursday, 20 December 2012 08:30:00", event.toLocaleString('en-GB'));
// expected output: 20/12/2012, 03:00:00

// Korean uses year-month-day order and 12-hour time with AM/PM
assert.throws(() => {
    assert.strictEqual("2012. 12. 20. 오전 3:00:00", event.toLocaleString('ko-KR', { timeZone: 'UTC' }));
});
// expected output: 2012. 12. 20. 오전 3:00:00