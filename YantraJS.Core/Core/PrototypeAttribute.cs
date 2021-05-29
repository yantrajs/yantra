using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class JSRuntimeAttribute: Attribute
    {
        public readonly Type StaticType;
        public readonly Type Prototype;
        public JSRuntimeAttribute(Type staticType, Type prototype)
        {
            this.Prototype = prototype;
            this.StaticType = staticType;
        }

        public bool PreventConstructorInvoke { get; set; }
    }

    public enum MemberType: int
    {
        Method = 1,
        Get = 2,
        Set = 4,
        Constructor = 8,
        StaticMethod = 0xF1,
        StaticGet = 0xF2,
        StaticSet = 0xF4
    }

    [AttributeUsage(AttributeTargets.Method , AllowMultiple = false, Inherited = false)]

    public class ConstructorAttribute: PrototypeAttribute
    {
        public ConstructorAttribute(): base(null, JSPropertyAttributes.Empty, MemberType.Constructor)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class SymbolAttribute: Attribute
    {
        public readonly string Name;

        public SymbolAttribute(string name)
        {
            this.Name = name;
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
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PrototypeAttribute: Attribute
    {
        public readonly KeyString Name;

        public readonly MemberType MemberType;

        public readonly JSPropertyAttributes Attributes;

        public readonly bool IsSymbol;

        public int Length { get; set; }

        public bool IsStatic => ((int)this.MemberType & 0xF0) > 0;

        public bool IsMethod => ((int)this.MemberType & 0x1) > 0;

        public bool IsGetProperty => ((int)this.MemberType & 0x2) > 0;
        public bool IsSetProperty => ((int)this.MemberType & 0x4) > 0;

        public JSPropertyAttributes ConfigurableValue =>
            this.Attributes == JSPropertyAttributes.Empty
            ? JSPropertyAttributes.ConfigurableValue
            : this.Attributes;

        public JSPropertyAttributes ReadonlyValue =>
            this.Attributes == JSPropertyAttributes.Empty
            ? JSPropertyAttributes.ReadonlyValue
            : this.Attributes;

        public JSPropertyAttributes ConfigurableProperty =>
            this.Attributes == JSPropertyAttributes.Empty
            ? JSPropertyAttributes.ConfigurableProperty
            : this.Attributes;

        public JSPropertyAttributes ConfigurableReadonlyValue =>
            this.Attributes == JSPropertyAttributes.Empty
            ? JSPropertyAttributes.ConfigurableReadonlyValue
            : this.Attributes;
        public PrototypeAttribute(string name, 
            JSPropertyAttributes attributes = JSPropertyAttributes.Empty, 
            MemberType memberType = MemberType.Method,
            bool isSymbol = false)
        {
            this.IsSymbol = isSymbol;
            this.Attributes = attributes;
            if (name != null)
            {
                this.Name = name;
            }
            this.MemberType = memberType;
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class GetProperty: PrototypeAttribute
    {
        public GetProperty(string name, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty,
            bool isSymbol = false) 
            :base(name, attributes, MemberType.Get, isSymbol)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SetProperty : PrototypeAttribute
    {
        public SetProperty(string name, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty,
            bool isSymbol = false)
            : base(name, attributes, MemberType.Set, isSymbol)
        {

        }
    }


    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StaticGetProperty : PrototypeAttribute
    {
        public StaticGetProperty(string name, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty, bool isSymbol = false)
            : base(name, attributes, MemberType.StaticGet, isSymbol)
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StaticSetProperty : PrototypeAttribute
    {
        public StaticSetProperty(string name, JSPropertyAttributes attributes = JSPropertyAttributes.ConfigurableProperty, bool isSymbol = false)
            : base(name, attributes, MemberType.StaticSet, isSymbol)
        {

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

        public StaticAttribute(string name, 
            JSPropertyAttributes attributes = JSPropertyAttributes.Empty,
            bool isSymbol = false) :
            base(name, attributes, MemberType.StaticMethod, isSymbol)
        {
        }
    }
}
