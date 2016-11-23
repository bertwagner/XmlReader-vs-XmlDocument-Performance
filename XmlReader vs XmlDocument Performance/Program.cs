using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace XmlReader_vs_XmlDocument_Performance
{
    class Program
    {
        static void Main(string[] args)
        {
            // We are using the Photography Stack Exchange users data dump as our sample file.
            // It is available at https://archive.org/details/stackexchange under the 
            // Attribution -ShareAlike Creative Commons license: http://creativecommons.org/licenses/by-sa/3.0/
            string filePath = "../../Users.xml";

            // Run the performance test on the XmlReaderTest method
            double xmlReaderTime = RunPerformanceTest(filePath, XmlReaderTest);
            Console.WriteLine("XmlReaderTest method averaged {0} seconds", xmlReaderTime);

            // Run the performance test on the XmlDocumentTest method
            double xmlDocumentTime = RunPerformanceTest(filePath, XmlDocumentTest);
            Console.WriteLine("XmlDocumentTest method averaged {0} seconds", xmlDocumentTime);

            // Pause the console to view results
            Console.ReadKey();
        }

        /// <summary>
        /// This method runs a passed in method 50 times and returns the average elapsed time
        /// </summary>
        /// <param name="filePath">XML file path</param>
        /// <param name="performanceTestMethod">Name of method to test</param>
        /// <returns></returns>
        public static double RunPerformanceTest(string filePath, Action<string> performanceTestMethod)
        {
            Stopwatch sw = new Stopwatch();

            int iterations = 50;
            double elapsedMilliseconds = 0;

            // Run the method 50 times to rule out any bias.
            for (var i = 0; i < iterations; i++)
            {
                sw.Restart();
                performanceTestMethod(filePath);
                sw.Stop();

                elapsedMilliseconds += sw.ElapsedMilliseconds;
            }

            // Calculate the average elapsed seconds per run
            double avergeSeconds = (elapsedMilliseconds / iterations) / 1000.0;

            return avergeSeconds;
        }

        public static void XmlReaderTest(string filePath)
        {
            // We create storage for ids of all of the rows from users where reputation == 1
            List<string> singleRepRowIds = new List<string>();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        if (reader.Name == "row" && reader.GetAttribute("Reputation") == "1")
                        {
                            singleRepRowIds.Add(reader.GetAttribute("Id"));
                        }
                    }
                }
            }
        }

        public static void XmlDocumentTest(string filePath)
        {
            List<string> singleRepRowIds = new List<string>();

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            var test = doc.GetElementsByTagName("row").Cast<XmlNode>().Where(x => x.Attributes["Reputation"].InnerText == "1").Select(x => x.Attributes["Id"].InnerText).ToList();
        }
    }
}
