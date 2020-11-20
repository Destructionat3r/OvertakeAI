using System;
using System.Collections.Generic;
using System.Linq;
using Accord.MachineLearning.DecisionTrees; // For Decision Tree
using Accord.MachineLearning.DecisionTrees.Learning; // For C45 Learning
using Accord.MachineLearning.DecisionTrees.Rules; // For Decision Set
using Accord.Math.Optimization.Losses; // For ZeroOneLoss
using Accord.Statistics.Filters; // For Codification
using OvertakeAI; //For Getting Data

namespace DecisionTreeC45Solution
{
    class DTC45
    {
        public void Run()
        {
            Library.Overtake overtake;
            string[] possibleResults = { "Won't Pass", "Will Pass" };

            Console.WriteLine("Decision Tree - C45 Learning");
            Console.Write("Amount of data to train: ");
            int train = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            double[][] inputs = new double[train][];
            int[] outputs = new int[train];

            for (int i = 0; i < train; i++)
            {
                overtake = OvertakeData.GetData();
                inputs[i] = new double[3] { overtake.InitialSeparationM, overtake.OvertakingSpeedMPS, overtake.OncomingSpeedMPS };
                outputs[i] = Convert.ToInt32(overtake.Success);
            }

            var learningAlgorithm = new C45Learning();
            DecisionTree tree = learningAlgorithm.Learn(inputs, outputs);

            Console.Write("Amount of data to predict: ");
            int test = Convert.ToInt32(Console.ReadLine());
            double[] query = new double[3];
            var scoreCard = new List<bool>();
            string answerOutcome;

            Console.WriteLine("Initial Seperation       Overtaking Speed       Oncoming Speed       Outcome       Prediction");
            for (int i = 0; i < test; i++)
            {
                overtake = OvertakeData.GetData();
                query[0] = overtake.InitialSeparationM;
                query[1] = overtake.OvertakingSpeedMPS;
                query[2] = overtake.OncomingSpeedMPS;
                outputs[i] = Convert.ToInt32(overtake.Success);
                int predictedSingle = tree.Decide(query); 
                string actualOutcome = overtake.Success ? "Will Pass" : "Won't Pass";
                scoreCard.Add(actualOutcome == possibleResults[predictedSingle]); 
                actualOutcome = Convert.ToBoolean(overtake.Success) ? "Will Pass" : "Won't Pass";
                answerOutcome = scoreCard[i] ? "Correct" : "Incorrect";
                Console.WriteLine($"{query[0],18}{query[1],23}{query[2],21}{actualOutcome,14}{answerOutcome,17}");
            }

            Console.WriteLine($"\nPerformance: {(scoreCard.Count(x => x) / Convert.ToDouble(scoreCard.Count)) * 100}%");

            int[] predicted = tree.Decide(inputs);
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            Console.WriteLine($"Training Error: {Math.Round(error, 2)}\n");

            Console.WriteLine("Decision Tree Rules:");
            DecisionSet rules = tree.ToRules();
            var codebook = new Codification("Possible Results", possibleResults);
            var encodedRules = rules.ToString(codebook, "Possible Results", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine($"{encodedRules}");

            Console.ReadKey();
        }
    }
}
