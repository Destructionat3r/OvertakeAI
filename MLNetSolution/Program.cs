using System;

namespace MLNetSolution
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(132, Console.WindowHeight);
            MLNet ml = new MLNet();
            ml.Run();
            Console.ReadKey();
        }
    }
}
