using System;
using System.Linq;
using System.Linq.Expressions;
using Tardigrade.Framework.Helpers;
using Tardigrade.Framework.Tests.Models;
using Xunit;

namespace Tardigrade.Framework.Tests
{
    public class ExpressionHelperTest
    {
        [Fact]
        public void TryParsePath_ValidCredentialIssuerExpression_Success()
        {
            Expression<Func<User, object>> expression = u => u.UserCredentials.Select(uc => uc.Credential.Issuer);
            bool isParseSuccessful = ExpressionHelper.TryParsePath(expression.Body, out string path);
            Assert.True(isParseSuccessful);
            Assert.Equal("UserCredentials.Credential.Issuer", path);
        }

        [Fact]
        public void TryParsePath_ValidCredentialIssuersExpression_Success()
        {
            Expression<Func<User, object>> expression = u => u.UserCredentials.Select(uc => uc.Credential.Issuers);
            bool isParseSuccessful = ExpressionHelper.TryParsePath(expression.Body, out string path);
            Assert.True(isParseSuccessful);
            Assert.Equal("UserCredentials.Credential.Issuers", path);
        }

        [Fact]
        public void TryParsePath_ValidCredentialsIssuerExpression_Success()
        {
            Expression<Func<User, object>> expression = u => u.UserCredentials.Select(uc => uc.Credentials.Select(c => c.Issuer));
            bool isParseSuccessful = ExpressionHelper.TryParsePath(expression.Body, out string path);
            Assert.True(isParseSuccessful);
            Assert.Equal("UserCredentials.Credentials.Issuer", path);
        }
    }
}