using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using HtmlAgilityPack;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;

namespace WindowsService1
{
    class UrlScrapping
    {
        /// <summary>
        /// This Part is for Selenium webDriver initiated calls
        /// </summary>
        IWebDriver driver;

        string url = @"http://www.oddsportal.com/results";

        //WebRequest request = WebRequest.Create("");
        FileStream text;
        Regex r;
        string m;
        HtmlWeb web;

        public UrlScrapping()
        {
            m = @"/basketball/";
            r = new Regex(m);
            web = new HtmlWeb();

        }

        //public static HttpWebResponse getResponse(string Url)
        //{
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
        //    request.Headers.Add()


        //    return null;
        //}


        public void setReferer(string referer)
        {


        }
        public static void LevelOne()
        {

            String match = @"/basketball/";
            Regex regex = new Regex(match);
            //If Error Results due to HtmlWeb object Instantiation UnComment The Following
            HtmlWeb web = new HtmlWeb();
            var document = @"http://www.oddsportal.com/results/#Basketball";
            //Get the Html Content This way and pass it to "HTMLAgility"
            HtmlDocument doc = web.Load(document);
            HtmlNode[] tr = doc.DocumentNode.SelectNodes("//td//a").ToArray();
            using (StreamWriter writer = new StreamWriter("urlResults.txt"))
            {

                foreach (HtmlNode t in tr)
                {
                    var value = t.GetAttributeValue("href", "n");
                    // Console.WriteLine(value);
                    Match ma = regex.Match(value);
                    if (ma.Success)
                    {

                        try
                        {

                            writer.WriteLine("http://oddsportal.com" + value + "\n");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }

                }


            }
        }


        //Returns the Main Filters as key value pair , Related to http://www.oddsportal.com/basketball/australia/seabl-women/results/ page

        /// <summary>
        /// If this Method Leveltwo() gets an Error the Exception is Thrown and returns a Dictionary With One Element in it with 
        /// Value (Error,Error)
        /// The Calling Method Should Get Aware of this Condition and Recall the method
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>


        public Dictionary<string, string> LevelTwo(string Url, string championship_id)
        {

            Dictionary<string, string> dict = new Dictionary<string, string>();
            string[] results;

            HtmlDocument doc = Utils.getWebDocument(Url);
            if (doc == null)
            { }

            string frontEndId;


            //Get The Precious Code 
            frontEndId = scrapeTournamentID(doc);

       




            //Put a Try Catch

            Regex r = new Regex(@"nob-border");
            Regex mem = new Regex(@"deactivate");
            Match m;

       


            //But For The Date Part Only Let us use this Xpath 

            var docu = doc.DocumentNode.SelectNodes("//div[@class='main-menu2 main-menu-gray']//li//a").ToArray(); //Returns Results=>(2017/1016,...)


            results = new string[docu.Length];
            int count = 0;
            foreach (var s in docu)
            {
                var a = "http://www.oddsportal.com" + s.Attributes["href"].Value;
                results[count] = s.InnerText + " : " + a;
                dict[s.InnerText] = a;

            }

            return dict;
        }

        public void getTournamentTable(string linkUrl)
        {


        }

        public void NextMatches()
        {


            if (web == null)
            {
                web = new HtmlWeb();
            }
            HtmlDocument doc = web.Load("http://oddsportal/matches");

            HtmlNode[] tr = doc.DocumentNode.SelectNodes("//td//a").ToArray();
            using (StreamWriter writer = new StreamWriter("urlResults.txt"))
            {
                foreach (HtmlNode t in tr)
                {
                    var value = t.GetAttributeValue("href", "n");
                    // Console.WriteLine(value);
                    Match ma = r.Match(value);
                    if (ma.Success)
                    {

                        try
                        {
                            writer.WriteLine("http://oddsportal.com" + value + "\n");
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    }
                }
            }
        }


        public List<Package> getResults(string year, string url, string championship_id)
        {
            List<Package> packages = new List<Package>() ;
            HtmlDocument doc = Utils.getWebDocument(url);
            if (doc == null)
                return null;

            string frontEndId;


            //Get The Precious Code 
            frontEndId = scrapeTournamentID(doc);

            var documentHtml = getTournamentTable(url, frontEndId);
            if (documentHtml == null)
                return null;


            string toRemove = "{\"s\":(.*),\"d\":{\"html\":\"(.*) ";
            Regex reg = new Regex(toRemove);

            var match = reg.Match(documentHtml);
            if (match.Success)
            {
                documentHtml = match.Groups[2].ToString();

            }
            //Put a Try Catch

            Regex r = new Regex(@"nob-border");
            Regex mem = new Regex(@"deactivate");
            Match m;



            HtmlDocument ment = new HtmlDocument();
            ment.LoadHtml(documentHtml);
            try
            {
                var table = ment.DocumentNode.SelectNodes("//table[@id='tournamentTable']//tbody//tr").ToArray();
                int count = 0;
                Package package = null;

                foreach (var a in table)
                {

                    //Console.Write(a.OuterHtml+"\n\n\n");
                    var className = a.GetAttributeValue("class", "None");

                    var deactivate = mem.Match(className);
                    m = r.Match(className);
                    if (m.Success)
                    {
                        // sendForProcessingPackage(package, championship_id);
                        packages.Add(package);
                        var spanText = ment.DocumentNode.SelectNodes(a.XPath + "//th//span")[0].GetAttributeValue("class", "None");
                        spanText = Utils.getSecondsFromClass(spanText);
                        var date = ConvertFromUnixTimestamp(double.Parse(spanText));

                        package = new Package(date.Year.ToString(), date);
                    }
                    if (deactivate.Success)
                    {
                        if (package != null)
                            package.addMember(a);


                    }
                    count++;
                }
            }
            catch (Exception e)
            {
                Console.Write(e.Message);
                return null;

            }

            return packages;
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }
        public List<Dictionary<string,string>> sendForProcessingPackage(Package pack, string champ_id)
        {
            DbWriter writer = new DbWriter();
            if (pack == null)
            {
                return null;
            }
            var matches = writeMatches(pack, champ_id);
            foreach (var match in matches)
            {

                // writer.writeMatchesToDB(champ_id, match);
                Console.Write("Packages Created");


            }

            return matches;
        }


        private static List<Dictionary<string, string>> writeMatches(Package package, string championship_id)
        {
            DbWriter writer = new DbWriter();
            Package pac = package;
            string match_id;
            List<Dictionary<string, string>> matches = new List<Dictionary<string, string>>();
            Dictionary<string, string> match = new Dictionary<string, string>();
            var nodesList = pac.getMember();
            foreach (var node in nodesList)
            {

                var td = node.SelectNodes(".//td").ToArray();
                var url = node.SelectSingleNode(".//td[2]//a").GetAttributeValue("href","none");
                var time = td[0].GetAttributeValue("class", "none");
                time = Utils.getSecondsFromClass(time);
                var date = ConvertFromUnixTimestamp(double.Parse(time));
                //URL part is yet to be tested
                match["url"] = url;
                match["championship_id"] = championship_id;
                match["match_id"] = Utils.getRandomId(6);
                match["date"] = pac.getName();
                match["time"] = date.TimeOfDay.ToString();
                match["players"] = td[1].InnerText;
                match["score"] = td[2].InnerText;
                match["match_id"]=writer.writeMatchesToDB(championship_id, match);
                matches.Add(match);

                Console.WriteLine(string.Format("{0}::{1}::{2}::{3}::{4}::{5}\t\n", match["championship_id"], match["match_id"], match["date"], match["time"], match["players"], match["score"]));
                //Console.WriteLine(node.InnerHtml+"\n\n\n\n");

            }
            return matches;
        }







        //private string getPageSources(string Url)
        //{
        //    var document = web.Load(Url);

        //    IJavaScriptExecutor exe = (IJavaScriptExecutor)driver1;
        //    var result = (string)exe.ExecuteScript("return document.documentElement.OuterHTML;");
        //    return result;
        //}




        private static string scrapeTournamentID(HtmlDocument doc)
        {
            Regex r = new Regex("\"id\":\"(.*)\",\"sid\":(.*),\"cid\":(.*),\"archive\":(.*)");
            var scripts = doc.DocumentNode.SelectNodes("//body//script").ToArray();
            string id;
            foreach (var script in scripts)
            {
                var a = r.Match(script.InnerHtml);
                if (a.Success)
                {
                    return a.Groups[1].ToString();
                }



            }

            return null;
        }


        private static string getTournamentTable(string url, string code)
        {
            string a;
            string finalString;

            using (var response = Utils.getResponseForTournament(url, code))
            {
                var encoding = Encoding.GetEncoding(response.CharacterSet);

                using (var responseStream = response.GetResponseStream())
                using (var reader = new StreamReader(responseStream, encoding))
                {

                    a = reader.ReadToEnd();
                    a = a.Replace("\\", string.Empty);
                    // obj = JObject.Parse(a);
                    //Console.Write(a);
                    var removeString = a.Remove(a.IndexOf("{"));
                    finalString = a.Replace(removeString, string.Empty);

                }
                return finalString;
            }
        }


        private static string levelThree()
        {
            return null;
        }


    }

    public class Package
    {

        private string packageName;
        List<HtmlNode> member;
        DateTime date;
        public Package(string Name, DateTime date)
        {
            this.packageName = Name;
            this.date = date;
            member = new List<HtmlNode>();
        }

        public void addMember(HtmlNode node)
        {
            member.Add(node);
        }

        public List<HtmlNode> getMember()
        {
            return member;

        }
        public string getName()
        {
            return packageName;
        }

    }
}
