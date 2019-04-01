using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment
{


    /// <summary>
    /// this is POCO class for Tweet
    /// .
    /// </summary>
    /// <remarks>
    /// This class used for serializing tweets
    /// </remarks>
    [Serializable]
    public class Tweet
    {
        public long id { get; set; }
        public DateTime stamp { get; set; }
        public string text { get; set; }
    }
}
