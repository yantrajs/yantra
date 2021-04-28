using System.Reflection;

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
}
