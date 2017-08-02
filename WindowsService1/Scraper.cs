using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using MySql.Data.MySqlClient;
using System.Threading.Tasks;


namespace WindowsService1
{
    class Scraper
    {
        List<String> list;
        MySqlConnection connection;
        public string ConnectionInfo = string.Format("server={0}; user id={1}; password={2}; database={3};charset=utf8",
            "localhost", "root", "root", "oddsportal");
        private String Url;
        private int n;

        public Scraper(String Url)
        {

            this.Url = Url;

        }



        public String ScrapeUrl()
        {


            string[] lines = File.ReadAllLines("urlResults.txt");
            list = new List<string>(lines);
            Parallel.ForEach(list, new ParallelOptions { MaxDegreeOfParallelism = 4 }, l =>
            {
                LevelTwo sc = new LevelTwo(l);


            });




            return "";
        }

        private String scrapeSecondLayer(string url)
        {

            return "";
        }


    }
}
