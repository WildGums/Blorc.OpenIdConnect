namespace Blorc.OpenIdConnect.JSInterop
{
    using System;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.JSInterop;

    /// <summary>
    /// Idea comes from https://stackoverflow.com/questions/55993757/can-not-receive-promise-in-blazor
    /// </summary>
    internal class PromiseHandler : IPromiseHandler
    {
        private static readonly object _lockObject = new object();
        private static int IdCounter = 1;

        private TaskCompletionSource<JsonElement> _taskCompletionSource { get; set; }

        

        public PromiseHandler(TaskCompletionSource<JsonElement> taskCompletionSource)
        {
            ArgumentNullException.ThrowIfNull(taskCompletionSource);

            _taskCompletionSource = taskCompletionSource;

            lock (_lockObject)
            {
                Id = IdCounter++;
            }
        }

        public int Id { get; }

        [JSInvokable]
        public void SetResult(string json)
        {
            var reader = new Utf8JsonReader(Encoding.UTF8.GetBytes(json));
            var jsonElement = JsonElement.ParseValue(ref reader);

            _taskCompletionSource.SetResult(jsonElement);
        }
    }
}
