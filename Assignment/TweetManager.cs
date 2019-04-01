using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Assignment
{
    /// <summary>
    /// This is main manager class , This class used for getting all Tweets and loding to hastable
    /// </summary>
    /// <remarks>
    /// This has following public property
    ///     TweetList : this is a hash table contains list of tweets
    /// this class has following public method
    ///     LoadAllTweetMessages : It requires start date & time and end Date time for retraiving tweets
    /// </remarks>
    class TweetManager
    {


        private const string apiUrl = "https://badapi.iqvia.io/api/v1/Tweets";

        // Private variable for holding all tweets
        private Hashtable _tweetList;
        // constent for maximum number of records can return from API
        private const int _intMaxRecords = 100;

        /// <summary>
        /// public property , which holds all tweet list
        /// </summary>
        public Hashtable TweetList { get { return _tweetList;  } }

        public TweetManager()
        {
            // intializing the Tweet list
            _tweetList = new Hashtable();
        }
        /// <summary>
        /// This method calls API and loads all tweets to TweetList
        /// </summary>
        /// <param name="startDate"> This should be dateTime format</param>
        /// <param name="endDate">This should be date datetime format</param>
        /// <returns> returns true if it secssfully process , In case of error it returns error message.
        /// </returns>
        public async Task<bool> LoadAllTweetMessages(DateTime startDate, DateTime endDate)
        {
            // checking for edge cases for start and end dates
            if(startDate==null || endDate == null)
            {
                throw new System.Exception("Unable to process,Start or End Date is null " );
            }      
            // checking for completion
            if (endDate<=startDate)
            {
                return true;
            }
            else {
                // get next start date.
                Console.WriteLine(" Processing for dates: " + startDate.ToString() + " and  " + endDate.ToString());
                
                // Calling Recursive function for all tweets, and getting the next start date
                startDate =await RecursiveGetMessage(startDate, endDate);
                
                
            }
            // calling recursively till start date and end were same
            return LoadAllTweetMessages(startDate, endDate).Result;

        }
        /// <summary>
        /// This calls external API and checks for total records if the total records is 100 then it assumes more tweets and divides the start date and end dates to half and recursively calls till it reaches less than 100..
        ///Example : first it starts with full start Date(01/01/2016) and end date 12/31/2017,
        ///if there were more than 100 tweets.Then it goes to 01/01/2016 to 12/31/2016. 
        ///If start date and end date become same it start reducing hours, if hours become same it 
        ///reduces minutes then second and milliseconds.
        /// </summary>
        /// <param name="startDate">Date time </param>
        /// <param name="endDate"> DateTime</param>
        /// <returns></returns>
        private async Task<DateTime> RecursiveGetMessage(DateTime startDate, DateTime endDate)
        {
            //check for number of records is less than 100
            if(GetTweetsAsync(startDate, endDate).Result == _intMaxRecords)
            {
                // calculating the next end date
                endDate = GetNextDate(startDate, endDate);

                return await RecursiveGetMessage(startDate, endDate);
            }
            else
            {
                // once it reaches less than 100 it retuns that date
                return endDate;
            }
        }

        /// <summary>
        /// Private function used for used for calculating next date. 
        /// This function calculates timespan and if the difference is in days, Hour, minutes, seconds, milliseconds 
        /// it reduces accordingly
        /// </summary>
        /// <param name="start">DateTime</param>
        /// <param name="end">DateTime</param>
        /// <returns>calculated next date</returns>
        private DateTime GetNextDate(DateTime start,DateTime end)
        {
            //Calculating the date diffrence between two dates 
            var dateDiff= (end - start);

            //if the dates is greater
            if (dateDiff.TotalDays > 0)
            {             
                return end.AddDays(-(dateDiff.TotalDays / 2));
            }
            // checking hours
            else if (dateDiff.TotalHours > 0)
            {
                return end.AddHours(-(dateDiff.TotalHours/2));
            }
            // checking only minutes
            else if (dateDiff.TotalMinutes > 0)
            {
                return end.AddMinutes(-(dateDiff.TotalMinutes));

            }
            else if (dateDiff.TotalSeconds > 0)
            {  
                return end.AddSeconds(-(dateDiff.TotalSeconds));

            }
            else if (dateDiff.TotalMilliseconds > 0)
            {
               return end.AddMilliseconds(-(dateDiff.TotalMilliseconds));

            }
            // both were matching return the same date.
            else { return start; }

            
        }

        /// <summary>
        /// This function calls external API and checks for duplicate and loads to TweetList 
        /// </summary>
        /// <param name="startDate"> start date</param>
        /// <param name="endDate">end date</param>
        /// <returns>Number of tweets retrieved,if any error it sends back error </returns>
        private async Task<int> GetTweetsAsync(DateTime startDate,DateTime endDate)
        {
           
            
            //this varibale holds total number of tweets recived
            int TweetCount = 0;

            //calling iqvia is for getting tweets for the specified time.
            using (HttpClient request = new HttpClient())
            {
                //url for the tweets
                request.BaseAddress = new Uri(apiUrl);
                request.DefaultRequestHeaders.Accept.Clear();

                using (HttpResponseMessage response = await request.GetAsync("?startDate=" + startDate + "&endDate=" + endDate))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        // reading all tweets to string
                        string tweetList = await response.Content.ReadAsStringAsync();
                        
                        // local variable for holding the tweets 
                        List<Tweet> listOfTweets = new List<Tweet>();

                        listOfTweets = JsonConvert.DeserializeObject<List<Tweet>>(tweetList);

                        // total number of Tweets
                        TweetCount = listOfTweets.Count;

                        foreach (Tweet twt in listOfTweets)
                        {
                            //making sure it has unique key for hash table
                            string key = twt.id.ToString() + twt.stamp.Ticks.ToString();
                            // checking for duplicate
                            if (!_tweetList.ContainsKey(key))
                            {
                                _tweetList.Add(key, twt);
                            }
                        }
                    }
                    else
                    {
                        // if status code is diffrent return as error
                        throw new System.Exception("Unable to connect to iqvia API, error code:" + response.StatusCode);
                    }
                }   
            }
            //return the total number of tweet recived
            return TweetCount;
        }
    }

}

