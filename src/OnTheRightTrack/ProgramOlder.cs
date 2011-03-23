//
//	Depth and Breadth First Search algorithms - C# Sample Application
//	By Leniel Braz de Oliveira Macaferi & Wellington Magalhães Leite - 2007.
//
//  UBM Computer Engineering course - 9th term [http://www.ubm.br/]
//  This program sample was developed and turned in as a term paper for the
//  Artificial Intelligence discipline.
//  The source code is provided "as is" without warranty.
//
//  Author:
//  http://lenielmacaferi.blogspot.com/2008/01/breadth-and-depth-first-search.html
//
//  This code uses some classes from the following package:
//  http://download.microsoft.com/download/e/5/f/e5f82108-9c06-496b-a13a-38bba77c7b2b/graphs.msi
//  
//  See this link for more details:
//  http://msdn2.microsoft.com/en-us/library/aa289152(VS.71).aspx
//  
//  Theory:
//  http://ai-depot.com/Tutorial/PathFinding-Blind.html

using System;

namespace OnTheRightTrack
{
  class ProgramOlder
  {
    static void Main(string[] args)
    {
      var pathFinding = new Pathfinding();

      Node start, end;

      // Vertexes
      pathFinding.Graph.AddNode("Arad", null);
      pathFinding.Graph.AddNode("Bucharest", null);
      pathFinding.Graph.AddNode("Craiova", null);
      pathFinding.Graph.AddNode("Dobreta", null);
      pathFinding.Graph.AddNode("Eforie", null);
      pathFinding.Graph.AddNode("Fagaras", null);
      pathFinding.Graph.AddNode("Giurgiu", null);
      pathFinding.Graph.AddNode("Hirsova", null);
      pathFinding.Graph.AddNode("Iasi", null);
      pathFinding.Graph.AddNode("Lugoj", null);
      pathFinding.Graph.AddNode("Mehadia", null);
      pathFinding.Graph.AddNode("Neamt", null);
      pathFinding.Graph.AddNode("Oradea", null);
      pathFinding.Graph.AddNode("Pitesti", null);
      pathFinding.Graph.AddNode("Rimnicu Vilcea", null);
      pathFinding.Graph.AddNode("Sibiu", null);
      pathFinding.Graph.AddNode("Timisoara", null);
      pathFinding.Graph.AddNode("Urziceni", null);
      pathFinding.Graph.AddNode("Vaslui", null);
      pathFinding.Graph.AddNode("Zerind", null);

      // Edges

      // Arad <-> Zerind
      pathFinding.Graph.AddUndirectedEdge("Arad", "Zerind", 75);
      // Arad <-> Timisoara
      pathFinding.Graph.AddUndirectedEdge("Arad", "Timisoara", 118);
      // Arad <-> Sibiu
      pathFinding.Graph.AddUndirectedEdge("Arad", "Sibiu", 140);

      // Bucharest <-> Urziceni
      pathFinding.Graph.AddUndirectedEdge("Bucharest", "Urziceni", 85);
      // Bucharest <-> Giurgiu
      pathFinding.Graph.AddUndirectedEdge("Bucharest", "Giurgiu", 90);
      // Bucharest <-> Pitesti
      pathFinding.Graph.AddUndirectedEdge("Bucharest", "Pitesti", 101);
      // Bucharest <-> Fagaras
      pathFinding.Graph.AddUndirectedEdge("Bucharest", "Fagaras", 211);

      // Craiova <-> Dobreta
      pathFinding.Graph.AddUndirectedEdge("Craiova", "Dobreta", 120);
      // Craiova <-> Pitesti
      pathFinding.Graph.AddUndirectedEdge("Craiova", "Pitesti", 138);
      // Craiova <-> Rimnicu Vilcea
      pathFinding.Graph.AddUndirectedEdge("Craiova", "Rimnicu Vilcea", 146);

      // Dobreta <-> Mehadia
      pathFinding.Graph.AddUndirectedEdge("Dobreta", "Mehadia", 75);

      // Eforie <-> Hirsova
      pathFinding.Graph.AddUndirectedEdge("Eforie", "Hirsova", 86);

      // Fagaras <-> Sibiu
      pathFinding.Graph.AddUndirectedEdge("Fagaras", "Sibiu", 99);

      // Hirsova <-> Urziceni
      pathFinding.Graph.AddUndirectedEdge("Hirsova", "Urziceni", 98);

      // Iasi <-> Neamt
      pathFinding.Graph.AddUndirectedEdge("Iasi", "Neamt", 87);
      // Iasi <-> Vaslui
      pathFinding.Graph.AddUndirectedEdge("Iasi", "Vaslui", 92);

      // Lugoj <-> Mehadia
      pathFinding.Graph.AddUndirectedEdge("Lugoj", "Mehadia", 70);
      // Lugoj <-> Timisoara
      pathFinding.Graph.AddUndirectedEdge("Lugoj", "Timisoara", 111);

      // Oradea <-> Zerind
      pathFinding.Graph.AddUndirectedEdge("Oradea", "Zerind", 71);
      // Oradea <-> Sibiu
      pathFinding.Graph.AddUndirectedEdge("Oradea", "Sibiu", 151);

      // Pitesti <-> Rimnicu Vilcea
      pathFinding.Graph.AddUndirectedEdge("Pitesti", "Rimnicu Vilcea", 97);

      // Rimnicu Vilcea <-> Sibiu
      pathFinding.Graph.AddUndirectedEdge("Rimnicu Vilcea", "Sibiu", 80);

      // Urziceni <-> Vaslui
      pathFinding.Graph.AddUndirectedEdge("Urziceni", "Vaslui", 142);

      start = pathFinding.Graph.Nodes["Oradea"];

      end = pathFinding.Graph.Nodes["Neamt"];

      Console.WriteLine("\nBreadth First Search algorithm");

      Pathfinding.BreadthFirstSearch(start, end);

      foreach(var n in pathFinding.Graph.Nodes)
        n.Value.Data = null;

      Console.WriteLine("\n\nDepth First Search algorithm");

      Pathfinding.DepthFirstSearch(start, end);

      Console.WriteLine("\n\nShortest path");

      Pathfinding.ShortestPath(start, end);

      pathFinding.Graph.Clear();

      Console.ReadKey();
    }
  }
}