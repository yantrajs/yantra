using System;
using System.Linq;
using System.Linq.Expressions;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS.ExpHelper
{
    public class JSNullBuilder
    {

        public static Expression Value =
            Expression.Field(null,
                typeof(JSNull).GetField(nameof(JSNull.Value)));
    }
}
