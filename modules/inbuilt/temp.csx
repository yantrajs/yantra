#r "nuget: YantraJS.Core, 1.0.1-CI-20201024-043043"

using System;
using WebAtoms.CoreJS.Core;

public class Product {

    public string Name {get;set;}

    public int Age{get;set;}

}

[Export]
public class Demo {


    public static string Print(Product product) {
        return $"Name is {product.Name} and age is {product.Age}";
    }


}