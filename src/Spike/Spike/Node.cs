using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spike
{
	public class Node
	{

		public Node(string key, object data);

		public Node(string key, object data, List<Node> neighbors);



		public virtual object Data { get; set; }

		public virtual string Key { get; }

		public virtual AdjacencyList Neighbors { get; }

		public virtual Node PathParent { get; set; }



		protected internal virtual void AddDirected(EdgeToNeighbor e);

		protected internal virtual void AddDirected(Node n);

		protected internal virtual void AddDirected(Node n, int cost);

	}
}
