using System.Collections.Generic;
using System.Diagnostics;

namespace OnTheRightTrack
{
    [DebuggerDisplay("Node Key:{key} Data:{data}")]
	public class Node:INode
	{
		
        public Node(int key):this(key,null,null){}

        public Node(int key,object data) : this(key, data,null) { }
            
		public Node(int key, object data, IList<Track> connections)
		{
			this.Key = key;
			this.Data = data;
			if (connections == null)
			{
				Connections = new List<Track>();
			}
			else
			{

				Connections = connections;
			}
		}
        public int Len{get;set;}

        public Path<Node> Path { get; set; }
       
		public object Data{get;set;}

		public int Key{get;private set;}	
		
        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		public virtual IList<Track> Connections{get;private set; }
		
		public virtual Node PathParent { get; set; }

		protected internal virtual void AddDirected(Track e)
		{
			Connections.Add(e);
		}


		protected internal virtual void AddDirected(Node n, int cost)
		{
			AddDirected(new Track(n, cost));
		}
	}
}