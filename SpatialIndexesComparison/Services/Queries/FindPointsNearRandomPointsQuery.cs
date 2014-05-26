namespace SpatialIndexesComparison.Services.Queries
{
    using System;
    using System.Configuration;
    using System.Globalization;

    using SpatialIndexesComparison.Enums;

    public class FindPointsNearRandomPointsQuery : Query
    {
        private readonly Random _random;
        private readonly double _distance;
        private readonly int _numberOfPoints;
        private readonly bool _andOr;
        private readonly DataEnum _data;
        private readonly string _qgisPathPrefix;

        public FindPointsNearRandomPointsQuery
            (
            IndexEnum index, 
            DataEnum data, 
            DataSizeEnum dataSize,
            Random random,
            int numberOfPoints = 1,
            double distance = 1,
            bool andOr = false
            )
            :base(index, dataSize)
        {
            this._random = random;
            this._distance = distance;
            this._numberOfPoints = numberOfPoints;
            this._andOr = andOr;
            this._data = data;
            this._qgisPathPrefix = ConfigurationManager.AppSettings["QgisPathPrefix"];
        }

        public override double Execute(PostgreDbService service)
        {
            foreach (var value in Enum.GetValues(typeof(IndexEnum)))
            {
                var index = (IndexEnum)value;
                if (index == IndexEnum.rtree || index == IndexEnum.noindex) continue;
                service.DisableIndex(index, this._dataSize, this._data);
                if (index == this.Index)
                    service.EnableIndex(this.Index, this._dataSize, this._data);
            }

            string tableName = this._data.ToString();
            tableName += (this._dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)this._dataSize);

            string commandText = @"SELECT * FROM " + tableName;
            if (_numberOfPoints > 0)
                commandText += " WHERE ";
            for (int i = 0; i < _numberOfPoints; i++)
            {
                var lon = _random.NextDouble() * 360 - 180;
                var lat = _random.NextDouble() * 180 - 90;
                if (i != 0) commandText += _andOr ? " AND " : " OR ";
                commandText +=
                    "ST_DWithin(geom, ST_SetSRID(ST_MakePoint(" +
                    lon.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                    ", " +
                    lat.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                    "), 4326), " +
                    _distance.ToString(CultureInfo.GetCultureInfo("en-GB")) +
                    ")";
            }

            return service.ExecuteSqlCommand(commandText);
        }

        public override double Execute(QGisService service)
        {
            string tableName = this._data.ToString();
            tableName += (this._dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)this._dataSize);

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
uri.setDataSource('public', '" + tableName + @"', 'geom')

layer = QgsVectorLayer(uri.uri(), 'test', 'postgres')

if not layer.isValid():
    print ""Layer failed to load!""

lon = " + lon + @"
lat = " + lat + @"
distance = " + _distance.ToString(CultureInfo.GetCultureInfo("en-GB")) + @"
point_geometry = QgsGeometry.fromPoint(QgsPoint(lon, lat))
first_point = QgsPoint(lat+distance, lon+distance)
second_point = QgsPoint(lat-distance, lon-distance)
index = QgsSpatialIndex()
for f in layer.getFeatures():
    index.insertFeature(f)
q = Queue.Queue()

def rtreeindex_distanceToRandomPoint():
    ids = index.intersects(QgsRectangle(first_point, second_point))
    for id in ids:
        layer.getFeatures(QgsFeatureRequest().setFilterFid(id)).nextFeature(f)
        if f.geometry().distance(point_geometry) < distance:
            q.put(f)

print timeit.timeit(rtreeindex_distanceToRandomPoint, number=1)";

            return service.ExecuteScript(script); ;
        }
    }
}