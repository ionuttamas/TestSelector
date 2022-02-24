using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestSelector.Services.TestMatcher.IntervalTree
{
    public class IntervalTree<T>
    {
        private class IntervalNode<TData>
        {
            private IntervalNode(int low, int high)
            {
                Low = low;
                High = high;
                Max = high;
                Min = low;
            }

            public IntervalNode(int low, int high, TData data) : this(low, high)
            {
                Data = data;
            }

            public int Low { get; }
            public int High { get; }
            public int Min { get; set; }
            public int Max { get; set; }
            public TData Data { get; }

            public IntervalNode<T> Left { get; set; }
            public IntervalNode<T> Right { get; set; }

            public bool Overlaps(int low, int high)
            {
                return IntervalTree<TData>.Overlaps(Low, High, low, high);
            }

            public bool Overlaps(IntervalNode<T> interval)
            {
                return Overlaps(interval.Low, interval.High);
            }
        }

        private IntervalNode<T> root;

        public void InsertInterval(int low, int high, T data)
        {
            var newNode = new IntervalNode<T>(low, high, data);

            if (root == null)
            {
                root = newNode;
                return;
            }

            var currentNode = root;

            while (currentNode!=null)
            {
                currentNode.Min = Math.Min(currentNode.Min, low);
                currentNode.Max = Math.Max(currentNode.Max, high);

                if (low < currentNode.Low)
                {
                    if (currentNode.Left == null)
                    {
                        currentNode.Left = newNode;
                        break;
                    }

                    currentNode = currentNode.Left;
                }
                else
                {
                    if (currentNode.Right == null)
                    {
                        currentNode.Right = newNode;
                        break;
                    }

                    currentNode = currentNode.Right;
                }
            }
        }

        public IEnumerable<T> GetOverlaps(int low, int high)
        {
            if(root==null)
                throw new InvalidOperationException("Root node is not initialized");

            var queue = new Queue<IntervalNode<T>>();
            queue.Enqueue(root);

            while (queue.Count>0)
            {
                var node = queue.Dequeue();

                if (node.Overlaps(low, high))
                    yield return node.Data;

                if (node.Left != null && Overlaps(node.Left.Min, node.Left.Max, low, high))
                {
                    queue.Enqueue(node.Left);
                }

                if (node.Right != null && Overlaps(node.Right.Min, node.Right.Max, low, high))
                {
                    queue.Enqueue(node.Right);
                } 
            }
        } 

        public static bool Overlaps(int firstLow, int firstHigh, int secondLow, int secondHigh)
        {
            return Math.Max(firstLow, secondLow) <= Math.Min(firstHigh, secondHigh);
        }
    }
}
