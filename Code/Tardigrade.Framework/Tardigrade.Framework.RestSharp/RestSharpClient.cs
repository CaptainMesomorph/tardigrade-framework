using RestSharp;
using System;
using System.Collections.Generic;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Models.Rest;
using Tardigrade.Framework.RestSharp.Serializers;

namespace Tardigrade.Framework.RestSharp
{
    /// <summary>
    /// Implementation of a REST client using RestSharp.
    /// </summary>
    public class RestSharpClient : Rest.IRestClient
    {
        protected RestClient RestClient { get; private set; }

        /// <summary>
        /// Instantiate an instance of this class.
        /// </summary>
        /// <param name="endpoint">Endpoint URL of the REST service.</param>
        public RestSharpClient(Uri endpoint)
        {
            RestClient = new RestClient(endpoint);
            RestClient.AddHandler("application/json", NewtonsoftJsonDeserializer.Instance);
            RestClient.AddHandler("text/json", NewtonsoftJsonDeserializer.Instance);
            RestClient.AddHandler("*+json", NewtonsoftJsonDeserializer.Instance);
        }

        /// <summary>
        /// <see cref="IRestClient{T, PK}.Delete(string)"/>
        /// </summary>
        public Response Delete(string resource)
        {
            resource = (string.IsNullOrWhiteSpace(resource) ? null : resource.Trim());
            RestRequest request = new RestRequest(resource, Method.DELETE);
            request.AddHeader("Accept", "application/json");
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            IRestResponse response = RestClient.Execute(request);

            if (response.ErrorException != null)
            {
                throw new RestException($"Error deleting object at resource {resource}.", response.ErrorException);
            }

            return new Response(response.StatusCode, response.StatusDescription);
        }

        /// <summary>
        /// <see cref="IRestClient{T, PK}.Get{Result}(string)"/>
        /// </summary>
        public Response<Result> Get<Result>(string resource) where Result : new()
        {
            resource = (string.IsNullOrWhiteSpace(resource) ? null : resource.Trim());
            RestRequest request = new RestRequest(resource, Method.GET);
            request.AddHeader("Accept", "application/json");
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            IRestResponse<Result> response = RestClient.Execute<Result>(request);

            if (response.ErrorException != null)
            {
                throw new RestException($"Error retrieving {typeof(Result).Name} object at resource {resource}.", response.ErrorException);
            }

            return new Response<Result>(response.StatusCode, response.StatusDescription, response.Data);
        }

        /// <summary>
        /// <see cref="IRestClient{T, PK}.Get()"/>
        /// </summary>
        public Response<IList<Result>> Get<Result>()
        {
            RestRequest request = new RestRequest(Method.GET);
            request.AddHeader("Accept", "application/json");
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            IRestResponse<List<Result>> response = RestClient.Execute<List<Result>>(request);

            if (response.ErrorException != null)
            {
                throw new RestException($"Error retrieving {typeof(Result).Name} objects.", response.ErrorException);
            }

            return new Response<IList<Result>>(response.StatusCode, response.StatusDescription, response.Data);
        }

        /// <summary>
        /// <see cref="IRestClient.Post{Payload, Result}(Payload, string)"/>
        /// </summary>
        public Response<Result> Post<Payload, Result>(Payload payload, string resource = null) where Result : new()
        {
            if (payload == null)
            {
                throw new ArgumentNullException("payload");
            }

            resource = (string.IsNullOrWhiteSpace(resource) ? null : resource.Trim());
            RestRequest request = new RestRequest(resource, Method.POST);
            request.AddHeader("Accept", "application/json");
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(payload);
            IRestResponse<Result> response = RestClient.Execute<Result>(request);

            if (response.ErrorException != null)
            {
                throw new RestException($"Error creating {typeof(Payload).Name} object.", response.ErrorException);
            }

            return new Response<Result>(response.StatusCode, response.StatusDescription, response.Data);
        }

        /// <summary>
        /// <see cref="IRestClient.Post{Payload}(Payload, string)"/>
        /// </summary>
        public Response<string> Post<Payload>(Payload payload, string resource = null)
        {
            if (payload == null)
            {
                throw new ArgumentNullException("payload");
            }

            resource = (string.IsNullOrWhiteSpace(resource) ? null : resource.Trim());
            RestRequest request = new RestRequest(resource, Method.POST);
            request.AddHeader("Accept", "application/json");
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(payload);
            IRestResponse response = RestClient.Execute(request);

            if (response.ErrorException != null)
            {
                throw new RestException($"Error creating {typeof(Payload).Name} object.", response.ErrorException);
            }

            return new Response<string>(response.StatusCode, response.StatusDescription, response.Content);
        }

        /// <summary>
        /// <see cref="IRestClient.Put{Payload}(Payload, string)"/>
        /// </summary>
        public Response Put<Payload>(Payload obj, string resource)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }

            resource = (string.IsNullOrWhiteSpace(resource) ? null : resource.Trim());
            RestRequest request = new RestRequest(resource, Method.PUT);
            request.AddHeader("Accept", "application/json");
            request.JsonSerializer = new NewtonsoftJsonSerializer();
            request.AddJsonBody(obj);
            IRestResponse response = RestClient.Execute(request);

            if (response.ErrorException != null)
            {
                throw new RestException($"Error updating {typeof(Payload).Name} object at resource {resource}.", response.ErrorException);
            }

            return new Response(response.StatusCode, response.StatusDescription);
        }
    }
}