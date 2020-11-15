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
            var title = "Decision Tree Method";
            Console.WriteLine(title);

            Console.Write("Amount of data to train: ");
            int train = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            double[][] inputs = new double[3][] { new double[train], new double[train], new double[train] };
            int[] outputs = new int[train];

            for (int i = 0; i < train; i++)
            {
                Library.Overtake overtake = OvertakeData.GetData();
                inputs[0][i] = overtake.InitialSeparationM;
                inputs[1][i] = overtake.OvertakingSpeedMPS;
                inputs[2][i] = overtake.OncomingSpeedMPS;
                outputs[i] = Convert.ToInt32(overtake.Success);
            }

            for (int i = 0; i < inputs[0].Length; i++)
            {
                Console.WriteLine($"Element {i}:");
                Console.WriteLine($"InitialSeparation = {inputs[0][i]:F1} metres");
                Console.WriteLine($"OvertakingSpeed = {inputs[1][i]:F1} m/s");
                Console.WriteLine($"OncomingSpeed = {inputs[2][i]:F1} m / s");
                Console.WriteLine($"Success = {outputs[i]}\n");
            }

            var learningAlgorithm = new C45Learning();
            DecisionTree tree = learningAlgorithm.Learn(inputs, outputs);

            Console.ReadKey();
        }
    }
}
