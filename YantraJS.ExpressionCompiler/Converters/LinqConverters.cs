using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Converters
{

    public static partial class LinqConverters
    {

        public static YLambdaExpression ToLLExpression(this LambdaExpression lambda)
        {
            var lc = new LinqConverter();
            return lc.VisitLambdaSpecific(lambda);
        }

    }
}
