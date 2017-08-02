using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1
{
    class MatchSpecs
    {


        public string id { get; set; }
        public string xhash { get; set; }
        public string xhashf { get; set; }
        public string ukeyBase { get; set; }
        public string isLive { get; set; }
        public string isPostponed { get; set; }
        public string isStarted { get; set; }
        public string isFinished { get; set; }
        public string isFinishedGracePeriod { get; set; }
        public string sportId { get; set; }
        public string versionId { get; set; }
        public string home { get; set; }
        public string away { get; set; }
        public string tournamentId { get; set; }
        public string eventBonus { get; set; }
    }
}

