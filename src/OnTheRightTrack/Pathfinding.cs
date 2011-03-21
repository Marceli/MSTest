using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace BreadthDepthFirstSearch
{
	class Pathfinding
	{
		private static Graph graph = new Graph();

		private static Hashtable dist = new Hashtable();
		private static Hashtable route = new Hashtable();

		/// <summary>
		/// Performs a Breadth Search search
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public static void BreadthFirstSearch(Node start, Node end)
		{
			var queue = new Queue<Node>();

			queue.Enqueue(start);

			while(queue.Count != 0)
			{
				var u = queue.Dequeue();

				// Check if node is the end
				if(u == end)
				{
					Console.Write("Path found.");

					break;
				}
				else
				{
					u.Data = "Visited";

					// Expands u's neighbors in the queue
					foreach(var edge in u.Connections)
					{
						if(edge.Neighbor.Data == null)
						{
							edge.Neighbor.Data = "Visited";

							if(edge.Neighbor != end)
							{
								edge.Neighbor.PathParent = u;

								PrintPath(edge.Neighbor);
							}
							else
							{
								edge.Neighbor.PathParent = u;

								PrintPath(edge.Neighbor);

								return;
							}

							Console.WriteLine();
						}
						/* shows the repeated nodes
            else
            {
              Console.Write(edge.Neighbor.Key);
            } */

						queue.Enqueue(edge.Neighbor);
					}
				}
			}
		}

		/// <summary>
		/// Performs a Depth First search
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		public static void DepthFirstSearch(Node start, Node end)
		{
			var stack = new Stack<Node>();

			stack.Push(start);

			while(stack.Count != 0)
			{
				var u = stack.Pop();

				// Check if node is the end
				if(u == end)
				{
					Console.WriteLine("Path found");

					break;
				}
				else
				{
					u.Data = "Visited";

					// Store n's neighbors in the stack
					foreach(Track edge in u.Connections)
					{
						if(edge.Neighbor.Data == null)
						{
							edge.Neighbor.Data = "Visited";

							if(edge.Neighbor != end)
							{
								edge.Neighbor.PathParent = u;

								PrintPath(edge.Neighbor);
							}
							else
							{
								edge.Neighbor.PathParent = u;

								PrintPath(edge.Neighbor);

								return;
							}

							Console.WriteLine();

							stack.Push(edge.Neighbor);
						}
						/* shows the repeated nodes
            else
            {
              Console.Write(edge.Neighbor.Key);
            } */
					}
				}
			}
		}

		/// <summary>
		/// Initializes the distance and route tables used for Dijkstra's Algorithm.
		/// </summary>
		/// <param name="start">The <b>Key</b> to the source Node.</param>
		static void InitDistRouteTables(string start)
		{
			// set the initial distance and route for each city in the pathFinding.Graph
			foreach(var n in graph.Nodes)
			{
				dist.Add(n.Key, Int32.MaxValue);
				route.Add(n.Key, null);
			}

			// set the initial distance of start to 0
			dist[start] = 0;
		}

		/// <summary>
		/// Relaxes the edge from the Node uCity to vCity.
		/// </summary>
		/// <param name="cost">The distance between uCity and vCity.</param>
		private static void Relax(Node uCity, Node vCity, int cost)
		{
			var distTouCity = (int)dist[uCity.Key];
			var distTovCity = (int)dist[vCity.Key];

			if(distTovCity > distTouCity + cost)
			{
				// update distance and route
				dist[vCity.Key] = distTouCity + cost;
				route[vCity.Key] = uCity;
			}
		}

		/// <summary>
		/// Retrieves the Node from the passed-in NodeList that has the <i>smallest</i> value in the distance table.
		/// </summary>
		/// <remarks>This method of grabbing the smallest Node gives Dijkstra's Algorithm a running time of
		/// O(<i>n</i><sup>2</sup>), where <i>n</i> is the number of nodes in the pathFinding.Graph.  A better approach is to
		/// use a <i>priority queue</i> data structure to store the nodes, rather than an array.  Using a priority queue
		/// will improve Dijkstra's running time to O(E lg <i>n</i>), where E is the number of edges.  This approach is
		/// preferred for sparse pathFinding.Graphs.  For more information on this, consult the README included in the download.</remarks>
		private static Node GetMin(IDictionary<string ,Node> nodes)
		{
			// find the node in nodes with the smallest distance value
			int minDist = Int32.MaxValue;
			Node minNode = null;
			foreach(var n in nodes)
			{
				if(((int)dist[n.Key]) <= minDist)
				{
					minDist = (int)dist[n.Key];
					minNode = n.Value;
				}
			}

			return minNode;
		}

		/// <summary>
		/// Dijkstra's Algorithm to find the shortest path.
		/// </summary>
		static public void ShortestPath(Node start, Node end)
		{
			if(start == end)
			{
				Console.WriteLine("There's no shortest path: start and end city are equal.");

				return;
			}
       
			InitDistRouteTables(start.Key);		// initialize the route & distance tables

			IDictionary<string,Node> nodes = graph.Nodes;	// nodes == Q

			/**** START DIJKSTRA ****/
			while(nodes.Count > 0)
			{
				Node u = GetMin(nodes);		// get the minimum node
				nodes.Remove(u.Key);			// remove it from the set Q

				// iterate through all of u's neighbors
				foreach(Track edge in u.Connections)
					Relax(u, edge.Neighbor, edge.Cost);		// relax each edge
			}	// repeat until Q is empty
			/**** END DIJKSTRA ****/

			// Display results
			string results = "The shortest path from " + start.Key + " to " + end.Key + " is " + dist[end.Key].ToString() + " miles and goes as follows: ";

			Stack traceBackSteps = new Stack();

			Node current = new Node(end.Key, null);

			Node prev = null;

			do
			{
				prev = current;
				current = (Node)route[current.Key];

				string temp = current.Key + " to " + prev.Key + "\r\n";
				traceBackSteps.Push(temp);
			} while(current.Key != start.Key);

			StringBuilder sb = new StringBuilder(30 * traceBackSteps.Count);
			while(traceBackSteps.Count > 0)
				sb.Append((string)traceBackSteps.Pop());

			Console.WriteLine(results + "\r\n\r\n" + sb.ToString());

			dist.Clear();
			route.Clear();
		}

		/// <summary>
		/// Prints the graph's edges
		/// </summary>
		static public void ShowEdges()
		{
			foreach(Node node in graph.Nodes.Values)
				foreach(Track edge in node.Connections)
					Console.WriteLine("{0} <-> {1} - {2} miles", node.Key, edge.Neighbor.Key, edge.Cost);
		}

		/// <summary>
		/// Prints the full path for each search iteration
		/// </summary>
		/// <param name="node"></param>
		static public void PrintPath(Node node)
		{
			if(node.PathParent != null)
				PrintPath(node.PathParent);

			Console.Write("{0} ", node.Key);
		}

		public virtual Graph Graph
		{
			get
			{
				return graph;
			}
			set
			{
				graph = value;
			}
		}

	}
}