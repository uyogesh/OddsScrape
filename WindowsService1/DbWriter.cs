using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace WindowsService1
{
    /// <summary>
    /// 
    /// </summary>
    class DbWriter
    {
        

        string countryQuery = "select country_id from country where(name='{0}');";
         string countryEntryQuery = "insert into country value('{0}','{1}');";

        //
         string championshipQuery = "select champ_id from championship where(champ_name='{0}');";
         string championshipEntryQuery = "insert into championship value('{0}','{1}','{2}');";

        //matchFinalEntry Query

         string matchFinalQuery = "select entry_id from {0} where (match_id = '{1}')";
         string matchFinalEntryQuery = "insert into {0} value('{1}','{2}','{3}','{4}','{5}','{6}','{7}');";

        
        public  string ConnectionInfo = string.Format("server={0}; user id={1}; password={2}; database={3};charset=utf8",
          "localhost", "root", "root", "oddsportal");
         MySqlConnection conn;


        public DbWriter()
        {
            conn = new MySqlConnection();


        }


        /// <summary>
        /// calling writecountry is the first step in what i call a chain Reaction,
        /// </summary>
        /// <param name="country"></param>
        /// <returns>country_id</returns>
        public  string writeCountry(string country)
        {
            string id = null;
            conn = new MySqlConnection(ConnectionInfo);
            string finalQuery = string.Format(countryQuery, country);
            conn.Open();
            var cmd = new MySqlCommand(finalQuery, conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {

               id = reader.GetString(0);
            }
            conn.Close();
            conn.Open();
            if (id == null)
            {
                //Write Country into db
                string country_id = Utils.getRandomId(6);
                string country_name = country;
                string final_query = string.Format(countryEntryQuery, country_id, country_name);
                var insert = new MySqlCommand(final_query, conn);
                int n = insert.ExecuteNonQuery();
                conn.Close();
                return country_id;

            }
            else
            {
                conn.Close();
                return id;

            }



        }





        public  string writeChampionship(string championship, string country_id)
        {
            string id = null;
            conn = new MySqlConnection(ConnectionInfo);
            string finalQuery = string.Format(championshipQuery, championship);
            conn.Open();
            var cmd = new MySqlCommand(finalQuery, conn);
            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                id = reader.GetString(0);
                break;
            }
            conn.Close();
            conn.Open();
            if (id == null)
            {
                //Write Country into db
                string championship_id = Utils.getRandomId(5);
                string championship_name = championship;
                string final_query = string.Format(championshipEntryQuery, country_id, championship_id, championship_name);
                var insert = new MySqlCommand(final_query, conn);
                int n = insert.ExecuteNonQuery();
                conn.Close();
                return championship_id;

            }
            else
            {
                conn.Close();
                return id;

            }


        }


        public  string writeMatchesToDB(string championship_id, Dictionary<string, string> match)
        {
          
                conn = new MySqlConnection(ConnectionInfo);
                string match_id = match["match_id"];
                string matchQueryIfExists = string.Format("select match_id from matches where (players='{0}' and score='{1}');", match["players"], match["score"]);
                string matchInsertQuery = string.Format("insert into matches values('{0}','{1}','{2}','{3}','{4}','{5}');",
                    match_id, championship_id, match["date"], match["time"], match["players"].Trim('\r'), match["score"]);



                string match_id_from_Db = null;
                //First Check If the Match Entry is already written in DB 
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(matchQueryIfExists, conn);
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    match_id_from_Db = reader.GetString(0);
                break;
                }
                conn.Close();
                conn.Open();
                if (match_id_from_Db == null)
                {
                    MySqlCommand cmdd = new MySqlCommand(matchInsertQuery, conn);
                    cmdd.ExecuteNonQuery();
                    conn.Close();
                    return match_id;
                }
                else {
                    conn.Close();
                    return match_id_from_Db;
                }
          


            //catch (Exception e)
            //{
            //    Console.Write(e.Message);
            //    return null;
            //}


        }

        //(1x2,o/u,Home/Away)  (FToT,1H,2H,1Q....)    ()
        public  string writeToMatchesFinal(string match_id, string match_type, string match_subtype, string[] json_Object)
        {
            //Fields
            //TAble Name = match_type
            //match_id |  match_subtype  |  HighestX | HighestY  | AverageX  |  Average Y |  UserPredX  | UserpredY
            conn.Open();
            string node_id = Utils.getRandomId(8);
            string checkIfAlreadyPresent = string.Format(matchFinalEntryQuery, match_type, match_id);
            conn.Close();
            conn.Open();
            string finalString = string.Format(matchFinalQuery, match_type, match_id, node_id, match_subtype, json_Object[0], json_Object[1], json_Object[2], json_Object[3]);
            MySqlCommand cmd = new MySqlCommand(checkIfAlreadyPresent, conn);

            var reader = cmd.ExecuteReader();
            while (reader.NextResult())
            {

                string id = reader.GetString(0);
                return id;
            }

            cmd = new MySqlCommand(finalString, conn);
            cmd.ExecuteNonQuery();
            conn.Close();
            return node_id;
        }


        /////////////////////////////////////////         1x2            ////////////////////////////////////////////
        ////Yet to be tested
        public void writeTo1x2(string match_id, string betscope, string Average1, string AverageX, string Average2, string AveragePayout, string Highest1, string HighestX, string Highest2, string HighestPayout, string UserPred1, string UserPredX, string UserPred2)
        {
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = Average1 + Average2 + AveragePayout;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from 1x2 where(hash = '{0}')", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return; }
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into 1x2 value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13});", match_id, betscope, Average1, AverageX, Average2, AveragePayout, Highest1, HighestX, Highest2, HighestPayout, UserPred1, UserPredX, UserPred2,hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();
        }



        ///////////////////////////////////////         Home/Away            ////////////////////////////////////////////





        public  void writeToHA(string match_id, string betscope, string Average1, string Average2, string AveragePayout, string Highest1, string Highest2, string HighestPayout, string UserPred1, string UserPred2)
        {
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = Average1 + Average2 + AveragePayout;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from homeaway where(hash = '{0}')", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return; }
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into homeaway value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10});", match_id, betscope, Average1, Average2, AveragePayout, Highest1, Highest2, HighestPayout, UserPred1, UserPred2, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();

        }


        ///////////////////////////////////////         Asian Handicap            ////////////////////////////////////////////




        public  string writeToAH(string match_id, string scope)
        {
            string scope_id="None";
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = match_id + scope;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash and scope_id from asianhandicap where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
                scope_id = res.GetString(1);
            }
            if (dbhash == hash)
                return scope_id;
            conn.Close();
            conn.Open();
            scope_id = Utils.getRandomId(9);

            string Query = string.Format("insert into asianhandicap value('{0}','{1}','{2}',{3});", match_id, scope_id, scope, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();


            return scope_id;

        }

        public  void writeToAHFull(string scope_id, string AH_value, string Average1, string Average2, string AveragePayout, string Highest1, string Highest2, string HighestPayout, string UserPred1, string UserPred2)  
            {
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = Average1 + Average2 + AveragePayout;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from asianhandicapfinal where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
                return;
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into asianhandicapfinal value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10});", scope_id, AH_value, Average1, Average2, AveragePayout, Highest1, Highest2, HighestPayout, UserPred1, UserPred2, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();

        }


        ///////////////////////////////////////         European Handicap            ////////////////////////////////////////////



        public  string writeToEH(string match_id, string scope)

        {
            string scope_id = "None";
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = match_id + scope;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash and scope_id from europianhandicap where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
                scope_id = res.GetString(1);
            }
            if (dbhash == hash)
                return scope_id;
            conn.Close();
            conn.Open();
            scope_id = Utils.getRandomId(9);

            string Query = string.Format("insert into europianhandicap value('{0}','{1}','{2}',{3});", match_id, scope_id, scope, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();


            return scope_id;
            
        }

        public  void writeToEHFull(string scope_id, string EH_value, string Average1, string AverageX,string Average2, string AveragePayout, string Highest1,string HighestX, string Highest2, string HighestPayout, string UserPred1, string UserPredX,string UserPred2)
        {
            
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = Average1 + Average2 + AveragePayout;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash and scope_id from europianhandicapfinal where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return;
            }
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into europianhandicapfinal value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13});s", scope_id, EH_value, Average1, AverageX, Average2, AveragePayout, Highest1, HighestX, Highest2, HighestPayout, UserPred1, UserPredX, UserPred2, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();



        }



        ///////////////////////////////////////         OverUnder           ////////////////////////////////////////////



        public  string writeToOU(string match_id, string scope)
        {
            string scope_id = "None";
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = match_id + scope;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash and scope_id from overunder where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
                scope_id = res.GetString(1);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return scope_id;
            }
            conn.Close();
            conn.Open();
            scope_id = Utils.getRandomId(9);

            string Query = string.Format("insert into overunder value('{0}','{1}','{2}',{3});", match_id, scope_id, scope, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();


            return scope_id;

          

        }

        public  void writeToOUFull(string scope_id, string OU_value, string AverageOver, string AverageUnder, string AveragePayout, string HighestOver, string HighestUnder, string HighestPayout, string UserPredOver, string UserPredUnder)
        {

            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = AverageOver + AverageUnder + AveragePayout;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from overunderfinal where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return;
            }
            conn.Close();
            conn.Open();

            string Query = string.Format("insert into overunderfinal value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10});", scope_id, OU_value, AverageOver, AverageUnder, AveragePayout, HighestOver, HighestUnder, HighestPayout, UserPredOver, UserPredUnder, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();
        }


        ///////////////////////////////////////         Double Chance            ////////////////////////////////////////////




        public  void writeToDC(string match_id, string betscope, string Average1X, string Average12, string AverageX2, string AveragePayout, string Highest1X, string Highest12, string HighestX2, string HighestPayout, string UserPred1X, string UserPred12, string UserPredX2)
        {

            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = Average1X + Average12 + AveragePayout;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from doublechance where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();

                return;
            }
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into doublechance value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13});", match_id, betscope, Average1X, Average12, AverageX2, AveragePayout, Highest1X, Highest12, HighestX2, HighestPayout, UserPred1X, UserPred12, UserPredX2, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();

        }


        ///////////////////////////////////////         HTFT            ////////////////////////////////////////////




        public  string writeToHTFT(string match_id, string scope)
        {
            string scope_id = "None";
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = match_id + scope;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash and scope_id from halffull where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
                scope_id = res.GetString(1);
            }
            if (dbhash == hash)
            {
                conn.Close();

                return scope_id;

            }
            conn.Close();
            conn.Open();
            scope_id = Utils.getRandomId(9);

            string Query = string.Format("insert into halffull value('{0}','{1}','{2}',{3});", match_id, scope_id, scope, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();


            return scope_id;

        }

        public  void writeToHTFTFull(string scope_id, string HTFT_value, string AverageOdd, string HighestOdd, string UserPredOdd)
        {


            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = AverageOdd + HighestOdd + UserPredOdd;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from halffullfinal where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return; }
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into halffullfinal value('{0}','{1}','{2}','{3}','{4}',{5});", scope_id, HTFT_value, AverageOdd, HighestOdd,UserPredOdd, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();



        }





        ///////////////////////////////////////         Odd/EVEN            ////////////////////////////////////////////


        public  void writeToOE(string match_id, string betscope, string AverageOdd, string AverageEven, string AveragePayout, string HighestOdd, string HighestEven, string HighestPayout, string UserPredOdd, string UserPredEven)
        {

            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = AverageOdd + AverageEven + AveragePayout;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from oddeven where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();

                return;
            }
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into oddeven value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}',{13});", match_id, betscope, AverageOdd, AverageEven, AveragePayout, HighestOdd, HighestEven,  HighestPayout, UserPredOdd, UserPredEven,  hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();



        }





        ///////////////////////////////////////         DNB            ////////////////////////////////////////////

        public  void writeToDNB(string match_id, string betscope, string Average1, string Average2, string AveragePayout, string Highest1, string Highest2, string HighestPayout, string UserPred1, string UserPred2)
        {


            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = Average1 + Average2 + AveragePayout;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from dnb where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();

                return;
            }
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into dnb value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',{10});", match_id, betscope, Average1, Average2, AveragePayout, Highest1, Highest2,  HighestPayout, UserPred1, UserPred2, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();
        }




        ///////////////////////////////////////         Cs            ////////////////////////////////////////////

        public string writeToCS(string match_id, string betscope)
        {
            string scope_id ="";
            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = match_id + betscope;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash and scope_id from correctscore where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
                scope_id = res.GetString(1);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return scope_id; }
            conn.Close();
            conn.Open();
            scope_id = Utils.getRandomId(8);
            string Query = string.Format("insert into correctscore value('{0}','{1}','{2}',{3});", match_id, scope_id,betscope, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();
            return scope_id;
        }


        public void writeToCSfull(string scope_id,string SC_value,string AverageOdd,string HighestOdd,string UserpredOdd)
        {

            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = AverageOdd + HighestOdd + UserpredOdd;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from csfinal where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return; }

            conn.Close();
            conn.Open();
            string Query = string.Format("insert into csfinal value('{0}','{1}','{2}','{3}','{4}',{5});", scope_id, SC_value, AverageOdd, HighestOdd, UserpredOdd, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();
        }

        public void writeToBTTS(string match_id, string scopename, string AverageYes, string AverageNo, string AveragePayout, string HighestYes, string HighestNo, string HighestPayout, string UserPredYes, string UserPredNo)
        {

            conn = new MySqlConnection(ConnectionInfo);
            conn.Open();
            int dbhash = 0;
            string seed = AverageYes + HighestYes;
            int hash = seed.GetHashCode();
            string SelectQuery = string.Format("select hash from btts where(hash = {0})", hash);
            MySqlCommand cmd = new MySqlCommand(SelectQuery, conn);
            var res = cmd.ExecuteReader();
            while (res.NextResult())
            {
                dbhash = res.GetInt16(0);
            }
            if (dbhash == hash)
            {
                conn.Close();
                return; }
            conn.Close();
            conn.Open();
            string Query = string.Format("insert into btts value('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',{8});", match_id, scopename, AverageYes, AverageNo, HighestYes, HighestNo, UserPredYes, UserPredNo, hash);
            MySqlCommand put = new MySqlCommand(Query, conn);
            put.ExecuteNonQuery();
            conn.Close();

        }

    }
}
