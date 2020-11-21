using System;
using System.IO; //For Writing To CSV File
using OvertakeAI; //For Getting Overtake Data
using System.Linq; //For Counting ScoreCard
using System.Text; //For Using StringBuilder
using System.Collections.Generic; //For ScoreCard Bool List
using static System.Console; //For Read/Write Line

namespace NeuralNetwork
{
    class NN
    {
        private static readonly List<string> PossibleResults = new List<string> { "False", "True" };

        public void Run()
        {
            //Get wanted variables from the user
            WriteLine("Neural Network");
            Write("Amount of data to train: ");
            int train = Convert.ToInt32(ReadLine());
            Write("Amount of nodes in hidden layer: ");
            int hiddenLayerNodes = Convert.ToInt32(ReadLine());
            Write("Learning Rate: ");
            double learningRate = Convert.ToDouble(ReadLine());
            Write("Amount of epochs: ");
            int epochs = Convert.ToInt32(ReadLine());

            //Get data OvertakeAI program
            var trainDataSet = GetInputs(train).ToArray();

            //Create neural network
            var network = new NNModel(3, hiddenLayerNodes, 2, learningRate);            

            WriteLine($"\nTraining network with {trainDataSet.Length} samples using {epochs} epochs...\n");

            //Train the neural network with desired amount of epochs
            for (var epoch = 0; epoch < epochs; epoch++)
                foreach (var data in trainDataSet)
                {
                    var targets = new[] { 0.01, 0.01 };
                    targets[PossibleResults.IndexOf(data.Last())] = 0.99;

                    var dataList = data.Take(3).Select(double.Parse).ToArray();
                    network.Train(NormalizeData(dataList), targets);
                }

            //Get the amount of data the user wants to predict against the neural network
            Write("Amount of data to predict: ");
            int test = Convert.ToInt32(ReadLine());
            WriteLine();

            var testDataSet = GetInputs(test).ToArray();
            var scoreCard = new List<bool>();

            //Test the data against neural network and make predictions
            foreach (var data in testDataSet)
            {
                var result = network.Query(NormalizeData(data.Take(3).Select(double.Parse).ToArray())).ToList();
                var answer = PossibleResults[PossibleResults.IndexOf(data.Last())];
                var predicted = PossibleResults[result.IndexOf(result.Max())];
                scoreCard.Add(answer == predicted);
            }

            string actualOutcome;
            string answerOutcome;

            //Print out all the test data
            WriteLine($"{"Initial Seperation",18}" +
                $"{"Overtaking Speed",23}" +
                $"{"Oncoming Speed",21}" +
                $"{"Outcome",14}" +
                $"{"Prediction",17}");

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


            //Count amount of correct values in score card to show accuracy percentage
            double accuracy = Math.Round((scoreCard.Count(x => x) / Convert.ToDouble(scoreCard.Count)) * 100, 2);
            WriteLine($"\nAccuracy: {accuracy}%");
            
            string path = @"..\..\..\neuralNetworkLog.csv";
            var csv = new StringBuilder();            

            //Check if neuralNetworkLog exists
            if (!File.Exists(path))
            {
                //Create file if it doesn't exist with no data
                File.WriteAllText(path, null);
                var csvHeadings = "TestNo,TrainAmount,HiddenLayerNodes,LearningRate,Epochs,TestAmount,Accuracy";
                csv.AppendLine(csvHeadings);
            }

            //Load data from neuralNetworkLog and skip headings
            List<string> loadedCsv = File.ReadAllLines(path).Skip(1).ToList();
            int testNo = loadedCsv.Count();

            //Output data to neuralNetworkLog csv file
            var csvData = $"{testNo + 1},{train},{hiddenLayerNodes},{learningRate},{epochs},{test},{accuracy}";
            csv.AppendLine(csvData);

            //Write the data to csv file
            File.AppendAllText(path, csv.ToString());
        }

        //Get data from the OvertakeAI program
        public static string[][] GetInputs(int loop)
        {
            Library.Overtake overtake;
            string[][] data = new string[loop][];

            for (int i = 0; i < loop; i++)
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

        //Normalize the data to be trained/tested
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
