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
        private YExpression[] VisitList(IList<Expression> list)
        {
            var r = new YExpression[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                var v =Visit(list[i]);
                if (v == null)
                    throw new ArgumentNullException();
                r[i] = v;
            }
            return r;
        }
    }
}
