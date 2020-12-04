using System;

namespace MLNet
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.SetWindowSize(146, Console.WindowHeight);
            MLN ml = new MLN();
            ml.Run();
            Console.ReadKey();
        }
    }
}
