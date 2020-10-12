using System;
using System.Linq;
using System.Linq.Expressions;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSUndefinedBuilder
    {
        public static Expression Value =
            Expression.Field(null,
                typeof(JSUndefined).GetField(nameof(Core.JSUndefined.Value)));
    }
}
