# Mendz.Graph
Provides an implementation of Graph Theory graph G = (V, E), that can generate its DOT notation, which can be used for rendering the graph. [Wiki](https://github.com/etmendz/Mendz.Graph/wiki)
## Namespaces
### Mendz.Graph
#### Contents
Name | Description
---- | -----------
Vertex | Represents a vertex (Graph Theory.
Edge | Represents an edge (Graph Theory).
Graph | Represents a graph (Graph Theory).
DOTFormatProvider | Represents a DOT notation format provider.
DOTFormatter | Rpresents a DOT notation formatter.
#### Vertex
The vertex is a fundamental unit in a graph. It is also known as the node.
In its most basic sense, a vertex is simply a value.
In this implementation, the vertex (value) is identified via ID or vertex number.
#### Edge
The edge is a fundamental unit in a graph.
The edge defines the link between two vertices.
In this implementation, the edge is identified via its edge ID (a tuple of the tail, directed indicator and the head).

By definition, an edge should link a subset of the graph's vertices.
Thus, by design, the Edge can only be created via Graph.AddEdge(),
which makes sure that the edge's endpoints exist in the graph's vertices.
#### Graph
A graph is defined as G = (V, E)
- where V is a collection of vertices,
- where E is a collection of edges, and
- where an edge is a (u, v) pair of vertices in V. 

The Graph is a class with properties Vertices and Edges,
- where Vertices is a collection of Vertex instances indexed by Vertex.ID,
- where Edges is a collection of Edge instances indexed by Edge.ID, and
- where an Edge is made up of a pair of Vertex instances in Graph.Vertices.

The Graph provides the methods to manage and maintain its Vertices and Edges.
Additional methods are provided to create alternative indexes of Vertices and Edges,
which are useful when creating lists and matrices based on the Graph's data.

Note that there is no effort to include search and traversal algorithms as part of the Graph.
The Graph is basically a data model for graph representations and algorithms.
#### DOTFormatProvider and DOTFormatter
Mendz.Graph provides the ability for the graph to generate its DOT notation via DOTFormatProvider and DOTFormatter.
The following format specs are supported:
- G: general, default; vertex shows ID
- V: vertex shows Value
- L: edge shows Label
- W: edge shows Weight
- X: vertex shows Value as label

As per MSDN specification, "G" is the default format. These specifications can be combined (ex. GLWX).
Note that Mendz.Graph's implementation to generate the DOT notation is intentionally basic and simple.
Developers are encouraged to create their own format providers and custom formatters.
## Example
The following is from the console application TestGraph's [Program.cs](https://github.com/etmendz/Mendz.Graph/blob/master/TestGraph/Program.cs):
```C#
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
```
Try to build and run (debug) this program.
In the console, test the output DOT notation at http://webgraphviz.com/ or through your favorite graph rendering engine.
## NuGet It...
https://www.nuget.org/packages/Mendz.Graph/
