using System;
using System.Collections.Generic;
using System.Text;

namespace Mendz.Graph
{
    /// <summary>
    /// Represents the default DOT formatter.
    /// </summary>
    /// <remarks>
    /// Supports format GVLWX where:
    /// <list type="bullet">
    /// <item>G: general, default; vertex shows ID</item>
    /// <item>V: vertex shows Value</item>
    /// <item>L: edge shows Label</item>
    /// <item>W: edge shows Weight</item>
    /// <item>X: vertex shows Value as label</item>
    /// </list>
    /// </remarks>
    public class DOTFormatter : ICustomFormatter
    {
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (formatProvider == null)
            {
                formatProvider = new DOTFormatProvider();
            }
            if (arg is Vertex v)
            {
                switch (format.Replace("X", "").Replace("L", "").Replace("W", ""))
                {
                    case "GV":
                        return "\"" + v.ID.ToString() + "\\n(" + v.Value.ToString() + ")\"";
                    case "VG":
                        return "\"" + v.Value.ToString() + "\\n(" + v.ID.ToString() + ")\"";
                    case "V":
                        return "\"" + v.Value.ToString() + "\"";
                    case "G":
                    default:
                        return v.ID.ToString();
                }
            }
            else if (arg is Edge e)
            {
                string attr = "";
                switch (format.Replace("X", "").Replace("G", "").Replace("V", ""))
                {
                    case "LW":
                        attr = " [label=\"" + e.Label + "\\n(" + e.Weight.ToString() + ")\"]";
                        break;
                    case "WL":
                        attr = " [label=\"" + e.Weight.ToString() + "\\n(" + e.Label + ")\"]";
                        break;
                    case "W":
                        attr = " [label=\"" + e.Weight.ToString() + "\"]";
                        break;
                    case "L":
                        attr = " [label=\"" + e.Label + "\"]";
                        break;
                    default:
                        attr = "";
                        break;
                }
                return e.Tail.ToString(format, formatProvider) + Edge.ToDirectedDOTNotation(e.Directed) + e.Head.ToString(format, formatProvider) + attr;
            }
            else if (arg is Graph g)
            {
                StringBuilder sb = new StringBuilder();
                HashSet<int> labeledVertices = new HashSet<int>();
                sb.AppendLine((g.DirectedEdgeCount > 0 ? "di" : "") + "graph " + g.Name + " {");
                sb.AppendLine(" node [fontsize = \"12\"];");
                sb.AppendLine(" edge [fontsize = \"8\"];");
                foreach (var edge in g.Edges.Values)
                {
                    if (format.Contains("X"))
                    {
                        labeledVertices.Add(edge.Tail.ID);
                        labeledVertices.Add(edge.Head.ID);
                    }
                    sb.AppendLine(" " + edge.ToString(format, formatProvider) + ";");
                }
                foreach (var id in labeledVertices)
                {
                    sb.AppendLine(" " + id.ToString() + " [label=\"" + g.Vertices[id].Value.ToString() + "\"];");
                }
                sb.AppendLine("}");
                return sb.ToString();
            }
            else
            {
                throw new ArgumentException("Unsupported argument type.", "arg");
            }
        }
    }
}
