namespace Clamp.Linker.ViewEngines.Benben.Compiler.Resolvers
{
    public interface IExpressionNameResolver
    {
        string ResolveExpressionName(object instance, string expressionName);
    }
}