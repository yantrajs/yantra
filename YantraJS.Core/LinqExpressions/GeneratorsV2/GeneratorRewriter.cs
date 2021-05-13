using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using YantraJS.Core.CodeGen;
using YantraJS.Core.LinqExpressions.Generators;
using YantraJS.ExpHelper;

namespace YantraJS.Core.LinqExpressions.GeneratorsV2
{

    public class FlattenBlocks: ExpressionVisitor
    {
        protected override Expression VisitBinary(BinaryExpression node)
        {

            if(Flatten(node.Right, last => node.Update(node.Left, node.Conversion, last), out var result))
            {
                return result;
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            if(node.Kind == GotoExpressionKind.Return)
            {
                if (Flatten(node.Value, x => node.Update(node.Target, x), out var block))
                    return block;
            }
            return base.VisitGoto(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            var vars = new List<ParameterExpression>();
            var args = new List<Expression>();
            var list = new List<Expression>();
            foreach(var a in node.Arguments)
            {
                var e = Visit(a);
                if(e.NodeType == ExpressionType.Block && e is BlockExpression block)
                {
                    vars.AddRange(block.Variables);
                    var p = Expression.Parameter(e.Type);
                    vars.Add(p);
                    args.Add(p);
                    var length = block.Expressions.Count;
                    var last = length - 1;
                    for (int i = 0; i < length; i++)
                    {
                        var be = Visit(block.Expressions[i]);
                        if(i == last)
                        {
                            list.Add(Expression.Assign(p, be));
                            continue;
                        }
                        list.Add(be);
                    }
                    continue;
                }
                args.Add(e);
            }
            list.Add(Expression.New(node.Constructor, args));
            // return base.VisitNew(node);
            return Expression.Block(vars, list);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            var vars = new List<ParameterExpression>( node.Variables);
            var list = new List<Expression>();
            foreach(var e in node.Expressions)
            {
                var visited = Visit(e);
                if(visited.NodeType == ExpressionType.Block && visited is BlockExpression block)
                {
                    vars.AddRange(block.Variables);
                    list.AddRange(block.Expressions);
                    continue;
                }
                list.Add(visited);
            }
            return Expression.Block(vars, list);
        }

        private bool Flatten(Expression exp, Func<Expression, Expression> p, out Expression result)
        {
            if (exp.NodeType != ExpressionType.Block) {
                result = null;
                return false;
            }
            var block = exp as BlockExpression;
            
            var list = new List<Expression>();
            var vars = block.Variables;
            var length = block.Expressions.Count;
            var last = length - 1;
            for (int i = 0; i < length; i++)
            {
                var e = block.Expressions[i];
                if (last == i)
                {
                    list.Add(p(Visit(e)));
                    continue;
                }
                list.Add(Visit(e));
            }
            result = Expression.Block(vars, list);
            return true;
            
        }
    }

    public class MethodRewriter
    {
        public class Finder: ExpressionVisitor
        {

            internal bool hasYield;

            protected override Expression VisitExtension(Expression node)
            {
                if (node is YieldExpression yield)
                {
                    hasYield = true;
                    var a = Visit(yield.Argument);
                    if (a == yield.Argument)
                        return yield;
                    return YieldExpression.New(a);
                }
                return base.VisitExtension(node);
            }

        }

        public static Expression Rewrite(MethodCallExpression exp)
        {
            var rw = new Rewriter();
            return rw.Visit(exp);

        }

        public class Rewriter: ExpressionVisitor
        {

            protected override Expression VisitExtension(Expression node)
            {
                if(node is YieldExpression yield)
                {
                    var a = Visit(yield.Argument);
                    if (a == yield.Argument)
                        return yield;
                    return YieldExpression.New(a);
                }
                return base.VisitExtension(node);
            }

            protected override Expression VisitNew(NewExpression node)
            {
                var f = new Finder();
                f.Visit(node);
                if (f.hasYield)
                {
                    var argList = new List<ParameterExpression>();
                    var setup = new List<Expression>();
                    foreach (var a in node.Arguments)
                    {
                        var p = Expression.Parameter(a.Type);
                        argList.Add(p);
                        setup.Add(Expression.Assign(p, Visit(a)));
                    }
                    setup.Add(Expression.New(node.Constructor, argList));
                    return Expression.Block(argList.ToArray(),
                        setup);

                }
                return base.VisitNew(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {

                var f = new Finder();
                f.Visit(node);
                if(f.hasYield)
                {
                    // rewrite...

                    var argList = new List<ParameterExpression>();
                    var setup = new List<Expression>();
                    foreach(var a in node.Arguments)
                    {
                        var p = Expression.Parameter(a.Type);
                        argList.Add(p);
                        setup.Add(Expression.Assign(p, Visit(a)));
                    }
                    setup.Add(Expression.Call(node.Object, node.Method, argList));
                    return Expression.Block(argList.ToArray(),
                        setup);
                }

                return node;
            }
        }

    }


    public class GeneratorRewriter: ExpressionVisitor
    {

        private ParameterExpression pe;
        private readonly ParameterExpression args;
        private readonly ParameterExpression nextJump;
        private readonly ParameterExpression nextValue;
        private readonly ParameterExpression exception;
        private readonly MemberExpression StackItem;
        private readonly MemberExpression Context;
        private readonly MemberExpression ScriptInfo;
        private readonly MemberExpression Closures;
        private LabelTarget generatorReturn;
        private readonly List<(ParameterExpression original, ParameterExpression box, int index)> lifted;
        private LabelTarget @return;
        private readonly ParameterExpression replaceArgs;
        private readonly ParameterExpression replaceStackItem;
        private readonly ParameterExpression replaceContext;
        private readonly ParameterExpression replaceScriptInfo;
        private readonly ParameterExpression replaceClosures;
        private List<(LabelTarget label, int id)> jumps = new List<(LabelTarget label, int id)>();

        public GeneratorRewriter(
            ParameterExpression pe, 
            LabelTarget @return, 
            ParameterExpression replaceArguments,
            ParameterExpression replaceStackItem,
            ParameterExpression replaceContext,
            ParameterExpression replaceScriptInfo,
            ParameterExpression replaceClosures)
        {
            this.pe = pe;
            this.args = Expression.Parameter(typeof(Arguments).MakeByRefType(), "args");
            this.nextJump = Expression.Parameter(typeof(int), "nextJump");
            this.nextValue = Expression.Parameter(typeof(JSValue), "nextValue");
            this.exception = Expression.Parameter(typeof(Exception), "ex");
            this.StackItem = Expression.Field(pe, "StackItem");
            this.Context = Expression.Field(pe, "Context");
            this.ScriptInfo = Expression.Field(pe, "ScriptInfo");
            this.Closures = Expression.Field(pe, "Closures");
            this.replaceArgs = replaceArguments;
            this.replaceStackItem = replaceStackItem;
            this.replaceContext = replaceContext;
            this.replaceScriptInfo = replaceScriptInfo;
            this.replaceClosures = replaceClosures;
            this.@return = @return;
            this.generatorReturn = Expression.Label(typeof(GeneratorState), "RETURN");
            this.lifted = new List<(ParameterExpression original, ParameterExpression box, int index)>();
        }

        public static LambdaExpression Rewrite(
           Expression body,
           LabelTarget r,
           ParameterExpression generator,
           ParameterExpression replaceArgs,
           ParameterExpression replaceStackItem,
           ParameterExpression replaceContext,
           ParameterExpression replaceScriptInfo,
           ParameterExpression replaceClosures)
        {
            var gw = new GeneratorRewriter(generator, r, replaceArgs, replaceStackItem, replaceContext, replaceScriptInfo, replaceClosures);
            var flatten = new FlattenBlocks();
            var innerBody = flatten.Visit( gw.Visit(body));

            // setup jump table...

            var jumpExp = gw.GenerateJumps();

            var (boxes, inits) = gw.LoadBoxes();

            var newBody = Expression.Block(
                boxes,

                // load boxes...
                inits,

                jumpExp,
                innerBody,
                Expression.Label(gw.generatorReturn, GeneratorStateBuilder.New(0))
                );

            return Expression.Lambda<JSGeneratorDelegateV2>(newBody, generator, gw.args, gw.nextJump, gw.nextValue, gw.exception);
        }

        private (ParameterExpression[] boxes, Expression init) LoadBoxes()
        {
            List<Expression> boxes = new List<Expression>(lifted.Count);
            List<ParameterExpression> vlist = new List<ParameterExpression>(lifted.Count);
            foreach(var v in lifted)
            {
                vlist.Add(v.box);
                boxes.Add(Expression.Assign( v.box, ClrGeneratorV2Builder.GetVariable(pe, v.index, v.original.Type )));
            }
            return (vlist.ToArray(), Expression.Block(boxes));
        }

        private Expression GenerateJumps()
        {
            var cases = new SwitchCase[jumps.Count];
            for (int i = 0; i < jumps.Count; i++)
            {
                var (label, id) = jumps[i];
                cases[i] = Expression.SwitchCase(Expression.Goto(label), Expression.Constant(id));
            }
            return Expression.Switch(nextJump, cases);
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            List<Expression> list = new List<Expression>();
            foreach(var v in node.Variables)
            {
                int index = lifted.Count;
                var box = Expression.Parameter(typeof(Box<>).MakeGenericType(v.Type));
                lifted.Add((v, box, index));
            }
            foreach(var s in node.Expressions)
            {
                list.Add(Visit(s));
            }
            return Expression.Block(
                list
                );
        }

        protected override Expression VisitGoto(GotoExpression node)
        {
            if(node.Kind == GotoExpressionKind.Return)
            {
                return Expression.Return(generatorReturn,
                    GeneratorStateBuilder.New(Visit(node.Value), 0));
            }
            return base.VisitGoto(node);
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node == replaceArgs)
                return args;
            if (node == replaceStackItem)
                return StackItem;
            if (node == replaceContext)
                return Context;
            if (node == replaceScriptInfo)
                return ScriptInfo;
            if (node == replaceClosures)
                return Closures;
            foreach(var l in lifted)
            {
                if (l.original == node)
                    return Expression.Field( l.box, "Value");
            }
            return base.VisitParameter(node);
        }

        private (LabelTarget label,int id) GetNextYieldJumpTarget()
        {
            int id = jumps.Count + 1;
            var label = Expression.Label(typeof(void), "next" + id);
            var r = (label, id);
            jumps.Add(r);
            return r;
        }

        protected override Expression VisitExtension(Expression node)
        {
            if (node is YieldExpression @yield) {
                var arg = Visit(yield.Argument);
                var (label, id) = GetNextYieldJumpTarget();
                return Expression.Block(
                    Expression.Return(generatorReturn, 
                        GeneratorStateBuilder.New( arg, id)),
                    Expression.Label(label),
                    nextValue
                    );

            }
            return base.VisitExtension(node);
        }

        /// <summary>
        /// Yield requires flattening as you cannot jump in and out in middle
        /// of call
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var rewritten = MethodRewriter.Rewrite(node);
            if (rewritten == node)
                return base.VisitMethodCall(node);
            return Visit(rewritten);
        }

        protected override Expression VisitTry(TryExpression node)
        {
            var hasFinally = node.Finally != null;
            var @catch = node.Handlers.FirstOrDefault();
            var hasCatch = @catch != null;

            LabelTarget catchLabel = null;
            int catchId = 0;
            LabelTarget finallyLabel = null;
            int finallyId = 0;

            var tryList = new List<Expression>();
            if (hasCatch)
            {
                (catchLabel, catchId) = GetNextYieldJumpTarget();
            }
            if (hasFinally)
            {
                (finallyLabel, finallyId) = GetNextYieldJumpTarget();
            }

            tryList.Add(ClrGeneratorV2Builder.Push(pe, catchId, finallyId));
            tryList.Add(Visit(node.Body));
            tryList.Add(ClrGeneratorV2Builder.Pop(pe));
            if (hasFinally)
            {
                tryList.Add(Expression.Goto(finallyLabel));
            }

            if (hasCatch) {
                tryList.Add(Expression.Label(catchLabel));
                tryList.Add(Expression.Assign(Visit(@catch.Variable), exception));
                tryList.Add(Visit(@catch.Body));

                if (hasFinally)
                {
                    tryList.Add(Expression.Goto(finallyLabel));
                }
            }

            if (hasFinally)
            {
                tryList.Add(Expression.Label(finallyLabel));
                tryList.Add(Visit(node.Finally));
            }

            return Expression.Block(tryList);
        }

    }

    public class GeneratorStateBuilder
    {
        private static Type type = typeof(GeneratorState);

        private static ConstructorInfo _newFromValue =
            type.PublicConstructor(typeof(JSValue), typeof(int));

        public static Expression New(Expression value, int id)
        {
            return Expression.New(_newFromValue, value, Expression.Constant(id));
        }

        public static Expression New(int id)
        {
            return Expression.New(_newFromValue, Expression.Constant(null, typeof(JSValue)), Expression.Constant(id));
        }

    }

    public class ClrGeneratorV2Builder
    {
        private static Type type = typeof(ClrGeneratorV2);

        private static MethodInfo _push = type.PublicMethod(
            nameof(ClrGeneratorV2.PushTry),
            typeof(int),
            typeof(int));

        private static MethodInfo _pop = type.PublicMethod(
            nameof(ClrGeneratorV2.Pop));


        private static MethodInfo _GetVariable
            = type.GetMethod("GetVariable");

        public static Expression Push(Expression exp, int c, int f)
        {
            return Expression.Call(exp, _push, Expression.Constant(c), Expression.Constant(f));
        }

        internal static Expression GetVariable(ParameterExpression pe, int id, Type type)
        {
            return Expression.Call(pe, _GetVariable.MakeGenericMethod(type), Expression.Constant(id));
        }

        internal static Expression Pop(ParameterExpression pe)
        {
            return Expression.Call(pe, _pop);
        }
    }
}
