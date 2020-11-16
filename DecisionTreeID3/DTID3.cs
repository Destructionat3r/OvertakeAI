using System;
using System.Data;
using System.Linq;
using Accord.MachineLearning.DecisionTrees; //For Decision Variable
using Accord.MachineLearning.DecisionTrees.Learning; // For ID3 Learning
using Accord.MachineLearning.DecisionTrees.Rules; //For Decision Set
using Accord.Math; //For DataTable Extensions
using Accord.Math.Optimization.Losses; //For ZeroOneLoss
using Accord.Statistics.Filters; //For Codification
using OvertakeAI; //For Getting Data

namespace DecisionTreeID3
{
    class DTID3
    {
        public void Run()
        {
            Library.Overtake overtake;
            var title = "Decision Tree - ID3 Learning";
            Console.WriteLine(title);

            Console.WriteLine("Decision Tree - ID3 Learning");
            Console.Write("Amount of data to train: ");
            int train = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine();

            DataTable data = new DataTable(title);
            data.Columns.Add("InitialSeperation", typeof(String));
            data.Columns.Add("OvertakingSpeed", typeof(String));
            data.Columns.Add("OncomingSpeed", typeof(String));
            data.Columns.Add("Success", typeof(String));

            for (int i = 0; i < train; i++)
            {
                overtake = OvertakeData.GetData();
                data.Rows.Add($"{overtake.InitialSeparationM:F0}", $"{overtake.OvertakingSpeedMPS:F0}", $"{overtake.OncomingSpeedMPS:F0}", overtake.Success);
            }

            var codebook = new Codification(data);
            DataTable symbols = codebook.Apply(data);
            int[][] inputs = symbols.ToJagged<int>(new string[] { "InitialSeperation", "OvertakingSpeed", "OncomingSpeed" });
            int[] outputs = symbols.ToArray<int>("Success");
            string[] outcomes = new string[] { "Won't Pass", "Will Pass" };

            var id3Learning = new ID3Learning()
            { 

            };

        }
    }
}
