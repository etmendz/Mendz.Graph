using System;
using Mendz.Graph;

namespace TestGraph
{
    class Program
    {
        static void Main(string[] args)
        {
            Graph g = new Graph();
            for (int i = 1; i <= 10; i++)
            {
                g.AddVertex(i, i.ToString() + "v");
            }
            g.AddEdge(1, 6, weight: 1.6, label: "e16");
            g.AddEdge(2, 7);
            g.AddEdge(3, 8);
            g.AddEdge(4, 9);
            g.AddEdge(5, 10, weight: 5.1, label: "e51");
            g.AddEdge(6, 5, weight: 6.5, label: "e65");
            g.AddEdge(7, 4);
            g.AddEdge(8, 3);
            g.AddEdge(9, 2);
            g.AddEdge(10, 1, weight: 10.1, label: "e101");
            g.AddEdge(1, 2);
            g.AddEdge(3, 2);
            g.AddEdge(3, 3, weight: 3.3, label: "e33");
            g.AddEdge(3, 5);
            g.AddEdge(3, 7);
            g.AddEdge(3, 9, weight: 3.9, label: "e39");
            Console.WriteLine("G render:");
            Console.WriteLine(g.ToString());
            Console.ReadKey(true);
            Console.WriteLine("GLWX render:");
            Console.WriteLine(g.ToString("GLWX", new DOTFormatProvider()));
            Console.ReadKey(true);
        }
    }
}
