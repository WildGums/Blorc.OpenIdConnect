namespace Blorc.OpenIdConnect.JSInterop
{
    using System;
    using System.Diagnostics;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.JSInterop;

    public static class IJSRuntimeExtensions
    {
        public static async Task<T> InvokeWithPromiseHandlerAsync<T>(this IJSRuntime jsRuntime, PromiseHandlerContext context, Func<T> defaultValue)
        {
            try
            {
                return await InvokeWithPromiseHandlerAsync<T>(jsRuntime, context);
            }
            catch (Exception)
            {
#if DEBUG
                Trace(-1, context.Identifier, $"Returning default value");
#endif

                return defaultValue();
            }
        }

        public static async Task<T> InvokeWithPromiseHandlerAsync<T>(this IJSRuntime jsRuntime, PromiseHandlerContext context)
        {
            var tcs = new TaskCompletionSource<JsonElement>();

            using (var promiseHandler = DotNetObjectReference.Create(new PromiseHandler(tcs)))
            {
                var id = promiseHandler.Value.Id;

                var args = context.Args;

                var finalArgs = new object?[(args?.Length ?? 0) + 1];
                finalArgs[0] = promiseHandler;

                if (args is not null)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        finalArgs[i + 1] = args[i];
                    }
                }

                var returnedResult = await jsRuntime.InvokeAsync<object?>(context.Identifier, finalArgs);
                if (returnedResult is JsonElement requestJsonResult)
                {
                    returnedResult = JsonSerializer.Deserialize<T>(requestJsonResult);
                }

                if (returnedResult is T typedReturnedResult)
                {
                    return typedReturnedResult;
                }

                // When no object is received, this is the promise that we need to "await"

#if DEBUG
                Trace(id, context.Identifier, $"Returned object: {GetResultAsString(returnedResult)} (Type: {returnedResult?.GetType()}), awaiting promise...");
#endif

                var stopwatch = Stopwatch.StartNew();

                for (var i = 0; i < context.MaximumRetryCount; i++)
                {
                    try
                    {
                        var result = await tcs.Task.WaitAsync(TimeSpan.FromMilliseconds(context.MaximumAwaitIntervalInMilliseconds));

#if DEBUG
                        Trace(id, context.Identifier, $"Successfully awaited promise, received: {GetResultAsString(result)} (Type: {result.GetType()})");
#endif

                        if (result is JsonElement promiseJsonResult)
                        {
                            var deserialized = JsonSerializer.Deserialize<T>(promiseJsonResult);
                            if (deserialized is T typedDeserialized)
                            {
                                return typedDeserialized;
                            }
                        }

                        throw new InvalidOperationException($"[{id}] [{context.Identifier}] Could not parse result of type '{result.GetType().Name ?? "null"}' into '{typeof(T).Name}'");
                    }
                    catch (TimeoutException)
                    {
                        // Allowed
                    }
#if DEBUG
                    catch (Exception ex)
#else
                    catch (Exception)
#endif
                    {
#if DEBUG
                        Trace(id, context.Identifier, $"Failed to await promise: {ex.Message}");
#endif

                        throw;
                    }
                }

                stopwatch.Stop();

#if DEBUG
                Trace(id, context.Identifier, $"Failed to await promise, timed out after {stopwatch.Elapsed}");
#endif

                throw new InvalidOperationException($"Failed to await promise, timed out after {stopwatch.Elapsed}");
            }
        }

#if DEBUG
        private static string GetResultAsString(object? jsonElement)
        {
            var result = jsonElement?.ToString() ?? "null";
            if (result.Length > 50)
            {
                result = $"{result.Substring(0, 50)}...";
            }

            return result;
        }

        [Conditional("DEBUG")]
        private static void Trace(int id, string identifier, string message)
        {
            Console.WriteLine($"[ {DateTime.Now.ToString("HH:mm:ss.fff") }] [ {id} ] [ {identifier} ] {message}");
        }
#endif
    }
}
