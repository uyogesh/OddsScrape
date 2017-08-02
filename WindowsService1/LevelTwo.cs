using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    class LevelTwo
    {

        private string title;
        private string country;
        private string championship;


        private UrlScrapping scrapper;
        //Ids Needed for joining Operation later in MySql
        private string country_id ="";
        private string championship_id="";


        DbWriter writer;


        public LevelTwo(String URL)
        {
            writer = new DbWriter();
            title = URL;
            scrapper = new UrlScrapping();
            getInfoFromTitle();
            
            
        }


        //Get the Country And Championship from the title
        private void getInfoFromTitle()
        {
            string[] seperator = new string[] { "/" };
            string[] res = title.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
            country = res[3];
            championship = res[4];
            
        }

        //Make a list of all available Links For Results
        // <table class="table-main" id ="tournamentTable">
        //      


        /// <summary>
        /// To be called from The Calling Class to Start the Full Scraping 
        /// Call This Class and Sit Back and Relax
        /// 
        ///Exception Handling to be done there as well 
        ///return 0; ===>Error Do that again
        ///return 1;===>Completed
        /// </summary>
        /// <returns></returns>
        //public int startChainReaction()
        //{
        //    writer = new DbWriter();


        //    if (writeToCountry() == Utils.ERROR_WRITING_TO_COUNTRY_CHAMPIONSHIP)
        //        return Utils.ERROR_WRITING_TO_COUNTRY_CHAMPIONSHIP;

        //    //Now Time to Go deep 
        //    Dictionary<string, string> choices = scrapper.LevelTwo(title, championship_id);
        //    if (deepMining(choices) == 0)
        //        return Utils.ERROR_WRITING_TO_DEEP_MINING;






        //    return Utils.SUCCESS_TOTAL;
        //}


        //Go deep
        private int deepMining(Dictionary<string, string> choices)
        {
            try
            {
                int len = choices.Count;
                foreach (KeyValuePair<string, string> a in choices)
                {
                    scrapper.getResults(a.Key, a.Value, championship_id);

                }
            }
            catch (Exception e)
            {
                return 0;
            }
            return 1;

        }



        public string writeToCountry()
        {
            //write to country
            country_id = writer.writeCountry(country);
            //Write to champpionship
            championship_id = writer.writeChampionship(championship, country_id);

            return country_id;
        }


        private bool checkIfNull()
        {
            if (country_id == null || championship_id == null)
                return false;
            return true;
        }

        public string getCountryID()
        {
            return country_id;
        }

        public string getChampionshipId()
        {
            return championship_id;

        }


    }
}
