﻿namespace TspLibNet.TSP
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Loads TSP files
    /// </summary>
    public class TspFileLoader
    {
        /// <summary>
        /// Loads TSP File structure from a stream
        /// </summary>
        /// <param name="reader">stream reader</param>
        /// <returns>TSP File structure loaded from a stream</returns>
        public TspFile Load(System.IO.StreamReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            List<string> lines = new List<string>();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                line = line.Trim();
                if(!string.IsNullOrEmpty(line))
                {
                    lines.Add(line);
                }
            }

            TspFile tspFile = new TspFile();
            List<string[]> sections = SplitToSections(lines);
            foreach(string[] section in sections)
            {
                string line = section[0];
                if (line.StartsWith("Name", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.Name = this.ReadStringFromLine("Name", line);
                }
                else if(line.StartsWith("Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.Type = this.ReadFileTypeFromLine("Type", line);
                }
                else if (line.StartsWith("Comment", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.Comment = this.ReadStringFromLine("Comment", line);
                }
                else if (line.StartsWith("Dimension", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.Dimension = this.ReadIntFromLine("Dimension", line);
                }
                else if (line.StartsWith("Capacity", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.Capacity = this.ReadIntFromLine("Capacity", line);
                }
                else if (line.StartsWith("Edge_Weight_Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.EdgeWeightType = this.ReadEdgeWeightTypeFromLine("Edge_Weight_Type", line);
                }
                else if (line.StartsWith("Edge_Weight_Format", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.EdgeWeightFormat = this.ReadEdgeWeightFormatFromLine("Edge_Weight_Format", line);
                }
                else if (line.StartsWith("Edge_Data_Format", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.EdgeDataFormat = this.ReadEdgeDataFormatFromLine("Edge_Data_Format", line);
                }
                else if (line.StartsWith("Node_Coord_Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.NodeCoordinatesType = this.ReadNodeCoordinatesTypeFromLine("Node_Coord_Type", line);
                }
                else if (line.StartsWith("Display_Data_Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.DisplayDataType = this.ReadDisplayDataTypeFromLine("Display_Data_Type", line);
                }                
                else if (line.StartsWith("Node_Coord_Section", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.DisplayDataType = Defines.DisplayDataType.Coordinates;
                    if (tspFile.NodeCoordinatesType == Defines.NodeCoordinatesType.NoCoordinates)
                    {
                        tspFile.NodeCoordinatesType = Defines.NodeCoordinatesType.Coordinates2D;
                    }                    

                    if(tspFile.NodeCoordinatesType == Defines.NodeCoordinatesType.Coordinates2D)
                    {
                        tspFile.Nodes = this.ReadIntAndDoublesArrayList("Node_Coord_Section", section);
                    }
                    else if(tspFile.NodeCoordinatesType == Defines.NodeCoordinatesType.Coordinates3D)
                    {
                        tspFile.Nodes = this.ReadIntAndDoublesArrayList("Node_Coord_Section", section);
                    }
                    else throw new NotSupportedException();
                }
                else if (line.StartsWith("Depot_Section", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.Depots = this.ReadIntList("Depot_Section", section);
                }
                else if (line.StartsWith("Demand_Section", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.Demands = this.ReadIntsArrayList("Demand_Section", section);
                }
                else if (line.StartsWith("Edge_Data_Section", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (tspFile.EdgeDataFormat == Defines.EdgeDataFormat.EdgeList)
                    {
                        tspFile.Edges = this.ReadIntsArrayList("Edge_Data_Section", section);
                    }
                    else if (tspFile.EdgeDataFormat == Defines.EdgeDataFormat.AdjacencyList)
                    {
                        tspFile.Edges = this.ReadIntsArrayList("Edge_Data_Section", section);
                    }
                    else throw new NotSupportedException();
                }
                else if (line.StartsWith("Fixed_Edges", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.FixedEdges = this.ReadIntsArrayList("Fixed_Edges", section);
                }
                else if (line.StartsWith("Display_Data_Section", StringComparison.InvariantCultureIgnoreCase))
                {
                    if(tspFile.DisplayDataType == Defines.DisplayDataType.Display2D)
                    {
                        tspFile.DisplayNodes = this.ReadIntAndDoublesArrayList("Display_Data_Section", section);
                    }
                    else throw new NotSupportedException();
                }
                else if (line.StartsWith("Tour_Section", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.Tour = this.ReadIntList("Tour_Section", section);
                }
                else if (line.StartsWith("Edge_Weight_Section", StringComparison.InvariantCultureIgnoreCase))
                {
                    tspFile.EdgeWeights = this.ReadDoubleList("Edge_Weight_Section", section);
                }
                else if (line.StartsWith("EOF", StringComparison.InvariantCultureIgnoreCase))
                {
                    break;
                }
                else throw new NotSupportedException();
            }

            return tspFile;
        }

        private bool StarstWithLetter(string text)
        {
            text = text.Trim();
            if (text.Length > 0)
            {
                if (text[0] >= 'A' && text[0] <= 'Z')
                    return true;
                if (text[0] >= 'a' && text[0] <= 'z')
                    return true;
            }

            return false;
        }

        private List<string[]> SplitToSections(IEnumerable<string> lines)
        {
            List<string[]> sections = new List<string[]>();
            List<string> section = new List<string>();
            List<string> data = new List<string>(lines);
            for (int i = 0; i < data.Count; i++)
            {
                string line = data[i];
                if (StarstWithLetter(line))
                {
                    if (section.Count == 0)
                    {
                        section.Add(line);
                    }
                    else
                    {
                        sections.Add(section.ToArray());
                        section.Clear();
                        section.Add(line);
                    }
                }
                else
                {
                    section.Add(line);
                }
            }

            if (section.Count > 0)
                sections.Add(section.ToArray());

            return sections;
        }

        private Defines.FileType ReadFileTypeFromLine(string sectionName, string line)
        {
            int index = line.IndexOf(':');
            if (index > 0)
                line = line.Substring(index + 1).Trim();

            if (line.StartsWith("TSP", StringComparison.InvariantCultureIgnoreCase)) 
                return Defines.FileType.TSP;
            if (line.StartsWith("ATSP", StringComparison.InvariantCultureIgnoreCase))
                return Defines.FileType.ATSP;
            if (line.StartsWith("CVRP", StringComparison.InvariantCultureIgnoreCase))
                return Defines.FileType.CVRP;
            if (line.StartsWith("HCP", StringComparison.InvariantCultureIgnoreCase))
                return Defines.FileType.HCP;
            if (line.StartsWith("SOP", StringComparison.InvariantCultureIgnoreCase))
                return Defines.FileType.SOP;
            if (line.StartsWith("TOUR", StringComparison.InvariantCultureIgnoreCase))
                return Defines.FileType.TOUR;

            throw new NotSupportedException();
        }

        private Defines.DisplayDataType ReadDisplayDataTypeFromLine(string sectionName, string line)
        {
            int index = line.IndexOf(':');
            if (index > 0)
                line = line.Substring(index + 1).Trim();

            if (line.StartsWith("TWOD_DISPLAY", StringComparison.InvariantCultureIgnoreCase))
                return Defines.DisplayDataType.Display2D;
            if (line.StartsWith("COORD_DISPLAY", StringComparison.InvariantCultureIgnoreCase))
                return Defines.DisplayDataType.Coordinates;
            if (line.StartsWith("NO_DISPLAY", StringComparison.InvariantCultureIgnoreCase))
                return Defines.DisplayDataType.NoDisplay;
            
            throw new NotSupportedException();
        }

        private Defines.EdgeDataFormat ReadEdgeDataFormatFromLine(string sectionName, string line)
        {
            int index = line.IndexOf(':');
            if (index > 0)
                line = line.Substring(index + 1).Trim();

            if (line.StartsWith("EDGE_LIST", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeDataFormat.EdgeList;
            if (line.StartsWith("ADJ_LIST", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeDataFormat.AdjacencyList;

            throw new NotSupportedException();
        }

        private Defines.EdgeWeightFormat ReadEdgeWeightFormatFromLine(string sectionName, string line)
        {
            int index = line.IndexOf(':');
            if (index > 0)
                line = line.Substring(index + 1).Trim();

            if (line.StartsWith("FUNCTION", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.Function;
            if (line.StartsWith("FULL_MATRIX", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.FullMatrix;

            if (line.StartsWith("UPPER_ROW", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.UpperRow;
            if (line.StartsWith("LOWER_ROW", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.LowerRow;
            if (line.StartsWith("UPPER_DIAG_ROW", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.UpperDiagonalRow;
            if (line.StartsWith("LOWER_DIAG_ROW", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.LowerDiagonalRow;

            if (line.StartsWith("UPPER_COL", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.UpperColumn;
            if (line.StartsWith("LOWER_COL", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.LowerColumn;
            if (line.StartsWith("UPPER_DIAG_COL", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.UpperDiagonalColumn;
            if (line.StartsWith("LOWER_DIAG_COL", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightFormat.LowerDiagonalColumn;

            throw new NotSupportedException();
        }

        private Defines.EdgeWeightType ReadEdgeWeightTypeFromLine(string sectionName, string line)
        {
            int index = line.IndexOf(':');
            if (index > 0)
                line = line.Substring(index + 1).Trim();

            if (line.StartsWith("EXPLICIT", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Explicit;
            if (line.StartsWith("EUC_2D", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Euclidean2D;
            if (line.StartsWith("EUC_3D", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Euclidean3D;
            if (line.StartsWith("MAX_2D", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Maximum2D;
            if (line.StartsWith("MAX_3D", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Maximum3D;
            if (line.StartsWith("MAN_2D", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Manhattan2D;
            if (line.StartsWith("MAN_3D", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Manhattan3D;
            if (line.StartsWith("CEIL_2D", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.EuclideanCeiled2D;
            if (line.StartsWith("GEO", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Geographical;
            if (line.StartsWith("ATT", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.PseudoEuclidean;
            if (line.StartsWith("XRAY1", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.XRay1;
            if (line.StartsWith("XRAY2", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.XRay2;
            if (line.StartsWith("SPECIAL", StringComparison.InvariantCultureIgnoreCase))
                return Defines.EdgeWeightType.Special;            

            throw new NotSupportedException();
        }

        private Defines.NodeCoordinatesType ReadNodeCoordinatesTypeFromLine(string sectionName, string line)
        {
            int index = line.IndexOf(':');
            if (index > 0)
                line = line.Substring(index + 1).Trim();

            if (line.StartsWith("TWOD_COORDS", StringComparison.InvariantCultureIgnoreCase))
                return Defines.NodeCoordinatesType.Coordinates2D;
            if (line.StartsWith("THREED_COORDS", StringComparison.InvariantCultureIgnoreCase))
                return Defines.NodeCoordinatesType.Coordinates3D;
            if (line.StartsWith("NO_COORDS", StringComparison.InvariantCultureIgnoreCase))
                return Defines.NodeCoordinatesType.NoCoordinates;

            throw new NotSupportedException();
        }
        
        private string ReadStringFromLine(string sectionName, string line)
        {
            int index = line.IndexOf(':');
            if (index > 0)
                line = line.Substring(index + 1).Trim();
            return line;
        }

        private int ReadIntFromLine(string sectionName, string line)
        {
            int index = line.IndexOf(':');
            if (index > 0)
                line = line.Substring(index + 1).Trim();
            return int.Parse(line);
        }

        private List<int> ReadIntList(string sectionName, string[] lines)
        {
            List<int> result = new List<int>();
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < lines.Length; i++)
            {
                builder.Append(lines[i]);
                builder.Append(' ');
            }

            foreach(string data in builder.ToString().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                int value = int.Parse(data);
                if (value == -1)
                    break;
                
                result.Add(value);
            }

            return result;
        }

        private List<double> ReadDoubleList(string sectionName, string[] lines)
        {
            List<double> result = new List<double>();
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < lines.Length; i++)
            {
                builder.Append(lines[i]);
                builder.Append(' ');
            }

            foreach (string data in builder.ToString().Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                double value = double.Parse(data);
                result.Add(value);
            }

            return result;
        }

        private List<int[]> ReadIntsArrayList(string sectionName, string[] lines)
        {
            List<int[]> result = new List<int[]>();
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < lines.Length; i++)
            {
                List<int> array = new List<int>();
                foreach (string data in lines[i].Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    array.Add(int.Parse(data));
                }

                result.Add(array.ToArray());
            }

            return result;
        }
        
        private List<object[]> ReadIntAndDoublesArrayList(string sectionName, string[] lines)
        {
            List<object[]> result = new List<object[]>();
            StringBuilder builder = new StringBuilder();
            for (int i = 1; i < lines.Length; i++)
            {
                List<object> array = new List<object>();
                string[] data = lines[i].Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                for (int j = 0; j < data.Length; j++)
                {
                    if (j == 0)
                    {
                        array.Add(int.Parse(data[j]));
                    }
                    else
                    {
                        array.Add(double.Parse(data[j]));
                    }
                }

                result.Add(array.ToArray());
            }

            return result;
        }        
    }
}
