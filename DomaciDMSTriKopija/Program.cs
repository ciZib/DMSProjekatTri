using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DomaciDMSTri
{
    class Program
    {
        static void DomaciDMSTriTest()
        {
            //0:1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 17, 19, 21, 31
            /*TEST*/
            string a = "0:1, 2, 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 17, 19, 21, 31"; //promeniti u sr.readline

            a = a.Replace(",", "");
            string[] delovi = a.Split(':');
            int br = int.Parse(delovi[0]);

            string aGrafovi = string.Join(" ", delovi.Skip(1)); //ubaciti u relacije[] i splitovati relacije oko " "
                                                                //Console.WriteLine(aGrafovi);

            /*test kraj*/
        }//DomaciDMSTriTest

        static void DomaciDMSTri()
        {
            Console.WriteLine("DOMACI 3 IZ DMS, DO 5. ZADATKA RADIO JA, 6,7,8 ZADATAK CHATGPT, 9 I 10 JA");
            Console.ReadLine();
            Console.Clear();
            Console.Write("v= ? (potrudi se da napises broj jer me mrzi da proveravam)");
            int v = int.Parse(Console.ReadLine());
            Console.Write("t= ? (potrudi se da napises broj jer me mrzi da proveravam)");
            int t = int.Parse(Console.ReadLine());
            StreamReader sr = new StreamReader("graf.txt");
            bool krajFajla = false;
            int brojLinija = sr.ReadToEnd().Split(new char[] { '\n' }).Length; //broj linija koje fajl ima
            sr = new StreamReader("graf.txt");
            int[] cvorovi = new int[brojLinija];
            int brojac = 0;
            string[] relacije = new string[brojLinija];

            while (!krajFajla)
            {
                string linija = sr.ReadLine();
                linija = linija.Replace(",", "");
                string[] delovi = linija.Split(':');
                cvorovi[brojac] = int.Parse(delovi[0]);
                relacije[brojac++] = string.Join("", delovi.Skip(1)); //napravi kako treba
                krajFajla = sr.EndOfStream;
            }//uzima sve vrednosti


            bool[,] grafoviMatrica = new bool[brojLinija, brojLinija];
            int noviBrojac = 0;
            foreach (var item in cvorovi)
            {
                string[] rastavljeneRelacija = relacije[noviBrojac++].Split(' ');
                foreach (var rastavljeniDeo in rastavljeneRelacija)
                {
                    grafoviMatrica[item, int.Parse(rastavljeniDeo)] = true;
                }
            }
            //popunjeno kako valja
            //Printuj2DBool(grafoviMatrica, cvorovi);
            //string[]
            Console.WriteLine("\n");
            Console.WriteLine("Prvi zadatak: Koliki je zbir stepena cvorova v i t u grafu G?");
            Console.WriteLine($"Zbir stepena {v} i {t} je: " + (IzracunajStepen(grafoviMatrica, v) + IzracunajStepen(grafoviMatrica, t)) + "\n\n");

            Console.WriteLine("Drugi zadatak: najkraci put od v do t u grafu G");
            List<int> put = NajkraciPut(grafoviMatrica, v, t);
            Console.WriteLine("Najkraći put: " + string.Join("–", put));

            Console.WriteLine("\n\nTreci zadatak: koji su susedi cvora v a da nisu susedi cvora t?");
            var razlika = RazlikaSuseda(grafoviMatrica, v, t);
            string ispis = razlika.Count > 0 ? string.Join(", ", razlika) : "/";
            Console.WriteLine($"Susedi {v} koji nisu susedi {t}: {ispis}");

            Console.WriteLine("\n\nCetvrti zadatak: Nmg da kucam pitanje mrzi me");
            List<int> manji = SusediSaManjimStepenom(grafoviMatrica, v, t);
            ispis = manji.Count > 0 ? string.Join(", ", manji) : "/";
            Console.WriteLine($"Cvorovi koji su susedi {v} ili {t}, a imaju stepen manji od prosečnog: " + ispis);

            Console.WriteLine("\n\nPeti zadatak: indukovani podgraf broj grana");
            int broj = BrojGranaUIndukovanomPodgrafu(grafoviMatrica, v, t);
            Console.WriteLine($"Broj grana u indukovanom podgrafu: {broj}");

            Console.WriteLine("\n\nSesti zadatak: Rastojanje do 2 od v i t suseda");
            var do2odV = BFSdoDubine(grafoviMatrica, v, 2);
            var do2odT = BFSdoDubine(grafoviMatrica, t, 2);
            var presek = do2odV.Intersect(do2odT).Where(x => x != v && x != t).OrderBy(x => x).ToList();
            // Prikaz rezultata
            if (presek.Count == 0) Console.WriteLine("/");
            else Console.WriteLine(string.Join(", ", presek));

            Console.WriteLine("\n\nSedmi zadatak: Zbir ekscentriceta cvorova");
            int eksV = Ekscentricitet(grafoviMatrica, v);
            int eksT = Ekscentricitet(grafoviMatrica, t);
            int zbir = eksV + eksT;
            Console.WriteLine($"Zbir ekscentriciteta čvorova {v} i {t} je: {zbir}");

            Console.WriteLine("\n\nOsmi zadatak: Povezane komponente nakon uklanjanja v, t i suseda");
            var iskljuceni = new HashSet<int> { v, t };
            iskljuceni.UnionWith(DobaviSusede(grafoviMatrica, v));
            iskljuceni.UnionWith(DobaviSusede(grafoviMatrica, t));
            broj = BrojKomponenti(grafoviMatrica, iskljuceni);
            Console.WriteLine($"Broj komponenti nakon uklanjanja: {broj}");

            Console.WriteLine("\n\nDeveti i Deseti zadatak: Rastojanje 3 i 10");
            var intMatrica = KonvertujBoolUMatricu(grafoviMatrica);
            var matrica3 = StepenujMatricu(intMatrica, 3);
            Console.WriteLine($"Broj puteva dužine 3 između {v} i {t}: {matrica3[v, t]}");
            var matrica10 = StepenujMatricu(intMatrica, 10);
            Console.WriteLine($"Broj puteva dužine 10 između {v} i {t}: {matrica10[v, t]}\n");
        }//DomaciDMSTri

        static int[,] KonvertujBoolUMatricu(bool[,] graf)
        {
            int n = graf.GetLength(0);
            int[,] matrica = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    matrica[i, j] = graf[i, j] ? 1 : 0;
            return matrica;
        }//KonvertujBoolUMatricu

        static int[,] MnoziMatrice(int[,] A, int[,] B)
        {
            int n = A.GetLength(0);
            int[,] rezultat = new int[n, n];
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    for (int k = 0; k < n; k++)
                        rezultat[i, j] += A[i, k] * B[k, j];
            return rezultat;
        }//MnoziMatrice

        static int[,] StepenujMatricu(int[,] matrica, int stepen)
        {
            int n = matrica.GetLength(0);
            int[,] rezultat = new int[n, n];

            for (int i = 0; i < n; i++) rezultat[i, i] = 1;

            while (stepen > 0)
            {
                if ((stepen & 1) == 1)
                    rezultat = MnoziMatrice(rezultat, matrica);
                matrica = MnoziMatrice(matrica, matrica);
                stepen >>= 1;
            }
            return rezultat;
        }//StepenujMatricu

        static HashSet<int> DobaviSusede(bool[,] graf, int cvor)
        {
            int n = graf.GetLength(0);
            HashSet<int> susedi = new HashSet<int>();
            for (int i = 0; i < n; i++)
            {
                if (graf[cvor, i])
                    susedi.Add(i);
            }
            return susedi;
        }//DobaviSusede

        // Funkcija da izračuna broj komponenti u podgrafu (bez nekih čvorova)
        static int BrojKomponenti(bool[,] graf, HashSet<int> iskljuceni)
        {
            int n = graf.GetLength(0);
            bool[] posecen = new bool[n];
            int brojKomponenti = 0;

            for (int i = 0; i < n; i++)
            {
                if (!posecen[i] && !iskljuceni.Contains(i))
                {
                    brojKomponenti++;
                    // BFS
                    Queue<int> red = new Queue<int>();
                    red.Enqueue(i);
                    posecen[i] = true;

                    while (red.Count > 0)
                    {
                        int curr = red.Dequeue();
                        for (int j = 0; j < n; j++)
                        {
                            if (graf[curr, j] && !posecen[j] && !iskljuceni.Contains(j))
                            {
                                red.Enqueue(j);
                                posecen[j] = true;
                            }
                        }
                    }
                }
            }
            return brojKomponenti;
        }//BrojKomponenti

        static int Ekscentricitet(bool[,] graf, int start) //chatgpt ne da mi se da ucim bfs
        {
            //ekscentricitet je najduzi najkraci put od cvora do nekog cvora
            int n = graf.GetLength(0);
            bool[] posecen = new bool[n];
            int[] dist = new int[n];
            for (int i = 0; i < n; i++) dist[i] = -1;

            Queue<int> red = new Queue<int>();
            red.Enqueue(start);
            posecen[start] = true;
            dist[start] = 0;

            while (red.Count > 0)
            {
                int curr = red.Dequeue();
                for (int i = 0; i < n; i++)
                {
                    if (graf[curr, i] && !posecen[i])
                    {
                        red.Enqueue(i);
                        posecen[i] = true;
                        dist[i] = dist[curr] + 1;
                    }
                }
            }
            return dist.Max();
        }//ekscentricitet

        static HashSet<int> BFSdoDubine(bool[,] graf, int start, int maxDubina) //specijalno hvala chatgptu
        {
            int n = graf.GetLength(0);
            bool[] posecen = new bool[n];
            int[] dist = new int[n];
            for (int i = 0; i < n; i++) dist[i] = -1;

            Queue<int> red = new Queue<int>();
            red.Enqueue(start);
            posecen[start] = true;
            dist[start] = 0;

            HashSet<int> rezultat = new HashSet<int> { start };

            while (red.Count > 0)
            {
                int curr = red.Dequeue();
                for (int i = 0; i < n; i++)
                {
                    if (graf[curr, i] && !posecen[i])
                    {
                        dist[i] = dist[curr] + 1;
                        if (dist[i] <= maxDubina)
                        {
                            red.Enqueue(i);
                            rezultat.Add(i);
                            posecen[i] = true;
                        }
                    }
                }
            }
            return rezultat;
        }//BFSdoDubine

        static int RastojanjeBFS(bool[,] graf, int v, int t) //chatgpt ne da mi se da ucim bfs
        {
            int n = graf.GetLength(0);
            bool[] posecen = new bool[n];
            int[] rastojanje = new int[n];
            for (int i = 0; i < n; i++) rastojanje[i] = -1;
            Queue<int> red = new Queue<int>();
            red.Enqueue(v);
            posecen[v] = true;
            rastojanje[v] = 0;

            while (red.Count > 0)
            {
                int trenutni = red.Dequeue();
                for (int i = 0; i < n; i++)
                {
                    if (graf[trenutni, i] && !posecen[i])
                    {
                        red.Enqueue(i);
                        posecen[i] = true;
                        rastojanje[i] = rastojanje[trenutni] + 1;

                        if (i == t) return rastojanje[i];
                    }
                }
            }
            return -1; // Ako nema puta između v i t
        }//RastojanjeBFS

        static int BrojGranaUIndukovanomPodgrafu(bool[,] graf, int v, int t)
        {
            //indukovani podgraf je skup v t i njihovih suseda
            int n = graf.GetLength(0);
            HashSet<int> cvorovi = new HashSet<int> { v, t };

            //Dodaje susede v i t
            for (int i = 0; i < n; i++)
            {
                if (graf[v, i]) cvorovi.Add(i);
                if (graf[t, i]) cvorovi.Add(i);
            }

            int brojGrana = 0;
            var lista = cvorovi.ToList();

            for (int i = 0; i < lista.Count; i++)
            {
                for (int j = i + 1; j < lista.Count; j++)
                {
                    int a = lista[i];
                    int b = lista[j];
                    if (graf[a, b]) brojGrana++;
                }
            }
            return brojGrana;
        }//BrojGranaUIndukovanomPodgrafu

        static List<int> SusediSaManjimStepenom(bool[,] graf, int v, int t)
        {
            int n = graf.GetLength(0);
            int ukupnoStepena = 0;

            for (int i = 0; i < n; i++) for (int j = 0; j < n; j++) if (graf[i, j]) ukupnoStepena++;

            double prosecniStepen = ukupnoStepena / (double)n;

            HashSet<int> susedi = new HashSet<int>(); //da budu distinktni
            for (int i = 0; i < n; i++)
            {
                if (graf[v, i]) susedi.Add(i);
                if (graf[t, i]) susedi.Add(i);
            }

            List<int> rezultat = new List<int>();
            foreach (int s in susedi)
            {
                int stepen = 0;
                for (int i = 0; i < n; i++)
                    if (graf[s, i]) stepen++;

                if (stepen < prosecniStepen)
                    rezultat.Add(s);
            }
            return rezultat;
        }//SusediSaManjimStepenom

        static int IzracunajStepen(bool[,] graf, int cvor)
        {
            int stepen = 0;
            int brojCvorova = graf.GetLength(0);
            for (int i = 0; i < brojCvorova; i++)
            {
                if (graf[cvor, i])
                    stepen++;
            }
            return stepen;
        }//IzracunajStepen

        static List<int> RazlikaSuseda(bool[,] graf, int v, int t)
        {
            int n = graf.GetLength(0);
            List<int> rezultat = new List<int>();

            for (int i = 0; i < n; i++)
            {
                if (graf[v, i] && !graf[t, i])
                    rezultat.Add(i);
            }
            return rezultat;
        }//RazlikaSuseda

        static List<int> NajkraciPut(bool[,] graf, int pocetak, int kraj)
        {
            int n = graf.GetLength(0);
            bool[] posecen = new bool[n];
            int[] prethodnik = new int[n];

            for (int i = 0; i < n; i++) prethodnik[i] = -1;

            Queue<int> red = new Queue<int>();
            red.Enqueue(pocetak);
            posecen[pocetak] = true;

            while (red.Count > 0)
            {
                int trenutni = red.Dequeue();
                for (int i = 0; i < n; i++)
                {
                    if (graf[trenutni, i] && !posecen[i])
                    {
                        red.Enqueue(i);
                        posecen[i] = true;
                        prethodnik[i] = trenutni;
                        if (i == kraj) break;
                    }
                }
            }

            List<int> put = new List<int>();

            for (int at = kraj; at != -1; at = prethodnik[at]) put.Add(at);
            put.Reverse();
            if (put[0] != pocetak) return new List<int>();
            return put;
        }//NajkraciPutBFS

        static void Printuj2DBool(bool[,] matrica, int[] a)
        {
            Console.WriteLine("\n\n");
            Console.Write("   ");
            foreach (var item in a)
            {
                Console.Write(item + "  ");
            }
            foreach (var item in a)
            {
                Console.Write($"\n{item} ");
                foreach (var item2 in a)
                {
                    Console.Write(matrica[item, item2] ? " 1 " : " 0 ");
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }//printuj2dbool

        static void Main(string[] args)
        {
            DomaciDMSTri();
        }
    }
}
