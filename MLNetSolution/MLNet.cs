using System;
using System.Collections.Generic; //For ScoreCard Bool List
using System.Linq; //For Counting ScoreCard
using System.IO; //For Writing To CSV File
using System.Text; //For Using StringBuilder
using OvertakeAI; //For Getting Overtake Data
using OvertakeAIML.Model; //For The ML.Net Model
using static System.Console; //For Read/Write Line

namespace MLNetSolution
{
    class MLNet
    {
        public void Run()
        {
            string path = "testData.csv";
            Library.Overtake overtake;
            bool firstSuccess = true;
            var csv = new StringBuilder();

            //Get amount of data the user wants the decision tree to train
            WriteLine("ML.Net");
            Write("Amount of data to train: ");
            int train = Convert.ToInt32(ReadLine());
            WriteLine();

            //Create file if it doesn't exist or overwrite it with null data
            File.WriteAllText(path, null);
            var data = "InitialSeperation,OvertakingSpeed,OncomingSpeed,Success";
            csv.AppendLine(data);

            //Get data from OvertakeAI program and add it to csv var
            for (int i = 0; i < train; i++)
            {
                overtake = OvertakeData.GetData();

                //Store first success result so table can be formatted correctly later
                if (i == 0)
                    firstSuccess = overtake.Success;

                data = $"{overtake.InitialSeparationM},{overtake.OvertakingSpeedMPS},{overtake.OncomingSpeedMPS},{overtake.Success}";
                csv.AppendLine(data);
            }

            //Write all the data to csv file
            File.AppendAllText(path, csv.ToString());

            //Create ML.Net model
            ModelBuilder.CreateModel();


            //Get the amount of data the user wants to predict against the decision tree
            Write("\nAmount of data to predict: ");
            int test = Convert.ToInt32(ReadLine());

            int[] testOutputs = new int[test];
            string actualOutcome;
            int index;
            var scoreCard = new List<bool>();
            string[] possibleResults = { "Won't Pass", "Will Pass" };
            string answerOutcome;

            //Test data and print it out
            WriteLine("\nUsing model to make predictions -- Comparing actual Success with predicted Success from sample data...\n");

            WriteLine($"Initial Seperation       Overtaking Speed       Oncoming Speed       Outcome       Prediction     Pass Chance     Won't Pass Chance");
            for (int i = 0; i < test; i++)
            {
                //Get the data from OvertakeAI
                overtake = OvertakeData.GetData();
                ModelInput testInputs = new ModelInput() 
                { 
                    InitialSeperation = (float)overtake.InitialSeparationM, 
                    OvertakingSpeed = (float)overtake.OvertakingSpeedMPS, 
                    OncomingSpeed = (float)overtake.OncomingSpeedMPS };
                string outcome = overtake.Success.ToString();
                actualOutcome = overtake.Success ? "Will Pass" : "Won't Pass";

                //Preict the result using the model
                var predictionResult = ConsumeModel.Predict(testInputs);

                //Compare actual outcome to the predicted outcome
                if (predictionResult.Prediction == "False")
                    index = 0;
                else
                    index = 1;

                scoreCard.Add(actualOutcome == possibleResults[index]);
                answerOutcome = scoreCard[i] ? "Correct" : "Incorrect";

                //Print out the prediction data
                Write($"{testInputs.InitialSeperation,18}" +
                    $"{testInputs.OvertakingSpeed,23}" +
                    $"{testInputs.OncomingSpeed,21}" +
                    $"{actualOutcome,14}" +
                    $"{answerOutcome,17}");

                if (firstSuccess == true)
                    WriteLine($"{Math.Round(predictionResult.Score[0] * 100, 2),15}%" +
                        $"{Math.Round(predictionResult.Score[1] * 100, 2),21}%");
                else
                    WriteLine($"{Math.Round(predictionResult.Score[1] * 100, 2),15}%" +
                        $"{Math.Round(predictionResult.Score[0] * 100, 2),21}%");
            }

            //Count amount of correct values in score card to show accuracy percentage
            WriteLine($"\nAccuracy: {(scoreCard.Count(x => x) / Convert.ToDouble(scoreCard.Count)) * 100}%");
            WriteLine("=============== End of process, hit any key to finish ===============");
        }
    }
}
