﻿namespace TspLibNet.Problems
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using TspLibNet.Graph;
    using TspLibNet.Tours;
    using TspLibNet.Graph.Nodes;
    using TspLibNet.Graph.Edges;
    using TspLibNet.Graph.EdgeWeights;
    using TspLibNet.Graph.FixedEdges;
    using TspLibNet.Graph.Depots;
    using TspLibNet.Graph.Demand;
    using TspLibNet.TSP;

    /// <summary>
    /// Symmetric Traveling Salesman Problem
    /// </summary>
    public class TravelingSalesmanProblem : ProblemBase
    {
        public TravelingSalesmanProblem(string name, string comment, INodeProvider nodeProvider, IEdgeProvider edgeProvider, IEdgeWeightsProvider edgeWeightsProvider, IFixedEdgesProvider fixedEdgesProvider)
            : base(name, comment, nodeProvider, edgeProvider, edgeWeightsProvider, fixedEdgesProvider)
        {
        }

        public static TravelingSalesmanProblem FromTspFile(TspFile tspFile)
        {
            if (tspFile.Type != TSP.Defines.FileType.TSP && tspFile.Type != TSP.Defines.FileType.ATSP)
            {
                throw new ArgumentOutOfRangeException("tspFile");
            }

            TspFileDataExtractor extractor = new TspFileDataExtractor(tspFile);
            var nodeProvider = extractor.MakeNodeProvider();
            var nodes = nodeProvider.GetNodes();
            var edgeProvider = extractor.MakeEdgeProvider(nodes);
            var edgeWeightsProvider = extractor.MakeEdgeWeightsProvider();
            var fixedEdgesProvider = extractor.MakeFixedEdgesProvider(nodes);
            return new TravelingSalesmanProblem(tspFile.Name, tspFile.Comment, nodeProvider, edgeProvider, edgeWeightsProvider, fixedEdgesProvider);
        }

        /// <summary>
        /// Validate given solution
        /// </summary>
        /// <param name="tour">Tour to check</param>
        /// <param name="errors">utputs list of errors found in tour</param>
        /// <returns>Whether tour is valid</returns>
        public override bool ValidateTour(ITour tour, out string[] errors)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets tour distance for a given problem
        /// </summary>
        /// <param name="tour">Tour to check</param>
        /// <returns>Tour distance</returns>
        public override double TourDistance(ITour tour)
        {
            throw new NotImplementedException();
        }
    }
}
