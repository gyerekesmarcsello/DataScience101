using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenetikusAlgoritmusokBeadando
{
    public class Gen
    {
        const int CGenMin = -100;
        const int CGenMax = 100;

        public int gen;

        public static Random rnd = new Random();

        public Gen()
        {
            gen = rnd.Next(CGenMin, CGenMax);
        }

        public static Gen Mutacio(Gen g)
        {
            return new Gen();
        }
    }

    public class Egyed
    {
        const int CGenszam = 4;
        const int CGenMutacio = 25; // 25%

        public Gen[] Genek;
        int JosagErtek = -1;

        public static Random rnd = new Random();

        public Egyed()
        {
            Genek = new Gen[CGenszam];
            for (int i = 0; i < Genek.Length; i++)
                Genek[i] = new Gen();
        }

        public double FvErtek()
        {
            return Genek[0].gen * Genek[0].gen + Genek[1].gen * Genek[1].gen +
                Genek[2].gen * Genek[2].gen + Genek[3].gen * Genek[3].gen;
        }

        public int Josag(int Celertek)
        {
            if (JosagErtek != -1)
                return JosagErtek;

            double fv = FvErtek();
            double diff = Math.Abs(fv - Celertek);

            double fret = 100000 / (0.5 * diff + 10);
            if (fret <= 0)
                fret = 1;
            int ret = (int)Math.Round(fret);

            JosagErtek = ret;
            return ret;
        }

        public void Kiir(int Celertek)
        {
            Console.WriteLine("{0}^2 + {1}^2 + {2}^2 + {3}^2 = {4}  | jóság: {5}",
                Genek[0].gen, Genek[1].gen, Genek[2].gen, Genek[3].gen, FvErtek(), Josag(Celertek));
        }

        public static Egyed Keresztezes(Egyed e1, Egyed e2)
        {
            // Ez a függvény a paraméterben megkapott egyedek génjeinek 
            // keresztezésével létrehoz egy új egyedet
            Egyed e = new Egyed();
            e.Genek[0] = e1.Genek[0];
            e.Genek[1] = e1.Genek[1];
            e.Genek[2] = e2.Genek[2];
            e.Genek[3] = e2.Genek[3];
            return e;
        }

        public static Egyed Mutacio(Egyed e)
        {
            // Ez a függvény létrehoz egy új és visszaad egy új egyedet, 
            // mely génjeit egy adott valószínűség (CGenMutacio) szerint mutálja
            Egyed ujE = e;
            if (rnd.Next(0, 101) < CGenMutacio) 
            {
                ujE.Genek[0]=Gen.Mutacio(ujE.Genek[0]);
            }
            if (rnd.Next(0, 101) < CGenMutacio)
            {
                ujE.Genek[1] = Gen.Mutacio(ujE.Genek[1]);
            }
            if (rnd.Next(0, 101) < CGenMutacio)
            {
                ujE.Genek[2] = Gen.Mutacio(ujE.Genek[2]);
            }
            if (rnd.Next(0, 101) < CGenMutacio)
            {
                ujE.Genek[3] = Gen.Mutacio(ujE.Genek[3]);
            }
            return ujE;
        }
    }

    public class GA
    {
        const int CEgyedMutacio = 25;

        public Egyed[] Populacio;
        public Egyed[] Szelektaltak;

        public int Celertek = 0;

        public static Random rnd = new Random();

        public GA(int Celertek)
        {
            Init(500, 20);
            this.Celertek = Celertek;
        }

        public GA(int Celertek, int PopulacioMeret, int SzelekcioMeret)
        {
            Init(PopulacioMeret, SzelekcioMeret);
            this.Celertek = Celertek;
        }

        private void Init(int PopulacioMeret, int SzelekcioMeret)
        {
            Populacio = new Egyed[PopulacioMeret];
            for (int i = 0; i < Populacio.Length; i++)
                Populacio[i] = new Egyed();

            Szelektaltak = new Egyed[SzelekcioMeret];
            for (int i = 0; i < Szelektaltak.Length; i++)
                Szelektaltak[i] = new Egyed();
        }

        public void JosagszamitasSzelekcio()
        {
            // Ez a függvény jóság szerint (csökkenő) sorbarendezi a teljes populációt
            // majd a rulettkerék algoritmussal kiválasztja azokat az egyedeket
            // akik létre fogják majd hozni a következő generációt
            for (int i = 0; i < Populacio.Length; i++)
            {
                for (int j = i; j < Populacio.Length; j++)
                {
                    if (Populacio[j].Josag(Celertek) > Populacio[i].Josag(Celertek)) 
                    {
                        Egyed seged = Populacio[i];
                        Populacio[i] = Populacio[j];
                        Populacio[j] = seged;
                    }
                }
            }
            /*//nem a rulettkerék megoldás
            for (int i = 0; i < Szelektaltak.Length; i++)
            {
                Szelektaltak[i] = Populacio[i];
            }*/
            int[] fitnesses = new int[Populacio.Length];
            double[] rateFitnesses = new double[Populacio.Length];

            int sum = 0;
            for (int i = 0; i < Populacio.Length; i++)
            {
                int josag = Populacio[i].Josag(Celertek);
                sum += josag;
                fitnesses[i] = josag;
            }
            for (int i = 0; i < rateFitnesses.Length; i++)
            {
                rateFitnesses[i] = (double)fitnesses[i]/sum;
            }
            int it = 0;
            while(it<Szelektaltak.Length){
                int random = rnd.Next(0, Populacio.Length);
                double r = rnd.NextDouble();
                if (r < rateFitnesses[random]*10) 
                {
                    Szelektaltak[it] = Populacio[random];
                    it++;
                }
            }
            
        }

        public void Keresztezodes()
        {
            // Ez a függvény a szelektált egyedek keresztezésével létrehozza a következő populációt
            for (int i = 0; i < Populacio.Length; i++)
            {
                Egyed e1 = Szelektaltak[rnd.Next(0, Szelektaltak.Length)];
                Egyed e2 = Szelektaltak[rnd.Next(0, Szelektaltak.Length)];
                Populacio[i] = Egyed.Keresztezes(e1, e2);
            }
        }

        public void Mutacio()
        {
            // Ez a függvény végigmegy a teljes populáción és egy adott valószínűség 
            // szerint (CEgyedMutacio) az adott egyedeket mutálja
            for (int i = 0; i < Populacio.Length; i++)
            {
                Populacio[i] = Egyed.Mutacio(Populacio[i]);
            }
        }
    }
}
