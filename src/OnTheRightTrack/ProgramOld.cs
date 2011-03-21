using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OnTheRightTrack
{
	class Program
	{
		static void Main(string[] args)
		{
			var streamReader = File.OpenText(@"..\..\input.txt");
			string input = null;
			var values = parseLine(streamReader.ReadLine());
			var adjacencylist = new AdjacencyList(values[0],values[1],values[2]);
			while ((input=streamReader.ReadLine())!=null)
			{
				var trackData = parseLine(input);
				var fromStopId = trackData[0]-1;
				var toStopId = trackData[1]-1;
				var lenght = trackData[2];
				adjacencylist.AddTrack(fromStopId,toStopId,lenght);

			}
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
