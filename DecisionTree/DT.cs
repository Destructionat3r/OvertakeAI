using System;
using System.Data; // for DataTable
using System.Linq;
using Accord.MachineLearning.DecisionTrees; // for DecisionVariable
using Accord.MachineLearning.DecisionTrees.Learning; // for ID3Learning
using Accord.MachineLearning.DecisionTrees.Rules; // for DecisionSet
using Accord.Math; // for DataTable extensions such as ToArray;
using Accord.Math.Optimization.Losses; // for ZeroOneLoss
using Accord.Statistics.Filters; // for Codification
using OvertakeAI;

namespace DecisionTreeSolution
{
    class DT
    {
        public void Run()
        {
            Library.Overtake overtake;
            string[] possibleResults = { "Won't Pass", "Will Pass" };

            var title = "Decision Tree Method";
            Console.WriteLine(title);

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

            overtake = OvertakeData.GetData();
            double[] query = { overtake.InitialSeparationM, overtake.OvertakingSpeedMPS, overtake.OncomingSpeedMPS };
            string actualOutcome;
            if (overtake.Success == false)
                actualOutcome = "Won't Pass";
            else
                actualOutcome = "Will Pass";

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

            Console.WriteLine($"{encodedRules}\n");

            Console.ReadKey();
        }
    }
}
