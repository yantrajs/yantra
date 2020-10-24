#r "nuget: YantraJS.Core, 1.0.1-CI-20201024-043043"

using System;
using WebAtoms.CoreJS.Core;


[Export]
public class JSUrl {

    private Uri uri;

    public JSUrl(in Arguments a) {
        this.uri = new Uri(a.Get1().ToString());
    }

    public string Host => uri.Host;

}