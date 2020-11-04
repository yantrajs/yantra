//'August 19, 1975 23:15:30 GMT+07:00'
let date1 = new Date("1975-08-19T16:15:30.000Z");
//'August 19, 1975 23:15:30 GMT-02:00'
let date2 = new Date("1975-08-20T01:15:30.000Z");

assert.strictEqual(date1.getTimezoneOffset() , date2.getTimezoneOffset())
