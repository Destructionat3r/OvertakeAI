using System;

namespace OvertakeAI
{
    public class OvertakeData
    {
        public void GetData()
        {
            string test = "Hello";

            // This example fetches and prints the first 1000 data items
            for (var i = 0; i < 1000; i++)
            {
                var overtake = Library.Overtake.GetNextOvertake();
                //Console.WriteLine($"{overtake.ToString()}\n");
                Console.WriteLine($"InitialSeparation = {overtake.InitialSeparationM:F1} metres");
                Console.WriteLine($"OvertakingSpeed = {overtake.OvertakingSpeedMPS:F1} m/s");
                Console.WriteLine($"OncomingSpeed = {overtake.OncomingSpeedMPS:F1} m/s");
                Console.WriteLine($"Success = {overtake.Success}\n");
            }

            Console.ReadKey(); // Keep the window open till a key is pressed
        }
    }
}
