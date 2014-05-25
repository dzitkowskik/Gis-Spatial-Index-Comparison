using System.Web.Mvc;

namespace SpatialIndexesComparison.Controllers
{
    using System;
    using System.Linq;

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

            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
                .SetTitle(
                    new Title
                    {
                        Text = "Comparison result",
                    })
                .SetXAxis(
                    new XAxis
                    {
                        Categories = new[] { "1k", "10k", "100k", "1M", "2M", "3M", "4M", "5M" }
                    })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Time (ms)" }, })
                .SetSeries(
                    new[]
                    {
                        this.GetSeries(QueryEnum.FindPointsNearRandomPoints, IndexEnum.gist, random, 1),
                        this.GetSeries(QueryEnum.FindPointsNearRandomPoints, IndexEnum.noindex, random, 1),
                        this.GetSeries(QueryEnum.FindPointsNearRandomPoints, IndexEnum.rtree, random, 1),
                        this.GetSeries(QueryEnum.FindPointsNearRandomPoints, IndexEnum.btree, random, 1),
                    });

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
                             this.GetSeries(viewModel.Query, viewModel.FirstIndex, random, viewModel.NumberOfQueries),
                             this.GetSeries(viewModel.Query, viewModel.SecondIndex, random, viewModel.NumberOfQueries),
                         };
            }
            else
            {
                series = Enum.GetValues(typeof(IndexEnum)).OfType<IndexEnum>()
                    .Select(t => this.GetSeries(viewModel.Query, t, random, viewModel.NumberOfQueries)).ToArray();
            }

            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
                .SetTitle(
                    new Title
                    {
                        Text = "Comparison result",
                    })
                .SetXAxis(
                    new XAxis
                    {
                        Categories = new[] { "10k", "100k", "1M", "2M", "3M", "4M", "5M" }
                    })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Time (ms)" }, })
                .SetSeries(series);

            viewModel.Chart = chart;

            return View(viewModel);
        }

        private Series GetSeries(QueryEnum queryEnum, IndexEnum index, Random random, int times)
        {
            return new Series
                   {
                       Name = index.ToString(),
                       Data = new Data(
                                                new object[]
                                                {
                                                    GetIndexSpeed(QueryFactory.Get(queryEnum, index, DataSizeEnum.Size1000, random), times),
                                                    GetIndexSpeed(QueryFactory.Get(queryEnum, index, DataSizeEnum.Size10000, random), times),
                                                    GetIndexSpeed(QueryFactory.Get(queryEnum, index, DataSizeEnum.Size100000, random), times),
                                                    GetIndexSpeed(QueryFactory.Get(queryEnum, index, DataSizeEnum.Size1000000, random), times),
                                                    GetIndexSpeed(QueryFactory.Get(queryEnum, index, DataSizeEnum.Size2000000, random), times),
                                                    GetIndexSpeed(QueryFactory.Get(queryEnum, index, DataSizeEnum.Size3000000, random), times),
                                                    /*GetIndexSpeed(QueryFactory.Get(queryEnum, index, DataSizeEnum.Size4000000, random), times),
                                                    GetIndexSpeed(QueryFactory.Get(queryEnum, index, DataSizeEnum.Size5000000, random), times)*/
                                                })
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
    }
}
