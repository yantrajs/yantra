#nullable enable
using System;
using YantraJS.Core;
using YantraJS.Core.Clr;

namespace Yantra.Core.Events
{
    public class Event : JavaScriptObject
    {

        [JSExportSameName]
        public static int NONE = 0;

        [JSExportSameName]
        public static int CAPTURING_PHASE = 1;

        [JSExportSameName]
        public static int AT_TARGET = 2;

        [JSExportSameName]
        public static int BUBBLING_PHASE = 3;

        [JSExport]
        public EventTarget? Target { get; internal set; }

        [JSExport]
        public EventTarget? CurrentTarget { get; internal set; }

        [JSExport]
        public string Type { get; private set; }

        [JSExport]
        public int EventPhase { get; internal set; }

        [JSExport]
        public bool DefaultPrevented { get; set; }

        internal bool PropagationStopped { get; private set; }

        [JSExport]
        public JSValue? ReturnValue { get; internal set; }

        [JSExport]
        public JSValue Bubbles { get; }

        [JSExport]
        public JSValue Cancelable { get; }

        [JSExport]
        public JSValue Composed { get; }

        [JSExport]
        public double Timestamp { get; }

        /// <summary>
        /// JavaScript Arguments Constructor is faster to invoke
        /// </summary>
        /// <param name="a"></param>
        public Event(in Arguments a) : base(a)
        {
            Type = a[0]?.ToString() ?? throw new InvalidOperationException($"type is a required field");

            this.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            var options = a[1];
            if (options == null)
            {
                this.Bubbles = JSBoolean.False;
                this.Cancelable = JSBoolean.False;
                this.Composed = JSBoolean.False;
                return;
            }

            this.Bubbles = options[KeyStrings.bubbles].BooleanValue ? JSBoolean.True : JSBoolean.False;
            this.Cancelable = options[KeyStrings.cancelable].BooleanValue ? JSBoolean.True : JSBoolean.False;
            this.Composed = options[KeyStrings.composed].BooleanValue ? JSBoolean.True : JSBoolean.False;
        }


        public static Event Create(string type)
        {
            return new Event(type);
        }
        private Event(string type): base(Arguments.Empty)
        {
            this.Type = type;
            this.Bubbles = JSBoolean.False;
            this.Composed = JSBoolean.False;
            this.Cancelable = JSBoolean.False;
        }

        [JSExport]
        public void StopPropoagation()
        {
            PropagationStopped = true;
        }


        [JSExport]
        public void StopImmediatePropoagation()
        {
            PropagationStopped = true;
        }
    }
}
