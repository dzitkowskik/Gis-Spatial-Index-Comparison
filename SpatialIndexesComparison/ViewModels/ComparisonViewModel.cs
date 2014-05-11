namespace SpatialIndexesComparison.ViewModels
{
    using DotNet.Highcharts;

    public class ComparisonViewModel
    {
        public Highcharts Chart { get; set; }
        public int FirstIndex { get; set; }
        public int SecondIndex { get; set; }
        public bool AllIndexes { get; set; }
        public int Data { get; set; }
        public int Query { get; set; }
        public bool CustomSql { get; set; }
        public string SqlText { get; set; }

        public ComparisonViewModel(Highcharts chart)
        {
            Chart = chart;
            FirstIndex = 1;
            SecondIndex = 0;
            AllIndexes = false;
            Data = 0;
            Query = 0;
            CustomSql = false;
        }
    }
}