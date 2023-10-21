let i = 0;
function recursive() {
    i++;
    recursive();
}

try {
    recursive();
} catch (error) {
    if (error instanceof RangeError) {
        if (/maximum\scall\sstack/i.test(error.message)) {
            console.log(`Failed after ${i} recursive calls`);
            return;
        }
    }
    throw new Error("Stack overflow expected");
}