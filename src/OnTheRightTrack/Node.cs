using System.Collections.Generic;
using System.Diagnostics;

namespace OnTheRightTrack
{
    [DebuggerDisplay("Node Key:{key} Data:{data}")]
	public class Node
	{
		
		private readonly IList<Track> connections;
       
		private readonly int key;
		private object data;

		

        private IDictionary<int,bool> forbidden = new Dictionary<int,bool>();
        public IDictionary<int, bool> Forbidden { get { return forbidden; } }
        public Node(int key):this(key,null,null){}
        public Path<Node> Path { get; set; }
        public Node(int key, object data): this(key, data, null)
        {
        }
		public Node(int key, object data, IList<Track> connections)
		{
			this.key = key;
			this.data = data;
			if (connections == null)
			{
				this.connections = new List<Track>();
			}
			else
			{
				this.connections = connections;
			}
		}
        public virtual int Len{get;set;}
       
		public virtual object Data
		{
			get { return data; }
			set { data = value; }
		}

		public virtual int Key
		{
			get { return key; }
		}
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
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