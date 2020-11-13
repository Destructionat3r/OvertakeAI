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

            DataTable data = new DataTable(title);


        }
    }
}
