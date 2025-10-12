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
using YantraJS.Core;
using YantraJS.Emit;
using YantraJS.Utils;
namespace YantraJS.Tests.Generator
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
    public class ES2023
    {
        [TestFolder("es\\2023")]
        public void Disposables()
        {

        }

    }

    [TestClass]
    public class ES2025
    {
        [TestFolder("es\\2025\\decimal")]
        public void Decimals()
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

    public class TestContextTraceListener : TraceListener
    {
        private TestContext value;

        public TestContextTraceListener(TestContext value)
        {
            this.value = value;
        }

        public override void Write(string message)
        {
            value.Write(message);
        }

        public override void WriteLine(string message)
        {
            value.WriteLine(message);
        }
    }

    [TestClass]
    public class Promise
    {

        public TestContext _testContext;

        public TestContext TestContext
        {
            get => _testContext;
            set
            {
                _testContext = value;
                Trace.Listeners.Add(new TestContextTraceListener(value));
                value.WriteLine("Test context set");
            }
        }

        // [AsyncTestFolder("es5\\Promise")]
        public void Run()
        {

        }

        [AsyncTestFolder("es6\\Async")]
        public void RunAsync()
        {

        }

    }

}
