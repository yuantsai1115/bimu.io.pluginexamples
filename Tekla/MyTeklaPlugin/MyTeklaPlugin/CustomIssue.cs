using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTeklaPlugin
{
    public class CustomIssue
    {
        public string title { get; set; } // Reserved
        public string description { get; set; } // Reserved
        public bimU.io.Client.Core.DataModels.Viewpoint viewpoint { get; set; } // Reserved
        public string snapshot { get; set; } // Reserved

        //General inspection
        public string position { get; set; }
        //Longitudinal reinforcement (主筋)
        public string sizeX { get; set; }
        public int numberX { get; set; }
        public string sizeY { get; set; }
        public int numberY { get; set; }
        public string sizeCorner { get; set; }

        //Confined area (圍束區)
        public string sizeStirrupConfined { get; set; }

        //general area (一般區)
        public string sizeStirrupMiddle { get; set; }

        //Tie reinforcement (繫筋)
        public string sizeTieX { get; set; }
        public int numberTieX { get; set; }
        public string sizeTieY { get; set; }
        public int numberTieY { get; set; }

        //Protective layer (保護層)
        public string cover { get; set; }


        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
