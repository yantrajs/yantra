using Microsoft.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAtoms.CoreJS.Core;
using WebAtoms.CoreJS.Emit;
using WebAtoms.CoreJS.Utils;
namespace WebAtoms.CoreJS.Tests.Generator
{


    [TestClass]
    public class ES5
    {
        [TestFolder("es5\\Objects")]
        public void Objects()
        {
            
        }


        [TestFolder("es5\\Objects\\Date")]
        public void Date()
        {

        }

    }

    [TestClass]
    public class Statements
    {
        [TestFolder("es5\\Statements")]
        public void Run()
        {

        }

    }

    [TestClass]
    public class Syntax
    {
        [TestFolder("es5\\Syntax")]
        public void Run()
        {

        }

    }


    [TestClass]
    public class Function
    {
        [TestFolder("es5\\Function")]
        public void Run()
        {

        }
    }

    [TestClass]
    public class ES6
    {
        [TestFolder("es6\\Syntax")]
        public void Syntax()
        {

        }

    }

    [TestClass]
    public class Modules
    {
        [ModuleFolder("es6\\Modules\\clr")]
        public void Clr()
        {

        }

    }



    [TestClass]
    public class String
    {
        [TestFolder("es5\\String")]
        public void Run()
        {
            
        }
    }

    [TestClass]
    public class Array
    {
        [TestFolder("es5\\Array")]
        public void Run()
        {

        }
    }

    [TestClass]
    public class Number
    {
        [TestFolder("es5\\Number")]
        public void Run()
        {

        }
    }

    [TestClass]
    public class Math
    {
        [TestFolder("es5\\Math")]
        public void Run()
        {

        }
    }

    [TestClass]
    public class Promise
    {
        [AsyncTestFolder("es5\\Promise")]
        public void Run()
        {

        }

        // [AsyncTestFolder("es6\\Async")]
        public void RunAsync()
        {

        }

    }

}
