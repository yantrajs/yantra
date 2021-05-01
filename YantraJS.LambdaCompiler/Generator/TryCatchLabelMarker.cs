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
        }

        protected override YExpression VisitLabel(YLabelExpression yLabelExpression)
        {
            labels.Create(yLabelExpression.Target, tryBlock);
            return base.VisitLabel(yLabelExpression);
        }

        protected override YExpression VisitTryCatchFinally(YTryCatchFinallyExpression tryCatchFinallyExpression)
        {
            return tryCatchFinallyExpression;
        }

    }
}
