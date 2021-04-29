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
        private Dictionary<ParameterExpression, YParameterExpression> parameters
            = new Dictionary<ParameterExpression, YParameterExpression>();

        private LabelMap labels
            = new LabelMap();

        private IList<YParameterExpression> Register(IList<ParameterExpression> plist)
        {
            var list = new List<YParameterExpression>();
            foreach (var p in plist)
            {
                var t = p.IsByRef && !p.Type.IsByRef ? p.Type.MakeByRefType() : p.Type;
                var yp = YExpression.Parameter(t, p.Name);
                parameters[p] = yp;
                list.Add(yp);
            }
            return list;
        }
    }
}
