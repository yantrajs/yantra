using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace YantraJS.Core.LinqExpressions.Generators
{

    public delegate Instruction MoveNext(ref JSValue value, ref int label, ref Exception exception);


    /// <summary>
    /// Instructions - source: TypeScript
    /// </summary>
    public enum Instruction: byte { 
        Next = 0,
        Throw = 1,
        Return = 2,
        Jump = 3,
        Yield = 4,
        YieldStar = 5,
        Catch = 6,
        EndFinally = 7
    }


    /*
     https://www.typescriptlang.org/play?target=1#code/GYVwdgxgLglg9mABAKgLYAoCUiDeAoRQxKAJwE9cCjqyYBTAGwBNEBDARgG4rrDV30tRiw4AaNu3FDmbAEzjWAZkydEPXkzj5eO4uUq7eMYInQBCVKmwAjEnVYBrboZr0Zl5y8IBfRBFZQEAAWpnTs2NpevNIsYZ5e3urUvogA7kEwDHTopCB0Kuq+-oEh6PkGujGIdPGIvsAwYKwMDBSROlUAtFyFeN5AA
     */

    public struct TryBlock {
        public int Begin;
        public int Catch;
        public int Finally;
        public int End;
    }

    /**
     * Return value of Generator delegate must be combination of
     * 
     */
    public struct Result {
        public Instruction Instruction;
        public JSValue Value;

        // useful only in case of jump
        public int Jump;
    }

    /**
     * Switch based generator is only perfect
     * 1. As they run on same thread
     * 2. Do not use too much of allocation
     * 3. Breaking statement into lambda equalivalent to Pausable VM could 
     *    run very slow as it would cause too many jumps (most likely tail calls) but
     *    still switch would perform faster.
     * 
     * 
     * Each method actually contains steps to manipulate execution stack. 
     * Block/TryCatch etc cannot directly add anything onto stack as code
     * may not even reach there, so in case of nested logic, every logic
     * just contains a logic to put items on stack and to execute it further.
     * 
     * Instructions cannot be executed within simple block as instructions need to
     * go on Stack in reverse order.
     * 
     * 
     * Generators are very bad for extensive logic, it is recommended to extract 
     * complicated logic into different functions or differnet statements.
     * 
     * Following statements will process slow...
     * 
     */
    public class ClrGenerator
    {

        private Stack<(int start, int @catch, int @finally, int @end)> CatchStack
            = new Stack<(int start, int @catch, int @finally, int end)>();

        private Stack<int> OpStack = new Stack<int>();

        public ClrGenerator(MoveNext moveNext)
        {
            this.moveNext = moveNext;
        }

        private bool stop = false;
        private JSValue result = null;
        private readonly MoveNext moveNext;

        public bool Next(JSValue next, out JSValue value)
        {

            int jump = 1;
            JSValue result = null;
            Instruction inst = Instruction.Next;
            Exception exception = null;

            while(inst != Instruction.Return)
            {
                try
                {
                    inst = moveNext(ref result, ref jump, ref exception);
                    switch (inst)
                    {
                        case Instruction.Next:
                            jump++;
                            continue;
                        case Instruction.Throw:

                            break;
                        case Instruction.Return:
                            break;
                        case Instruction.Jump:
                            continue;
                        case Instruction.Yield:
                            break;
                        case Instruction.YieldStar:
                            break;
                        case Instruction.Catch:
                            break;
                        case Instruction.EndFinally:
                            break;
                        default:
                            break;
                    }
                } catch (Exception ex)
                {
                    if(CatchStack.Count == 0)
                    {
                        throw ex;
                    }
                    var (start, @catch, @finally, end) = CatchStack.Pop();
                    if (@catch == 0)
                    {
                        // goto finally...
                        OpStack.Push(end);
                    } else
                    {
                        // goto catch...
                        OpStack.Push(end);
                        OpStack.Push(@finally);
                    }
                }
            }


            value = null;
            return false;
        }

    }

    public class YieldExpression: Expression
    {
        public YieldExpression New(Expression argument)
        {
            return new YieldExpression(argument);
        }

        private YieldExpression(Expression argument)
        {
            Argument = argument;
        }

        public Expression Argument { get; }

        public override Type Type => Argument.Type;

        public override ExpressionType NodeType => ExpressionType.Extension;
    }

    public class YieldRewriter : ExpressionVisitor
    {
        List<ParameterExpression> lifedVariables = new List<ParameterExpression>();

        public ParameterExpression generator;

        public static Expression Rewrite(Expression body, ParameterExpression generator)
        {
            return (new YieldRewriter(generator)).Visit(body);
        }

        public YieldRewriter(ParameterExpression generator)
        {
            this.generator = generator;
        }

    }

    public class YieldFinder: ExpressionVisitor {

        private bool found = false;

        public static bool ContainsYield(Expression node)
        {
            var finder = new YieldFinder();
            finder.Visit(node);
            return finder.found;
        }

        protected override Expression VisitExtension(Expression node)
        {
            if (node is YieldExpression)
            {
                found = found || true;
            }
            return node;
        }

        public override Expression Visit(Expression node)
        {
            if (node is LambdaExpression)
                return node;
            return base.Visit(node);
        }
    }
}
