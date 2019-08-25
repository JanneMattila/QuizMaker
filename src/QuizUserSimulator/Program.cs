using System;
using System.Threading.Tasks;

namespace QuizUserSimulator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Quiz User Simulator");
            var simulator = new Simulator();
            await simulator.SimulateUserAsync("https://localhost:44390/QuizHub");
        }
    }
}
