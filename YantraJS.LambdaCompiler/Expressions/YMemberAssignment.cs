using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using YantraJS.Core;

namespace YantraJS.Expressions
{
    public enum BindingType
    {
        MemberAssignment,
        MemberListInit,
        ElementInit
    }

    public class YBinding
    {
        public readonly MemberInfo Member;
        public readonly BindingType BindingType;
        public YBinding(MemberInfo member, BindingType bindingType)
        {
            this.BindingType = bindingType;
            this.Member = member;
        }
    }

    public class YElementInit: YBinding
    {
        public readonly MethodInfo AddMethod;
        public readonly IFastEnumerable<YExpression> Arguments;

        public YElementInit(MethodInfo addMethod, IFastEnumerable<YExpression> arguments)
            : base(addMethod, BindingType.ElementInit)
        {
            this.AddMethod = addMethod;
            this.Arguments = arguments;
        }


        public YElementInit(MethodInfo addMethod, params YExpression[] arguments)
            : base(addMethod, BindingType.ElementInit)
        {
            this.AddMethod = addMethod;
            this.Arguments = arguments.AsSequence();
        }
    }

    public class YMemberElementInit : YBinding
    {
        public readonly YElementInit[] Elements;

        public YMemberElementInit(MemberInfo member, IEnumerable<YElementInit> inits)
            : base(member, BindingType.MemberListInit)
        {
            this.Elements = inits.ToArray();
        }


        public YMemberElementInit(MemberInfo member, YElementInit[] inits)
            :base(member, BindingType.MemberListInit)
        {
            this.Elements = inits;
        }
    }

    public class YMemberAssignment: YBinding
    {
        public YExpression Value;

        public YMemberAssignment(MemberInfo field, YExpression value)
            : base(field, BindingType.MemberAssignment)
        {
            this.Value = value;
        }
    }
}