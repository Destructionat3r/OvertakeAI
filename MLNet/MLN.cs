using System.IO; //For Writing To CSV File
using OvertakeAI; //For Getting Overtake Data
using System.Linq; //For Counting ScoreCard
using System.Text; //For Using StringBuilder
using OvertakeAIML.Model; //For The ML.Net Model
using System.Collections.Generic; //For ScoreCard Bool List
using static System.Math; //For Round
using static System.Console; //For Read/Write Line
using static System.Convert; //For Convert

namespace MLNet
{
    class MLN
    {
        public void Run()
        {
            Library.Overtake overtake;
            bool firstSuccess = true;

            WriteLine("ML.Net");

            //Get amount of data the user to train
            int trainAmount = GetUserInput("Amount of data to train");

            //Get the amount of data the user wants to predict against the model
            int testAmount = GetUserInput("Amount of data to predict");
            WriteLine();

            string path = @"..\..\..\testData.csv";
            var csv = new StringBuilder();

            //Create file if it doesn't exist or overwrite it with null data
            File.WriteAllText(path, null);

            var overtakeData = "InitialSeperation," +
                "OvertakingSpeed," +
                "OncomingSpeed," +
                "Success";

            csv.AppendLine(overtakeData);

            //Get data from OvertakeAI program and add it to csv var
            for (int i = 0; i < trainAmount; i++)
            {
                overtake = OvertakeData.GetData();

                //Store first success result so table can be formatted correctly later
                if (i == 0)
                    firstSuccess = overtake.Success;

                overtakeData = $"{overtake.InitialSeparationM}," +
                    $"{overtake.OvertakingSpeedMPS}," +
                    $"{overtake.OncomingSpeedMPS}," +
                    $"{overtake.Success}";

                csv.AppendLine(overtakeData);
            }

            //Write all the data to csv file
            File.AppendAllText(path, csv.ToString());

            //Create ML.Net model
            ModelBuilder.CreateModel();

            int[] testOutputs = new int[testAmount];
            string actualOutcome;
            int outcomeIndex;
            var scoreCard = new List<bool>();
            string[] possibleOutcomes = { "Won't Pass", "Will Pass" };
            string predictionOutcome;
            
            WriteLine($"\n{"Initial Seperation",18}" +
                $"{"Overtaking Speed",23}" +
                $"{"Oncoming Speed",21}" +
                $"{"Outcome",14}" +
                $"{"Prediction",17}" +
                $"{"Pass Chance",16}" +
                $"{"Won't Pass Chance",22}");

            for (int i = 0; i < testAmount; i++)
            {
                //Get the data from OvertakeAI
                overtake = OvertakeData.GetData();

                ModelInput testInputs = new ModelInput() 
                { 
                    InitialSeperation = (float)overtake.InitialSeparationM, 
                    OvertakingSpeed = (float)overtake.OvertakingSpeedMPS, 
                    OncomingSpeed = (float)overtake.OncomingSpeedMPS 
                };

                actualOutcome = overtake.Success ? "Will Pass" : "Won't Pass";

                //Preict the result using the model
                var predictionResult = ConsumeModel.Predict(testInputs);

                //Compare actual outcome to the predicted outcome
                if (predictionResult.Prediction == "False")
                    outcomeIndex = 0;
                else
                    outcomeIndex = 1;

                scoreCard.Add(actualOutcome == possibleOutcomes[outcomeIndex]);
                predictionOutcome = scoreCard[i] ? "Correct" : "Incorrect";

                //Print out the prediction data
                Write($"{testInputs.InitialSeperation,18}" +
                    $"{testInputs.OvertakingSpeed,23}" +
                    $"{testInputs.OncomingSpeed,21}" +
                    $"{actualOutcome,14}" +
                    $"{predictionOutcome,17}");

                if (firstSuccess == true)
                    WriteLine($"{Round(predictionResult.Score[0] * 100, 2),15}%" +
                        $"{Round(predictionResult.Score[1] * 100, 2),21}%");
                else
                    WriteLine($"{Round(predictionResult.Score[1] * 100, 2),15}%" +
                        $"{Round(predictionResult.Score[0] * 100, 2),21}%");
            }

            //Count amount of correct values in score card to show accuracy percentage
            WriteLine($"\nAccuracy: {Round((scoreCard.Count(x => x) / ToDouble(scoreCard.Count)) * 100, 2)}%");
        }

        //Make sure user is inputting an int when required
        public int GetUserInput(string text)
        {
            int input;

            Write($"{text}: ");

            while (!int.TryParse(ReadLine(), out input))
            {
                Write($"{text}: ");
            }

            return input;
        }
    }
}
