var a = "a;"
for (var i = 0; i < 10; i++) {
    switch (a) {
        case "b":
            a += "b";
            for (var n = 0; n < 2; n++) {
                console.log(n);
            }
            break;
        case "n":
            if (a) {
                a += "c";
                break;
            }
        case "m":
            console.log("n");
            break;
        default:
            a += "c";
    }
}

console.log(a);