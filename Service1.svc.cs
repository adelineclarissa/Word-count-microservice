using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

namespace WordCountService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public string WordCount(string filePath)
        {
            // handle if the filename doesn't contain .txt
            if(!filePath.Contains(".txt"))
            {
                filePath += ".txt";
                Console.WriteLine("Modified file path: " + filePath);
            }

            // get the full file path
            string fullFilePath = "";

            if(filePath.Contains("\\") || filePath.Contains("/"))
            {
                fullFilePath = filePath;
            } 
            else
            {
                // NOTE: the file should be located at the same directory as this code
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                fullFilePath = Path.Combine(baseDirectory, filePath);
            }
            

            try
            {
                
                StreamReader reader = new StreamReader(fullFilePath);

                Dictionary<string, int> wordCounts = new Dictionary<string, int>();

                // read all the text as one chunk of string
                string fileContent = reader.ReadToEnd();

                // use regex to split the text into words
                string[] words = Regex.Split(fileContent, @"\W+");

                foreach (string word in words)
                {
                    string cleanedWord = word.Trim().ToLower(); // Remove leading/trailing whitespace and convert to lowercase
                    if (!string.IsNullOrEmpty(cleanedWord))
                    {
                        if (wordCounts.ContainsKey(cleanedWord))
                        {
                            wordCounts[cleanedWord]++;
                        }
                        else
                        {
                            wordCounts[cleanedWord] = 1;
                        }
                    }
                }

                // convert the dictionary to a json format
                string jsonString = DictionaryToJson(wordCounts);
                return jsonString;


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return (fullFilePath);
            }

        }

        static string DictionaryToJson(Dictionary<string, int> dictionary)
        {
            StringBuilder json = new StringBuilder();
            json.Append("{");

            foreach (var kvp in dictionary)
            {
                json.Append($"\"{kvp.Key}\": {kvp.Value},");
            }

            // Remove the trailing comma if there are items in the dictionary
            if (dictionary.Count > 0)
            {
                json.Length--; // Remove the last character (comma)
            }

            json.Append("}");

            return json.ToString();
        }
    }

}
