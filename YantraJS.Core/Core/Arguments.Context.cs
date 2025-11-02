using System;
using System.Collections.Generic;
using System.Text;

namespace YantraJS.Core;

public partial struct Arguments
{

    public JSContext Context { get
        {
            return JSContext.Current;
        }
    }

}
