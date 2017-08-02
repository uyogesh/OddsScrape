using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO; 
using System.Threading.Tasks;

namespace WindowsService1
{
    class Starter
    {
        public Starter()
        {

            method();
        }
        public void method()
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
            Parallel.ForEach(links, new ParallelOptions { MaxDegreeOfParallelism = 2 }, champ =>
            //foreach(var champ in links)
            {
                two = new LevelTwo(champ);
                country_id = two.writeToCountry();
                champ_id = two.getChampionshipId();

                dateDict = scrapping.LevelTwo(links[0], champ_id);
                foreach (var date in dateDict)
                {
                    listofPackages = scrapping.getResults(date.Key, date.Value, champ_id);
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


        public static void runDriver(Dictionary<string, string> a)
        {
            Driver driver = new Driver();
            driver.start("http://oddsportal.com" + a["url"], a["match_id"]);

        }
    }
}

