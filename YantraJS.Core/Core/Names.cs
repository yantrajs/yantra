#nullable enable
using Yantra;
using Yantra.Core;

namespace YantraJS.Core
{
    [JSRegistrationGenerator]
    internal static partial class Names
    {
        public static void RegisterGeneratedClasses(this JSContext context)
        {
            RegisterAll(context);
        }
    }
}
