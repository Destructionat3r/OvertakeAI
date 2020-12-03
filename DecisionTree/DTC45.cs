using OvertakeAI; //For Getting Overtake Data
using System.Linq; //For Counting ScroreCard
using System.Globalization; // For CultureInfo
using Accord.Statistics.Filters; //For Codification
using System.Collections.Generic; //For ScoreCard Bool List
using Accord.Math.Optimization.Losses; //For ZeroOneLoss
using Accord.MachineLearning.DecisionTrees; //For Decision Tree
using Accord.MachineLearning.DecisionTrees.Rules; //For Decision Set
using Accord.MachineLearning.DecisionTrees.Learning; //For C45 Learning
using static System.Math; //For Round
using static System.Console; //For Read/Write Line
using static System.Convert; //For Convert

namespace DecisionTreeC45
{
    class DTC45
    {
        public void Run()
        {
            Library.Overtake overtake;

            WriteLine("Decision Tree - C45 Learning");

            //Get amount of data the user wants the decision tree to train
            int trainAmount = GetUserInput("Amount of data to train");

            double[][] trainInputs = new double[trainAmount][];
            int[] trainOutputs = new int[trainAmount];

            //Get data from OvertakeAI program and insert it into train inputs and outputs arrays
            for (int i = 0; i < trainAmount; i++)
            {
                overtake = OvertakeData.GetData();

                trainInputs[i] = new double[3] 
                { 
                    overtake.InitialSeparationM, 
                    overtake.OvertakingSpeedMPS, 
                    overtake.OncomingSpeedMPS 
                };

                trainOutputs[i] = ToInt32(overtake.Success);
            }

            //Train decison tree using C4.5 algorithm using the trainInputs and trainOutputs
            var learningAlgorithm = new C45Learning();
            DecisionTree tree = learningAlgorithm.Learn(trainInputs, trainOutputs);

            //Get the amount of data the user wants to predict against the decision tree
            int testAmount = GetUserInput("Amount of data to predict");

            double[] testInputs;
            int outcomeIndex;
            string actualOutcome;
            var scoreCard = new List<bool>();
            string[] possibleOutcomes = { "Won't Pass", "Will Pass" };
            string predictedOutcome;

            WriteLine($"\n{"Initial Seperation",18}" +
                $"{"Overtaking Speed",23}" +
                $"{"Oncoming Speed",21}" +
                $"{"Outcome",14}" +
                $"{"Prediction",17}");

            //Loop for amount of times that want to be predicted
            for (int i = 0; i < testAmount; i++)
            {
                //Get the data from OvertakeAI
                overtake = OvertakeData.GetData();

                testInputs = new double[3] { 
                    overtake.InitialSeparationM, 
                    overtake.OvertakingSpeedMPS,
                    overtake.OncomingSpeedMPS 
                };

                actualOutcome = overtake.Success ? "Will Pass" : "Won't Pass";

                //Preict the result using the decision tree
                outcomeIndex = tree.Decide(testInputs); 

                //Compare actual outcome to the predicted outcome 
                scoreCard.Add(actualOutcome == possibleOutcomes[outcomeIndex]);

                //Print out the data
                predictedOutcome = scoreCard[i] ? "Correct" : "Incorrect";
                WriteLine($"{testInputs[0],18}" +
                    $"{testInputs[1],23}" +
                    $"{testInputs[2],21}" +
                    $"{actualOutcome,14}" +
                    $"{predictedOutcome,17}");
            }

            //Count amount of correct values in score card to show accuracy percentage
            WriteLine($"\nAccuracy: {Round((scoreCard.Count(x => x) / ToDouble(scoreCard.Count)) * 100, 2)}%");

            //Get the training error of the decision tree
            int[] predicted = tree.Decide(trainInputs);
            double error = new ZeroOneLoss(trainOutputs).Loss(predicted);
            WriteLine($"Training Error: {Round(error, 2)}\n");

            //Print out the rules that the decision tree came up with
            WriteLine("Decision Tree Rules:");
            DecisionSet rules = tree.ToRules();
            var codebook = new Codification("Possible Results", possibleOutcomes);
            var encodedRules = rules.ToString(codebook, "Possible Results", CultureInfo.InvariantCulture);
            WriteLine($"{encodedRules}");
        }

        //Make sure user is inputting an int when required
        public int GetUserInput(string text)
        {
            int input;

            Write($"{text}: ");

            //Keep asking for input until a valid input is given
            while (!int.TryParse(ReadLine(), out input))
            {
                Write($"{text}: ");
            }

            return input;
        }
    }
}
