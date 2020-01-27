using System;
using System.Threading.Tasks;
using ITOps.EndpointConfig;
using NServiceBus;
using NServiceBus.Logging;

namespace Divergent.Sales
{
    class Host
    {
        static readonly ILog Log = LogManager.GetLogger<Host>();
        IEndpointInstance endpoint;

        public static string EndpointName => "Divergent.Sales";

        public async Task Start()
        {
            try
            {
                var endpointConfiguration = new EndpointConfiguration(EndpointName)
                    .Configure();

                endpoint = await Endpoint.Start(endpointConfiguration);
            }
            catch (Exception ex)
            {
                FailFast("Failed to start.", ex);
            }
        }

        public async Task Stop()
        {
            try
            {
                await endpoint?.Stop();
            }
            catch (Exception ex)
            {
                FailFast("Failed to stop correctly.", ex);
            }
        }

        async Task OnCriticalError(ICriticalErrorContext context)
        {
            try
            {
                await context.Stop();
            }
            finally
            {
                FailFast($"Critical error, shutting down: {context.Error}", context.Exception);
            }
        }

        void FailFast(string message, Exception exception)
        {
            try
            {
                Log.Fatal(message, exception);
            }
            finally
            {
                Environment.FailFast(message, exception);
            }
        }
    }
}
