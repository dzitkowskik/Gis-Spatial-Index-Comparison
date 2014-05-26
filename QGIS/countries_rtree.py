#!/usr/bin/env Python

from qgis.core import *
import Queue
import timeit
import random
import pickle
import os.path

QgsApplication.setPrefixPath("C:/Program Files/QGIS Valmiera/apps/qgis", True)
QgsApplication.initQgis()

uri = QgsDataSourceURI()
uri.setConnection('localhost', '5432', 'spatial_index_comparison', 'postgres', 'boss')
uri.setDataSource('public', 'countries', 'geom')

layer = QgsVectorLayer(uri.uri(), 'test', 'postgres')

if not layer.isValid():
    print "Layer failed to load!"
	
index = QgsSpatialIndex()
for f in layer.getFeatures():
	index.insertFeature(f)

print 'Index created!'	

def rtreeindex_touches():
	i = 1
	for feature in layer.getFeatures():
		ids = index.intersects(feature.geometry().boundingBox())
		print i
		i = i+1
		for id in ids:
			iter = layer.getFeatures(QgsFeatureRequest().setFilterFid(id)).nextFeature(f)
			touches = f.geometry().touches(feature.geometry())
			
def rtreeindex_distanceToRandomPoint():
	distance = 1.5
	lat = random.uniform(0, 360) - 180
	lon = random.uniform(0, 180) - 90
	point_geometry = QgsGeometry.fromPoint(QgsPoint(lat,lon))
	first_point = QgsPoint(lat+distance, lon+distance)
	second_point = QgsPoint(lat-distance, lon-distance)
	q = Queue.Queue()
	ids = index.intersects(QgsRectangle(first_point, second_point))
	for id in ids:
		layer.getFeatures(QgsFeatureRequest().setFilterFid(id)).nextFeature(f)
		if f.geometry().distance(point_geometry) < distance:
			q.put(f) 

def rtreeindex_nearestNeighbours():
    k = 5
    for feature in layer.getFeatures():
        nearestIds = index.nearestNeighbor(feature.geometry().asPoint(), k)
    
print "Touches with rtree index: %s seconds " % timeit.timeit(rtreeindex_touches,number=1)
#print "Nearest neighbours test with rtree index: %s seconds " % timeit.timeit(rtreeindex_nearestNeighbours, number=1)
#print "Distance to random point test with rtree index: %s seconds " % timeit.timeit(rtreeindex_distanceToRandomPoint, number=1)