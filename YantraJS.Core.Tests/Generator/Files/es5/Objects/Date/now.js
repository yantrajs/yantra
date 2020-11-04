
// this example takes 2 seconds to run
const start = Date.now();
// console.log(start);
// Set timeout not implemented - as of 10/05/2020, so unit test is pending 
setTimeout(() => {
    const millis = Date.now() - start;

    //assert.strictEqual(2, Math.floor(millis / 1000));
    // expected output: seconds elapsed = 2
}, 2000);