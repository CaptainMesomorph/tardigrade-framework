using Microsoft.Extensions.Logging;
using System;

namespace Tardigrade.Framework.Extensions
{
    /// <summary>
    /// This static class contains extension methods for the IConfiguration interface.
    /// </summary>
    public static class LoggerExtension
    {
        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Critical<TContext>(
            this ILogger<TContext> logger,
            Exception exception,
            string message,
            params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Critical)) logger.Log(LogLevel.Critical, exception, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Critical<TContext>(this ILogger<TContext> logger, string message, params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Critical)) logger.Log(LogLevel.Critical, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Debug<TContext>(
            this ILogger<TContext> logger,
            Exception exception,
            string message,
            params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Debug)) logger.Log(LogLevel.Debug, exception, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Debug<TContext>(this ILogger<TContext> logger, string message, params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Debug)) logger.Log(LogLevel.Debug, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Error<TContext>(
            this ILogger<TContext> logger,
            Exception exception,
            string message,
            params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Error)) logger.Log(LogLevel.Error, exception, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Error<TContext>(this ILogger<TContext> logger, string message, params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Error)) logger.Log(LogLevel.Error, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Information<TContext>(
            this ILogger<TContext> logger,
            Exception exception,
            string message,
            params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Information)) logger.Log(LogLevel.Information, exception, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Information<TContext>(this ILogger<TContext> logger, string message, params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Information)) logger.Log(LogLevel.Information, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Trace<TContext>(
            this ILogger<TContext> logger,
            Exception exception,
            string message,
            params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Trace)) logger.Log(LogLevel.Trace, exception, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Trace<TContext>(this ILogger<TContext> logger, string message, params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Trace)) logger.Log(LogLevel.Trace, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Warning<TContext>(
            this ILogger<TContext> logger,
            Exception exception,
            string message,
            params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Warning)) logger.Log(LogLevel.Warning, exception, message, args);
        }

        /// <summary>
        /// <see cref="ILogger{TCategoryName}"/>
        /// </summary>
        public static void Warning<TContext>(this ILogger<TContext> logger, string message, params object[] args)
        {
            if (logger == null) return;

            if (logger.IsEnabled(LogLevel.Warning)) logger.Log(LogLevel.Warning, message, args);
        }
    }
}