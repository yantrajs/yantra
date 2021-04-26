using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using YantraJS.Expressions;

namespace YantraJS.Converters
{

    public partial class LinqConverter
    {

        public YLambdaExpression Visit(LambdaExpression lambda)
        {
            var plist = Register(lambda.Parameters);
            return new YLambdaExpression(
                lambda.Name ?? "Unknown",
                Visit(lambda.Body),
                plist);
        }

    }
}
