using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Drawing;
using HtmlAgilityPack;
using System.IO;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium.Firefox;
using System.Web;
using System.Diagnostics;
using Newtonsoft.Json;

namespace ConsoleApplication1
{
    class Program
    {


        //        GET http://fb.oddsportal.com/ajax-sport-country-tournament-archive/3/d2rfLrG8/X0/1/0/1/?_=1497962834158 HTTP/1.1
        //Host: fb.oddsportal.com
        //Connection: keep-alive
        //User-Agent: Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.104 Safari/537.36
        //Accept: */*
        //Referer: http://www.oddsportal.com/basketball/africa/african-championship/results/
        //Accept-Encoding: gzip, deflate
        //Accept-Language: en-US,en;q=0.8
        //Cookie: _ga=GA1.2.734757002.1497182252; _gid=GA1.2.1090293299.1497597665





        static void Main(string[] args)
        {
            List<string> links = new List<string>();
            Dictionary<string, string> dateDict;
            UrlScrapping scrapping = new UrlScrapping();
            UrlScrapping.LevelOne();
            List<Package> listofPackages;
            LevelTwo two;
            string champ_id;
            string country_id;
            using (StreamReader reader = new StreamReader("urlResults.txt"))
            {
                var link = reader.ReadLine();
                links.Add(link);

            }
            Parallel.ForEach(links,new ParallelOptions { MaxDegreeOfParallelism = 2 }, champ =>
            //foreach(var champ in links)
            {
                two = new LevelTwo(champ);
                country_id = two.writeToCountry();
                champ_id = two.getChampionshipId();

            dateDict = scrapping.LevelTwo(links[0],champ_id);
            foreach(var date in dateDict)
            {
                listofPackages =  scrapping.getResults(date.Key,date.Value,champ_id);
                    if (listofPackages != null)
                    {
                        foreach (var pack in listofPackages)
                        {
                            if (pack != null)
                            {
                                
                                var a = scrapping.sendForProcessingPackage(pack, champ_id);
                                var count = a.Count;


                                Parallel.ForEach(a, new ParallelOptions { MaxDegreeOfParallelism = 2 }, aa =>
                                {
                                    try
                                    {
                                        Task t2 = Task.Factory.StartNew(() => runDriver(aa));
                                        

                                        Task.WaitAll(t2);
                                        
                                    }
                                    catch (Exception e)
                                    {

                                    }
                                });

                            }
                        }
                    }

                }


            });


        }

        public static void runDriver(Dictionary<string,string> a)
        {
            Driver driver = new Driver();
            driver.start("http://oddsportal.com"+a["url"],a["match_id"]);

        }


    }
}
