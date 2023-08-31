using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Language;

namespace PowerShellScriptSimilarity
{
    class Program
    {
        static void Main(string[] args)
        {
          

            string folderPath = "C:\\Users\\TEK\\Desktop\\PS AST\\samples\\";

            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Folder not found.");
                return;
            }

            List<List<string>> scriptTokens = new List<List<string>>();

            // Tokenize scripts and store tokens in a list for each script
            foreach (string scriptFilePath in Directory.GetFiles(folderPath, "*.ps1"))
            {
                string scriptContent = File.ReadAllText(scriptFilePath);
                List<string> tokens = TokenizePowerShellScript(scriptContent);
                scriptTokens.Add(tokens);
            }

            // Calculate and print Jaccard similarity scores
            Console.WriteLine("Jaccard Similarity Scores:");

            double totalSimilarityScore = 0;
            int totalComparisons = 0;

            for (int i = 0; i < scriptTokens.Count; i++)
            {
                for (int j = i + 1; j < scriptTokens.Count; j++)
                {
                    double similarity = CalculateJaccardSimilarity(scriptTokens[i], scriptTokens[j]);
                    Console.WriteLine($"Scripts {i + 1} and {j + 1}: {similarity:P}");
                    totalSimilarityScore += similarity;
                    totalComparisons++;
                }
            }

            // Calculate and print average similarity score
            double averageSimilarityScore = totalSimilarityScore / totalComparisons;
            Console.WriteLine($"Average Similarity Score: {averageSimilarityScore:P}");
        }

        static List<string> TokenizePowerShellScript(string script)
        {
            List<string> tokens = new List<string>();

            Token[] parsedTokens;
            ParseError[] errors;

            // Tokenize the script
            Parser.ParseInput(script, out parsedTokens, out errors);

            if (errors.Length > 0)
            {
                Console.WriteLine("Parsing errors:");
                foreach (var error in errors)
                {
                    Console.WriteLine($"Line {error.Extent.StartLineNumber}: {error.Message}");
                }
            }
            else
            {
                tokens.AddRange(parsedTokens.Select(token => token.Text));
            }

            return tokens;
        }

        static double CalculateJaccardSimilarity(List<string> list1, List<string> list2)
        {
            double intersectionCount = list1.Intersect(list2).Count();
            double unionCount = list1.Union(list2).Count();

            return intersectionCount / unionCount;
        }
    }
}
