using System.Collections.Generic;
using System.Linq.Expressions;
using YantraJS.Expressions;

namespace YantraJS.Converters
{
    public class LabelMap
    {
        private Dictionary<LabelTarget, YLabelTarget> labels 
            = new Dictionary<LabelTarget, YLabelTarget>();

        public YLabelTarget this[LabelTarget label]
        {
            get
            {
                if (labels.TryGetValue(label, out var r))
                    return r;
                r = YExpression.Label(label.Name, label.Type);
                labels[label] = r;
                return r;
            }
        }
    }
}
