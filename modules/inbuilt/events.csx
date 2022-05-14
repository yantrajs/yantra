#r "nuget: YantraJS.Core,1.2.1"
#r "nuget: YantraJS.NodePollyfill,1.1.107"
using System;
using System.Linq;
using System.Collections.Generic;
using YantraJS.Core;
using YantraJS.Core.Clr;
using YantraJS.Core.Core.Storage;


[Export]
public class EventEmitter: YantraJS.NodePollyfill.EventEmitter {

}
