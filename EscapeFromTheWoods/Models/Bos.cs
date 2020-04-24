using Syroot.Windows.IO;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EscapeFromTheWoods.Models
{
    class Bos
    {
        #region Constructor
        public Bos(int iD, int maxX, int maxY, int aantalApen, int aantalBomen)
        {
            MinX = 0;
            MinY = 0;
            MaxX = maxX;
            MaxY = maxY;

            ID = iD;

            Bomen = new List<Boom>();
            MaakBomen(aantalBomen);
            Apen = new List<Aap>();
            PlaatsApen(aantalApen);

            bitmap = new Bitmap(maxX * 22, maxY * 22);

            ZijnApenKlaarMetSpringen = false;

        }
        #endregion

        #region Properties
        public int MinX { get; private set; }
        public int MinY { get; private set; }
        public int MaxX { get; private set; }
        public int MaxY { get; private set; }
        public int ID { get; private set; }
        public List<Boom> Bomen { get; private set; }
        public List<Aap> Apen { get; private set; }
        public bool ZijnApenKlaarMetSpringen;
        public Bitmap bitmap;
        #endregion

        #region HelperFunctions
        #endregion

        #region Functions
        public void LaatApenSpringen()
        {
            Console.WriteLine("Bos {0} apen beginnen met springen", ID);
            List<Task> tasks = new List<Task>();
            foreach (Aap aap in Apen)
            {

                tasks.Add(Task.Run(() => aap.Spring(this)));
                Console.WriteLine("Aap {0} van bos {1} begint met springen", aap.Naam, ID);
            }
            Task.WaitAll(tasks.ToArray());
            ZijnApenKlaarMetSpringen = true;
            Console.WriteLine("Bos {0} apen klaar met springen", ID);
        }

        private void MaakBomen(int aantalBomen)
        {
            Random random = new Random();
            for (int i = 1; i <= aantalBomen; i++)
            {
                int x = random.Next(MaxX);
                int y = random.Next(MaxY);

                Boom toAdd = new Boom(x, y, i);
                if (Bomen.Contains(toAdd))
                {
                    i--;
                }
                else
                {
                    Bomen.Add(toAdd);
                }
            }
        }

        private void PlaatsApen(int aantalApen)
        {
            for (int i = 1; i <= aantalApen; i++)
            {
                Random random = new Random();
                int index = random.Next(Bomen.Count);
                int naamNummer = random.Next(Aap.NrOfNames());
                Aap.AapNamen naam = (Aap.AapNamen)naamNummer;
                Aap toAdd = new Aap(naam.ToString(), Bomen[index], this.ID);
                if (Bomen[index].ApenInBoom.Count != 0)
                {
                    i--;
                }
                else
                {
                    Bomen[index].VoegAapToeAanBoom(toAdd);
                    Apen.Add(toAdd);
                }
            }
        }

        public KeyValuePair<double, Boom> VindDichtstbijzijndeBoom(Aap aapDieSpringt)
        {

            double kortsteAfstand = double.MaxValue;
            Boom kortsteBoom = null;
            foreach (var boom in Bomen)
            {
                double afstandTeChecken = Math.Abs(Boom.LengteTussenBomen(aapDieSpringt.TijdelijkeBoom, boom));
                if (afstandTeChecken != 0)
                {
                    if (!aapDieSpringt.VorigeBomen.Values.Contains(boom))
                    {
                        if (afstandTeChecken < kortsteAfstand)
                        {
                            kortsteBoom = boom;
                            kortsteAfstand = afstandTeChecken;
                        }
                    }
                }
            }
            KeyValuePair<double, Boom> toReturn = new KeyValuePair<double, Boom>(kortsteAfstand, kortsteBoom);
            return toReturn;
        }

        public double AfstandNaarRandVanBos(Boom boomOmVanTeZoeken)
        {
            double afstandVanRand =
                (new List<double>()
                {
                    MaxX - boomOmVanTeZoeken.X,
                    MaxY - boomOmVanTeZoeken.Y,
                    boomOmVanTeZoeken.X - MinX,
                    boomOmVanTeZoeken.Y - MinY}
                ).Min();

            return afstandVanRand;
        }

        private void DrawBm()
        {
            Graphics g = Graphics.FromImage(bitmap);
            Pen p = new Pen(Color.Green, 1);

            foreach (Boom boom in Bomen)
            {
                g.DrawEllipse(p, boom.X * 22, boom.Y * 22, 20, 20);
            }

            List<Color> usedColors = new List<Color>();
            foreach (Aap aap in Apen)
            {
                Random rnd = new Random();
                Color c = Color.FromArgb(255, rnd.Next(256 - 0), rnd.Next(256 - 0), rnd.Next(256 - 0));
                while(usedColors.Contains(c))
                {
                    c = Color.FromArgb(255, rnd.Next(256 - 0), rnd.Next(256 - 0), rnd.Next(256 - 0));
                }
                Brush b = new SolidBrush(c);
                for (int i = 0; i < aap.VorigeBomen.Count; i++)
                {
                    if (i == 0)
                    {
                        g.FillEllipse(b, aap.VorigeBomen[i].X * 22, aap.VorigeBomen[i].Y * 22, 20, 20);
                    }
                    else
                    {
                        p.Color = c;
                        int centerX1 = aap.VorigeBomen[i - 1].X * 22 + 11;
                        int centerY1 = aap.VorigeBomen[i - 1].Y * 22 + 11;
                        int centerX2 = aap.VorigeBomen[i].X * 22 + 11;
                        int centerY2 = aap.VorigeBomen[i].Y * 22 + 11;
                        g.DrawLine(p, centerX1, centerY1, centerX2, centerY2);
                    }
                }
            }

        }

        public void SaveBm()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DrawBm();

                KnownFolder documents = new KnownFolder(KnownFolderType.Documents);
                string path = documents.Path + @"\OefThread" + @"\Bitmaps\" + ID.ToString() + "_escapeRoutes.jpg";
                bitmap.Save(path, ImageFormat.Jpeg);
            }
        }
        #endregion
    }
}


