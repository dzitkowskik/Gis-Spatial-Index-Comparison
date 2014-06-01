#!/usr/bin/env Python

from qgis.core import *
import Queue
import timeit
import random

QgsApplication.setPrefixPath("C:/Program Files/QGIS Valmiera/apps/qgis", True)
QgsApplication.initQgis()

uri = QgsDataSourceURI()
uri.setConnection('localhost', '5432', 'spatial_index_comparison', 'postgres', 'boss')
uri.setDataSource('public', 'random_points_5000000', 'geom')

layer = QgsVectorLayer(uri.uri(), 'test', 'postgres')

if not layer.isValid():
    print "Layer failed to load!"



def CreateIndex():
	index = QgsSpatialIndex()
	for f in layer.getFeatures():
		index.insertFeature(f)
		
print timeit.timeit(CreateIndex,number=1)