using System;
using System.Linq;
using System.Linq.Expressions;
using Tardigrade.Framework.Helpers;
using Tardigrade.Shared.Tests.Models;
using Xunit;

namespace Tardigrade.Framework.Tests
{
    public class ExpressionHelperTest
    {
        [Fact]
        public void TryParsePath_ValidExpressionUserCredentialsCredentialIssuer_Success()
        {
            Expression<Func<User, object>> expression = u => u.UserCredentials.Select(uc => uc.LatestCredential.LatestIssuer);
            bool isParseSuccessful = ExpressionHelper.TryParsePath(expression.Body, out string path);
            Assert.True(isParseSuccessful);
            Assert.Equal("UserCredentials.LatestCredential.LatestIssuer", path);
        }

        [Fact]
        public void TryParsePath_ValidExpressionUserCredentialsCredentialIssuers_Success()
        {
            Expression<Func<User, object>> expression = u => u.UserCredentials.Select(uc => uc.LatestCredential.Issuers);
            bool isParseSuccessful = ExpressionHelper.TryParsePath(expression.Body, out string path);
            Assert.True(isParseSuccessful);
            Assert.Equal("UserCredentials.LatestCredential.Issuers", path);
        }

        [Fact]
        public void TryParsePath_ValidExpressionUserCredentialsCredentials_Success()
        {
            Expression<Func<User, object>> expression = u => u.UserCredentials.Select(uc => uc.Credentials);
            bool isParseSuccessful = ExpressionHelper.TryParsePath(expression.Body, out string path);
            Assert.True(isParseSuccessful);
            Assert.Equal("UserCredentials.Credentials", path);
        }

        [Fact]
        public void TryParsePath_ValidExpressionUserCredentialsCredentialsIssuer_Success()
        {
            Expression<Func<User, object>> expression =
                u => u.UserCredentials.Select(uc => uc.Credentials.Select(c => c.LatestIssuer));
            bool isParseSuccessful = ExpressionHelper.TryParsePath(expression.Body, out string path);
            Assert.True(isParseSuccessful);
            Assert.Equal("UserCredentials.Credentials.LatestIssuer", path);
        }
    }
}