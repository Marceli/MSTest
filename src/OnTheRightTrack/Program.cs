using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OnTheRightTrack
{
	class Program
	{
        static int vertexCount;
        static int edgeCount;
        static int lenght;
		static void Main(string[] args)
		{
            
            Console.WriteLine("Algorytm przetwarza plik z danymi domyslnie: input.txt");
            Console.WriteLine("Algorytm sprawdza czy droga o zadanej dlugosci w podanym grafie jest mozliwa.");
            Console.WriteLine("Drogi krotsze lub dluzsze sa odrzucane");
            Console.WriteLine("Drogi zawierajace cykla sa dozwolone np dozwolone jest: A->B->C-A->B->C ->...");

            Console.WriteLine("Algorytm zaklada ze przejad powrotny po podanym odcinku jest niadozwolony");
            Console.WriteLine("np: zakazane jest A->B->A lub A->B->C->D->B->A");
            Console.WriteLine();
            Console.WriteLine("Podaj nazwe pliku z danymi np : input.txt");
            Console.WriteLine("Struktura pliku wiersz pierwszy try liczby int V,E,S ilosc przystankow, ilosc drog, wymagana roga");
            Console.WriteLine("V - ilosc przystankow");
            Console.WriteLine("E - ilosc drog");
            Console.WriteLine("S - dlugosc wymaganej drogi");
            Console.WriteLine("E wierszy nastepnych A B D");
            Console.WriteLine("A przystanek Z");
            Console.WriteLine("B przystanek DO");
            Console.WriteLine("D dlugosc odcinka z A do B");


           
            var pathFinding = new Pathfinding();
            var filename = @"input.txt";
            if (args.Count() == 1)
                filename = args[0];

            if (!File.Exists(filename))
            {
                 return;
            }


            InitGraph(pathFinding, filename);
            
            var stopwatch=Stopwatch.StartNew();
            bool success=false;
            for (var i = 1; i < vertexCount; i++)
            {
                
                if (pathFinding.MyDepthFirstSearch(pathFinding.Graph.Nodes[i], lenght))
                {
                    success = true;
                    break;
                }
                Console.WriteLine("new vertex");
                Console.WriteLine("Run time:" + stopwatch.ElapsedMilliseconds);
            }
            if(success)
                Console.WriteLine("Success!!!");
            else
                Console.WriteLine("Failure!!!");


            Console.WriteLine();
            Console.WriteLine("Run time:"+stopwatch.ElapsedMilliseconds);
			Console.ReadKey();
		}

        private static void InitGraph(Pathfinding pathFinding,string filename)
        {
            using (var streamReader = File.OpenText(filename))
            {

                var graphDefinition = (from s in streamReader.ReadLine().Split(' ') select int.Parse(s)).ToArray();
                vertexCount = graphDefinition[0];
                edgeCount = graphDefinition[1];
                lenght = graphDefinition[2];

                //adding Vertexes
                for (var i = 1; i <= vertexCount; i++)
                    pathFinding.Graph.AddNode(i, null);

                //adding Edges
                int[] trackData;
                for (var i = 0; i < edgeCount; i++)
                {
                    trackData = (from s in streamReader.ReadLine().Split(' ') select int.Parse(s)).ToArray();
                    pathFinding.Graph.AddUndirectedEdge(trackData[0], trackData[1], trackData[2]);
                }
            }
        }
	}
}
