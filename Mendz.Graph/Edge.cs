using System;
using System.Text.RegularExpressions;

namespace Mendz.Graph
{
    /// <summary>
    /// Represents an edge (Graph Theory).
    /// </summary>
    /// <remarks>
    /// <para>
    /// The edge is a fundamental unit in a graph.
    /// The edge defines the link between two vertices.
    /// In this implementation, the edge is identified via its edge ID.
    /// </para>
    /// <para>
    /// By definition, an edge should link a subset of the graph's vertices.
    /// Thus, by design, the Edge can only be created via Graph.AddEdge(),
    /// which makes sure that the edge's endpoints exist in the graph's vertices.
    /// </para>
    /// </remarks>
    public sealed class Edge : IComparable<Edge>, IFormattable
    {
        public const string DIRECTED_DOT_NOTATION = " -> ";
        public const string UNDIRECTED_DOT_NOTATION = " -- ";

        private Tuple<Vertex, Vertex> _endpoints;

        private (int tail, int directed, int head) _id;
        /// <summary>
        /// Gets the edge's ID.
        /// </summary>
        /// <remarks>
        /// The ID is a tuple of Tail.ID, directed indicator (0 means undirected and 1 means directed), and Head.ID.
        /// </remarks>
        public (int tail, int directed, int head) ID
        {
            get => _id;
        }

        /// <summary>
        /// Gets the name of the edge in DOT notation.
        /// </summary>
        public string Name
        {
            get => ToString();
        }

        /// <summary>
        /// Gets or sets the label of the edge.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Gets the endpoints of the edge.
        /// </summary>
        public Tuple<Vertex, Vertex> EndPoints
        {
            get => _endpoints;
        }

        /// <summary>
        /// Gets the tail of the edge.
        /// </summary>
        public Vertex Tail
        {
            get => _endpoints.Item1;
        }

        /// <summary>
        /// Gets the head of the edge.
        /// </summary>
        public Vertex Head
        {
            get => _endpoints.Item2;
        }

        private bool _directed;
        /// <summary>
        /// Gets or sets if the edge is directed (true) or undirected (false).
        /// </summary>
        public bool Directed
        {
            get => _directed;
            set
            {
                if (_directed != value)
                {
                    _directed = value;
                    SetID();
                }
            }
        }

        /// <summary>
        /// Gets or sets the weight of the edge.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Gets or sets expansion properties to the edge.
        /// </summary>
        public dynamic Expansion { get; set; }

        /// <summary>
        /// Creates an instance of the edge.
        /// </summary>
        /// <param name="tail">The tail vertex.</param>
        /// <param name="head">The head vertex.</param>
        /// <param name="directed">Indicates if the edge is directed (true) or undirected (false).</param>
        /// <param name="weight">The weight of the edge. Default is 0 (unweighted).</param>
        /// <param name="label">The label of the edge. Default is an empty string.</param>
        private Edge(Vertex tail, Vertex head, bool directed = false, double weight = 0, string label = "")
        {
            _endpoints = new Tuple<Vertex, Vertex>(tail, head);
            _directed = directed;
            Weight = weight;
            Label = label;
            SetID();
        }

        /// <summary>
        /// Creates an edge.
        /// </summary>
        /// <param name="tail">The tail vertex.</param>
        /// <param name="head">The head vertex.</param>
        /// <param name="directed">Indicates if the edge is directed or not.</param>
        /// <param name="weight">The weight of the edge. Default is 0 (unweighted).</param>
        /// <param name="label">The label of the edge.</param>
        /// <returns>The edge created.</returns>
        internal static Edge Create(Vertex tail, Vertex head, bool directed = false, double weight = 0, string label = "") => new Edge(tail, head, directed, weight, label);

        /// <summary>
        /// Deconstructs an edge to its ID parts.
        /// </summary>
        /// <param name="tail">The tail ID.</param>
        /// <param name="directed">An indicator if directed (1) or undirected (0).</param>
        /// <param name="head">The head ID.</param>
        public void Deconstruct(out int tail, out int directed, out int head)
        {
            tail = _endpoints.Item1.ID;
            directed = ToDirectedIndicator(_directed);
            head = _endpoints.Item2.ID;
        }

        /// <summary>
        /// Sets the edge's ID.
        /// </summary>
        private void SetID()
        {
            (int tail, int directed, int head) = this;
            _id = (tail, directed, head);
        }

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Edge))
            {
                return false;
            }
            return _id.Equals(((Edge)obj).ID);
        }

        public override int GetHashCode() => _id.GetHashCode();

        /// <summary>
        /// Builds and returns a basic DOT representation of the edge.
        /// </summary>
        /// <returns>A DOT representation of the edge.</returns>
        public override string ToString() => ToString("G", new DOTFormatProvider());
        #endregion

        #region Implements
        /// <summary>
        /// Compares this edge to another edge.
        /// </summary>
        /// <param name="other">The other Edge instance to compare to.</param>
        /// <returns>A number that can be used to sort edges.</returns>
        /// <remarks>The comparison is via Edge.IDs.</remarks>
        public int CompareTo(Edge other) => ID.CompareTo(other.ID);

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

        #region Utilities
        /// <summary>
        /// Translates a directed flag to its indicator equivalent.
        /// </summary>
        /// <param name="directed">The directed flag.</param>
        /// <returns>The directed flag's indicator equivalent.</returns>
        public static int ToDirectedIndicator(bool directed) => (directed ? 1 : 0);

        /// <summary>
        /// Translates a directed DOT notation to its indicator equivalent.
        /// </summary>
        /// <param name="directedDOTNotation">The directed DOT notation.</param>
        /// <returns>The directed DOT notation's indicator equivalent.</returns>
        public static int ToDirectedIndicator(string directedDOTNotation)
        {
            if (directedDOTNotation.Trim() == DIRECTED_DOT_NOTATION.Trim())
            {
                return ToDirectedIndicator(true);
            }
            else if (directedDOTNotation.Trim() == UNDIRECTED_DOT_NOTATION.Trim())
            {
                return ToDirectedIndicator(false);
            }
            else
            {
                throw new ArgumentOutOfRangeException("directedDOTNotation", "Only Edge.DIRECTED_DOT_NOTATION or Edge.UNDIRECTED_DOT_NOTATION values are recognized.");
            }
        }

        /// <summary>
        /// Translates the directed flag to its DOT notation equivalent.
        /// </summary>
        /// <param name="directed">The directed flag.</param>
        /// <returns>The directed flag's DOT notation equivalent.</returns>
        public static string ToDirectedDOTNotation(bool directed) => (directed ? DIRECTED_DOT_NOTATION : UNDIRECTED_DOT_NOTATION);

        /// <summary>
        /// Translates the directed indicator to its DOT notation equivalent.
        /// </summary>
        /// <param name="directed">The directed indicator.</param>
        /// <returns>The directed indicator's DOT notation equivalent.</returns>
        public static string ToDirectedDOTNotation(int directed)
        {
            if (directed == 1 || directed == 0)
            {
                return ToDirectedDOTNotation((directed == 1 ? true : false));
            }
            else
            {
                throw new ArgumentOutOfRangeException("directed", "Only 1 or 0 can be recognized.");
            }
        }

        /// <summary>
        /// Returns an edge ID(-like) value based on an edge name(-like) value.
        /// </summary>
        /// <param name="edgeName">The edge name.</param>
        /// <returns>The edge ID representation of an edge's name.</returns>
        public static (int tail, int directed, int head) NameToID(string edgeName)
        {
            if (!Regex.IsMatch(edgeName, @"^\d+ -[->] \d+$"))
            {
                throw new ArgumentException("Invalid edge name.", "edgeName");
            }
            string[] id = edgeName.Split(' ');
            return (Convert.ToInt32(id[0]), ToDirectedIndicator(id[1]), Convert.ToInt32(id[2]));
        }

        /// <summary>
        /// Returns an edge name(-like) value based on an edge ID(-like) value.
        /// </summary>
        /// <param name="edgeID">The edge ID.</param>
        /// <returns>The edge name representation of the edge ID.</returns>
        public static string IDToName((int tail, int directed, int head) edgeID)
        {
            if (edgeID.tail <= 0 || edgeID.directed < 0 || edgeID.directed > 1 || edgeID.head <= 0)
            {
                throw new ArgumentException("Invalid edge ID.", "edgeID");
            }
            return edgeID.tail.ToString() + ToDirectedDOTNotation(edgeID.directed) + edgeID.head.ToString();
        }
        #endregion
    }
}
