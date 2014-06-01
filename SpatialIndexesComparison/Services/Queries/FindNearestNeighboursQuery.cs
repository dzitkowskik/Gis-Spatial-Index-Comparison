namespace SpatialIndexesComparison.Services.Queries
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.Web.UI.WebControls;

    using SpatialIndexesComparison.Enums;

    public class FindNearestNeighboursQuery : Query
    {
        private readonly Random _random;
        private readonly double _distance;
        private readonly int _numberOfPoints;
        private readonly bool _andOr;
        private readonly string _qgisPathPrefix;
        private readonly DataEnum _data;
        private readonly int _nearestNeighboursCount;

        public FindNearestNeighboursQuery
            (
            IndexEnum index, 
            DataEnum data,
            DataSizeEnum dataSize,
            Random random,
            int numberOfPoints = 1,
            double distance = 1,
            bool andOr = false,
            int nearestNeighboursCount = 5)
            :base(index, dataSize)
        {
            this._random = random;
            this._distance = distance;
            this._numberOfPoints = numberOfPoints;
            this._andOr = andOr;
            this._nearestNeighboursCount = nearestNeighboursCount;
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

            string commandText =
@"SELECT 1 
FROM " + tableName + @" AS A 
WHERE A.geom IN 
(
	SELECT B.geom
	FROM " + tableName + @" AS B
	ORDER BY A.geom <-> B.geom
	LIMIT " + _nearestNeighboursCount + @"
)";

            return service.ExecuteSqlCommand(commandText);
        }

        public override double Execute(QGisService service)
        {
            string tableName = this._data.ToString();
            tableName += (this._dataSize == DataSizeEnum.None ? string.Empty : "_" + (int)this._dataSize);

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

k = " + _nearestNeighboursCount + @"
index = QgsSpatialIndex()
for f in layer.getFeatures():
    index.insertFeature(f)

def rtreeindex_nearestNeighbours():
    for feature in layer.getFeatures():
        nearestIds = index.nearestNeighbor(feature.geometry().asPoint(), k)

print timeit.timeit(rtreeindex_nearestNeighbours, number=1)";

            return service.ExecuteScript(script);
        }
    }
}