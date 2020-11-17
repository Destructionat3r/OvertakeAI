using System;
using System.IO;
using OvertakeAI;
using OvertakeAIML.Model;

namespace MLNetSolution
{
    class MLNet
    {
        public void Run()
        {
            string path = "testData.csv";
            Library.Overtake overtake;

            Console.WriteLine("ML.Net");
            Console.Write("Amount of data to train: ");
            int train = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            File.WriteAllText(path, null);
            using (StreamWriter sw = new StreamWriter(path))
            {
                sw.WriteLine("InitialSeperation,OvertakingSpeed,OncomingSpeed,Success");
                for (int i = 0; i < train; i++)
                {
                    overtake = OvertakeData.GetData();                    
                    sw.WriteLine($"{overtake.InitialSeparationM},{overtake.OvertakingSpeedMPS},{overtake.OncomingSpeedMPS},{overtake.Success}");
                }
            }
            ModelBuilder.CreateModel();

            overtake = OvertakeData.GetData();

            ModelInput sampleData = new ModelInput()
            { 
                InitialSeperation = (float)overtake.InitialSeparationM,
                OvertakingSpeed = (float)overtake.OvertakingSpeedMPS,
                OncomingSpeed = (float)overtake.OncomingSpeedMPS,
            };

            // Make a single prediction on the sample data and print results
            var predictionResult = ConsumeModel.Predict(sampleData);

            Console.WriteLine("\nUsing model to make single prediction -- Comparing actual Success with predicted Success from sample data...\n\n");
            Console.WriteLine($"InitialSeperation: {sampleData.InitialSeperation}");
            Console.WriteLine($"OvertakingSpeed: {sampleData.OvertakingSpeed}");
            Console.WriteLine($"OncomingSpeed: {sampleData.OncomingSpeed}");
            Console.WriteLine($"ActualResult: {overtake.Success}");
            Console.WriteLine($"\nPredicted Success Value: {predictionResult.Prediction} \nPredicted Success Scores: [{String.Join(",", predictionResult.Score)}]\n");
            Console.WriteLine("=============== End of process, hit any key to finish ===============");
            Console.ReadKey();
        }
    }
}
