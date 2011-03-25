using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace OnTheRightTrack
{
    public class Path<T> : IEnumerable<T> where T : class
    {
        public T LastStep { get; private set; }
        public Path<T> PreviousSteps { get; private set; }
        public int TotalCost { get; private set; }
        private Path(T lastStep, Path<T> previousSteps, int totalCost)
        {
            LastStep = lastStep;
            PreviousSteps = previousSteps;
            TotalCost = totalCost;
        }
        public Path(T start) : this(start, null, 0) { }
        public Path<T> AddStep(T step, int stepCost)
        {
            return new Path<T>(step, this, TotalCost + stepCost);
        }
        public IEnumerator<T> GetEnumerator()
        {
            for (Path<T> p = this; p != null; p = p.PreviousSteps)
                yield return p.LastStep;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
        public bool IsMoveBack(T from,T to)
        {
            T prevoiusNode =default(T);
            foreach (var node in this)
            {
                if (node == to && prevoiusNode == from)
                    return true;
                prevoiusNode = node;
            }
            return false;
        }
        public string GetPath()
        {
            StringBuilder sb = new StringBuilder();
            foreach(var node in this.Reverse())
            {
                sb.AppendFormat("{0} -> ", node);
            }
            return sb.ToString(0, sb.Length - 3);

        }
    }
}
