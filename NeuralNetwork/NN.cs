using System.IO; //For Writing To CSV File
using OvertakeAI; //For Getting Overtake Data
using System.Linq; //For Counting ScoreCard
using System.Text; //For Using StringBuilder
using System.Collections.Generic; //For ScoreCard Bool List
using Accord.Math.Optimization.Losses; //For ZeroOneLoss
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

            int[] trainingPrediction = new int[trainAmount];
            int[] trainingOutcome = new int[trainAmount];
            int trainingIndex = 0;
            double[] errorList = new double[epochs];

            WriteLine($"\nTraining network with {trainDataSet.Length} samples using {epochs} epochs...");
            
            //Train the data and get the training error for every epoch
            for (var epoch = 0; epoch < epochs; epoch++)
            { 
                Write($"Epoch {epoch + 1} of {epochs} - ");

                //Train the neural network with each bit of training data
                foreach (var data in trainDataSet)
                {
                    var targets = new[] { 0.01, 0.01 };
                    targets[PossibleResults.IndexOf(data.Last())] = 0.99;

                    var dataList = data.Take(3).Select(double.Parse).ToArray();
                    network.Train(NormalizeData(dataList, maxValues), targets);
                }

                //Test trained data against the neural network
                foreach (var data in trainDataSet)
                {
                    var trainingResult = network.Query(NormalizeData(data.Take(3).Select(double.Parse).ToArray(), maxValues)).ToList();
                    trainingOutcome[trainingIndex] = PossibleResults.IndexOf(data.Last());
                    trainingPrediction[trainingIndex] = trainingResult.IndexOf(trainingResult.Max());
                    trainingIndex++;
                }

                trainingIndex = 0;

                //Get training error on neural network each epoch
                errorList[epoch] = Round(new ZeroOneLoss(trainingOutcome).Loss(trainingPrediction), 2);
                WriteLine($"Training Error: {Round(errorList[epoch], 2):F2}");
            }

            var testDataSet = GetOvertakeData(testAmount).ToArray();
            var scoreCard = new List<bool>();

            //Test the data against neural network and make predictions
            foreach (var data in testDataSet)
            {
                var testingResult = network.Query(NormalizeData(data.Take(3).Select(double.Parse).ToArray(), maxValues)).ToList();
                var testingOutcome = PossibleResults[PossibleResults.IndexOf(data.Last())];
                var testingPrediction = PossibleResults[testingResult.IndexOf(testingResult.Max())];
                scoreCard.Add(testingOutcome == testingPrediction);
            }

            //Count amount of correct values in score card to show accuracy percentage
            double accuracy = Round((scoreCard.Count(x => x) / ToDouble(scoreCard.Count)), 4);

            string actualOutcome;
            string predictedOutcome;

            //Print out all the test data
            WriteLine($"\n{"Initial Seperation (m)",21}" +
                $"{"Overtaking Speed (m/s)",28}" +
                $"{"Oncoming Speed (m/s)",26}" +
                $"{"Outcome",14}" +
                $"{"Prediction",17}");

            for (var i = 0; i < testDataSet.Length; i++)
            {
                actualOutcome = ToBoolean(testDataSet[i][3]) ? "Will Pass" : "Won't Pass";
                predictedOutcome = scoreCard[i] ? "Correct" : "Incorrect";

                WriteLine($"{Round(ToDouble(testDataSet[i][0]), 2).ToString("F"),14}" +
                    $"{Round(ToDouble(testDataSet[i][1]), 2).ToString("F"),27}" +
                    $"{Round(ToDouble(testDataSet[i][2]), 2).ToString("F"),27}" +
                    $"{actualOutcome,22}" +
                    $"{predictedOutcome,17}");
            }

            //Find the lowest training error and the epoch it reached that point
            double minTrainingError = errorList.Min();
            int epochNum = errorList.ToList().IndexOf(minTrainingError) + 1;
            double finalTrainingError = Round(errorList.Last(), 2);

            WriteLine($"\nAccuracy: {Round(accuracy * 100, 2)}%");
            WriteLine($"Lowest Training Error: {minTrainingError:F2} At Epoch {epochNum}");
            WriteLine($"Ending Training Error: {finalTrainingError}");

            //Format variables into a single string
            var nNTestData = $"{trainAmount}," +
                $"{hiddenLayerNodes}," +
                $"{learningRate}," +
                $"{minTrainingError}," +
                $"{epochNum}," +
                $"{finalTrainingError}," +
                $"{epochs}," +
                $"{testAmount}," +
                $"{accuracy}";

            OutputData.OutputCsv(nNTestData);

            WriteLine("\nEnter own values to predict");
            double initialSeperation = GetUserDoubleInput("Initial Seperation (m)");
            double overtakingSpeed = GetUserDoubleInput("Overtaking Speed (mps)");
            double oncomingSpeed = GetUserDoubleInput("Oncoming Speed (mps)");

            double[] userTest = new double[3]
            {
                initialSeperation,
                overtakingSpeed,
                oncomingSpeed
            };

            var userResult = network.Query(NormalizeData(userTest, maxValues)).ToList();
            var userPrediction = PossibleResults[userResult.IndexOf(userResult.Max())];
            userPrediction = true ? "Will Pass" : "Won't Pass";

            WriteLine($"\n{"Initial Seperation (m)",21}" +
                $"{"Overtaking Speed (m/s)",28}" +
                $"{"Oncoming Speed (m/s)",26}" +
                $"{"Prediction",17}");

            WriteLine($"{Round(userTest[0],2),14}" +
                    $"{Round(userTest[1],2),27}" +
                    $"{Round(userTest[2],2),27}" +
                    $"{userPrediction,25}");
        }
        
        //Make sure user is inputting an int when required
        public int GetUserIntInput(string text)
        {
            int input;

            Write($"{text}: ");

            //Keep asking for input until a valid input is given
            while (!int.TryParse(ReadLine(), out input))
                Write($"{text}: ");

            return input;
        }

        //Make sure user is inputting an int when required
        public double GetUserDoubleInput(string text)
        {
            double input;

            Write($"{text}: ");

            //Keep asking for input until a valid input is given
            while (!double.TryParse(ReadLine(), out input))
                Write($"{text}: ");

            return input;
        }

        //Get data from the OvertakeAI program
        public static string[][] GetOvertakeData(int loop)
        {
            Library.Overtake overtake;
            string[][] overtakeData = new string[loop][];

            for (var i = 0; i < loop; i++)
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

            for (var i = 0; i < input.Length; i++)
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

            //Clamp input to maximum value if it's higher than the maximum value
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
