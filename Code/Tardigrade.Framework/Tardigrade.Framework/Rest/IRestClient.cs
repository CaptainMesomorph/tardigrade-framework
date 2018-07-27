using System.Collections.Generic;
using Tardigrade.Framework.Models.Rest;

namespace Tardigrade.Framework.Rest
{
    /// <summary>
    /// Interface for RESTful service operations.
    /// </summary>
    public interface IRestClient
    {
        /// <summary>
        /// Delete an object.
        /// </summary>
        /// <param name="resource">Resource endpoint.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="Exceptions.RestException">Error deleting the object.</exception>
        Response Delete(string resource);

        /// <summary>
        /// Retrieve an object.
        /// </summary>
        /// <typeparam name="Result">Object type associated with the REST call result.</typeparam>
        /// <param name="resource">Resource endpoint.</param>
        /// <returns>Object with the unique identifier.</returns>
        /// <exception cref="Exceptions.RestException">Error retrieving the object.</exception>
        Response<Result> Get<Result>(string resource) where Result : new();

        /// <summary>
        /// Retrieve all objects.
        /// </summary>
        /// <typeparam name="Result">Object type associated with the REST call.</typeparam>
        /// <returns>All objects.</returns>
        /// <exception cref="Exceptions.RestException">Error retrieving all objects.</exception>
        Response<IList<Result>> Get<Result>();

        /// <summary>
        /// Create an object.
        /// </summary>
        /// <typeparam name="Payload">Object type associated with the payload of the REST call.</typeparam>
        /// <typeparam name="Result">Object type associated with the REST call result.</typeparam>
        /// <param name="payload">Object to create.</param>
        /// <param name="resource">Resource endpoint.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="System.ArgumentNullException">payload is nulll.</exception>
        /// <exception cref="Exceptions.RestException">Error creating the object.</exception>
        Response<Result> Post<Payload, Result>(Payload payload, string resource = null) where Result : new();

        /// <summary>
        /// Create an object.
        /// </summary>
        /// <typeparam name="Payload">Object type associated with the payload of the REST call.</typeparam>
        /// <param name="payload">Object to create.</param>
        /// <param name="resource">Resource endpoint.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="System.ArgumentNullException">payload is nulll.</exception>
        /// <exception cref="Exceptions.RestException">Error creating the object.</exception>
        Response<string> Post<Payload>(Payload payload, string resource = null);

        /// <summary>
        /// Update an object.
        /// </summary>
        /// <typeparam name="Payload">Object type associated with the payload of the REST call.</typeparam>
        /// <param name="obj">Object to update.</param>
        /// <param name="resource">Resource endpoint.</param>
        /// <returns>Response result.</returns>
        /// <exception cref="System.ArgumentNullException">obj is nulll.</exception>
        /// <exception cref="Exceptions.RestException">Error updating the object.</exception>
        Response Put<Payload>(Payload obj, string resource);
    }
}