using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleNet4
{
    class TwentyMinutes
    {

        public static void Vote20Min(string story, string storyId, string thread, string msgId, int vote)
        {
            string voteURL = "https://www.20min.ch/community/storydiscussion/vote.tmpl?storyid=27426069&thread=3373&msgid=3373&vote=0";
            voteURL = "https://www.20min.ch/community/storydiscussion/vote.tmpl"
                + "?storyid=" + storyId
                + "&thread=" + thread
                + "&msgid=" + msgId
                + " &vote=" + vote.ToString(System.Globalization.CultureInfo.InvariantCulture);

            // string story = "https://www.20min.ch/schweiz/news/story/foobar-" + storyId;

            using (WebClientEx wc = new WebClientEx())
            {

                wc.Headers.Add("Accept", "*/*");
                wc.Headers.Add("Accept-Encoding", "gzip, deflate, br");

                wc.DownloadString(story);

                string value = "{\"thread" + thread + "_msg" + msgId + "_" + storyId + "\":1,\"casts\":[\"thread" + thread + "_msg" + msgId + "_" + storyId + "\"]}";
                value = System.Uri.EscapeDataString(value);

                System.Net.Cookie talkback = new System.Net.Cookie("talkback", value, "/", "www.20min.ch");
                System.Net.Cookie tda = new System.Net.Cookie("tda", "abc", "/20min.ch", "vv.20min.ch");
                wc.CookieContainer.Add(talkback);
                wc.CookieContainer.Add(tda);

                wc.Headers.Add("Referer", story);
                wc.Headers.Add("X-Requested-With", "XMLHttpRequest");
                string ds = wc.DownloadString(voteURL);
                System.Console.WriteLine(ds);
            }


        }



        public static void Vote20Min()
        {
            string url = "https://www.20min.ch/schweiz/news/story/GA-wird-teurer--kein-Studentenrabatt-mehr-27426069";

            string storyId = "27426069";
            string thread = "3373";
            string msgId = "3373";
            int vote = 1;

            // @eine GA Besitzerin
            thread = "3367";
            msgId = "3385";
            vote = -1;


            // Zugfahren ist etwas für die armen
            // Path: /community/storydiscussion/vote.tmpl?storyid=27426069&thread=3399&msgid=3399&vote=1
            thread = "3399";
            msgId = "3399";
            vote = -1;

            Vote20Min(url, storyId, thread, msgId, vote);
        }


        public static void Test()
        {
            for (int i = 0; i < 10; ++i)
            {
                Vote20Min();
                System.Threading.Thread.Sleep(5000);
            }
        }

    }
}
