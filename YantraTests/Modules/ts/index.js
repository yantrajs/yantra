//// import * as ts from "./typescript";
var ts = require("./typescript");
console.log("Typescript module loaded...");
const source = "let x: string  = 'string'";

let result = ts.transpileModule(source, { compilerOptions: { module: ts.ModuleKind.CommonJS } });

console.log(JSON.stringify(result));