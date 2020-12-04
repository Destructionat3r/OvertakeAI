using System.IO; //For Writing To CSV File
using System.Linq; //For Counting ScoreCard
using System.Collections.Generic;
using System.Text;

namespace NeuralNetwork
{
    class OutputData
    {
        public static void OutputCsv(string data)
        {
            string path = @"..\..\..\neuralNetworkLog.csv";
            var csv = new StringBuilder();

            //Check if neuralNetworkLog exists
            if (!File.Exists(path))
            {
                //Create file if it doesn't exist with no data
                File.WriteAllText(path, null);

                var nNHeadings = "TestNo," +
                    "TrainAmount," +
                    "HiddenLayerNodes," +
                    "LearningRate," +
                    "LowestTrainingError," +
                    "LowestTrainingErrorEpoch," +
                    "FinalTrainingError," +
                    "TotalEpochs," +
                    "TestAmount," +
                    "Accuracy";

                csv.AppendLine(nNHeadings);
            }

            //Load data from neuralNetworkLog and skip headings
            List<string> loadedCsv = File.ReadAllLines(path).Skip(1).ToList();

            int testNo = loadedCsv.Count() + 1;

            //Format testNo to be before the test data
            var nNData = $"{testNo},{data}";

            csv.AppendLine(nNData);

            //Write the data to csv file
            File.AppendAllText(path, csv.ToString());
        }
    }
}
