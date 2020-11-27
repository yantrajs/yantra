using System.Linq.Expressions;
using YantraJS.Core;

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
