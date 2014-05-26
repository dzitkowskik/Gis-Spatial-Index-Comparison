namespace SpatialIndexesComparison.ViewModels
{
    using System;
    using System.Collections.Generic;

    using DotNet.Highcharts.Options;

    using SpatialIndexesComparison.Enums;

    [Serializable]
    public class ReportViewModel
    {
        public DataEnum Data { get; set; }
        public QueryEnum Query { get; set; }
        public List<TestResult> Results { get; set; } 

        public ReportViewModel(ComparisonViewModel viewModel, Series[] series)
        {
            Data = viewModel.Data;
            Query = viewModel.Query;
            Results = new List<TestResult>();
            foreach (var s in series)
            {
                var index = (IndexEnum)Enum.Parse(typeof(IndexEnum), s.Name);
                for(int i = 0; i < s.Data.ArrayData.Length; i++)
                {
                    var size = DataSizeEnum.None;
                    if (Data != DataEnum.countries)
                        size = (DataSizeEnum)Enum.GetValues(typeof(DataSizeEnum)).GetValue(i+1);
                    Results.Add(new TestResult
                                {
                                    Result = (double)s.Data.ArrayData[i],
                                    Index = index,
                                    DataSize = size
                                });
                }
            }
        }
    }

    [Serializable]
    public class TestResult
    {
        public DataSizeEnum DataSize { get; set; }
        public IndexEnum Index { get; set; }
        public double Result { get; set; }
    }
}