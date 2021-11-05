using System.Collections.Generic;
using System.Net;
using Tardigrade.Framework.Extensions;
using Tardigrade.Framework.Models.Rest;
using Tardigrade.Framework.Rest;
using Tardigrade.Framework.RestSharp.Tests.Clients;
using Tardigrade.Framework.RestSharp.Tests.Factories;
using Tardigrade.Framework.RestSharp.Tests.Models;
using Xunit;
using Xunit.Abstractions;

namespace Tardigrade.Framework.RestSharp.Tests
{
    public class UsersRestClientTest
    {
        private readonly IRestClient _client;
        private readonly ITestOutputHelper _output;

        public UsersRestClientTest(ITestOutputHelper output)
        {
            _output = output;
            _client = new UsersRestClient();
        }

        [Fact]
        public void Crud_NewUser_Success()
        {
            // Create.
            User original = DataFactory.RandomUser;
            Response<User> created = _client.Post<User, User>(content: original);
            Assert.Equal(HttpStatusCode.Created, created.StatusCode);
            Assert.NotNull(created.Data);
            Assert.NotNull(created.Data.Id);
            Assert.Equal(original.Email, created.Data.Email);
            Assert.Equal(original.FirstName, created.Data.FirstName);
            Assert.Equal(original.LastName, created.Data.LastName);
            _output.WriteLine($"Created new user:\n{created.Data.ToJson()}");

            // Retrieve single.
            Response<User> retrieved =  _client.Get<User>(created.Data.Id);
            Assert.Equal(HttpStatusCode.OK, retrieved.StatusCode);
            Assert.NotNull(retrieved.Data);
            Assert.Equal(created.Data.Id, retrieved.Data.Id);
            _output.WriteLine($"Retrieved newly created user:\n{retrieved.ToJson()}");

            // Update.
            const string newMiddleName = "Perry";
            User retrievedUser = retrieved.Data;
            retrievedUser.MiddleNames = newMiddleName;
            Response updatedResponse =  _client.Put(retrievedUser.Id, retrievedUser);
            Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);

            Response<User> updated =  _client.Get<User>(retrievedUser.Id);
            Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
            Assert.NotNull(updated.Data);
            Assert.Equal(retrievedUser.Id, updated.Data.Id);
            Assert.Equal(newMiddleName, updated.Data.MiddleNames);
            _output.WriteLine($"Updated the MiddleNames property of the newly created user:\n{updated.ToJson()}");

            // Delete.
            Response deleteResponse =  _client.Delete(created.Data.Id);
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            Response headResponse =  _client.Head(created.Data.Id);
            Assert.Equal(HttpStatusCode.NotFound, headResponse.StatusCode);
            _output.WriteLine($"Successfully deleted user {created.Data.Id}.");
        }

        [Fact]
        public async void CrudAsync_NewUser_Success()
        {
            // Create.
            User original = DataFactory.RandomUser;
            Response<User> created = await _client.PostAsync<User, User>(content: original);
            Assert.Equal(HttpStatusCode.Created, created.StatusCode);
            Assert.NotNull(created.Data);
            Assert.NotNull(created.Data.Id);
            Assert.Equal(original.Email, created.Data.Email);
            Assert.Equal(original.FirstName, created.Data.FirstName);
            Assert.Equal(original.LastName, created.Data.LastName);
            _output.WriteLine($"Created new user:\n{created.Data.ToJson()}");

            // Retrieve single.
            Response<User> retrieved = await _client.GetAsync<User>(created.Data.Id);
            Assert.Equal(HttpStatusCode.OK, retrieved.StatusCode);
            Assert.NotNull(retrieved.Data);
            Assert.Equal(created.Data.Id, retrieved.Data.Id);
            _output.WriteLine($"Retrieved newly created user:\n{retrieved.ToJson()}");

            // Update.
            const string newMiddleName = "Perry";
            User retrievedUser = retrieved.Data;
            retrievedUser.MiddleNames = newMiddleName;
            Response updatedResponse = await _client.PutAsync(retrievedUser.Id, retrievedUser);
            Assert.Equal(HttpStatusCode.OK, updatedResponse.StatusCode);

            Response<User> updated = await _client.GetAsync<User>(retrievedUser.Id);
            Assert.Equal(HttpStatusCode.OK, updated.StatusCode);
            Assert.NotNull(updated.Data);
            Assert.Equal(retrievedUser.Id, updated.Data.Id);
            Assert.Equal(newMiddleName, updated.Data.MiddleNames);
            _output.WriteLine($"Updated the MiddleNames property of the newly created user:\n{updated.ToJson()}");

            // Delete.
            Response deleteResponse = await _client.DeleteAsync(created.Data.Id);
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            Response headResponse = await _client.HeadAsync(created.Data.Id);
            Assert.Equal(HttpStatusCode.NotFound, headResponse.StatusCode);
            _output.WriteLine($"Successfully deleted user {created.Data.Id}.");
        }

        [Fact]
        public async void Retrieve_ExistingUser_Success()
        {
            // Arrange.
            const string userId = "e6fc9cd91fd849d3";

            // Act.
            Response<User> response = await _client.GetAsync<User>(userId);

            // Assert.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.Equal(userId, response.Data.Id);

            string content = response.Data?.ToJson();
            _output.WriteLine($"Status code: {response.StatusCode}\nContent: {content}");
        }

        [Fact]
        public async void RetrieveAll_ExistingUsers_Success()
        {
            // Arrange.

            // Act.
            Response<IList<User>> response = await _client.GetAllAsync<User>();

            // Assert.
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);

            string content = response.Data?.ToJson();
            _output.WriteLine($"Status code: {response.StatusCode}\nContent: {content}");
        }
    }
}