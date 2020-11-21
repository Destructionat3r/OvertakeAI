using System;
using System.IO; //For Writing To CSV File
using OvertakeAI; //For Getting Overtake Data
using System.Linq; //For Counting ScoreCard
using System.Text; //For Using StringBuilder
using System.Collections.Generic; //For ScoreCard Bool List
using static System.Console; //For Read/Write Line

namespace NeuralNetworkSolution
{
    class NNSolution
    {
        private static readonly List<string> PossibleResults = new List<string> { "False", "True" };

        public void Run()
        {
            WriteLine("Neural Network");
            Write("Amount of data to train: ");
            int train = Convert.ToInt32(ReadLine());
            Write("Amount of nodes in hidden layer: ");
            int hiddenLayerNodes = Convert.ToInt32(ReadLine());
            Write("Learning Rate: ");
            double learningRate = Convert.ToDouble(ReadLine());
            Write("Amount of epochs: ");
            int epochs = Convert.ToInt32(ReadLine());

            var trainDataSet = GetInputs(train).ToArray();

            var network = new NeuralNetwork(3, hiddenLayerNodes, 2, learningRate);            

            WriteLine($"\nTraining network with {trainDataSet.Length} samples using {epochs} epochs...\n");

            for (var epoch = 0; epoch < epochs; epoch++)
                foreach (var data in trainDataSet)
                {
                    var targets = new[] { 0.01, 0.01 };
                    targets[PossibleResults.IndexOf(data.Last())] = 0.99;

                    var dataList = data.Take(3).Select(double.Parse).ToArray();
                    network.Train(NormalizeData(dataList), targets);
                }

            var scoreCard = new List<bool>();

            Write("Amount of data to predict: ");
            int test = Convert.ToInt32(ReadLine());
            WriteLine();

            var testDataSet = GetInputs(test).ToArray();

            foreach (var data in testDataSet)
            {
                var result = network.Query(NormalizeData(data.Take(3).Select(double.Parse).ToArray())).ToList();
                var answer = PossibleResults[PossibleResults.IndexOf(data.Last())];
                var predicted = PossibleResults[result.IndexOf(result.Max())];

                scoreCard.Add(answer == predicted);
            }

            string actualOutcome;
            string answerOutcome;
            WriteLine("Initial Seperation       Overtaking Speed       Oncoming Speed       Outcome       Prediction");

            for (var i = 0; i < testDataSet.Length; i++)
            {
                actualOutcome = Convert.ToBoolean(testDataSet[i][3]) ? "Will Pass" : "Won't Pass";
                answerOutcome = scoreCard[i] ? "Correct" : "Incorrect";
                WriteLine($"{testDataSet[i][0], 18}" +
                    $"{testDataSet[i][1], 23}" +
                    $"{testDataSet[i][2], 21}" +
                    $"{actualOutcome, 14}" +
                    $"{answerOutcome, 17}");
            }

            double accuracy = (scoreCard.Count(x => x) / Convert.ToDouble(scoreCard.Count)) * 100;
            WriteLine($"\nAccuracy: {accuracy}%");

            //Create file if it doesn't exist and insert headings then print data or just print data
            string path = "neuralNetworkLog.csv";
            var csv = new StringBuilder();

            if (!File.Exists(path))
            {
                var csvHeadings = "TrainAmount,HiddenLayerNodes,LearningRate,Epochs,TestAmount,Accuracy";
                csv.AppendLine(csvHeadings);
            }

            var csvData = $"{train},{hiddenLayerNodes},{learningRate},{epochs},{test},{accuracy}";
            csv.AppendLine(csvData);

            //Write the data to csv file
            File.AppendAllText(path, csv.ToString());
        }

        public static string[][] GetInputs(int train)
        {
            Library.Overtake overtake;
            string[][] data = new string[train][];

            for (int i = 0; i < train; i++)
            {
                overtake = OvertakeData.GetData();
                data[i] = new string[4] 
                { 
                    overtake.InitialSeparationM.ToString(), 
                    overtake.OvertakingSpeedMPS.ToString(), 
                    overtake.OncomingSpeedMPS.ToString(), 
                    overtake.Success.ToString() 
                };
            }

            return data;
        }

        private static double[] NormalizeData(double[] input)
        {
            var maxInitialSeperation = 280;
            var maxOvertakingSpeed = 33;
            var maxOncomingSpeed = 33;

            var normalized = new[]
            {
                (input[0]/maxInitialSeperation) + 0.01,
                (input[1]/maxOvertakingSpeed) + 0.01,
                (input[2]/maxOncomingSpeed) + 0.01
            };

            return normalized;
        }
    }
}
