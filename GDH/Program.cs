using GDH.Managers;

namespace GDH
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TelemetryCounter.InitProvider();
            GDH.signInOrUp();
        }
    }
}
