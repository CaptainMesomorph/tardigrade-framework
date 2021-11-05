using RestSharp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Helpers;
using Tardigrade.Framework.Models.Rest;
using Tardigrade.Framework.Rest;

namespace Tardigrade.Framework.RestSharp
{
    /// <summary>
    /// Implementation of a REST client using RestSharp.
    /// </summary>
    public class RestSharpClient : Rest.IRestClient
    {
        /// <summary>
        /// Underlying REST client.
        /// </summary>
        protected RestClient RestClient { get; }

        /// <summary>
        /// Instantiate an instance of this class.
        /// </summary>
        /// <param name="endpoint">Endpoint URL of the REST service.</param>
        /// <exception cref="ArgumentNullException">endpoint is null.</exception>
        public RestSharpClient(Uri endpoint)
        {
            if (endpoint == null) throw new ArgumentNullException(nameof(endpoint));

            RestClient = new RestClient(endpoint);
        }

        /// <inheritdoc />
        public Response Delete(string resource)
        {
            return AsyncHelper.RunSync(() => DeleteAsync(resource));
        }

        /// <inheritdoc />
        public async Task<Response> DeleteAsync(string resource)
        {
            resource = (string.IsNullOrWhiteSpace(resource) ? string.Empty : resource.Trim());
            var request = new RestRequest(resource, Method.DELETE);
            IRestResponse response = await RestClient.ExecuteAsync(request);

            if (response.ErrorException != null)
            {
                throw new RestException(
                    $"Error deleting resource {resource}: {response.ErrorMessage}.",
                    response.ErrorException);
            }

            return response.IsSuccessful
                ? new Response(response.StatusCode, response.StatusDescription)
                : new Response(response.StatusCode, response.Content);
        }

        /// <inheritdoc />
        public Response<TResult> Get<TResult>(string resource)
        {
            return AsyncHelper.RunSync(() => GetAsync<TResult>(resource));
        }

        /// <inheritdoc />
        public Response<IList<TResult>> GetAll<TResult>(string resource = null)
        {
            return AsyncHelper.RunSync(() => GetAllAsync<TResult>(resource));
        }

        /// <inheritdoc />
        public async Task<Response<IList<TResult>>> GetAllAsync<TResult>(string resource = null)
        {
            return await GetAsync<IList<TResult>>(resource);
        }

        /// <inheritdoc />
        public async Task<Response<TResult>> GetAsync<TResult>(string resource)
        {
            resource = (string.IsNullOrWhiteSpace(resource) ? string.Empty : resource.Trim());
            var request = new RestRequest(resource, Method.GET);
            IRestResponse<TResult> response = await RestClient.ExecuteGetAsync<TResult>(request);

            if (response.ErrorException != null)
            {
                throw new RestException(
                    $"Error retrieving resource {resource} of type {typeof(TResult).Name}: {response.ErrorMessage}.",
                    response.ErrorException);
            }

            return response.IsSuccessful
                ? new Response<TResult>(response.StatusCode, response.StatusDescription, response.Data)
                : new Response<TResult>(response.StatusCode, response.Content, default);
        }

        /// <inheritdoc />
        public Response Head(string resource)
        {
            return AsyncHelper.RunSync(() => HeadAsync(resource));
        }

        /// <inheritdoc />
        public async Task<Response> HeadAsync(string resource)
        {
            resource = (string.IsNullOrWhiteSpace(resource) ? string.Empty : resource.Trim());
            var request = new RestRequest(resource, Method.HEAD);
            IRestResponse response = await RestClient.ExecuteAsync(request);

            if (response.ErrorException != null)
            {
                throw new RestException(
                    $"Error checking resource {resource} for existence: {response.ErrorMessage}.",
                    response.ErrorException);
            }

            return response.IsSuccessful
                ? new Response(response.StatusCode, response.StatusDescription)
                : new Response(response.StatusCode, response.Content);
        }

        /// <inheritdoc />
        public Response<TResult> Post<TContent, TResult>(
            string resource = null,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json)
        {
            return AsyncHelper.RunSync(() => PostAsync<TContent, TResult>(resource, content, contentFormat));
        }

        /// <inheritdoc />
        public async Task<Response<TResult>> PostAsync<TContent, TResult>(
            string resource = null,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json)
        {
            resource = (string.IsNullOrWhiteSpace(resource) ? string.Empty : resource.Trim());
            var request = new RestRequest(resource, Method.POST);

            if (content != null)
            {
                switch (contentFormat)
                {
                    case ContentFormat.Json:
                        request.AddJsonBody(content);
                        break;

                    case ContentFormat.Xml:
                        request.AddXmlBody(content);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(contentFormat),
                            contentFormat,
                            $"Invalid ContentFormat specified - {contentFormat}.");
                }
            }

            IRestResponse<TResult> response = await RestClient.ExecutePostAsync<TResult>(request);

            if (response.ErrorException != null)
            {
                throw new RestException(
                    $"Error creating resource {resource} of type {typeof(TContent).Name}: {response.Content}.",
                    response.ErrorException);
            }

            return response.IsSuccessful
                ? new Response<TResult>(response.StatusCode, response.StatusDescription, response.Data)
                : new Response<TResult>(response.StatusCode, response.Content, default);
        }

        /// <inheritdoc />
        public Response<string> Post<TContent>(
            string resource = null,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json)
        {
            return AsyncHelper.RunSync(() => PostAsync(resource, content, contentFormat));
        }

        /// <inheritdoc />
        public async Task<Response<string>> PostAsync<TContent>(
            string resource = null,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json)
        {
            resource = (string.IsNullOrWhiteSpace(resource) ? string.Empty : resource.Trim());
            var request = new RestRequest(resource, Method.POST);

            if (content != null)
            {
                switch (contentFormat)
                {
                    case ContentFormat.Json:
                        request.AddJsonBody(content);
                        break;

                    case ContentFormat.Xml:
                        request.AddXmlBody(content);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(contentFormat),
                            contentFormat,
                            $"Invalid ContentFormat specified - {contentFormat}.");
                }
            }

            IRestResponse response = await RestClient.ExecutePostAsync<string>(request);

            if (response.ErrorException != null)
            {
                throw new RestException(
                    $"Error creating resource {resource} of type {typeof(TContent).Name}: {response.ErrorMessage}.",
                    response.ErrorException);
            }

            return response.IsSuccessful
                ? new Response<string>(response.StatusCode, response.StatusDescription, response.Content)
                : new Response<string>(response.StatusCode, response.Content, default);
        }

        /// <inheritdoc />
        public Response Put<TContent>(
            string resource,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json)
        {
            return AsyncHelper.RunSync(() => PutAsync(resource, content, contentFormat));
        }

        /// <inheritdoc />
        public async Task<Response> PutAsync<TContent>(
            string resource,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json)
        {
            resource = (string.IsNullOrWhiteSpace(resource) ? string.Empty : resource.Trim());
            var request = new RestRequest(resource, Method.PUT);

            if (content != null)
            {
                switch (contentFormat)
                {
                    case ContentFormat.Json:
                        request.AddJsonBody(content);
                        break;

                    case ContentFormat.Xml:
                        request.AddXmlBody(content);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException(
                            nameof(contentFormat),
                            contentFormat,
                            $"Invalid ContentFormat specified - {contentFormat}.");
                }
            }

            IRestResponse response = await RestClient.ExecuteAsync(request);

            if (response.ErrorException != null)
            {
                throw new RestException(
                    $"Error updating resource {resource} of type {typeof(TContent).Name}: {response.ErrorMessage}.",
                    response.ErrorException);
            }

            return response.IsSuccessful
                ? new Response(response.StatusCode, response.StatusDescription)
                : new Response(response.StatusCode, response.Content);
        }
    }
}