using System;
using System.Collections.Generic;
using System.Text;

namespace WebAtoms.CoreJS.Core
{
    public enum MemberType
    {
        Method = 0,
        Get,
        Set
    }

    /// <summary>
    /// Should only be defined on static method and field
    /// </summary>
    /// <remarks>
    /// This is done to reduce number of method calls, checking receiver type and throwing TypeError
    /// on instance method will require one more method call and which will slow down if inlining is not supported
    /// on AOT platforms
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class PrototypeAttribute: Attribute
    {
        public readonly string Name;

        public readonly MemberType MemberType;

        public PrototypeAttribute(string name, MemberType memberType = MemberType.Method)
        {
            this.Name = name;
            this.MemberType = memberType;
        }
    }

    /// <summary>
    /// Should only be defined on static method and field
    /// </summary>
    /// <remarks>
    /// This is done to reduce number of method calls, checking receiver type and throwing TypeError
    /// on instance method will require one more method call and which will slow down if inlining is not supported
    /// on AOT platforms
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StaticAttribute : PrototypeAttribute
    {

        public StaticAttribute(string name, MemberType memberType = MemberType.Method) :base(name, memberType)
        {
        }
    }
}
