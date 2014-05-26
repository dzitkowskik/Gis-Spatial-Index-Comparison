namespace SpatialIndexesComparison.ViewModels
{
    using DotNet.Highcharts;

    using SpatialIndexesComparison.Enums;

    public class ComparisonViewModel
    {
        public Highcharts Chart { get; set; }
        public IndexEnum FirstIndex { get; set; }
        public IndexEnum SecondIndex { get; set; }
        public bool AllIndexes { get; set; }
        public DataEnum Data { get; set; }
        public QueryEnum Query { get; set; }
        public int NumberOfQueries { get; set; }

        public ComparisonViewModel(Highcharts chart)
        {
            NumberOfQueries = 4;
            Chart = chart;
            FirstIndex = IndexEnum.gist;
            SecondIndex = IndexEnum.rtree;
            AllIndexes = false;
            Data = DataEnum.countries;
            Query = QueryEnum.FindNearestNeighbours;
        }
    }
}