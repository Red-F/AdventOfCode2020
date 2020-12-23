using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode.Days
{
    [Day(2020, 23)]
    public class Day23 : BaseDay
    {
        public override string PartOne(string input) => PlayOne(input.Select(c => c - '0').ToArray());

        public override string PartTwo(string input) => PlayTwo(input.Select(c => c - '0').ToArray());

        private static string PlayOne(int[] cups)
        {
            var (currentNode, list) = CreateList(cups, cups.Length);
            for (var i = 0; i < 100; i++) currentNode = PlayRound(currentNode, list);
            return list[1].ToString().Substring(1);
        }

        private static string PlayTwo(int[] cups)
        {
            var (currentNode, list) = CreateList(cups, 1000000);
            for (var i = 0; i < 10000000; i++) currentNode = PlayRound(currentNode, list);
            var value1 = (long) list[1].Next.Value;
            var value2 = (long) list[1].Next.Next.Value;
            return (value1 * value2).ToString();
        }

        private static Node<int> PlayRound(Node<int> currentNode, Node<int>[] list)
        {
            var currentValue = currentNode.Value;
            var pickedUp = currentNode.Take(3);
            var pickedUpValues = new[] {pickedUp.Value, pickedUp.Next.Value, pickedUp.Next.Next.Value};
            var destinationNode = FindDestinationNode(list, currentValue, pickedUpValues);
            destinationNode.Insert(pickedUp);
            currentNode = currentNode.Next;
            return currentNode;
        }

        private static Node<int> FindDestinationNode(IReadOnlyList<Node<int>> list, in int currentValue, int[] pickedUpValues)
        {
            var destinationCupValue = currentValue;
            do
            {
                if (--destinationCupValue < 1) destinationCupValue = list.Count - 1;
            } while (pickedUpValues.Contains(destinationCupValue));
            return list[destinationCupValue];
        }

        private static (Node<int> root, Node<int>[] list) CreateList(IReadOnlyList<int> items, int desiredNrOfItems)
        {
            var nrOfItems = items.Count;
            var list = new Node<int>[desiredNrOfItems + 1];
            var root = new Node<int>(items[0]);
            list[items[0]] = root;
            var end = root;
            for (var i = 1; i < nrOfItems; i++)
            {
                end = end.Append(new Node<int>(items[i]));
                list[items[i]] = end;
            }
            for (var i = nrOfItems + 1; i < desiredNrOfItems + 1; i++)
            {
                end = end.Append(new Node<int>(i));
                list[i] = end;
            }
            return (root, list);
        }
    }

    internal class Node<T>
    {
        public Node(T value)
        {
            Value = value;
            Next = this;
        }

        public Node<T> Next;

        private Node<T> Last
        {
            get
            {
                var node = this;
                while (node.Next != node) node = node.Next;
                return node;
            }
        }

        public T Value { get; }

        public Node<T> Append(Node<T> node)
        {
            node.Next = Next;
            Next = node;
            return node;
        }

        public Node<T> Take(int i)
        {
            var next = Next;
            var last = next;
            for (var j = 0; j < i - 1; j++) last = last.Next;
            Next = last.Next;
            last.Next = last;
            return next;
        }

        public void Insert(Node<T> node)
        {
            var last = node.Last;
            var tmp = this.Next;
            this.Next = node;
            last.Next = tmp;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(Value);
            var node = Next;
            while (node != this)
            {
                sb.Append(node.Value);
                node = node.Next;
            }
            return sb.ToString();
        }
    }
}