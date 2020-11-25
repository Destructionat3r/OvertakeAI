using System.IO; //For Writing To CSV File
using OvertakeAI; //For Getting Overtake Data
using System.Linq; //For Counting ScoreCard
using System.Text; //For Using StringBuilder
using System.Collections.Generic; //For ScoreCard Bool List
using static System.Math; //For Round and Clamp
using static System.Console; //For Read/Write Line
using static System.Convert; //For Convert

namespace NeuralNetwork
{
    class NN
    {
        private static readonly List<string> PossibleResults = new List<string> { "False", "True" };

        public void Run()
        {
            //Get wanted variables from the user
            WriteLine("Neural Network");
            int trainAmount = GetUserIntInput("Amount of data to train");
            int hiddenLayerNodes = GetUserIntInput("Amount of nodes in hidden layer");
            double learningRate = GetUserDoubleInput("Learning Rate");
            int epochs = GetUserIntInput("Amount of epochs");

            //Get the amount of data the user wants to predict against the neural network
            int testAmount = GetUserIntInput("Amount of data to predict");

            //Get data OvertakeAI program
            var trainDataSet = GetOvertakeData(trainAmount).ToArray();

            //Get max values of trainDataSet to normalize data later on
            var maxValues = GetMaxValues(trainDataSet);

            //Create neural network
            var network = new NNModel(3, hiddenLayerNodes, 2, learningRate);            

            WriteLine($"\nTraining network with {trainDataSet.Length} samples using {epochs} epochs...");

            //Train the neural network with desired amount of epochs
            for (var epoch = 0; epoch < epochs; epoch++)
            { 
                SetCursorPosition(0, 8);
                WriteLine($"Epoch {epoch + 1} of {epochs}");

                foreach (var data in trainDataSet)
                {
                    var targets = new[] { 0.01, 0.01 };
                    targets[PossibleResults.IndexOf(data.Last())] = 0.99;

                    var dataList = data.Take(3).Select(double.Parse).ToArray();
                    network.Train(NormalizeData(dataList, maxValues), targets);
                }
            }

            var testDataSet = GetOvertakeData(testAmount).ToArray();
            var scoreCard = new List<bool>();

            //Test the data against neural network and make predictions
            foreach (var data in testDataSet)
            {
                var result = network.Query(NormalizeData(data.Take(3).Select(double.Parse).ToArray(), maxValues)).ToList();
                var answer = PossibleResults[PossibleResults.IndexOf(data.Last())];
                var predicted = PossibleResults[result.IndexOf(result.Max())];
                scoreCard.Add(answer == predicted);
            }

            string actualOutcome;
            string predictedOutcome;

            //Print out all the test data
            WriteLine($"\n{"Initial Seperation",18}" +
                $"{"Overtaking Speed",23}" +
                $"{"Oncoming Speed",21}" +
                $"{"Outcome",14}" +
                $"{"Prediction",17}");

            for (var i = 0; i < testDataSet.Length; i++)
            {
                actualOutcome = ToBoolean(testDataSet[i][3]) ? "Will Pass" : "Won't Pass";
                predictedOutcome = scoreCard[i] ? "Correct" : "Incorrect";
                WriteLine($"{testDataSet[i][0], 18}" +
                    $"{testDataSet[i][1], 23}" +
                    $"{testDataSet[i][2], 21}" +
                    $"{actualOutcome, 14}" +
                    $"{predictedOutcome, 17}");
            }

            //Count amount of correct values in score card to show accuracy percentage
            double accuracy = Round((scoreCard.Count(x => x) / ToDouble(scoreCard.Count)) * 100, 2);
            WriteLine($"\nAccuracy: {accuracy}%");
            
            string path = @"..\..\..\neuralNetworkLog.csv";
            var csv = new StringBuilder();            

            //Check if neuralNetworkLog exists
            if (!File.Exists(path))
            {
                //Create file if it doesn't exist with no data
                File.WriteAllText(path, null);
                var nNHeadings = "TestNo,TrainAmount,HiddenLayerNodes,LearningRate,Epochs,TestAmount,Accuracy";
                csv.AppendLine(nNHeadings);
            }

            //Load data from neuralNetworkLog and skip headings
            List<string> loadedCsv = File.ReadAllLines(path).Skip(1).ToList();
            int testNo = loadedCsv.Count();

            //Output data to neuralNetworkLog csv file
            var nNData = $"{testNo + 1},{trainAmount},{hiddenLayerNodes},{learningRate},{epochs},{testAmount},{accuracy}";
            csv.AppendLine(nNData);

            //Write the data to csv file
            File.AppendAllText(path, csv.ToString());
        }
        
        //Make sure user is inputting an int when required
        public int GetUserIntInput(string text)
        {
            int input;

            Write($"{text}: ");

            while (!int.TryParse(ReadLine(), out input))
            {
                Write($"{text}: ");
            }

            return input;
        }

        //Make sure user is inputting an int when required
        public double GetUserDoubleInput(string text)
        {
            double input;

            Write($"{text}: ");

            while (!double.TryParse(ReadLine(), out input))
            {
                Write($"{text}: ");
            }

            return input;
        }

        //Get data from the OvertakeAI program
        public static string[][] GetOvertakeData(int loop)
        {
            Library.Overtake overtake;
            string[][] overtakeData = new string[loop][];

            for (int i = 0; i < loop; i++)
            {
                overtake = OvertakeData.GetData();
                overtakeData[i] = new string[4] 
                { 
                    overtake.InitialSeparationM.ToString(), 
                    overtake.OvertakingSpeedMPS.ToString(), 
                    overtake.OncomingSpeedMPS.ToString(), 
                    overtake.Success.ToString() 
                };
            }

            return overtakeData;
        }

        //Get max values from training data
        private static double[] GetMaxValues(string[][] input)
        {
            double maxInitalSeperation = 0;
            double maxOvertakingSpeed = 0;
            double maxOncomingSpeed = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (ToDouble(input[i][0]) > maxInitalSeperation)
                    maxInitalSeperation = ToDouble(input[i][0]);

                if (ToDouble(input[i][1]) > maxOvertakingSpeed)
                    maxOvertakingSpeed = ToDouble(input[i][1]);

                if (ToDouble(input[i][2]) > maxOncomingSpeed)
                    maxOncomingSpeed = ToDouble(input[i][2]);
            }

            double[] maxValues = new double[] { 
                maxInitalSeperation, 
                maxOvertakingSpeed, 
                maxOncomingSpeed 
            };

            return maxValues;
        }

        //Normalize the data to be trained/tested
        private static double[] NormalizeData(double[] input, double[] maxValues)
        {
            var maxInitialSeperation = maxValues[0];
            var maxOvertakingSpeed = maxValues[1];
            var maxOncomingSpeed = maxValues[2];

            var normalized = new[]
            {
                (Clamp(input[0], 0, maxInitialSeperation)/maxInitialSeperation) + 0.01,
                (Clamp(input[1], 0, maxOvertakingSpeed)/maxOvertakingSpeed) + 0.01,
                (Clamp(input[2], 0, maxOncomingSpeed)/maxOncomingSpeed) + 0.01
            };

            return normalized;
        }
    }
}
