using System.Web.Mvc;

namespace SpatialIndexesComparison.Controllers
{
    using DotNet.Highcharts.Helpers;
    using DotNet.Highcharts.Options;

    using SpatialIndexesComparison.Enums;
    using SpatialIndexesComparison.Services;
    using SpatialIndexesComparison.ViewModels;

    public class GisIndexController : Controller
    {
        public ActionResult Index()
        {
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
                .SetSeries(
                    new[]
                    {
                        this.GetSeries("gistindex", 3, false, 3, 10),
                        this.GetSeries("noindex", 3, false, 3, 10),
                    });

            return View(new ComparisonViewModel(chart));
        }

        [HttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost()
        {
            var viewModel = new ComparisonViewModel(null);

            TryUpdateModel(viewModel);
            
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
                .SetSeries(
                    new[]
                    {
                        this.GetSeries(((IndexEnum)viewModel.FirstIndex).ToString(), 3, false, 3, 10),
                        this.GetSeries(((IndexEnum)viewModel.SecondIndex).ToString(), 3, false, 3, 10),
                    });
            viewModel.Chart = chart;
            return View(viewModel);
        }

        private Series GetSeries(string nameOfIndex, int conditionNumber, bool andOr, double distance, int times)
        {
            return new Series
                   {
                       Name = nameOfIndex,
                       Data = new Data(
                                                new object[]
                                                {
                                                    GetIndexSpeed(nameOfIndex, conditionNumber, andOr, distance, 10000, times),
                                                    GetIndexSpeed(nameOfIndex, conditionNumber, andOr, distance, 100000, times),
                                                    GetIndexSpeed(nameOfIndex, conditionNumber, andOr, distance, 1000000, times),
                                                    GetIndexSpeed(nameOfIndex, conditionNumber, andOr, distance, 2000000, times),
                                                    GetIndexSpeed(nameOfIndex, conditionNumber, andOr, distance, 3000000, times),
                                                    GetIndexSpeed(nameOfIndex, conditionNumber, andOr, distance, 4000000, times),
                                                    GetIndexSpeed(nameOfIndex, conditionNumber, andOr, distance, 5000000, times),
                                                })
                   };
        }

        private double GetIndexSpeed(string nameOfIndex, int conditionNumber, bool andOr, double distance, int limit, int times)
        {
            var service = new PostgreDbService();
            double result = 0.0d;

            if (times < 3) times = 3;

            int max = int.MinValue;
            int min = int.MaxValue;

            for (int i = 0; i < times; i++)
            {
                var speed = service.FirstSelect(nameOfIndex, conditionNumber, andOr, distance, limit);
                if (speed < min) min = speed;
                if (speed > max) max = speed;
                result += speed;
            }
            result -= min;
            result -= max;
            return result / (times - 2.0d);
        }
    }
}
