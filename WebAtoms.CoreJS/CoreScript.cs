using Esprima.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WebAtoms.CoreJS.Core;

namespace WebAtoms.CoreJS
{
    public struct ScriptFunction
    {
        public System.Linq.Expressions.Expression This;

        public System.Linq.Expressions.Expression Parameters;

        public System.Linq.Expressions.Expression Body;

        public ScriptLocation Location;
    }

    public struct ScriptLocation
    {
        public string Location { get; set; }

        public Esprima.Ast.INode Node { get; set; }
    }

    public class CoreScript
    {
        public Expression<JSFunctionDelegate> MethodExpression { get; }
        public JSFunctionDelegate Method { get; }

        private static Dictionary<Type, Func<CoreScript, Esprima.Ast.INode, Expression, Expression, Expression>> visitors;

        static CoreScript()
        {
            visitors = new Dictionary<Type, Func<CoreScript, Esprima.Ast.INode, Expression, Expression, Expression>>();

            foreach(var method in typeof(CoreScript).GetMethods().Where((x) => 
                x.Name == "Visit" 
                && x.ReturnType == typeof(System.Linq.Expressions.Expression) 
                && x.GetParameters().Length == 3
                && typeof(Esprima.Ast.INode).IsAssignableFrom(x.GetParameters()[0].ParameterType))
                .Select((x) => (type: x.GetParameters()[0].ParameterType, method:x)))
            {
                var self = Expression.Parameter(typeof(CoreScript));
                var node = Expression.Parameter(typeof(Esprima.Ast.INode));
                var te = Expression.Parameter(typeof(System.Linq.Expressions.Expression));
                var args = Expression.Parameter(typeof(System.Linq.Expressions.Expression));
                var body = Expression.Call(self, method.method, node, te, args);
                var lambda = Expression.Lambda<Func<CoreScript, Esprima.Ast.INode, Expression, Expression, Expression>>(body, self, node, te, args);
                visitors[method.type] = lambda.Compile();
            }
        }

        public CoreScript(string code, string location = null)
        {

            Esprima.JavaScriptParser parser =
                new Esprima.JavaScriptParser(code, new Esprima.ParserOptions {
                Loc = true,
                SourceType = SourceType.Script
                });

            var script = parser.ParseScript();

            var thisExpression = Expression.Parameter(typeof(JSValue));

            var parameterExpression = Expression.Parameter(typeof(JSArray));

            this.MethodExpression = Expression.Lambda<JSFunctionDelegate>(Prepare(script, thisExpression, parameterExpression));

            this.Method = this.MethodExpression.Compile();

        }
        Expression Prepare(
            Esprima.Ast.INode node, 
            Expression thisExpression, 
            Expression parameterExpression)
        {
            if(!visitors.TryGetValue(node.GetType(), out var fx))
            {
                throw new Exception($"No visitor for type ${node.GetType().FullName}");
            }
            return fx(this,
                node,
                thisExpression,
                parameterExpression);
        }

        private Expression Call<T>(Func<JSValue[], T> d, IEnumerable<Expression> expList)
        {
            return Expression.Call(d.Method, expList);
        }

        public Expression Visit(Esprima.Ast.Script script, Expression t, Expression a)
        {
            return Expression.Block(script.Body.Select((x) => Prepare(x, t, a)));
        }


        public Expression Visit(Esprima.Ast.ArrayExpression ae, Expression t, Expression a)
        {
            return Call(JSArray.NewInstance, ae.Elements.Select((x) => Prepare(x, t, a)));
        }

        public Expression Visit(Esprima.Ast.BinaryExpression be, Expression t, Expression a)
        {
            return Expression.Constant(JSUndefined.Value);
        }
        

    }
}
