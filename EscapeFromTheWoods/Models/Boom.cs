using System;
using System.Collections.Generic;

namespace EscapeFromTheWoods.Models
{
    class Boom
    {

        #region Constructor
        public Boom(int x, int y, int iD)
        {
            X = x;
            Y = y;
            ID = iD;
            
            ApenInBoom = new List<Aap>();
        }
        #endregion

        #region Properties
        
        public int X { get; private set; }
        public int Y { get; private set; }
        public int ID { get; private set; }
        public List<Aap> ApenInBoom { get; private set; }
        #endregion

        #region HelperFunctions
        public override bool Equals(object obj)
        {
            return obj is Boom boom &&
                   X == boom.X &&
                   Y == boom.Y;
        }
        

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }

        #endregion

        #region Functions
        public static double LengteTussenBomen(Boom boom1, Boom boom2)
        {
            double toReturn = Math.Sqrt(Math.Pow(boom2.X - boom1.X, 2) + Math.Pow(boom2.Y - boom1.Y, 2));

            return toReturn;
        }
        public bool ZitAapInBoom( Aap aapOmTeChecken)
        {
            return ApenInBoom.Contains(aapOmTeChecken);
        }
        public void VoegAapToeAanBoom(Aap toeTeVoegen)
        {
            if(ApenInBoom.Contains(toeTeVoegen))
            {
                Console.WriteLine("Aap {0} zit al in boom {1}",toeTeVoegen.Naam,ID);
            }
            else
            {
                ApenInBoom.Add(toeTeVoegen);
            }
        }
        public void VerwijderAapUitBoom(Aap teVerwijderen)
        {
            if (!ApenInBoom.Contains(teVerwijderen))
            {
                Console.WriteLine("Aap {0} zit nie in boom {1}", teVerwijderen.Naam, ID);
            }
            else
            {
                ApenInBoom.Remove(teVerwijderen);
            }
        }



        #endregion
    }
}
