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
		static void Main(string[] args)
		{
            var pathFinding = new Pathfinding();

			var streamReader = File.OpenText(@"..\..\input.txt");
			string input = null;
			var values = parseLine(streamReader.ReadLine());

            //adding Vertexes
            for (var i = 1; i <= values[0]; i++)
            {
                pathFinding.Graph.AddNode(i, null);
            }
            //adding Edges
			while ((input=streamReader.ReadLine())!=null)
			{
                
				var trackData = parseLine(input);
                pathFinding.Graph.AddUndirectedEdge(trackData[0], trackData[1], trackData[2]);
			}
            var start = pathFinding.Graph.Nodes[1];
            var end = pathFinding.Graph.Nodes[3];
            var stopwatch=Stopwatch.StartNew();
            Pathfinding.MyDepthFirstSearch(start, 1000);
            Console.WriteLine();
            Console.WriteLine("Run time:"+stopwatch.ElapsedMilliseconds);
			Console.ReadKey();
		}

		private static int[] parseLine(string readLine)
		{
			var result = new int[3];
			var values = readLine.Split(' ');
			for (var i=0;i<values.Count();i++)
			{
				result[i] = int.Parse(values[i]);
			}
			return result;
		}
	}
}
