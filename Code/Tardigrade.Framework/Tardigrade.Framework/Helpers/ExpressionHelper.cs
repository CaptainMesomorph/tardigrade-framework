using System;
using System.Linq.Expressions;
using Tardigrade.Framework.Extensions;

namespace Tardigrade.Framework.Helpers
{
    /// <summary>
    /// Helper class to extend the functionality of the Expression class.
    /// </summary>
    public static class ExpressionHelper
    {
        /// <summary>
        /// Called recursively to parse an expression tree representing a property path such as can be passed to
        /// the Include method of IQueryable.
        /// This involves parsing simple property accesses like o =&gt; o.Products as well as calls to Select like
        /// o =&gt; o.Products.Select(p =&gt; p.OrderLines).
        /// <a href="https://stackoverflow.com/questions/42904414/multiple-includes-in-ef-core">Multiple Includes() in EF Core</a>
        /// <a href="https://github.com/dotnet/ef6/blob/master/src/EntityFramework/Internal/DbHelpers.cs#L260">public static bool TryParsePath(Expression expression, out string path)</a>
        /// </summary>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="path">The expression parsed into an include path, or null if the expression did not match.</param>
        /// <returns>True if matching succeeded; false if the expression could not be parsed.</returns>
        /// <exception cref="ArgumentNullException">expression is null.</exception>
        public static bool TryParsePath(Expression expression, out string path)
        {
            if (expression == null) throw new ArgumentNullException(nameof(expression));

            path = null;

            // Removes boxing.
            Expression withoutConvert = expression.RemoveConvert();

            if (withoutConvert is MemberExpression memberExpression)
            {
                string thisPart = memberExpression.Member.Name;

                if (!TryParsePath(memberExpression.Expression, out string parentPart))
                {
                    return false;
                }

                path = parentPart == null ? thisPart : (parentPart + "." + thisPart);
            }
            else if (withoutConvert is MethodCallExpression callExpression)
            {
                if (callExpression.Method.Name == "Select" && callExpression.Arguments.Count == 2)
                {
                    if (!TryParsePath(callExpression.Arguments[0], out string parentPart))
                    {
                        return false;
                    }

                    if (parentPart != null)
                    {
                        if (callExpression.Arguments[1] is LambdaExpression subExpression)
                        {
                            if (!TryParsePath(subExpression.Body, out string thisPart))
                            {
                                return false;
                            }

                            if (thisPart != null)
                            {
                                path = parentPart + "." + thisPart;
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            return true;
        }
    }
}