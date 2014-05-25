namespace SpatialIndexesComparison.Services.Queries
{
    using System;
    using System.Configuration;
    using System.Globalization;

    using SpatialIndexesComparison.Enums;

    public class FindNearestNeighboursQuery : Query
    {
        private readonly Random _random;
        private readonly double _distance;
        private readonly int _numberOfPoints;
        private readonly bool _andOr;
        private readonly string _qgisPathPrefix;

        public FindNearestNeighboursQuery(IndexEnum index, DataSizeEnum dataSize, Random random, int numberOfPoints = 1, double distance = 0.1, bool andOr = false)
            :base(index, dataSize)
        {
            this._random = random;
            this._distance = distance;
            this._numberOfPoints = numberOfPoints;
            this._andOr = andOr;
            this._qgisPathPrefix = ConfigurationManager.AppSettings["QgisPathPrefix"];
        }

        public override double Execute(PostgreDbService service)
        {
            foreach (var value in Enum.GetValues(typeof(IndexEnum)))
            {
                var index = (IndexEnum)value;
                if (index == IndexEnum.rtree || index == IndexEnum.noindex) continue;
                service.CreateIndex(index, this._dataSize);
                service.DisableIndex(index, this._dataSize);
                if (index == this.Index) 
                    service.EnableIndex(this.Index, this._dataSize);
            }

            string commandText = @"" + (int)_dataSize;

            return service.ExecuteSqlCommand(commandText);
        }

        public override double Execute(QGisService service)
        {
            var lon = (_random.NextDouble() * 360 - 180).ToString(CultureInfo.GetCultureInfo("en-GB"));
            var lat = (_random.NextDouble() * 180 - 90).ToString(CultureInfo.GetCultureInfo("en-GB"));

            var script = @"#!/usr/bin/env Python

from qgis.core import *
import Queue
import timeit
import random

QgsApplication.setPrefixPath(""" + this._qgisPathPrefix + @""", True)
QgsApplication.initQgis()

uri = QgsDataSourceURI()
uri.setConnection('localhost', '5432', 'spatial_index_comparison', 'postgres', 'boss')
uri.setDataSource('public', 'random_points_" + (int)_dataSize + @"', 'geom')

layer = QgsVectorLayer(uri.uri(), 'test', 'postgres')

if not layer.isValid():
    print ""Layer failed to load!""
";

            return service.ExecuteScript(script);
        }
    }
}