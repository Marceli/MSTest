using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnTheRightTrack
{
    public interface INode 
    {
        Path<Node> Path { get; set; }
        
        int Key { get; }
        
    }
}
