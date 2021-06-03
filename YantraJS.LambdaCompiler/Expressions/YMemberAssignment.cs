using System.Reflection;

namespace YantraJS.Expressions
{
    public class YBinding
    {

    }

    public class YMemberAssignment: YBinding
    {
        public MemberInfo Member;
        public YExpression Value;

        public YMemberAssignment(MemberInfo field, YExpression value)
        {
            this.Member = field;
            this.Value = value;
        }
    }
}