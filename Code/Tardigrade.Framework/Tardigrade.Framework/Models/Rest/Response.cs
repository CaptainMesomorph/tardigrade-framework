using System.Net;

namespace Tardigrade.Framework.Models.Rest
{
    /// <summary>
    /// Class that defines a HTTP response.
    /// </summary>
    public class Response
    {
        /// <summary>
        /// HTTP status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Description associated with the status.
        /// </summary>
        public string StatusDescription { get; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="statusDescription">Description associated with the status.</param>
        public Response(HttpStatusCode statusCode, string statusDescription)
        {
            StatusCode = statusCode;
            StatusDescription = statusDescription;
        }
    }

    /// <summary>
    /// Class that defines a HTTP response with a defined response object.
    /// </summary>
    /// <typeparam name="T">Type of the response object.</typeparam>
    public class Response<T> : Response
    {
        /// <summary>
        /// Object associated with the response.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// Create an instance of this class.
        /// </summary>
        /// <param name="statusCode">HTTP status code.</param>
        /// <param name="statusDescription">Description associated with the status.</param>
        /// <param name="data">Response object.</param>
        public Response(HttpStatusCode statusCode, string statusDescription, T data) : base(statusCode, statusDescription)
        {
            Data = data;
        }
    }
}