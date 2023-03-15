namespace Blorc.OpenIdConnect.JSInterop
{
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.JSInterop;

    /// <summary>
    /// Code comes from https://stackoverflow.com/questions/55993757/can-not-receive-promise-in-blazor
    /// </summary>
    internal class PromiseHandler : IPromiseHandler
    {
        private TaskCompletionSource<JsonElement> _taskCompletionSource { get; set; }

        public PromiseHandler(TaskCompletionSource<JsonElement> taskCompletionSource)
        {
            ArgumentNullException.ThrowIfNull(taskCompletionSource);

            _taskCompletionSource = taskCompletionSource;
        }

        [JSInvokable]
        public void SetResult(string json)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            var jsonElement = JsonElement.ParseValue(ref reader);

            _taskCompletionSource.SetResult(jsonElement);
        }
    }
}
