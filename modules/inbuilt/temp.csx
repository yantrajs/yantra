#r "nuget: YantraJS.Core,1.2.1"
using System;
using YantraJS.Core;
using YantraJS.Core.Clr;

public class Product {

    public string Name {get;set;}

    public int Age{get;set;}

}

[Export]
public class Demo {


    public static string Print(Product product) {
        return $"Name is {product.Name} and age is {product.Age}";
    }

    public string Add(string a, string b) {
        return $"{a} {b}";
    }

    public string Add(string a, string b, string c) {
        return $"{a} {b} {c}";
    }

}