using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace WindowsService1
{
    class Utils
    {


        //Definition for Errors 
        public static int ERROR_WRITING_TO_COUNTRY_CHAMPIONSHIP = 0;
        public static int SUCCESS_TOTAL = 100;
        public static int ERROR_WRITING_TO_DEEP_MINING = 01;

        private static Random random = new Random();
        public static string getRandomId(int length)
        {


            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());



        }



        public static MatchSpecs getMatchDataFromHtml(string url)
        {

            var doa = Utils.getWebDocument(url);
            var html = doa.DocumentNode.SelectNodes("//script").ToArray();
            var node = html[8];
            string results = "";
            Regex r = new Regex("(.*)PageEvent((.*));var(.*)");
            Match m = r.Match(node.OuterHtml);

            if (m.Success)
            {
                results = m.Groups[2].ToString();
                results = results.Split('(')[1];
                results = results.Split(')')[0];
                results = results.Replace("[]", "0");

            }
            
            MatchSpecs res =  JsonConvert.DeserializeObject<MatchSpecs>(results);
            return res;
        }
        public static string getSecondsFromClass(string text)
        {
            string ret = "00";
            Regex r = new Regex("datet t(.*)");
            Match m = r.Match(text);
            if (m.Success)
            {
                ret = m.Groups[1].ToString();
                ret = ret.Split(new char[] { '-' })[0];          
            }

            return ret;

        }

        public static HtmlDocument getWebDocument(string url)
        {
            try
            {
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                return doc;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// To populate the Tournament Table with all Tournaments 
        /// </summary>
        /// <param name="Referer"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static HttpWebResponse getResponse(string Referer, string code)
        {
            string req = "http://fb.oddsportal.com/ajax-sport-country-tournament-archive/3/" + code + "/X0/1/0/1/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req);
            request.Method = "GET";
            request.Referer = Referer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.104 Safari/537.36";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();



            return response;
        }

        public static HttpWebResponse getResponseForTournament(string Referer, string code)
        {
            string req = "http://fb.oddsportal.com/ajax-sport-country-tournament-archive/3/" + code + "/X0/1/0/1/";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req);
            request.Method = "GET";
            request.Referer = Referer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.104 Safari/537.36";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();



            return response;
        }



        /// <summary>
        /// Populate the Odds Data 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Referer"></param>
        /// <returns></returns>
        public static HttpWebResponse getResponseForMatch(string url, string Referer)
        {
            
            string req = url;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(req);
            WebHeaderCollection collect = request.Headers;
            collect.Add("Accept-Language", "en;q=0.8");
            request.Accept = "*/*";
            
            request.Method = "GET";
            request.Referer = Referer;
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.104 Safari/537.36";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();



            return response;
        }

    }

}
