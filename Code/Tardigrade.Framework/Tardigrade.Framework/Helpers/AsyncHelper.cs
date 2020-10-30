// Copyright (c) Microsoft Corporation, Inc. All rights reserved.
// Licensed under the MIT License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Tardigrade.Framework.Helpers
{
    /// <summary>
    /// Helper class to run asynchronous methods within a synchronous process.
    /// <a href="https://cpratt.co/async-tips-tricks/">C# Async Tips &amp; Tricks</a>
    /// <a href="https://www.ryadel.com/en/asyncutil-c-helper-class-async-method-sync-result-wait/">AsyncUtil – C# Helper class to run async methods as sync and vice-versa</a>
    /// <a href="https://github.com/aspnet/AspNetIdentity/blob/master/src/Microsoft.AspNet.Identity.Core/AsyncHelper.cs">AsyncHelper.cs</a>
    /// <a href="https://stackoverflow.com/questions/9343594/how-to-call-asynchronous-method-from-synchronous-method-in-c">How to call asynchronous method from synchronous method in C#?</a>
    /// </summary>
    public static class AsyncHelper
    {
        private static readonly TaskFactory TaskFactory = new TaskFactory(
            CancellationToken.None,
            TaskCreationOptions.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

        /// <summary>
        /// Execute an asynchronous method which returns void synchronously.
        /// Usage: AsyncHelper.RunSync(() => asyncMethod());
        /// </summary>
        /// <param name="func">Asynchronous method to execute.</param>
        public static void RunSync(Func<Task> func)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            CultureInfo uiCulture = CultureInfo.CurrentUICulture;

            TaskFactory
                .StartNew(() =>
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = uiCulture;
                    return func();
                })
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }

        /// <summary>
        /// Execute an asynchronous method which has a return type synchronously.
        /// Usage: T result = AsyncHelper.RunSync(() => asyncMethod{T}());
        /// </summary>
        /// <typeparam name="T">Return type.</typeparam>
        /// <param name="func">Asynchronous method to execute.</param>
        /// <returns>Result of the method execution.</returns>
        public static T RunSync<T>(Func<Task<T>> func)
        {
            CultureInfo culture = CultureInfo.CurrentCulture;
            CultureInfo uiCulture = CultureInfo.CurrentUICulture;

            return TaskFactory
                .StartNew(() =>
                {
                    Thread.CurrentThread.CurrentCulture = culture;
                    Thread.CurrentThread.CurrentUICulture = uiCulture;
                    return func();
                })
                .Unwrap()
                .GetAwaiter()
                .GetResult();
        }
    }
}