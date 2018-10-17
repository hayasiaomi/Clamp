using System;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal abstract class HandlebarsExpressionVisitor : ExpressionVisitor
    {
        private readonly CompilationContext _compilationContext;

        protected HandlebarsExpressionVisitor(CompilationContext compilationContext)
        {
            _compilationContext = compilationContext;
        }

        protected virtual CompilationContext CompilationContext
        {
            get { return _compilationContext; }
        }

        public override Expression Visit(Expression exp)
        {
            if (exp == null)
            {
                return null;
            }
            switch ((HtmlTemplateExpressionType)exp.NodeType)
            {
                case HtmlTemplateExpressionType.StatementExpression:
                    return VisitStatementExpression((StatementExpression)exp);
                case HtmlTemplateExpressionType.StaticExpression:
                    return VisitStaticExpression((StaticExpression)exp);
                case HtmlTemplateExpressionType.HelperExpression:
                    return VisitHelperExpression((HelperExpression)exp);
                case HtmlTemplateExpressionType.BlockExpression:
                    return VisitBlockHelperExpression((BlockHelperExpression)exp);
                case HtmlTemplateExpressionType.HashParametersExpression:
                    return VisitHashParametersExpression((HashParametersExpression)exp);
                case HtmlTemplateExpressionType.PathExpression:
                    return VisitPathExpression((PathExpression)exp);
                case HtmlTemplateExpressionType.IteratorExpression:
                    return VisitIteratorExpression((IteratorExpression)exp);
                case HtmlTemplateExpressionType.DeferredSection:
                    return VisitDeferredSectionExpression((DeferredSectionExpression)exp);
                case HtmlTemplateExpressionType.PartialExpression:
                    return VisitPartialExpression((PartialExpression)exp);
                case HtmlTemplateExpressionType.BoolishExpression:
                    return VisitBoolishExpression((BoolishExpression)exp);
                case HtmlTemplateExpressionType.SubExpression:
                    return VisitSubExpression((SubExpressionExpression)exp);
                default:
                    return base.Visit(exp);
            }
        }

        protected virtual Expression VisitStatementExpression(StatementExpression sex)
        {
            return sex;
        }

        protected virtual Expression VisitPathExpression(PathExpression pex)
        {
            return pex;
        }

        protected virtual Expression VisitHelperExpression(HelperExpression hex)
        {
            return hex;
        }

        protected virtual Expression VisitBlockHelperExpression(BlockHelperExpression bhex)
        {
            return bhex;
        }

        protected virtual Expression VisitStaticExpression(StaticExpression stex)
        {
            return stex;
        }

        protected virtual Expression VisitIteratorExpression(IteratorExpression iex)
        {
            return iex;
        }

        protected virtual Expression VisitDeferredSectionExpression(DeferredSectionExpression dsex)
        {
            return dsex;
        }

        protected virtual Expression VisitPartialExpression(PartialExpression pex)
        {
            return pex;
        }

        protected virtual Expression VisitBoolishExpression(BoolishExpression bex)
        {
            return bex;
        }

        protected virtual Expression VisitSubExpression(SubExpressionExpression subex)
        {
            return subex;
        }

        protected virtual Expression VisitHashParametersExpression(HashParametersExpression hpex)
        {
            return hpex;
        }
    }
}

