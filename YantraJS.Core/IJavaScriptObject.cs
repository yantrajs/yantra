#nullable enable

namespace YantraJS.Core
{
    public interface IJavaScriptObject
    {
        /// <summary>
        /// This handle makes is easy to reference
        /// object back and forth from CLR and JavaScript runtime.
        /// 
        /// Do not manually modify this.
        /// </summary>
        JSValue JSHandle { get; set; }
    }

    public class JavaScriptObject : IJavaScriptObject
    {
        private JSValue handle;
        JSValue IJavaScriptObject.JSHandle {
            get => handle;
            set => handle = value;
        }
    }
}
