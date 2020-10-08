using System;
using System.Linq;
using System.Linq.Expressions;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSNullBuilder : TypeHelper<Core.JSNull>
    {
        public static Expression Value =
            Expression.Field(null,
                Field(nameof(JSNull.Value)));
    }
}
