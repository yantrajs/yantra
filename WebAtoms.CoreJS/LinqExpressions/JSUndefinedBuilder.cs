using System;
using System.Linq;
using System.Linq.Expressions;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSUndefinedBuilder : TypeHelper<Core.JSUndefined>
    {
        public static Expression Value =
            Expression.Field(null,
                Field(nameof(Core.JSUndefined.Value)));
    }
}
