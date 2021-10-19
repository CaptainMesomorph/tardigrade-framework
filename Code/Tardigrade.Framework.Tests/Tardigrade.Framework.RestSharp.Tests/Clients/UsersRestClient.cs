using RestSharp;
using RestSharp.Authenticators;
using System;

namespace Tardigrade.Framework.RestSharp.Tests.Clients
{
    internal class UsersRestClient : RestSharpClient
    {
        public UsersRestClient() : base(new Uri("https://dev.tikforce.com/users-dev/"))
        {
            var authenticator = new HttpBasicAuthenticator("tikmeapi", "_x36pgY;9!]ZC8]A");
            RestClient.Authenticator = authenticator;
            RestClient.AddDefaultHeader("Authorization-Token", "8CE494A8-0DFF-4DEC-B1F7-972BDA43F0C1");
            RestClient.AddDefaultHeader("Ocp-Apim-Subscription-Key", "b423ac3302e844db922c637450a1cc34");
        }
    }
}