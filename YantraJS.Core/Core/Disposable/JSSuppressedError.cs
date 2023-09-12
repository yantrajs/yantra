using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Yantra.Core;
using YantraJS.Core.Clr;

namespace YantraJS.Core.Core.Error
{
    [JSClassGenerator("SuppressedError"), JSBaseClass("Error")]
    public partial class JSSuppressedError: JSError
    {

        [JSExport("error")]
        public JSValue Error { get; set; }

        [JSExport("suppressed")]
        public JSValue Suppressed { get; set; }

        public JSSuppressedError(in Arguments a,
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0) : base(
                new Arguments(JSUndefined.Value, a[2] ?? new JSString("Suppressed Error")),
                function: function,
                filePath: filePath,
                line: line)
        {
            Error = a[0];
            Suppressed = a[1];
        }

        public JSSuppressedError(
            JSValue error,
            JSValue suppressed,
            string message = "An error was suppressed during disposal.",
            [CallerMemberName] string function = null,
            [CallerFilePath] string filePath = null,
            [CallerLineNumber] int line = 0)
            :base(new JSException(new JSString(message), function, filePath, line))
        {
            Error = error;
            this.Exception.With(this);
            Suppressed = suppressed;
        }
    }
}
