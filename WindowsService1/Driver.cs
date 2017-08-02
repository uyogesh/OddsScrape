using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.PhantomJS;
using OpenQA.Selenium;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;


namespace WindowsService1
{
    class Driver
    {
        DbWriter writer;
        PhantomJSDriver driver;
        PhantomJSOptions opt;
        IJavaScriptExecutor exec;
        PhantomJSDriverService service;
        Dictionary<string, string> Bettypes;
        string match_id;
        string url;
        int failSafe=0;
        bool done = false;
       
        public Driver() {
            writer = new DbWriter();
            service = PhantomJSDriverService.CreateDefaultService();
            service.HideCommandPromptWindow = true;
            opt = new PhantomJSOptions();
            opt.AddAdditionalCapability("phantomjs.page.settings.userAgent", "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome / 40.0.2214.94 Safari / 537.36");
            opt.AddAdditionalCapability("IsJavaScriptEnabled", true);
            //opt.AddAdditionalCapability("IsApplicationCacheEnabled", true);
            driver = new PhantomJSDriver(service, opt);
            exec = (IJavaScriptExecutor)driver;
            driver.Navigate().GoToUrl("http://www.oddsportal.com");
            login();
            
        }
        //Get Bet Types and scrape the shit out of them
        public void betTypes()
        {
            takeScreenshot();
            string a = "//div[@id='bettype-tabs']//li[@style='display: block;']//a";
               // string a = "//div[@id='bettype-tabs']//a";
            var activeLink = driver.FindElementByXPath("//div[@id='bettype-tabs']//li[@class=' active']").Text;

            switchBettype(activeLink);


            var hidden = "//div[@id='bettype-tabs']//li[14]//a";
            var li = driver.FindElementsByXPath(a).ToList();
            var hid = driver.FindElementsByXPath(hidden).ToList();
                
                string js;
                Bettypes = new Dictionary<string, string>();
                 
                foreach (var link in li) 
                {
                    js = link.GetAttribute("onmousedown");
                    js = js.Split(';')[0];
                    Bettypes[link.Text] = js;

                }
            foreach (var link in hid)
            {
                try
                {
                    js = link.GetAttribute("onmousedown");
                    js = js.Split(';')[1];
                    Bettypes[link.Text] = js;
                }
                catch (Exception w)
                {
                }
            }
           
                foreach (var dict in Bettypes)
                {
                try
                {
                    switchBettype(dict);
                }
                catch (Exception e)
                {
                    Console.WriteLine("/////////////////LOG: Exception retrieving " + dict.Key +" From" + url );     
                }
                }
               
            


        }



        private void switchBettype(KeyValuePair<string,string> dict)
        {
            try {

                switch (dict.Key)
                {
                    case "1X2":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        get1x2();
                        break;

                    case "Home/Away":
                        
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getHA();
                        break;

                    case "AH":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getAH();
                        break;

                    case "O/U":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getOU();
                        break;

                    case "DNB":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getDNB();
                        break;

                    case "EH":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getEH();
                        break;

                    case "Double Chance":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getDC();
                        break;

                    case "Half Time / Full Time":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getHTFT();
                        break;

                    case "Odd or Even":
                      
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getOE();
                        break;

                    case "Over/Under":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getOU();
                        break;

                    case "TQ":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getTQ();
                        break;
                    case "CS":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getCS();
                        break;

                    case "Winner":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getWinner();
                        break;

                    case "BTS":
                        exec.ExecuteScript(dict.Value); Console.WriteLine("DEBUG> "+dict.Key+" From "+url);
                        getBTTS();
                        break;


                    default:
                        break;

                }
            }
            catch (Exception e) { }
            finally {
                setDone();
            }
        }


        private void switchBettype(string dict)
        {

            switch (dict)
            {
                case "1x2":
                    get1x2();
                    break;

                case "Home/Away":
                    getHA();
                    break;

                case "AH":
                    getAH();
                    break;

                case "O/U":
                    getOU();
                    break;

                case "DNB":
                    getDNB();
                    break;

                case "EH":
                    getEH();
                    break;

                case "Double Chance":
                    getDC();
                    break;

                case "Half Time / Full Time":
                    getHTFT();
                    break;

                case "Odd or Even":
                    getOE();
                    break;

                case "Over/Under":
                    getOU();
                    break;

                case "TQ":
                    getTQ();
                    break;
                case "CS":
                    getCS();
                    break;

                case "Winner":
                    getWinner();
                    break;

                case "BTS":
                    getBTTS();
                    break;


                default:
                    break;

            }

        }


        public void start(string url, string match_id)
        {
            
            this.url = url;
            this.match_id = match_id;
          
            goToURL();
            driver.Dispose();
            driver.Quit();
            
        }

        private void goToURL()
        {
           
            driver.Navigate().GoToUrl(url);
            WaitForAjax(driver);
            try
            {
                betTypes();
            }
            catch {

            }
        }




        //http://www.oddsportal.com/basketball/australia-oceania/oceania-championship-women/australia-new-zealand-zeMmBbNr/
        public void getHA() {

            string text;
            string javascript;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");
            writeToHA(subtypeName);

            foreach (var list in subList)
            {
                text = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                Thread.Sleep(3000);
                WaitForAjax(driver);

                writeToHA(text);

            }


        }

        private void writeToHA(string scopename) {

            
                var table = driver.FindElementByClassName("table-container");
                var tr = table.FindElement(By.CssSelector(".aver"));
                var child = tr.FindElements(By.XPath(".//td"));
                var AverageX = child[1].Text;
                var AverageY = child[2].Text;
                var AveragePercent = child[3].Text;



                tr = table.FindElement(By.CssSelector(".highest"));
                child = tr.FindElements(By.XPath(".//td"));
                var highestX = child[1].Text;
                var highestY = child[2].Text;
                var highestPercent = child[3].Text;

                tr = table.FindElement(By.CssSelector("tr.foot.predictions"));
                child = tr.FindElements(By.XPath(".//td"));
                var userpredX = child[1].Text;
                userpredX = userpredX.Split('\r')[0];
                var userpredY = child[2].Text;
                userpredY = userpredY.Split('\r')[0];

            writer.writeToHA(match_id,scopename,AverageX,AverageY,AveragePercent,highestX,highestY,highestPercent,userpredX,userpredY);

        }

        public static void WaitForAjax( IWebDriver driver, int timeoutSecs = 10, bool throwException = false)
        {
            for (var i = 0; i < timeoutSecs; i++)
            {
                  var ajaxIsComplete = (bool)(driver as IJavaScriptExecutor).ExecuteScript("return jQuery.active == 0");
                if (ajaxIsComplete) return;
                Thread.Sleep(1000);
            }
            if (throwException)
            {
                throw new Exception("WebDriver timed out waiting for AJAX call to complete");
            }
        }




        private void get1x2()
        {
            string text;
            string javascript;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");
            writeTo1x2(subtypeName);

            foreach (var list in subList)
            {
                text = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                Thread.Sleep(3000);
                WaitForAjax(driver);

                writeTo1x2(text);

            }
        }

        private void writeTo1x2(string scopename) {

            var table = driver.FindElementByClassName("table-container");
            var tr = table.FindElement(By.CssSelector(".aver"));
            var child = tr.FindElements(By.XPath(".//td"));
            var Average1 = child[1].Text;
            var AverageX = child[2].Text;
            var Average2 = child[3].Text;
            var AveragePercent = child[4].Text;



            tr = table.FindElement(By.CssSelector(".highest"));
            child = tr.FindElements(By.XPath(".//td"));
            var highest1 = child[1].Text;
            var highestX = child[2].Text;
            var highest2 = child[3].Text;
            var highestPercent = child[4].Text;

            tr = table.FindElement(By.CssSelector("tr.foot.predictions"));
            child = tr.FindElements(By.XPath(".//td"));

            var userpred1 = child[1].Text;
            userpred1 = userpred1.Split('\r')[0];

            var userpredX = child[2].Text;
            userpredX = userpredX.Split('\r')[0];
            var userpred2 = child[3].Text;
            userpred2 = userpred2.Split('\r')[0];

            writer.writeTo1x2(match_id, scopename, Average1, AverageX, Average2, AveragePercent, highest1, highestX, highest2, highestPercent, userpred1, userpredX, userpred2);
        }
        private void takeScreenshot()
        {
           driver.GetScreenshot().SaveAsFile("image1.png", ScreenshotImageFormat.Png);

        }
        private void login()
        {
            try
            {
                var username = driver.FindElementById("login-username");
                var password = driver.FindElementById("login-password");
                username.SendKeys("oddsbetting");
                password.SendKeys("oddsbettings");
                var button = driver.FindElementByName("login-submit");
                button.Click();
            }
            catch (Exception e)
            {
                return;

            }
        }


        private void getOU()
        {
            string text;
            string javascript;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");
            writeToOU(subtypeName);
             
            foreach (var list in subList)
            {
                text = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                Thread.Sleep(3000);
                WaitForAjax(driver);
               
                writeToOU(text);
               
            }

        }


        private void writeToOU(string betscope)
        {
            

            IWebElement Tables;
            List<IWebElement> td;
            Match match;
            string ou_value;
            string AverageOver;
            string AverageUnder;
            string AveragePayout;
            string HighestOver;
            string HighestUnder;
            string HighestPayout;
            string userpredOver;
            string userpredUnder;
            IWebElement clickable;
            string text;
            string results;
            string scope_id;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var table = driver.FindElementsByXPath("//div[@class='table-container']");
            int len = 1;
            Regex r = new Regex(@"Over/Under (.*)");
            scope_id = writer.writeToOU(match_id, betscope);
            foreach (IWebElement div in table)
            {
                clickable = div.FindElement(By.XPath(".//div/strong/a"));
                text = clickable.GetAttribute("onClick");
                text = text.Split(';')[0];
                results = (string)exec.ExecuteScript(text);


                Tables = div.FindElement(By.XPath(".//tr[@class='aver']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();
                ou_value = clickable.Text;
                match = r.Match(ou_value);
                if (match.Success)
                {
                    ou_value = match.Groups[1].ToString();
                }

                AverageOver = td[2].Text;
                AverageUnder = td[3].Text;
                AveragePayout = td[4].Text;

                Tables = div.FindElement(By.XPath(".//table//tr[@class='highest']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();
                HighestOver = td[2].Text;
                HighestUnder = td[3].Text;
                HighestPayout = td[4].Text;

                Tables = div.FindElement(By.XPath(".//table//tr[@class='foot predictions']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();

                userpredOver = td[2].Text;
                userpredUnder = td[3].Text;
         
                
                writer.writeToOUFull(scope_id, ou_value, AverageOver, AverageUnder, AveragePayout, HighestOver, HighestUnder, HighestPayout, userpredOver, userpredUnder);




                len++;
            }
        }




        /// <summary>
        /// BetType Number 4
        /// 
        /// Double Chance
        /// Completed
        /// </summary>

        private void getDC()
        {

            string javascript;
            string name;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li[@class=' active']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");
            Console.Write(subtypeName);
            writeToDC(subtypeName);
          
            foreach (var list in subList)
            {
                name = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                Thread.Sleep(3000);
                WaitForAjax(driver);
                
                Console.Write(name+"\n\n");
                writeToDC(name);
            }
        }

        private void writeToDC(string scope)
        {
            var table = driver.FindElementByClassName("table-container");
            var tr = table.FindElement(By.CssSelector(".aver"));
            var child = tr.FindElements(By.XPath(".//td"));
            var Average1X = child[1].Text;
            var Average12 = child[2].Text;
            var AverageX2 = child[3].Text;
            var AveragePercent = child[4].Text;



            tr = table.FindElement(By.CssSelector(".highest"));
            child = tr.FindElements(By.XPath(".//td"));
            var highest1X = child[1].Text;
            var highest12 = child[2].Text;
            var highestX2 = child[3].Text;
            var highestPercent = child[4].Text;

            tr = table.FindElement(By.CssSelector("tr.foot.predictions"));
            child = tr.FindElements(By.XPath(".//td"));
            var userpredX1 = child[1].Text;
            userpredX1 = userpredX1.Split('\r')[0];
            var userpred12 = child[2].Text;
            userpred12 = userpred12.Split('\r')[0];
            var userpredX2 = child[3].Text;
            userpredX2 = userpredX2.Split('\r')[0];


            //Write to DB
            writer.writeToDC(match_id,scope,Average1X,Average12,AverageX2,AveragePercent,highest1X,highest12,highestX2,highestPercent,userpredX1,userpred12,userpredX2);
        }




        /// <summary>
        /// BetType Number 5 
        /// <table-container> iterate through the table  -12.5 +2.5 etc
        /// AsianHandicap
        /// </summary>


        private void getAH()
        {
            string text;
            string scope_id;
            string javascript;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");
          
            writeToAH(subtypeName);
            foreach (var list in subList)
            {
                text = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                WaitForAjax(driver);
                
                writeToAH(text);

                
            }

        }

        private void writeToAH(string scope)
        {
            IWebElement Tables;
            List<IWebElement> td;
            Match match;
            string AH_value;
            string AverageOver;
            string AverageUnder;
            string AveragePayout;
            string HighestOver;
            string HighestUnder;
            string HighestPayout;
            string userpredOver;
            string userpredUnder;
            IWebElement clickable;
            string text;
            string results;
            string scope_id;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var table = driver.FindElementsByXPath("//div[@class='table-container']").ToList();
            int len = 1;
            Regex r = new Regex(@"Asian handicap (.*)");
            
            scope_id = writer.writeToAH(match_id, scope);
            foreach (IWebElement div in table)
            {
                
                clickable = div.FindElement(By.XPath(".//div/strong/a"));
                text = clickable.GetAttribute("onClick");
                text = text.Split(';')[0];
                results = (string)exec.ExecuteScript(text);


                Tables = div.FindElement(By.XPath(".//tr[@class='aver']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();
                AH_value = clickable.Text;
                match = r.Match(AH_value);
                if (match.Success)
                {
                    AH_value = match.Groups[1].ToString();
                }

                AverageOver = td[2].Text;
                AverageUnder = td[3].Text;
                AveragePayout = td[4].Text;

                Tables = div.FindElement(By.XPath(".//table//tr[@class='highest']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();
                HighestOver = td[2].Text;
                HighestUnder = td[3].Text;
                HighestPayout = td[4].Text;

                Tables = div.FindElement(By.XPath(".//table//tr[@class='foot predictions']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();

                userpredOver = td[2].Text;
                userpredUnder = td[3].Text;

                //Write to DB
                writer.writeToAHFull( scope_id,  AH_value,  AverageOver,  AverageUnder,  AveragePayout,  HighestOver,  HighestUnder,  HighestPayout,  userpredOver,  userpredUnder);

               


               
                len++;
            }
        }



        /// <summary>
        /// Bet Type Number 6
        /// 
        /// Draw No Bet
        /// </summary>


        private void getDNB()
        {
            string text;
            string javascript;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            takeScreenshot();
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li[@class=' active']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a").ToList();

            writeToDNB(subtypeName);
            foreach (var list in subList)
            {
               
                text = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                WaitForAjax(driver);

                writeToDNB(text);

            }

        }

        private void writeToDNB(string scopename)
        {
            var table = driver.FindElementByClassName("table-container");
            var tr = table.FindElement(By.CssSelector(".aver"));
    
            var child = tr.FindElements(By.XPath(".//td"));
            var Average1 = child[1].Text;
            var Average2 = child[2].Text;
            var AveragePayout = child[3].Text;



            tr = table.FindElement(By.CssSelector(".highest"));
            child = tr.FindElements(By.XPath(".//td"));
            var highest1 = child[1].Text;
            var highest2 = child[2].Text;
            var highestPayout = child[3].Text;

            tr = table.FindElement(By.CssSelector("tr.foot.predictions"));
            child = tr.FindElements(By.XPath(".//td"));

            var userpred1 = child[1].Text;
            userpred1 = userpred1.Split('\r')[0];
            var userpred2 = child[2].Text;
            userpred2 = userpred2.Split('\r')[0];

            writer.writeToDNB(match_id,scopename,Average1,Average2,AveragePayout,highest1,highest2,highestPayout,userpred1,userpred2);
            
        }


        /// <summary>
        /// Bet Type Number 7
        /// 
        /// To qualify
        /// </summary>


        private void getTQ()
        {
        }

        private void writeTQ()
        {
        }



        /// <summary>
        /// Bet Type Number 8
        /// 
        /// Correct Score
        /// </summary>


        private void getCS()
        {
           
            string text;
            string javascript;
            string scope_id;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");
            writeToCS(subtypeName);
            foreach (var list in subList)
            {
                text = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                Thread.Sleep(3000);
                WaitForAjax(driver);
                writeToCS(text);

            }

        }

        private void writeToCS(string scope)
        {

            IWebElement Tables;
            List<IWebElement> td;
            Match match;
            string cs_value;
            string AverageOdd;
            string HighestOdd;
            string userpredOdd;
            IWebElement clickable;
            string text;
            string results;
            string scope_id;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var table = driver.FindElementsByXPath("//div[@class='table-container']").ToList();
            int len = 0;
            scope_id = writer.writeToCS(match_id, scope);
            foreach (IWebElement diva in table)
            {
                var div = driver.FindElementsByXPath("//div[@class='table-container']").ToList()[len];

                if (div.GetAttribute("style") == "display: none;")
                {
                    len++;
                }
                else {

                    clickable = div.FindElement(By.XPath(".//div/strong/a"));
                    text = clickable.GetAttribute("onClick");
                    text = text.Split(';')[0];
                    results = (string)exec.ExecuteScript(text);

                    Tables = div.FindElement(By.XPath(".//tr[@class='aver']"));
                    td = Tables.FindElements(By.CssSelector("td")).ToList();
                    cs_value = clickable.Text;


                    AverageOdd = td[1].Text;

                    Tables = div.FindElement(By.XPath(".//table//tr[@class='highest']"));
                    td = Tables.FindElements(By.CssSelector("td")).ToList();
                    HighestOdd = td[1].Text;

                    Tables = div.FindElement(By.XPath(".//table//tr[@class='foot predictions']"));
                    td = Tables.FindElements(By.CssSelector("td")).ToList();

                    userpredOdd = td[1].Text;
                    
                    
                    writer.writeToCSfull(scope_id, cs_value, AverageOdd,  HighestOdd, userpredOdd);

                    
                    len++;
                }
            }

        }




        /// <summary>
        /// Bet Type Number 9
        /// 
        /// Half/Time Ful/Time
        /// </summary>


        private void getHTFT()
        {
            string text;
            string javascript;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");
            writeToHTFT(subtypeName);
            Console.ReadKey();
            foreach (var list in subList)
            {
                text = list.Text;

                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                Thread.Sleep(3000);
                WaitForAjax(driver);

                writeToHTFT(text);
            }

        }

        private void writeToHTFT(string scopename)
        {
            IWebElement Tables;
            List<IWebElement> td;
            Match match;
            string scope_id;
            string ou_value;
            string AverageOdd;
            string HighestOdd;
            string userpredOdd;
            IWebElement clickable;
            string text;
            string results;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var table = driver.FindElementsByXPath("//div[@class='table-container']");
            int len = 1;
            scope_id = writer.writeToCS(match_id,scopename);

            foreach (IWebElement div in table)
            {

                clickable = div.FindElement(By.XPath(".//div/strong/a"));
                text = clickable.GetAttribute("onClick");
                text = text.Split(';')[0];
                exec.ExecuteScript(text);

                
                Tables = div.FindElement(By.XPath(".//tr[@class='aver']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();
                ou_value = clickable.Text;
                

                AverageOdd = td[2].Text;
                
                Tables = div.FindElement(By.XPath(".//table//tr[@class='highest']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();
                HighestOdd = td[2].Text;
                
                Tables = div.FindElement(By.XPath(".//table//tr[@class='foot predictions']"));
                td = Tables.FindElements(By.CssSelector("td")).ToList();

                userpredOdd = td[2].Text;

                writer.writeToHTFTFull(scope_id,ou_value,AverageOdd,HighestOdd,userpredOdd);
                
                len++;
            }
        }



        /// <summary>
        /// Bet Type Number 10
        /// 
        /// Odd or Even
        /// </summary>


        private void getOE()
        {

            string text;
            string javascript;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            takeScreenshot();
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li[@class=' active']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a").ToList();

            writeToOE(subtypeName);
            foreach (var list in subList)
            {

                text = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                WaitForAjax(driver);

                writeToOE(text);

            }

        }

        private void writeToOE(string scope)
        {
            var table = driver.FindElementByClassName("table-container");
            var tr = table.FindElement(By.CssSelector(".aver"));

            var child = tr.FindElements(By.XPath(".//td"));
            var AverageOdd = child[1].Text;
            var AverageEven = child[2].Text;
            var AveragePayout = child[3].Text;
            


            tr = table.FindElement(By.CssSelector(".highest"));
            child = tr.FindElements(By.XPath(".//td"));
            var highestOdd= child[1].Text;
            var highestEven = child[2].Text;
            var highestPayout = child[3].Text;
            
            tr = table.FindElement(By.CssSelector("tr.foot.predictions"));
            child = tr.FindElements(By.XPath(".//td"));

            
            var userpredOdd = child[1].Text;
            userpredOdd = userpredOdd.Split('\r')[0];
            var userpredEven = child[2].Text;
            userpredEven = userpredEven.Split('\r')[0];
            writer.writeToOE(match_id,scope,AverageOdd,AverageEven,AveragePayout,highestOdd,highestEven,highestPayout,userpredOdd,userpredEven);
            
        }




        /// <summary>
        /// Bet Type Number 11
        /// 
        /// Winner
        /// </summary>


        private void getWinner()
        {
        }

        private void writeWinner()
        {
        }


        /// <summary>
        /// Bet Type Number 12
        /// 
        /// Europian Handicap
        /// </summary>


        private void getEH()
        {
            string text;
            string javascript;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");

            writeToEH(subtypeName);
            foreach (var list in subList)
            {
                text = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                Thread.Sleep(3000);
                WaitForAjax(driver);

                writeToEH(text);

            }

        }


        

        private void writeToEH(string scopename)
        {


            IWebElement Tables;
            List<IWebElement> td;
            Match match;
            string scope_id;
            string ou_value;
            string Average1;
            string AverageX;
            string Average2;
            string AveragePayout;
            string Highest1;
            string HighestX;
            string Highest2;
            string HighestPayout;
            string userpred1;
            string userpredX;
            string userpred2;
            IWebElement clickable;
            string text;
            string results;
            IJavaScriptExecutor exec = (IJavaScriptExecutor)driver;
            var table = driver.FindElementsByXPath("//div[@class='table-container']").ToList();
            int len = 0;
            scope_id = writer.writeToCS(match_id, scopename);
            foreach (IWebElement diva in table)
            {

                var div = driver.FindElementsByXPath("//div[@class='table-container']").ToList()[len];

                if (div.GetAttribute("style") == "display: none;")
                {
                    len++;
                }
                else {

                    clickable = div.FindElement(By.XPath(".//div/strong/a"));
                    text = clickable.GetAttribute("onClick");
                    text = text.Split(';')[0];
                    results = (string)exec.ExecuteScript(text);


                    Tables = div.FindElement(By.XPath(".//tr[@class='aver']"));
                    td = Tables.FindElements(By.CssSelector("td")).ToList();
                    ou_value = clickable.Text;

                    Average1 = td[2].Text;
                    AverageX = td[3].Text;
                    Average2 = td[4].Text;
                    AveragePayout = td[5].Text;

                    Tables = div.FindElement(By.XPath(".//table//tr[@class='highest']"));
                    td = Tables.FindElements(By.CssSelector("td")).ToList();
                    Highest1 = td[2].Text;
                    HighestX = td[3].Text;
                    Highest2 = td[4].Text;
                    HighestPayout = td[5].Text;

                    Tables = div.FindElement(By.XPath(".//table//tr[@class='foot predictions']"));
                    td = Tables.FindElements(By.CssSelector("td")).ToList();

                    userpred1 = td[2].Text;
                    userpredX = td[3].Text;
                    userpred2 = td[4].Text.Split('\r')[0];

                    writer.writeToEHFull(scope_id,ou_value,Average1,AverageX,Average2,AveragePayout,Highest1,HighestX,Highest2,HighestPayout,userpred1, userpredX, userpred2);


                    len++;
                }
            }

        }


        /// <summary>
        /// Bet Type Number 13
        /// 
        /// Both teams to Score
        /// </summary>


        private void getBTTS()
        {

            string javascript;
            string name;
            var subtype = driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li[@class=' active']//strong");
            var subtypeName = subtype.Text;
            var subList = driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a");
            Console.Write(subtypeName);
            writeToBTTS(subtypeName);

            foreach (var list in subList)
            {
                name = list.Text;
                javascript = list.GetAttribute("onmousedown");
                javascript = javascript.Split(';')[0];
                exec.ExecuteScript(javascript);
                Thread.Sleep(3000);
                WaitForAjax(driver);

                Console.Write(name + "\n\n");
                writeToBTTS(name);
            }
        
        }

        private void writeToBTTS(string scopename)
        {


            var table = driver.FindElementByClassName("table-container");
            var tr = table.FindElement(By.CssSelector(".aver"));
            var child = tr.FindElements(By.XPath(".//td"));
            var AverageYes = child[1].Text;
            var AverageNo = child[2].Text;
            var AveragePayout = child[3].Text;
            


            tr = table.FindElement(By.CssSelector(".highest"));
            child = tr.FindElements(By.XPath(".//td"));
            var highestYes = child[1].Text;
            var highestNo = child[2].Text;
            var highestPayout = child[3].Text;
            
            tr = table.FindElement(By.CssSelector("tr.foot.predictions"));
            child = tr.FindElements(By.XPath(".//td"));
            var userpredYes = child[1].Text;
            userpredYes = userpredYes.Split('\r')[0];
            var userpredNo = child[2].Text;
            userpredNo = userpredNo.Split('\r')[0];

            writer.writeToBTTS(match_id,scopename,AverageYes,AverageNo,AveragePayout,highestYes,highestNo,highestPayout,userpredYes,userpredNo);
            //Write to DB
           
        }


        private void setDone()
        {
            this.done = true;
        }

        public bool getDone() {
            return done;
        }

        private IWebElement getCurrentScope()
        {

            return this.driver.FindElementByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li[@class='active']//strong");
        }

        private List<IWebElement> getAllScope()
        {

            return this.driver.FindElementsByXPath("//div[@id='bettype-tabs-scope']//ul[@style='display: block;']//li//a").ToList();
        }


    }

    
}
 