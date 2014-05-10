using System.Web.Mvc;

namespace SpatialIndexesComparison.Controllers
{
    using DotNet.Highcharts.Helpers;
    using DotNet.Highcharts.Options;

    using SpatialIndexesComparison.Services;

    public class GisIndexController : Controller
    {
        public ActionResult Index()
        {
            DotNet.Highcharts.Highcharts chart = new DotNet.Highcharts.Highcharts("chart")
                .SetTitle(new Title
                {
                    Text = "Comparison result",
                })
                .SetXAxis(
                    new XAxis
                    {
                        Categories = new[] { "10k", "100k", "1M", "2M", "3M", "4M", "5M" }
                    })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Time (s)" }, })
                .SetSeries(
                    new []
                    {
                        new Series{Name = "No index", Data = new Data(new object[]
                                                                      {
                                                                          GetIndexSpeed("noindex", 1, true, 1, 10000, 1),
                                                                          GetIndexSpeed("noindex", 1, true, 1, 100000, 1),
                                                                          GetIndexSpeed("noindex", 1, true, 1, 1000000, 1),
                                                                          GetIndexSpeed("noindex", 1, true, 1, 2000000, 1),
                                                                          GetIndexSpeed("noindex", 1, true, 1, 3000000, 1),
                                                                          GetIndexSpeed("noindex", 1, true, 1, 4000000, 1),
                                                                          GetIndexSpeed("noindex", 1, true, 1, 5000000, 1),
                                                                      })},
                        new Series{Name = "GIST index", Data = new Data(new object[]
                                                                        {
                                                                            GetIndexSpeed("gistindex", 1, true, 1, 10000, 1),
                                                                            GetIndexSpeed("gistindex", 1, true, 1, 100000, 1),
                                                                            GetIndexSpeed("gistindex", 1, true, 1, 1000000, 1),
                                                                            GetIndexSpeed("gistindex", 1, true, 1, 2000000, 1),
                                                                            GetIndexSpeed("gistindex", 1, true, 1, 3000000, 1),
                                                                            GetIndexSpeed("gistindex", 1, true, 1, 4000000, 1),
                                                                            GetIndexSpeed("gistindex", 1, true, 1, 5000000, 1),
                                                                        })},
                    });

            return View(chart);
        }

        private double GetIndexSpeed(string nameOfIndex, int conditionNumber, bool andOr, double distance, int limit, int times)
        {
            var service = new PostgreDbService();
            var result = 0.0d;
            for (int i = 0; i < times; i++)
                result += service.FirstSelect(nameOfIndex, conditionNumber, andOr, distance, limit);
            return result/times;
        }
    }
}
