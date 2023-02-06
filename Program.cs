using Newtonsoft.Json.Linq;

namespace JsonParseFromCsv
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Setup Parameters
            string[] specificColumns =
            {
                "UserId",
                "CreationTime",
                "WorkSpaceName",
                "DatasetName",
                "ReportName",
                "DistributionMethod",
                "ConsumptionMethod",
                "ArtifactKind"
            };
            string inputPath = @"C:\Users\MParry\OneDrive - Warburtons\Desktop\Total Report Usage.csv";
            string outputPath = @"C:\Users\MParry\OneDrive - Warburtons\Desktop\Output.csv";


            string textInput = File.ReadAllText(inputPath); // Reads input csv to string.
            string[] TextArray = textInput.Split(Environment.NewLine); // puts csv into an array

            string resultCSV = ProcessInputToString(TextArray, specificColumns); // Loops through the array, parses the data,and returns a string

            File.WriteAllText(outputPath, resultCSV); // writes the resulting csv file

            Console.WriteLine("Finished");
            Console.ReadKey();
        }

        public static string ProcessInputToString(string [] TextArray, string [] specificColumns)
        {
            List<string> AllLines = new List<string>();
            int counter = 0;

            // Builds Body
            foreach (var item in TextArray.Skip(1))     // Skips Heading
            {
                if (item != "")
                {
                    string[] Line = item.Split("\"{");
                    string bodyBuilder = "";
                    string End = "{" + Line[1];
                    End = End.Replace("\"\"", "\"");
                    End = End.Remove(End.Length - 2);

                    // Parses JSON and adds requried items to the bodyBuilder
                    dynamic parsed_json = JObject.Parse(End);
                    for (int i = 0; i < specificColumns.Length; i++)
                    {
                        string temp = "";
                        temp += $"\"{parsed_json.GetValue(specificColumns[i])}\",";
                        bodyBuilder += temp;
                    }
                    bodyBuilder = bodyBuilder.Remove(bodyBuilder.Length - 1);
                    AllLines.Add(bodyBuilder + "\r\n");
                }

                // Logs progress
                if (counter % 1000 == 0)
                {
                    Console.WriteLine("Processed Line " + counter);
                }
                counter += 1;
            }

            // Builds Heading
            string headingBuilder = "";
            for (int i = 0; i < specificColumns.Length; i++)
            {
                headingBuilder += specificColumns[i] + ",";
            }
            headingBuilder = headingBuilder.Remove(headingBuilder.Length - 1);


            string Result = headingBuilder + "\r\n" + String.Join(String.Empty, AllLines.ToArray()); // Joins header & body list content to one big string
            return Result;
        }
    }
}