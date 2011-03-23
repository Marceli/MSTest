using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace OnTheRightTrack
{
    [DebuggerDisplay("Track Lenght:{cost} to:{Neighbor.Key}")]
	public class Track
	{
		// Fields
		private int cost;
		private Node neighbor;

		public Track(Node neighbor)
			: this(neighbor, 0)
		{
		}

		public Track(Node neighbor, int cost)
		{
			this.cost = cost;
			this.neighbor = neighbor;
		}

		// Properties
		public virtual int Cost
		{
			get
			{
				return this.cost;
			}
		}

		public virtual Node Neighbor
		{
			get
			{
				return this.neighbor;
			}
		}
	}


}
