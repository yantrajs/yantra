using System;
using System.Linq;
using System.Linq.Expressions;
using YantraJS.Core;

namespace YantraJS.ExpHelper
{
    public class JSNullBuilder
    {

        public static Expression Value =
            Expression.Field(null,
                typeof(JSNull).GetField(nameof(JSNull.Value)));
    }
}
