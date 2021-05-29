using System.Linq.Expressions;
using YantraJS.Core;
using Exp = YantraJS.Expressions.YExpression;
using Expression = YantraJS.Expressions.YExpression;
using ParameterExpression = YantraJS.Expressions.YParameterExpression;
using LambdaExpression = YantraJS.Expressions.YLambdaExpression;
using LabelTarget = YantraJS.Expressions.YLabelTarget;
using SwitchCase = YantraJS.Expressions.YSwitchCaseExpression;
using GotoExpression = YantraJS.Expressions.YGoToExpression;
using TryExpression = YantraJS.Expressions.YTryCatchFinallyExpression;

namespace YantraJS
{
    public class LoopScope: LinkedStackItem<LoopScope>
    {
        public readonly LabelTarget Break;
        public readonly LabelTarget Continue;
        public readonly string Name;
        public readonly bool IsSwitch;

        public LoopScope(
            LabelTarget breakTarget, 
            LabelTarget continueTarget,
            bool isSwitch = false, 
            string name = null)
        {
            this.Name = name;
            this.Break = breakTarget;
            this.Continue = continueTarget;
            this.IsSwitch = isSwitch;
        }
        public LoopScope Get(string name)
        {
            var start = this;
            while (start != null  && start.Name != name)
                start = start.Parent;
            return start;
        }
    }
}
