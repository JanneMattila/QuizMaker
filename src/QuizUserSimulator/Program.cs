using System;
using System.Threading.Tasks;

namespace QuizUserSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpoint = Environment.GetEnvironmentVariable("Endpoint") ?? "https://localhost:44390/QuizHub";

            Console.WriteLine("Quiz User Simulator");
            Console.WriteLine($"Connecting to endpoint: {endpoint}");

            var simulator = new Simulator();
            await simulator.SimulateUserAsync(endpoint);
        }
    }
}
