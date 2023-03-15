namespace Blorc.OpenIdConnect.JSInterop
{
    using System;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.JSInterop;

    internal static class IJSRuntimeExtensions
    {
        public static async Task<T> InvokeWithPromiseHandlerAsync<T>(this IJSRuntime jsRuntime, string identifier, params object?[]? args)
        {
            var tcs = new TaskCompletionSource<JsonElement>();

            using (var promiseHandler = DotNetObjectReference.Create(new PromiseHandler(tcs)))
            {
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
                Console.WriteLine($"Returned object: {returnedResult?.ToString() ?? "null"} (Type: {returnedResult?.GetType()})");
#endif

                var result = await tcs.Task.WaitAsync(TimeSpan.FromSeconds(10));

                if (result is JsonElement promiseJsonResult)
                {
                    var deserialized = JsonSerializer.Deserialize<T>(promiseJsonResult);
                    if (deserialized is T typedDeserialized)
                    {
                        return typedDeserialized;
                    }
                }

                throw new InvalidOperationException($"Could not parse result of type '{result.GetType().Name ?? "null"}' into '{typeof(T).Name}'");
            }
        }
    }
}
