using System.Web.Mvc;

namespace SpatialIndexesComparison.Controllers
{
    using DotNet.Highcharts.Helpers;
    using DotNet.Highcharts.Options;

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
                        Categories = new[] { "10", "100", "1000", "10000", "100000", "500000", "1000000" }
                    })
                .SetYAxis(new YAxis { Title = new YAxisTitle { Text = "Time (s)" }, })
                .SetSeries(
                    new []
                    {
                        new Series{Name = "GIST", Data = new Data(new object[] { 0.01, 0.03, 0.9, 2.0, 5, 15, 30 })},
                        new Series{Name = "SP-GIST", Data = new Data(new object[] { 0.01, 0.02, 0.1, 0.7, 1.3, 7.0, 20 })},
                    });

            return View(chart);
        }
    }
}
