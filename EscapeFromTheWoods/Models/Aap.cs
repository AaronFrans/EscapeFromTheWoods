using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Models
{
    class Aap
    {


        #region Constructor
        public Aap(string naam, Boom tijdelijkeBoom, int bosID)
        {
            TijdelijkeBoom = tijdelijkeBoom;
            ID = iDCounter;
            iDCounter++;
            Naam = naam;
            VorigeBomen = new Dictionary<int, Boom>();
            this.bosID = bosID;
        }
        #endregion

        #region Properties

        private readonly int bosID;
        private static int iDCounter;

        public Boom TijdelijkeBoom { get; private set; }
        public int ID { get; private set; }
        public string Naam { get; private set; }
        public Dictionary<int, Boom> VorigeBomen { get; private set; }
        public enum AapNamen
        {
            Olivia,
            Oliver,
            George,
            Ava,
            Anita,
            Renee,
            Elliott,
            Octavio,
            Rev,
            Makoa,
            Alexander,
            Natalie,
            Ajay,
            Arthur,
            Marvin,
            Tae,
            Micheal,
            Dwight,
            Pam,
            Angela,
            Jim,
            Andy,
            Kelly,
            Ryan,
            BJ,
            Toby,
            Errin,
            Stanley,
            Mose,
            Creed,
        }


        #endregion

        #region HelperFunctions



        public static int NrOfNames()
        {
           return Enum.GetNames(typeof(AapNamen)).Length;
        }

        #endregion

        #region Functions
        public void Spring(Bos bosWaarAapInIs)
        {
            
            KeyValuePair<double, Boom> pairAfstandEnBoom = bosWaarAapInIs.VindDichtstbijzijndeBoom(this);
            double afstandNaarGrens = bosWaarAapInIs.AfstandNaarRandVanBos(TijdelijkeBoom);

            if(afstandNaarGrens < pairAfstandEnBoom.Key)
            {
                GaUitBos(bosWaarAapInIs.ID);
            }
            else
            {
                int sprongId = VorigeBomen.Count;
                TijdelijkeBoom.VerwijderAapUitBoom(this);
                VorigeBomen.Add(sprongId, TijdelijkeBoom);
                TijdelijkeBoom = pairAfstandEnBoom.Value;
                TijdelijkeBoom.VoegAapToeAanBoom(this);
                Spring(bosWaarAapInIs);
            }
        }
        public void GaUitBos(int bosID)
        {
            int sprongId = VorigeBomen.Count;
            TijdelijkeBoom.VerwijderAapUitBoom(this);
            VorigeBomen.Add(sprongId, TijdelijkeBoom);
            TijdelijkeBoom = null;
            Console.WriteLine("Aap {0} van bos {1} klaar met springen", Naam, bosID);
        }

        public override bool Equals(object obj)
        {
            return obj is Aap aap &&
                   bosID == aap.bosID &&
                   ID == aap.ID &&
                   Naam == aap.Naam;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(bosID, ID, Naam);
        }
        #endregion



    }
}
