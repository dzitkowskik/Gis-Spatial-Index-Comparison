#!/usr/bin/env Python

from qgis.core import *
import Queue
import timeit
import random

QgsApplication.setPrefixPath("C:/Program Files/QGIS Valmiera/apps/qgis", True)
QgsApplication.initQgis()

uri = QgsDataSourceURI()
uri.setConnection('localhost', '5432', 'spatial_index_comparison', 'postgres', 'boss')
uri.setDataSource('public', 'random_points_1000', 'geom')

layer = QgsVectorLayer(uri.uri(), 'test', 'postgres')

if not layer.isValid():
    print "Layer failed to load!"

def noindex_touches():
    for feature in layer.getFeatures():
        for f in layer.getFeatures():
            touches = f.geometry().touches(feature.geometry())


class Job(object):
    def __init__(self, priority, geom):
        self.priority = priority
        self.geom = geom
        return
    def __cmp__(self, other):
        return cmp(self.priority, other.priority)
            
def rtreeindex_distanceToRandomPoint():
    distance = 1.5
    lat = random.uniform(0, 360) - 180
    lon = random.uniform(0, 180) - 90
    point_geometry = QgsGeometry.fromPoint(QgsPoint(lat,lon))
    q = Queue.Queue()
    for f in layer.getFeatures():
        if f.geometry().distance(point_geometry) < distance:
            q.put(f)

def noindex_nearestNeighbours():
    k = 3
    for feature in layer.getFeatures():
        q = Queue.PriorityQueue(k)
        for f in layer.getFeatures():
            dist = feature.geometry().distance(f.geometry())
            if q.full():
                min = q.get()
                if(min.priority < dist):
                    q.put(Job(dist, f))
                else:
                    q.put(min)
            else:
                q.put(Job(dist, f))


time = timeit.timeit(noindex_touches,number=1)
print time
time2 = timeit.timeit(noindex_nearestNeighbours, number=1)
print time2
time3 = timeit.timeit(rtreeindex_distanceToRandomPoint, number=1)
print time3
QgsApplication.exitQgis()