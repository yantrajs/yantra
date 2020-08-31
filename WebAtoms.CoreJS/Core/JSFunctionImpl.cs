using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public delegate JSValue JSFunctionImplDelegate(JSValue _this, JSArguments args, JSVariable[] closures);

    public class JSFunctionImpl: JSFunction
    {
        internal JSFunctionImpl(
            JSFunctionImplDelegate f,
            string name, 
            string source,
            JSVariable[] closures)
            : base((t, a) => f(t,a, closures), name, source)
        {
        }
    }
}
