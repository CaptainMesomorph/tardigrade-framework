using System.Collections.Generic;
using System.Threading.Tasks;
using Tardigrade.Framework.Exceptions;
using Tardigrade.Framework.Models.Rest;

namespace Tardigrade.Framework.Rest
{
    /// <summary>
    /// Interface for RESTful service operations.
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        /// Delete a resource.
        /// </summary>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="RestException">Error deleting the resource.</exception>
        Response Delete(string resource);

        /// <summary>
        /// Delete a resource.
        /// </summary>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="RestException">Error deleting the resource.</exception>
        Task<Response> DeleteAsync(string resource);

        /// <summary>
        /// Retrieve a resource.
        /// </summary>
        /// <typeparam name="TResult">Resource type to be retrieved.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <returns>Resource identified by the endpoint.</returns>
        /// <exception cref="RestException">Error retrieving the resource.</exception>
        Response<TResult> Get<TResult>(string resource);

        /// <summary>
        /// Retrieve all resources.
        /// </summary>
        /// <typeparam name="TResult">Resource type to be retrieved.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <returns>Resources identified by the endpoint.</returns>
        /// <exception cref="RestException">Error retrieving the resource.</exception>
        Response<IList<TResult>> GetAll<TResult>(string resource = null);

        /// <summary>
        /// Retrieve all resources.
        /// </summary>
        /// <typeparam name="TResult">Resource type to be retrieved.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <returns>Resources identified by the endpoint.</returns>
        /// <exception cref="RestException">Error retrieving the resource.</exception>
        Task<Response<IList<TResult>>> GetAllAsync<TResult>(string resource = null);

        /// <summary>
        /// Retrieve a resource.
        /// </summary>
        /// <typeparam name="TResult">Resource type to be retrieved.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <returns>Resource identified by the endpoint.</returns>
        /// <exception cref="RestException">Error retrieving the resource.</exception>
        Task<Response<TResult>> GetAsync<TResult>(string resource);

        /// <summary>
        /// Check existence of a resource.
        /// </summary>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="RestException">Error checking resource existence.</exception>
        Response Head(string resource);

        /// <summary>
        /// Check existence of a resource.
        /// </summary>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="RestException">Error checking resource existence.</exception>
        Task<Response> HeadAsync(string resource);

        /// <summary>
        /// Create a resource.
        /// </summary>
        /// <typeparam name="TContent">Type associated with the resource to create.</typeparam>
        /// <typeparam name="TResult">Type associated with the response result.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <param name="content">Content of the resource to create.</param>
        /// <param name="contentFormat">Format of the content associated with the create request, i.e. JSON or XML.</param>
        /// <returns>The created resource.</returns>
        /// <exception cref="RestException">Error creating the resource.</exception>
        Response<TResult> Post<TContent, TResult>(
            string resource = null,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json);

        /// <summary>
        /// Create a resource.
        /// </summary>
        /// <typeparam name="TContent">Type associated with the resource to create.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <param name="content">Content of the resource to create.</param>
        /// <param name="contentFormat">Format of the content associated with the create request, i.e. JSON or XML.</param>
        /// <returns>String representation of the created resource.</returns>
        /// <exception cref="RestException">Error creating the resource.</exception>
        Response<string> Post<TContent>(
            string resource = null,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json);

        /// <summary>
        /// Create a resource.
        /// </summary>
        /// <typeparam name="TContent">Type associated with the resource to create.</typeparam>
        /// <typeparam name="TResult">Type associated with the response result.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <param name="content">Content of the resource to create.</param>
        /// <param name="contentFormat">Format of the content associated with the create request, i.e. JSON or XML.</param>
        /// <returns>The created resource.</returns>
        /// <exception cref="RestException">Error creating the resource.</exception>
        Task<Response<TResult>> PostAsync<TContent, TResult>(
            string resource = null,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json);

        /// <summary>
        /// Create a resource.
        /// </summary>
        /// <typeparam name="TContent">Type associated with the resource to create.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <param name="content">Content of the resource to create.</param>
        /// <param name="contentFormat">Format of the content associated with the create request, i.e. JSON or XML.</param>
        /// <returns>String representation of the created resource.</returns>
        /// <exception cref="RestException">Error creating the resource.</exception>
        Task<Response<string>> PostAsync<TContent>(
            string resource = null,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json);

        /// <summary>
        /// Update a resource.
        /// </summary>
        /// <typeparam name="TContent">Type associated with the resource to update.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <param name="content">Content of the resource to update.</param>
        /// <param name="contentFormat">Format of the content associated with the create request, i.e. JSON or XML.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="RestException">Error updating the resource.</exception>
        Response Put<TContent>(
            string resource,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json);

        /// <summary>
        /// Update a resource.
        /// </summary>
        /// <typeparam name="TContent">Type associated with the resource to update.</typeparam>
        /// <param name="resource">Resource endpoint. If null or blanks, will default to an empty string.</param>
        /// <param name="content">Content of the resource to update.</param>
        /// <param name="contentFormat">Format of the content associated with the create request, i.e. JSON or XML.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="RestException">Error updating the resource.</exception>
        Task<Response> PutAsync<TContent>(
            string resource,
            TContent content = default,
            ContentFormat contentFormat = ContentFormat.Json);
    }
}