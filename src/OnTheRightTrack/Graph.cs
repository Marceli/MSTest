using System;
using System.Collections.Generic;

namespace OnTheRightTrack
{
	public class Graph
	{
		// Fields
		private readonly IDictionary<string, Node> nodes;

		// Methods
		public Graph()
		{
			nodes = new Dictionary<string, Node>();
		}

		public Graph(IDictionary<string, Node> nodes)
		{
			this.nodes = nodes;
		}

		public virtual int Count
		{
			get { return nodes.Count; }
		}

		public virtual IDictionary<string, Node> Nodes
		{
			get { return nodes; }
		}

		public virtual void AddNode(Node n)
		{
			if (nodes.ContainsKey(n.Key))
			{
				throw new ArgumentException("There already exists a node in the graph with key " + n.Key);
			}
			nodes.Add(n.Key, n);
		}

		public virtual Node AddNode(string key, object data)
		{
			if (nodes.ContainsKey(key))
			{
				throw new ArgumentException("There already exists a node in the graph with key " + key);
			}
			var n = new Node(key, data);
			nodes.Add(key, n);
			return n;
		}

		public virtual void AddUndirectedEdge(Node u, Node v)
		{
			AddUndirectedEdge(u, v, 0);
		}

		public virtual void AddUndirectedEdge(string uKey, string vKey)
		{
			AddUndirectedEdge(uKey, vKey, 0);
		}

		public virtual void AddUndirectedEdge(Node u, Node v, int cost)
		{
			if (!nodes.ContainsKey(u.Key) || !nodes.ContainsKey(v.Key))
			{
				throw new ArgumentException("One or both of the nodes supplied were not members of the graph.");
			}
			u.AddDirected(v, cost);
			v.AddDirected(u, cost);
		}

		public virtual void AddUndirectedEdge(string uKey, string vKey, int cost)
		{
			if (!nodes.ContainsKey(uKey) || !nodes.ContainsKey(vKey))
			{
				throw new ArgumentException("One or both of the nodes supplied were not members of the graph.");
			}
			AddUndirectedEdge(nodes[uKey], nodes[vKey], cost);
		}

		public virtual void Clear()
		{
			nodes.Clear();
		}

		public virtual bool Contains(Node n)
		{
			return Contains(n.Key);
		}

		public virtual bool Contains(string key)
		{
			return nodes.ContainsKey(key);
		}

		// Properties
	}
}