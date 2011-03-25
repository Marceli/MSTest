using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace OnTheRightTrack
{
	class Pathfinding
	{
      
        public Graph Graph{get;set;}
        
       
        public Pathfinding()
        {
            this.Graph = new Graph();
        }

        

        /// <summary>
        /// Performs a Depth First search
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public bool  MyDepthFirstSearch(Node start, int s)
        {
            var stack = new Stack<Node>();
            var path = new Path<Node>(start);
            var len = 0;
            start.Path = path;
            stack.Push(start);
            if (len == s)
            {
                Console.WriteLine("Success!!!");
                return true;
            }

            while (stack.Count != 0)
            {
                var u = stack.Pop();
                len = u.Len;
                    

                    // Store n's neighbors in the stack
                    foreach (Track edge in u.Connections)
                    {
                            //Console.WriteLine("Current step: {0} -> {1} ", u.Key, edge.Neighbor.Key);
                        Node from;
                        edge.Neighbor.Path=u.Path.AddStep(edge.Neighbor, edge.Cost);
                        if (u.Path.IsCycle(edge.Neighbor,out from))
                        {
                            int cycleLen = edge.Neighbor.Path.TotalCost - from.Path.TotalCost;
                            if (s % cycleLen == from.Path.TotalCost)
                            {
                                Console.WriteLine(pathLine + "Success!!!");
                                return true;

                            }




                        }
                            if(u.Path.IsMoveBack(u,edge.Neighbor))
                            {
                                continue;
                            }
                            //edge.Neighbor.Data = "Visited";
                            edge.Neighbor.PathParent = u;
                           
                            edge.Neighbor.Path=u.Path.AddStep(edge.Neighbor, edge.Cost);
                            if (edge.Neighbor.Path.TotalCost>=s)
                            {
                                var pathLine=edge.Neighbor.Path.GetPath();
                                if (edge.Neighbor.Path.TotalCost == s)
                                {
                                    //Console.WriteLine("Success!!!");
                                    Console.WriteLine(pathLine + "Success!!!");
                                    return true;
                                }
                                //Console.WriteLine("Too long!!!");
                                continue;
                            }
                            stack.Push(edge.Neighbor);
                        
                    }
                }
            return false;        
        }
    }
}
            
        
