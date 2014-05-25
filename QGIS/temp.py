from qgis.core import *
import Queue
import timeit
import random

QgsApplication.setPrefixPath("C:/Program Files/QGIS Valmiera/apps/qgis", True)
QgsApplication.initQgis()

uri = QgsDataSourceURI()
uri.setConnection('localhost', '5432', 'spatial_index_comparison', 'postgres', 'boss')
uri.setDataSource('public', 'random_points_1000000', 'geom')

layer = QgsVectorLayer(uri.uri(), 'test', 'postgres')

if not layer.isValid():
    print "Layer failed to load!"

distance = 1.5
lat = random.uniform(0, 360) - 180
lon = random.uniform(0, 180) - 90
point_geometry = QgsGeometry.fromPoint(QgsPoint(lat,lon))
first_point = QgsPoint(lat+distance, lon+distance)
second_point = QgsPoint(lat-distance, lon-distance)
index = QgsSpatialIndex()
for f in layer.getFeatures():
    index.insertFeature(f)

def rtreeindex_distanceToRandomPoint():
    q = Queue.Queue()
    ids = index.intersects(QgsRectangle(first_point, second_point))
    for id in ids:
        layer.getFeatures(QgsFeatureRequest().setFilterFid(id)).nextFeature(f)
        if f.geometry().distance(point_geometry) < distance:
            q.put(f)
print timeit.timeit(rtreeindex_distanceToRandomPoint,number=1)