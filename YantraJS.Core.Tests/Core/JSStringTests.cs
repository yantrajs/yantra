using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;
using YantraJS.Generator;

namespace YantraJS.Tests.Core
{
    [TestClass]
    public class CodeTest: BaseTest
    {
        [TestMethod]
        public void Function()
        {

            ILCodeGenerator.GenerateLogs = true;

            // this.context.Eval("class A { constructor(a) { this.a = a; } } class B extends A { constructor(a) { super(a); } }");
            // Assert.AreEqual(1, context.Eval("x = {get f() { return 1; }}; x.f = 5; x.f"));
            // this.context["array"] = new JSArray( new JSNumber(1) );
//            this.context.Eval(@"

//(function(){return 1; /***/ })()
//");
            this.context.Eval(@"
function (expression) {
            if (expression == null) return Functions.Identity;
            if (typeof expression === Types.String) {
                // get from cache
                var f = funcCache[expression];
                if (f != null) {
                    return f;
                }

                if (expression.indexOf(""=>"") === -1) {
                    var regexp = new RegExp(""[$]+"", ""g"");

                    var maxLength = 0;
                    var match;
                    while ((match = regexp.exec(expression)) != null) {
                        var paramNumber = match[0].length;
                        if (paramNumber > maxLength) {
                            maxLength = paramNumber;
                        }
                    }

                    var argArray = [];
                    for (var i = 1; i <= maxLength; i++) {
                        var dollar = """";
                        for (var j = 0; j < i; j++) {
                            dollar += ""$"";
                        }
                        argArray.push(dollar);
                    }

                    var args = Array.prototype.join.call(argArray, "","");

                    f = new Function(args, ""return "" + expression);
                    funcCache[expression] = f;
                    return f;
                }
                else {
                    var expr = expression.match(/^[(\s]*([^()]*?)[)\s]*=>(.*)/);
                    f = new Function(expr[1], (expr[2].match(/\breturn\b/) ? expr[2] : ""return "" + expr[2]));
                    funcCache[expression] = f;
                    return f;
                }
            }
            return expression;
        },;
");
}

    }
}
