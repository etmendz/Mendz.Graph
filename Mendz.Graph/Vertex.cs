using System;

namespace Mendz.Graph
{
    /// <summary>
    /// Represents a vertex (Graph Theory).
    /// </summary>
    /// <remarks>
    /// The vertex is a fundamental unit in a graph.
    /// In its most basic sense, a vertex is simply a value.
    /// In this implementation, the vertex (value) is identified via ID or vertex number.
    /// </remarks>
    public class Vertex : IComparable<Vertex>, IFormattable
    {
        /// <summary>
        /// Gets the ID of the vertex.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Gets or sets the value of the vertex.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Gets or sets the weight of the vertex.
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Constructor to create an instance of the vertex given its ID and value.
        /// </summary>
        /// <param name="id">The ID of the vertex.</param>
        /// <param name="value">The value of the vertex.</param>
        public Vertex(int id, object value)
        {
            ID = id;
            Value = value;
        }

        #region Overrides
        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Vertex))
            {
                return false;
            }
            return ID == ((Vertex)obj).ID;
        }

        public override int GetHashCode() => ID;

        public override string ToString() => ToString("G", new DOTFormatProvider());
        #endregion

        #region Implements
        /// <summary>
        /// Compares this vertex to another vertex.
        /// </summary>
        /// <param name="other">The other Vertex instance to compare to.</param>
        /// <returns>A number that can be used to sort vertices.</returns>
        /// <remarks>The comparison is via Vertex.IDs.</remarks>
        public int CompareTo(Vertex other) => ID.CompareTo(other.ID);

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
