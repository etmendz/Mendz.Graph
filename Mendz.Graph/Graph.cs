using System;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Mendz.Graph
{
    /// <summary>
    /// Represents a graph (Graph Theory). 
    /// </summary>
    /// <remarks>
    /// <para>
    /// A graph is defined as G = (V, E)
    /// where V is a collection of vertices,
    /// where E is a collection of edges, and
    /// where an edge is a (u, v) pair of vertices in V. 
    /// </para>
    /// <para>
    /// The Graph is a class with properties Vertices and Edges,
    /// where Vertices is a collection of Vertex instances indexed by Vertex.ID,
    /// where Edges is a collection of Edge instances indexed by Edge.ID, and
    /// where an Edge is made up of a pair of Vertex instances in Graph.Vertices.
    /// </para>
    /// <para>
    /// The Graph provides the methods to manage and maintain its Vertices and Edges.
    /// Additional methods are provided to create alternative indexes of Vertices and Edges,
    /// which are useful when creating lists and matrices based on the Graph's data.
    /// </para>
    /// <para>
    /// Note that there is no effort to include search and traversal algorithms as part of the Graph.
    /// The Graph is basically a data model for graph representations and algorithms.
    /// </para>
    /// </remarks>
    public sealed class Graph : IFormattable
    {
        private object o = new object();
        private object iv = new object();
        private object ie = new object();
        private volatile int _directedEdgeCount = 0;

        /// <summary>
        /// Gets or sets the name of the graph.
        /// </summary>
        public string Name { get; set; }

        private ConcurrentDictionary<int, Vertex> _vertices = new ConcurrentDictionary<int, Vertex>();
        private ReadOnlyDictionary<int, Vertex> _readOnlyVertices;
        /// <summary>
        /// Gets the vertices of the graph.
        /// </summary>
        public ReadOnlyDictionary<int, Vertex> Vertices
        {
            get => _readOnlyVertices;
        }

        private Vertex[] _indexedVertices = null;

        /// <summary>
        /// Gets the order of the graph.
        /// </summary>
        public int Order
        {
            get => _vertices.Count;
        }

        private ConcurrentDictionary<(int tail, int directed, int head), Edge> _edges = new ConcurrentDictionary<(int tail, int directed, int head), Edge>();
        private ReadOnlyDictionary<(int tail, int directed, int head), Edge> _readOnlyEdges;
        /// <summary>
        /// Gets the edges of the graph.
        /// </summary>
        public ReadOnlyDictionary<(int tail, int directed, int head), Edge> Edges
        {
            get => _readOnlyEdges;
        }

        private Edge[] _indexedEdges = null;

        /// <summary>
        /// Gets the size of the graph.
        /// </summary>
        public int Size
        {
            get => _edges.Count;
        }

        /// <summary>
        /// Gets the count of directed edges.
        /// </summary>
        public int DirectedEdgeCount
        {
            get => _directedEdgeCount;
        }

        /// <summary>
        /// Gets or sets expansion properties to the graph.
        /// </summary>
        public dynamic Expansion { get; set; }

        /// <summary>
        /// Constructor to create an instance of the graph.
        /// </summary>
        /// <param name="name">The name of the graph.</param>
        public Graph(string name = "G")
        {
            Name = name;
            _readOnlyVertices = new ReadOnlyDictionary<int, Vertex>(_vertices);
            _readOnlyEdges = new ReadOnlyDictionary<(int tail, int directed, int head), Edge>(_edges);
        }

        /// <summary>
        /// Creates a vertex given its value and adds it to the graph.
        /// </summary>
        /// <param name="value">The value of the vertex to create and add.</param>
        /// <returns>True if successful, otherwise, false.</returns>
        public bool AddVertex(int id, object value) => AddVertex(new Vertex(id, value));

        /// <summary>
        /// Adds a vertex to the graph.
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        /// <returns>True if successful, otherwise, false.</returns>
        public bool AddVertex(Vertex vertex)
        {
            lock (o)
            {
                if (_vertices.TryAdd(vertex.ID, vertex))
                {
                    lock (iv)
                    {
                        _indexedVertices = null;
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Removes a vertex from the graph.
        /// </summary>
        /// <param name="id">The ID of the vertex to remove.</param>
        /// <returns>The vertex removed.</returns>
        /// <remarks>Incident edges are also removed from the graph.</remarks>
        public Vertex RemoveVertex(int id)
        {
            lock (o)
            {
                if (_vertices.TryRemove(id, out Vertex removedVertex))
                {
                    lock (iv)
                    {
                        _indexedVertices = null;
                    }
                    bool removed = false;
                    if (_edges.Count > 0)
                    {
                        object obj = new object();
                        _edges
                            .AsParallel()
                            .Where((edge) => edge.Value.Tail.ID == id || edge.Value.Head.ID == id)
                            .ForAll((edge) =>
                                {
                                    lock (obj)
                                    {
                                        Edge e = RemoveEdge(edge.Key);
                                        if (!removed && e != null)
                                        {
                                            removed = true;
                                        }
                                    }
                                });
                    }
                    if (removed)
                    {
                        lock (ie)
                        {
                            _indexedEdges = null;
                        }
                    }
                }
                return removedVertex;
            }
        }

        /// <summary>
        /// Removes a vertex from the graph.
        /// </summary>
        /// <param name="vertex">The vertex to remove.</param>
        /// <returns>The vertex removed.</returns>
        public Vertex RemoveVertex(Vertex vertex) => RemoveVertex(vertex.ID);

        /// <summary>
        /// Returns a Vertex given a vertex ID.
        /// </summary>
        /// <param name="id">The vertex ID.</param>
        /// <returns>The Vertex with the given vertex ID.</returns>
        public Vertex GetVertex(int id) => _vertices[id];

        /// <summary>
        /// Creates and adds an edge to the graph.
        /// </summary>
        /// <param name="tailID">The vertex ID of the tail.</param>
        /// <param name="headID">The vertex ID of the head.</param>
        /// <param name="directed">Indicates if the edge is directed or not.</param>
        /// <param name="weight">The weight of the edge. Default is 0 (unweighted).</param>
        /// <param name="label">The label of the edge.</param>
        /// <returns>True if the edge is added successfully. Otherwise, false.</returns>
        public bool AddEdge(int tailID, int headID, bool directed = false, double weight = 0, string label = "") => AddEdge(Edge.Create(_vertices[tailID], _vertices[headID], directed, weight, label));

        /// <summary>
        /// Adds an edge to the graph.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        /// <returns>True if the edge is added successfully. Otherwise, false.</returns>
        private bool AddEdge(Edge edge)
        {
            lock (o)
            {
                if (_edges.TryAdd(edge.ID, edge))
                {
                    lock (ie)
                    {
                        _indexedEdges = null;
                    }
                    if (edge.Directed)
                    {
                        Interlocked.Increment(ref _directedEdgeCount);
                    }
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        /// Removes an edge from the graph.
        /// </summary>
        /// <param name="id">The ID of the edge to remove.</param>
        /// <returns>The edge removed.</returns>
        public Edge RemoveEdge((int tail, int directed, int head) id)
        {
            lock (o)
            {
                if (_edges.TryRemove(id, out Edge removedEdge))
                {
                    lock (ie)
                    {
                        _indexedEdges = null;
                    }
                    if (removedEdge.Directed)
                    {
                        Interlocked.Decrement(ref _directedEdgeCount);
                    }
                }
                return removedEdge;
            }
        }

        /// <summary>
        /// Removes an edge from the graph.
        /// </summary>
        /// <param name="edge">The edge to remove.</param>
        /// <returns>The edge removed.</returns>
        public Edge RemoveEdge(Edge edge) => RemoveEdge(edge.ID);

        /// <summary>
        /// Returns an Edge given an edge id.
        /// </summary>
        /// <param name="id">The edge ID.</param>
        /// <returns>The Edge with the given edge id.</returns>
        public Edge GetEdge((int tail, int directed, int head) id) => _edges[id];

        /// <summary>
        /// Returns an Edge given an edge name.
        /// </summary>
        /// <param name="name">The edge name.</param>
        /// <returns>The Edge with the given edge name.</returns>
        public Edge GetEdge(string name) =>_edges[Edge.NameToID(name)];

        /// <summary>
        /// Creates an index of vertices.
        /// </summary>
        /// <returns>The indexed vertices.</returns>
        /// <remarks>
        /// It is recommended that IndexVertices() is called after all Add*()/Remove*() calls are done.
        /// </remarks>
        public Vertex[] IndexVertices()
        {
            lock (iv)
            {
                if (_indexedVertices == null)
                {
                    _indexedVertices = _vertices.Values.ToArray<Vertex>();
                    Array.Sort(_indexedVertices);
                }
                return _indexedVertices;
            }
        }

        /// <summary>
        /// Creates an index of edges.
        /// </summary>
        /// <returns>The indexed edges.</returns>
        /// <remarks>
        /// It is recommended that IndexEdges() is called after all Add*()/Remove*() calls are done.
        /// </remarks>
        public Edge[] IndexEdges()
        {
            lock (ie)
            {
                if (_indexedEdges == null)
                {
                    _indexedEdges = _edges.Values.ToArray<Edge>();
                    Array.Sort(_indexedEdges);
                }
                return _indexedEdges;
            }
        }

        #region Overrides
        /// <summary>
        /// Builds and returns a basic DOT representation of the graph.
        /// Directed and mixed graphs are returned as digraph [name] {...}.
        /// Undirected graphs are returned as graph [name] {...}.
        /// </summary>
        /// <returns>A DOT representation of the graph.</returns>
        /// <remarks>
        /// Treats mixed graphs as undirected graphs (graph vs. digraph).
        /// </remarks>
        public override string ToString() => ToString("G", new DOTFormatProvider());
        #endregion

        #region Implements
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                formatProvider = new DOTFormatProvider();
            }
            var formatter = (ICustomFormatter)formatProvider.GetFormat(typeof(ICustomFormatter));
            return formatter.Format(format, this, formatProvider);
        }
        #endregion
    }
}
