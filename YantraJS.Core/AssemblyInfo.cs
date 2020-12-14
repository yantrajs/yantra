using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

#if !WEB_ATOMS
[assembly: InternalsVisibleTo("YantraJS.Tests")]
[assembly: InternalsVisibleTo("YantraJS.Core.Tests")]

// used by Dynamic Assembly to access internals
[assembly: InternalsVisibleTo("YantraJS.Runtime")]
#endif
