using System.Web.Mvc;

namespace SpatialIndexesComparison.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DotNet.Highcharts.Enums;
    using DotNet.Highcharts.Helpers;
    using DotNet.Highcharts.Options;

    using SpatialIndexesComparison.Enums;
    using SpatialIndexesComparison.Services;
    using SpatialIndexesComparison.Services.Queries;
    using SpatialIndexesComparison.ViewModels;

    public class GisIndexController : Controller
    {
        public ActionResult Index()
        {
            var random = new Random();

            var series = new[]
                    {
                        this.GetSeries(QueryEnum.FindNearestNeighbours, IndexEnum.gist, random, 4, DataEnum.countries),
                        this.GetSeries(QueryEnum.FindNearestNeighbours, IndexEnum.rtree, random, 4, DataEnum.countries),
                    };

            DotNet.Highcharts.Highcharts chart = this.GetChart(series, DataEnum.countries);

            return View(new ComparisonViewModel(chart));
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost()
        {
            var random = new Random();
            var viewModel = new ComparisonViewModel(null);
            TryUpdateModel(viewModel);

            Series[] series;

            if (!viewModel.AllIndexes)
            {
                series = new[]
                         {
                             this.GetSeries(viewModel.Query, viewModel.FirstIndex, random, viewModel.NumberOfQueries, viewModel.Data),
                             this.GetSeries(viewModel.Query, viewModel.SecondIndex, random, viewModel.NumberOfQueries, viewModel.Data),
                         };
            }
            else
            {
                series = Enum.GetValues(typeof(IndexEnum)).OfType<IndexEnum>()
                    .Select(t => this.GetSeries(viewModel.Query, t, random, viewModel.NumberOfQueries, viewModel.Data)).ToArray();
            }

            DotNet.Highcharts.Highcharts chart = this.GetChart(series, viewModel.Data);

            viewModel.Chart = chart;

            var report = (Session["report"] as List<ReportViewModel>) ?? new List<ReportViewModel>();
            report.Add(new ReportViewModel(viewModel, series));
            Session["report"] = report;

            return View(viewModel);
        }

        private DotNet.Highcharts.Highcharts GetChart(Series[] series, DataEnum data)
        {
            Chart chart = null;
            XAxis xAxis = null;
            PlotOptions options = null;
            switch (data)
            {
                case DataEnum.random_points:
                    xAxis = new XAxis { Categories = new[] { "50", "100", "500", "1k", "5k", "10k", "50k", "100k", "500k", "1M", "2M", "3M", "4M", "5M" } };
                    options = new PlotOptions();
                    chart = new Chart();
                    break;
                case DataEnum.countries:
                    xAxis = new XAxis { Categories = series.Select(t => t.Name).ToArray() };
                    options = new PlotOptions { Column = new PlotOptionsColumn { Stacking = Stackings.Normal } };
                    chart = new Chart { DefaultSeriesType = ChartTypes.Column };
                    
                    // ReSharper disable once PossibleNullReferenceException
                    var results = new object[series.Length];
                    for (int i = 0; i < series.Length; i++)
                        results[i] = series.FirstOrDefault(x => x.Name.Equals(series[i].Name)).Data.ArrayData[0];
                    series = new[]{ new Series{ Name = data.ToString(), Data = new Data(results)} };

                    break;
            }
            
            return new DotNet.Highcharts.Highcharts("chart")
                .InitChart(chart)
                .SetTitle(new Title{ Text = "Comparison result" })
                .SetXAxis(xAxis)
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Time (ms)" }, })
                .SetSeries(series)
                .SetPlotOptions(options);
        }

        private Series GetSeries(QueryEnum queryEnum, IndexEnum index, Random random, int times, DataEnum dataEnum)
        {
            var data = new List<object>();
            if (dataEnum == DataEnum.countries)
            {
                var speed = GetIndexSpeed(QueryFactory.Get(queryEnum, index, dataEnum, DataSizeEnum.None, random), times);
                data.Add(speed);
            }
            else
            {
                foreach (var value in Enum.GetValues(typeof(DataSizeEnum)))
                {
                    if((DataSizeEnum)value == DataSizeEnum.None) continue;
                    var speed = GetIndexSpeed(QueryFactory.Get(queryEnum, index, dataEnum, (DataSizeEnum)value, random), times);
                    if (speed.Equals(-1.0d))
                        break;
                    data.Add(speed);
                }
            }

            return new Series
                   {
                       Name = index.ToString(),
                       Data = new Data(data.ToArray())
                   };
        }

        private double GetIndexSpeed(Query query, int times)
        {
            double result = 0.0d;
            //if (times < 3) times = 3;
            double max = double.MinValue;
            double min = double.MaxValue;

            if (query.Index == IndexEnum.rtree)
            {
                using (var service = new QGisService())
                {
                    for (int i = 0; i < times; i++)
                    {
                        var speed = query.Execute(service);
                        if (speed.Equals(-1.0d)) 
                            return -1.0d;
                        speed *= 1000; // to miliseconds
                        if (speed < min) min = speed;
                        if (speed > max) max = speed;
                        result += speed;
                    }
                }
            }
            else
            {
                using (var service = new PostgreDbService())
                {
                    for (int i = 0; i < times; i++)
                    {
                        var speed = query.Execute(service);
                        if (speed.Equals(-1.0d)) 
                            return -1.0d;
                        if (speed < min) min = speed;
                        if (speed > max) max = speed;
                        result += speed;
                    }
                }
            }
            if (times >= 3)
            {
                result -= min;
                result -= max;
            }
            return result / (times - (times >= 3 ? 2.0d : 0.0d));
        }

        public ActionResult Report()
        {
            var report = Session["report"] as List<ReportViewModel>;
            if(report != null)
                return View(report);
            return RedirectToAction("Index");
        }

        public ActionResult Clear()
        {
            Session["report"] = new List<ReportViewModel>();
            return RedirectToAction("Index");
        }
    }
}
