using OpenTelemetry;
using OpenTelemetry.Metrics;
using System.Diagnostics.Metrics;

namespace GDH.Managers
{
    public class TelemetryCounter
    {
        private Meter _meter;
        private Counter<int> _loginErrors;

        public TelemetryCounter(string meterName, string meterVersion, string counterName, string unit, string description)
        {
            _meter = new Meter(meterName, meterVersion);
            _loginErrors = _meter.CreateCounter<int>(
                name: counterName,
                unit: unit,
                description: description
            );
        }

        public static void InitProvider()
        {
            Sdk.CreateMeterProviderBuilder()
                           .AddMeter("GDH.LoginFailures")
                           .AddMeter("GDH.ExecutedCommands")
                           .AddPrometheusHttpListener(options => options.UriPrefixes = new string[1] { "http://localhost:9184" })
                           .Build();
        }

        public void Increment(int nb = 1)
        {
            Console.WriteLine("add");
            _loginErrors.Add(nb);
        }

        public void Decrement(int nb = 1)
        {
            _loginErrors.Add(-nb);
        }
    }
}
