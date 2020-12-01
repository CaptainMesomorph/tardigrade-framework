using System;
using System.Linq.Expressions;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the Expression class.
    /// </summary>
    public static class ExpressionExtension
    {
        /// <summary>
        /// Remove conversion operation from the expression.
        /// <a href="https://stackoverflow.com/questions/42904414/multiple-includes-in-ef-core">Multiple Includes() in EF Core</a>
        /// <a href="https://github.com/dotnet/ef6/blob/master/src/EntityFramework/Utilities/ExpressionExtensions.cs#L186">public static Expression RemoveConvert(this Expression expression)</a>
        /// </summary>
        /// <param name="expression">Expression to check.</param>
        /// <returns>Expression without conversion operation.</returns>
        /// <exception cref="ArgumentNullException">expression is null.</exception>
        public static Expression RemoveConvert(this Expression expression)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            while (expression.NodeType == ExpressionType.Convert ||
                   expression.NodeType == ExpressionType.ConvertChecked)
            {
                expression = ((UnaryExpression)expression).Operand;
            }

            return expression;
        }
    }
}