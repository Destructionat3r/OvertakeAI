using System;
using System.Collections.Generic; //For ScoreCard Bool List
using System.Linq; //For Counting ScroreCard
using Accord.MachineLearning.DecisionTrees; //For Decision Tree
using Accord.MachineLearning.DecisionTrees.Learning; //For C45 Learning
using Accord.MachineLearning.DecisionTrees.Rules; //For Decision Set
using Accord.Math.Optimization.Losses; //For ZeroOneLoss
using Accord.Statistics.Filters; //For Codification
using OvertakeAI; //For Getting Overtake Data

namespace DecisionTreeC45Solution
{
    class DTC45
    {
        public void Run()
        {
            Library.Overtake overtake;
            string[] possibleResults = { "Won't Pass", "Will Pass" };


            //Get amount of data the user wants the decision tree to train
            Console.WriteLine("Decision Tree - C45 Learning");
            Console.Write("Amount of data to train: ");
            int train = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            double[][] trainInputs = new double[train][];
            int[] trainOutputs = new int[train];

            //Get data from OvertakeAI program and insert it into train inputs and outputs arrays
            for (int i = 0; i < train; i++)
            {
                overtake = OvertakeData.GetData();
                trainInputs[i] = new double[3] { overtake.InitialSeparationM, overtake.OvertakingSpeedMPS, overtake.OncomingSpeedMPS };
                trainOutputs[i] = Convert.ToInt32(overtake.Success);
            }

            //Run decison tree using C4.5 algorithm using the train inputs and outputs
            var learningAlgorithm = new C45Learning();
            DecisionTree tree = learningAlgorithm.Learn(trainInputs, trainOutputs);


            //Get the amount of data the user wants to predict against the decision tree
            Console.Write("Amount of data to predict: ");
            int test = Convert.ToInt32(Console.ReadLine());

            double[] testInputs = new double[3];
            int[] testOutputs = new int[test];
            int predictedSingle;
            string actualOutcome;
            var scoreCard = new List<bool>();
            string answerOutcome;

            //Test data and print it out
            Console.WriteLine("Initial Seperation       Overtaking Speed       Oncoming Speed       Outcome       Prediction");
            for (int i = 0; i < test; i++)
            {
                //Get the data from OvertakeAI
                overtake = OvertakeData.GetData();
                testInputs[0] = overtake.InitialSeparationM;
                testInputs[1] = overtake.OvertakingSpeedMPS;
                testInputs[2] = overtake.OncomingSpeedMPS;
                trainOutputs[i] = Convert.ToInt32(overtake.Success);

                //Preict the result using the decision tree
                predictedSingle = tree.Decide(testInputs); 

                //Compare actual outcome to the predicted outcome 
                actualOutcome = overtake.Success ? "Will Pass" : "Won't Pass";
                scoreCard.Add(actualOutcome == possibleResults[predictedSingle]); 
                actualOutcome = Convert.ToBoolean(overtake.Success) ? "Will Pass" : "Won't Pass";
                answerOutcome = scoreCard[i] ? "Correct" : "Incorrect";
                Console.WriteLine($"{testInputs[0],18}{testInputs[1],23}{testInputs[2],21}{actualOutcome,14}{answerOutcome,17}");
            }

            //Count amount of correct values in score card to show accuracy percentage
            Console.WriteLine($"\nAccuracy: {(scoreCard.Count(x => x) / Convert.ToDouble(scoreCard.Count)) * 100}%");

            //Get the training error of the decision tree
            int[] predicted = tree.Decide(trainInputs);
            double error = new ZeroOneLoss(trainOutputs).Loss(predicted);
            Console.WriteLine($"Training Error: {Math.Round(error, 2)}\n");


            //Print out the rules that the decision tree came up with
            Console.WriteLine("Decision Tree Rules:");
            DecisionSet rules = tree.ToRules();
            var codebook = new Codification("Possible Results", possibleResults);
            var encodedRules = rules.ToString(codebook, "Possible Results", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine($"{encodedRules}");

            Console.ReadKey();
        }
    }
}
