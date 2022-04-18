using System;
using System.Collections.Generic;
using System.Text;
using YantraJS.Core;
using YantraJS.Expressions;

namespace YantraJS.Generator
{
    public class TryCatchLabelMarker: YExpressionMapVisitor
    {
        private ILTryBlock tryBlock;
        private readonly LabelInfo labels;

        public TryCatchLabelMarker(ILTryBlock tryBlock, LabelInfo labels)
        {
            this.tryBlock = tryBlock;
            this.labels = labels;
        }


        public static void Collect(YTryCatchFinallyExpression body, ILTryBlock tryBlock, LabelInfo labels)
        {
            TryCatchLabelMarker t = new TryCatchLabelMarker(tryBlock, labels);
            t.Visit(body.Try);
            if (body.Catch != null)
                t.Visit(body.Catch.Body);
            if (body.Finally != null)
                t.Visit(body.Finally);
        }

        protected override YExpression VisitLabel(YLabelExpression yLabelExpression)
        {
            labels.Create(yLabelExpression.Target, tryBlock, false);
            return base.VisitLabel(yLabelExpression);
        }

        //protected override YExpression VisitReturn(YReturnExpression yReturnExpression)
        //{
        //    labels.Create(yReturnExpression.Target, tryBlock, false);
        //    return base.VisitReturn(yReturnExpression);
        //}

        protected override YExpression VisitLoop(YLoopExpression yLoopExpression)
        {
            labels.Create(yLoopExpression.Break, tryBlock, false);
            labels.Create(yLoopExpression.Continue, tryBlock, false);
            return base.VisitLoop(yLoopExpression);
        }

        protected override YExpression VisitTryCatchFinally(YTryCatchFinallyExpression tryCatchFinallyExpression)
        {
            return tryCatchFinallyExpression;
        }

        protected override YExpression VisitLambda(YLambdaExpression yLambdaExpression)
        {
            return yLambdaExpression;
        }

        //protected override YExpression VisitRelay(YRelayExpression relayExpression)
        //{
        //    return relayExpression;
        //}
    }
}
