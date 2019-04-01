using System;
using System.IO;
using System.Net.Http;
using System.Text;

namespace Assignment
{
    class Program
    {
        // out put file for the result
        static string _FilePath = @"C:\temp\tweets.txt";

        // start datetime
        static DateTime start = DateTime.Parse("2016-01-01 00:00:00 AM");

        //end date time
        static DateTime end = DateTime.Parse("2017-12-31 11:59:59 PM");
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, please wait..processing....");

            TweetManager tweetProcess = new TweetManager();
            try
            {
                /// loading all tweet messages
                if (tweetProcess.LoadAllTweetMessages(start, end).Result)
                {
                    using (var fileStore = new StreamWriter(_FilePath))
                    {
                        //Writing the header
                        fileStore.WriteLine($"ID \t DateTime \t Tweet");
                        fileStore.Flush();
                        foreach (Tweet tw in tweetProcess.TweetList.Values)
                        {
                            string id = tw.id.ToString();
                            string stamp = tw.stamp.ToString();
                            string text = tw.text.ToString();

                            var newLine = $"{id} \t {stamp} \t {text}";
                            fileStore.WriteLine(newLine);
                            fileStore.Flush();
                        }
                    }

                }
                Console.WriteLine("completed press any key to exit,Total Tweets : " + tweetProcess.TweetList.Count.ToString());
                Console.ReadLine();
            }
            catch(IOException IOError)
            {
                Console.WriteLine("error Processing , Please make sure you have folder and permission for C:\\Temp " + IOError.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("error processing " + ex.Message);
            }
        }
    }
}
