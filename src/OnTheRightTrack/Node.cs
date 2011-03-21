using System.Collections.Generic;

namespace BreadthDepthFirstSearch
{
	public class Node
	{
		// Fields
		private readonly IList<Track> connections;
		private readonly string key;
		private object data;

		public Node(string key, object data)
			: this(key, data, null)
		{
		}

		public Node(string key, object data, IList<Track> connections)
		{
			this.key = key;
			this.data = data;
			if (connections == null)
			{
				connections = new List<Track>();
			}
			else
			{
				connections = connections;
			}
		}

		// Properties
		public virtual object Data
		{
			get { return data; }
			set { data = value; }
		}

		public virtual string Key
		{
			get { return key; }
		}

		public virtual IList<Track> Connections
		{
			get { return connections; }
		}

		public virtual Node PathParent { get; set; }

		protected internal virtual void AddDirected(Track e)
		{
			connections.Add(e);
		}


		protected internal virtual void AddDirected(Node n, int cost)
		{
			AddDirected(new Track(n, cost));
		}
	}
}