using System;
using System.Linq;
using System.Linq.Expressions;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class JSUndefinedBuilder
    {
        public static Expression Value =
            Expression.Field(null,
                typeof(JSUndefined).GetField(nameof(Core.JSUndefined.Value)));
    }
}
