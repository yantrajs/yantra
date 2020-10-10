using Esprima.Ast;
using Microsoft.Build.Tasks.Deployment.Bootstrapper;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Core.Generator;
using WebAtoms.CoreJS.Extensions;

namespace WebAtoms.CoreJS
{
    public interface IElementEnumerator
    {
        bool MoveNext();

        JSValue Current { get; }

        uint Index { get; }
    }

    //public ref struct OwnElementEnumeratorWithoutHoles
    //{
    //    JSObject value;
    //    int length;
    //    IEnumerator<(uint Key, JSProperty Value)> en;
    //    UInt32Trie<JSProperty> elements;

    //    public int CurrentIndex;
    //    public JSValue Current => CurrentProperty.IsValue 
    //        ? CurrentProperty.value 
    //        : CurrentProperty.get.InvokeFunction(new Arguments(this.value));
    //    public JSProperty CurrentProperty;

    //    public OwnElementEnumeratorWithoutHoles(JSValue value)
    //    {
    //        this.value = value as JSObject;
    //        this.elements = this.value.elements;
    //        CurrentIndex = -1;
    //        CurrentProperty = new JSProperty();
    //        if (value is JSArray a)
    //        {
    //            this.en = null;
    //            this.length = (int)a._length;
    //        } else
    //        {
    //            this.en = this.elements?.AllValues?.GetEnumerator();
    //            this.length = -1;
    //        }
    //    }

    //    public bool MoveNext()
    //    {
    //        if(en != null)
    //        {
    //            if (en.MoveNext())
    //            {
    //                var c = en.Current;
    //                this.CurrentIndex = (int)c.Key;
    //                this.CurrentProperty = c.Value;
    //                return true;
    //            }
    //            return false;
    //        }
    //        CurrentIndex++;
    //        while(CurrentIndex < length)
    //        {
    //            if(this.elements.TryGetValue((uint)CurrentIndex, out CurrentProperty)){
    //                return true;
    //            }
    //        }
    //        return false;
    //    }
    //}

    //public ref struct OwnEntriesEnumerator
    //{

    //    JSObject value;
    //    IEnumerator<(uint Key, JSProperty Value)> elements;
    //    private PropertySequence.Enumerator properties;

    //    public bool IsUint;

    //    public uint Index;

    //    public JSValue Current;

    //    public JSProperty CurrentProperty;


    //    public OwnEntriesEnumerator(JSValue value)
    //    {
    //        this.value = value as JSObject;
    //        IsUint = false;
    //        Index = 0;
    //        Current = null;
    //        CurrentProperty = new JSProperty();
    //        if (this.value == null)
    //        {
    //            this.elements = null;
    //            this.properties = new PropertySequence.Enumerator();
    //        }
    //        else
    //        {
    //            this.elements = this.value.elements?.AllValues?.GetEnumerator();
    //            if (this.value.ownProperties != null)
    //            {
    //                this.properties = new PropertySequence.Enumerator(this.value.ownProperties);
    //            } else
    //            {
    //                this.properties = new PropertySequence.Enumerator();
    //            }
                
    //        }
    //    }

    //    public bool MoveNext()
    //    {
    //        if(elements != null)
    //        {
    //            if (elements.MoveNext())
    //            {
    //                var c = elements.Current;
    //                IsUint = true;
    //                Index = c.Key;
    //                CurrentProperty = c.Value;
    //                this.Current = c.Value.value;
    //                return true;
    //            }
    //        }
    //        if (properties.MoveNext())
    //        {
    //            IsUint = false;
    //            Index = 0;
    //            CurrentProperty = properties.Current;
    //            Current = this.value.GetValue(CurrentProperty);
    //            return true;
    //        }
    //        return false;
    //    }


    //}
}
