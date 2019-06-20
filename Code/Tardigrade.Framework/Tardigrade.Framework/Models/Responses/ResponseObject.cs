namespace Tardigrade.Framework.Models.Responses
{
    /// <summary>
    /// Defines information associated with a HTTP Response.
    /// </summary>
    public class ResponseObject
    {
        /// <summary>
        /// Message associated with the response.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// URL of the HTTP request.
        /// </summary>
        public string RequestUrl { get; set; }

        /// <summary>
        /// HTTP status code associated with the response.
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// Description for the HTTP status code.
        /// </summary>
        public string StatusDescription { get; set; }
    }
}