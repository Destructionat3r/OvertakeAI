using System;
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
                inputs[i] = new double[3] { Math.Round(overtake.InitialSeparationM, 2), Math.Round(overtake.OvertakingSpeedMPS), Math.Round(overtake.OncomingSpeedMPS) };
                outputs[i] = Convert.ToInt32(overtake.Success);
            }

            var learningAlgorithm = new C45Learning();
            DecisionTree tree = learningAlgorithm.Learn(inputs, outputs);

            overtake = OvertakeData.GetData();
            double[] query = { overtake.InitialSeparationM, overtake.OvertakingSpeedMPS, overtake.OncomingSpeedMPS };
            string actualOutcome = overtake.Success ? "Will Pass" : "Won't Pass";

            int predictedSingle = tree.Decide(query);
            Console.WriteLine($"Initial Seperation: {query[0]:F1}m" +
                $"\nOvertaking Speed: {query[1]:F1}m/s" +
                $"\nOncoming Speed: {query[2]:F1}m/s" +
                $"\nPredicted Outcome: {possibleResults[predictedSingle]}" +
                $"\nActual Outcome: {actualOutcome}\n");

            int[] predicted = tree.Decide(inputs);
            double error = new ZeroOneLoss(outputs).Loss(predicted);
            Console.WriteLine($"Training Error: {Math.Round(error, 2)}\n");

            DecisionSet rules = tree.ToRules();
            var codebook = new Codification("Possible Results", possibleResults);
            var encodedRules = rules.ToString(codebook, "Possible Results", System.Globalization.CultureInfo.InvariantCulture);
            Console.WriteLine($"{encodedRules}");

            Console.ReadKey();
        }
    }
}
