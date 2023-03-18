namespace Blorc.OpenIdConnect.JSInterop
{
    using System;
    using System.Diagnostics;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.JSInterop;

    internal static class IJSRuntimeExtensions
    {
        public static async Task<T> InvokeWithPromiseHandlerAsync<T>(this IJSRuntime jsRuntime, string identifier, Func<T> defaultValue)
        {
            return await InvokeWithPromiseHandlerAsync<T>(jsRuntime, identifier, Array.Empty<object?>(), defaultValue);
        }

        public static async Task<T> InvokeWithPromiseHandlerAsync<T>(this IJSRuntime jsRuntime, string identifier, object?[]? args, Func<T> defaultValue)
        {
            try
            {
                return await InvokeWithPromiseHandlerAsync<T>(jsRuntime, identifier, args);
            }
            catch (Exception)
            {
#if DEBUG
                Trace(0, identifier, $"Returning default value");
#endif

                return defaultValue();
            }
        }

        public static async Task<T> InvokeWithPromiseHandlerAsync<T>(this IJSRuntime jsRuntime, string identifier)
        {
            return await InvokeWithPromiseHandlerAsync<T>(jsRuntime, identifier, Array.Empty<object?>());
        }

        public static async Task<T> InvokeWithPromiseHandlerAsync<T>(this IJSRuntime jsRuntime, string identifier, object?[]? args)
        {
            var tcs = new TaskCompletionSource<JsonElement>();

            using (var promiseHandler = DotNetObjectReference.Create(new PromiseHandler(tcs)))
            {
                var id = promiseHandler.Value.Id;

                var finalArgs = new object?[(args?.Length ?? 0) + 1];
                finalArgs[0] = promiseHandler;

                if (args is not null)
                {
                    for (var i = 0; i < args.Length; i++)
                    {
                        finalArgs[i + 1] = args[i];
                    }
                }

                var returnedResult = await jsRuntime.InvokeAsync<object?>(identifier, finalArgs);
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
                Trace(id, identifier, $"Returned object: {GetResultAsString(returnedResult)} (Type: {returnedResult?.GetType()}), awaiting promise...");
#endif

                try
                {
                    var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(10));

#if DEBUG
                    Trace(id, identifier, $"Successfully awaited promise, received: {GetResultAsString(result)} (Type: {result.GetType()})");
#endif

                    if (result is JsonElement promiseJsonResult)
                    {
                        var deserialized = JsonSerializer.Deserialize<T>(promiseJsonResult);
                        if (deserialized is T typedDeserialized)
                        {
                            return typedDeserialized;
                        }
                    }

                    throw new InvalidOperationException($"[{id}] [{identifier}] Could not parse result of type '{result.GetType().Name ?? "null"}' into '{typeof(T).Name}'");
                }
#if DEBUG
                catch (Exception ex)
#else
                catch (Exception)
#endif
                {
#if DEBUG
                    Trace(id, identifier, $"Failed to await promise: {ex.Message}");
#endif

                    throw;
                }
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
            Console.WriteLine($"[{id}] [{identifier}] {message}");
        }
#endif
    }
}
