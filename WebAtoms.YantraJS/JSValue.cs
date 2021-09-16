using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using WebAtoms;
using YantraJS.Core.Clr;

namespace YantraJS.Core
{

    public static class JSValueExt
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue ToJSValue(this IJSValue value)
        {
            return value == null ? JSNull.Value : value as JSValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static JSValue ToJSValueOrUndefined(this IJSValue value)
        {
            return value == null ? JSUndefined.Value : value as JSValue;
        }

    }

    public partial class JSValue : IJSValue
    {
        IJSValue IJSValue.this[string name]
        {
            get => this[name];
            set => this[name] = value.ToJSValue();
        }
        IJSValue IJSValue.this[IJSValue keyOrSymbol]
        {
            get =>
                this[keyOrSymbol as JSValue];
            set =>
                this[keyOrSymbol as JSValue] = value.ToJSValue();
        }
        IJSValue IJSValue.this[int name]
        {
            get => this[(uint)name];
            set => this[(uint)name] = value.ToJSValue();
        }


        public IJSContext Context => JSContext.Current;

        public bool IsValueNull => this.IsNull;

        public bool IsDate => this is JSDate;

        public bool IsWrapped => this is ClrProxy;

        public long LongValue => this.BigIntValue;

        public float FloatValue => (float)this.DoubleValue;

        public DateTime DateValue => (this as JSDate).value.LocalDateTime;

        public IEnumerable<WebAtoms.JSProperty> Entries
        {
            get
            {
                foreach (var (Key, Value) in this.GetAllEntries(true))
                {
                    yield return new WebAtoms.JSProperty(Key.ToString(), Value);
                }
            }
        }

        public string DebugView => "JSValue";

        public IJSValue CreateNewInstance(params IJSValue[] args)
        {
            var a = new Arguments(args);
            return this.CreateInstance(a);
        }

        public IJSValue CreateNewInstance()
        {
            return this.CreateInstance(Arguments.Empty);
        }

        public IJSValue CreateNewInstance(IJSValue arg1)
        {
            return this.CreateInstance(new Arguments(JSUndefined.Value, arg1.ToJSValue()));
        }

        public IJSValue CreateNewInstance(IJSValue arg1, IJSValue arg2)
        {
            return this.CreateInstance(new Arguments(JSUndefined.Value, 
                arg1.ToJSValue(),
                arg2.ToJSValue()
            ));
        }

        public IJSValue CreateNewInstance(IJSValue arg1, IJSValue arg2, IJSValue arg3)
        {
            return this.CreateInstance(new Arguments(JSUndefined.Value,
                arg1.ToJSValue(),
                arg2.ToJSValue(),
                arg3.ToJSValue()
            ));
        }

        public IJSValue CreateNewInstance(IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4)
        {
            return this.CreateInstance(new Arguments(JSUndefined.Value,
                arg1.ToJSValue(),
                arg2.ToJSValue(),
                arg3.ToJSValue(),
                arg4.ToJSValue()
            ));
        }

        public IJSValue CreateNewInstance(IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5)
        {
            return this.CreateInstance(new Arguments(JSUndefined.Value,
                new JSValue[] {
                    arg1.ToJSValue(),
                    arg2.ToJSValue(),
                    arg3.ToJSValue(),
                    arg4.ToJSValue(),
                    arg5.ToJSValue()
                }
            ));
        }

        public IJSValue CreateNewInstance(IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6)
        {
            return this.CreateInstance(new Arguments(JSUndefined.Value,
                new JSValue[] {
                    arg1.ToJSValue(),
                    arg2.ToJSValue(),
                    arg3.ToJSValue(),
                    arg4.ToJSValue(),
                    arg5.ToJSValue(),
                    arg6.ToJSValue()
                }
            ));
        }

        public IJSValue CreateNewInstance(IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6, IJSValue arg7)
        {
            return this.CreateInstance(new Arguments(JSUndefined.Value,
                new JSValue[] {
                    arg1.ToJSValue(),
                    arg2.ToJSValue(),
                    arg3.ToJSValue(),
                    arg4.ToJSValue(),
                    arg5.ToJSValue(),
                    arg6.ToJSValue(),
                    arg7.ToJSValue()
                }
            ));
        }

        public IJSValue CreateNewInstance(IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6, IJSValue arg7, IJSValue arg8)
        {
            return this.CreateInstance(new Arguments(JSUndefined.Value,
                new JSValue[] {
                    arg1.ToJSValue(),
                    arg2.ToJSValue(),
                    arg3.ToJSValue(),
                    arg4.ToJSValue(),
                    arg5.ToJSValue(),
                    arg6.ToJSValue(),
                    arg7.ToJSValue(),
                    arg8.ToJSValue()
                }
            ));
        }

        public void DefineProperty(string name, JSPropertyDescriptor pd)
        {
            JSFunction pget = null;
            JSFunction pset = null;
            JSValue pvalue = null;
            var value = pd.Value as JSValue;
            var get = pd.Get as JSFunction;
            var set = pd.Set as JSFunction;
            var pt = JSPropertyAttributes.Empty;
            if (pd.Configurable ?? false)
                pt |= JSPropertyAttributes.Configurable;
            if (pd.Enumerable ?? false)
                pt |= JSPropertyAttributes.Enumerable;
            if (!pd.Writable ?? false)
                pt |= JSPropertyAttributes.Readonly;
            if (get != null)
            {
                pt |= JSPropertyAttributes.Property;
                pget = get;
            }
            if (set != null)
            {
                pt |= JSPropertyAttributes.Property;
                pset = set;
            }
            if (get == null && set == null)
            {
                pt |= JSPropertyAttributes.Value;
                pvalue = value;
                pget = value as JSFunction;
            }
            var pAttributes = pt;
            var target = this as JSObject;
            ref var ownProperties = ref target.GetOwnProperties();
            KeyString key = name;
            ownProperties.Put(key.Key) = new JSProperty(key, pget, pset, pvalue, pAttributes);
        }

        public bool DeleteProperty(string name)
        {
            return this.Delete(name).BooleanValue;
        }

        public bool HasProperty(string name)
        {
            if (!(this is JSObject @object))
                return false;
            ref var ownProperties = ref @object.GetOwnProperties();
            KeyString key = name;
            return ownProperties.HasKey(key.Key);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, params IJSValue[] args)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), args);
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined());
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IJSValue arg1)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), arg1.ToJSValue());
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IJSValue arg1, IJSValue arg2)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), arg1.ToJSValue(), arg2.ToJSValue());
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IJSValue arg1, IJSValue arg2, IJSValue arg3)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), arg1.ToJSValue(), arg2.ToJSValue(), arg3.ToJSValue());
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), arg1.ToJSValue(), arg2.ToJSValue(), arg3.ToJSValue(), arg4.ToJSValue());
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), new JSValue[] {
                arg1.ToJSValue(), 
                arg2.ToJSValue(), 
                arg3.ToJSValue(), 
                arg4.ToJSValue(),
            });
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), new JSValue[] {
                arg1.ToJSValue(),
                arg2.ToJSValue(),
                arg3.ToJSValue(),
                arg4.ToJSValue(),
                arg5.ToJSValue(),
            });
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6, IJSValue arg7)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), new JSValue[] {
                arg1.ToJSValue(),
                arg2.ToJSValue(),
                arg3.ToJSValue(),
                arg4.ToJSValue(),
                arg5.ToJSValue(),
                arg6.ToJSValue()
            });
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6, IJSValue arg7, IJSValue arg8)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), new JSValue[] {
                arg1.ToJSValue(),
                arg2.ToJSValue(),
                arg3.ToJSValue(),
                arg4.ToJSValue(),
                arg5.ToJSValue(),
                arg6.ToJSValue(),
                arg7.ToJSValue()
            });
            return InvokeFunction(a);
        }

        public IJSValue InvokeFunction(IJSValue thisValue, IList<IJSValue> args)
        {
            var a = new Arguments(thisValue.ToJSValueOrUndefined(), args);
            return InvokeFunction(a);
        }

        public IJSValue InvokeMethod(string name, params IJSValue[] args)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, args);
            return fx(a);
        }

        public IJSValue InvokeMethod(string name)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this);
            return fx(a);
        }

        public IJSValue InvokeMethod(string name, IJSValue arg1)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, arg1.ToJSValue());
            return fx(a);
        }

        public IJSValue InvokeMethod(string name, IJSValue arg1, IJSValue arg2)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, arg1.ToJSValue(), arg2.ToJSValue());
            return fx(a);
        }

        public IJSValue InvokeMethod(string name, IJSValue arg1, IJSValue arg2, IJSValue arg3)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, arg1.ToJSValue(), arg2.ToJSValue(), arg3.ToJSValue());
            return fx(a);
        }

        public IJSValue InvokeMethod(string name, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, arg1.ToJSValue(), arg2.ToJSValue(), arg3.ToJSValue(), arg4.ToJSValue());
            return fx(a);
        }

        public IJSValue InvokeMethod(string name, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, new JSValue[] {
                arg1.ToJSValue(), 
                arg2.ToJSValue(), 
                arg3.ToJSValue(), 
                arg4.ToJSValue(),
                arg5.ToJSValue()
            });
            return fx(a);
        }

        public IJSValue InvokeMethod(string name, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, new JSValue[] {
                arg1.ToJSValue(),
                arg2.ToJSValue(),
                arg3.ToJSValue(),
                arg4.ToJSValue(),
                arg5.ToJSValue(),
                arg6.ToJSValue()
            });
            return fx(a);
        }

        public IJSValue InvokeMethod(string name, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6, IJSValue arg7)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, new JSValue[] {
                arg1.ToJSValue(),
                arg2.ToJSValue(),
                arg3.ToJSValue(),
                arg4.ToJSValue(),
                arg5.ToJSValue(),
                arg6.ToJSValue(),
                arg7.ToJSValue()
            });
            return fx(a);
        }

        public IJSValue InvokeMethod(string name, IJSValue arg1, IJSValue arg2, IJSValue arg3, IJSValue arg4, IJSValue arg5, IJSValue arg6, IJSValue arg7, IJSValue arg8)
        {
            var fx = GetMethod(name);
            if (fx == null)
                throw JSContext.Current.NewTypeError($"Method {name} not found on {this}");
            var a = new Arguments(this, new JSValue[] {
                arg1.ToJSValue(),
                arg2.ToJSValue(),
                arg3.ToJSValue(),
                arg4.ToJSValue(),
                arg5.ToJSValue(),
                arg6.ToJSValue(),
                arg7.ToJSValue(),
                arg8.ToJSValue()
            });
            return fx(a);
        }

        public IList<IJSValue> ToArray()
        {
            return new AtomEnumerable(this);
        }

        public T Unwrap<T>()
        {
            if (this is ClrProxy proxy)
                return (T)proxy.value;
            return (T)(object)null;
        }

        bool IJSValue.InstanceOf(IJSValue jsClass)
        {
            return this.InstanceOf(jsClass.ToJSValue()).BooleanValue;
        }
    }
}
