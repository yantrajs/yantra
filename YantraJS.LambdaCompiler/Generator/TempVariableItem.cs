using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace YantraJS.Generator
{

    public class AddressScope : LinkedStack<AddressScope.AddressScopeItem> {


        public bool RequiresAddress => Top.RequiresAddress;

        public AddressScopeItem Push(bool requiresAddress)
        {
            return Push(new AddressScopeItem(requiresAddress));
        }


        public class AddressScopeItem: LinkedStackItem<AddressScopeItem>
        {
            public readonly bool RequiresAddress;

            public AddressScopeItem(bool requiresAddress)
            {
                this.RequiresAddress = requiresAddress;
            }
        }

        internal AddressScopeItem Push(ParameterInfo p)
        {
            return new AddressScopeItem(p.IsIn || p.IsOut || p.ParameterType.IsByRef || p.IsRetval);
        }
    }

    public class TempVariables: LinkedStack<TempVariables.TempVariableItem>
    {
        private readonly ILGenerator il;

        private Dictionary<Type, Queue<LocalBuilder>> queues = new Dictionary<Type, Queue<LocalBuilder>>();

        public TempVariables(ILGenerator il)
        {
            this.il = il;
        }

        public TempVariableItem Push()
        {
            return Push(new TempVariableItem(this));
        }

        public LocalBuilder this[Type type]
        {
            get
            {
                return Top.Get(type);
            }
        }

        public class TempVariableItem : LinkedStackItem<TempVariableItem>
        {
            private readonly TempVariables scope;

            private List<LocalBuilder> reserved = new List<LocalBuilder>();

            public TempVariableItem(TempVariables scope)
            {
                this.scope = scope;
            }


            public LocalBuilder Get(Type type)
            {
                if(!scope.queues.TryGetValue(type, out  var q))
                {
                    q = new Queue<LocalBuilder>();
                    scope.queues[type] = q;
                }
                LocalBuilder v;
                if (q.Count > 0)
                {
                    v = q.Dequeue();
                    reserved.Add(v);
                    return v;
                }

                v = scope.il.DeclareLocal(type);
                reserved.Add(v);
                return v;
            }

            public override void Dispose()
            {
                base.Dispose();

                if (reserved == null)
                    return;

                foreach(var l in reserved)
                {
                    scope.queues[l.LocalType].Enqueue(l);
                }

                reserved = null;
            }
        }

    }
}
