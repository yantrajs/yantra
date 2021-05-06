using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using YantraJS.Core.Typed;

namespace YantraJS.Core.Core.DataView
{
   public static class DataViewStatic
    {
        internal static DataView AsDataView(this JSValue v,
        [CallerMemberName] string helper = null)
        {
            if (!(v is DataView dataView))
                throw JSContext.Current.NewTypeError($"DataView.prototype.{helper} called on non DataView");
            return dataView;
        }
    }
}
