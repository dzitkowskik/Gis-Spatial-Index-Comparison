layer = qgis.utils.iface.activeLayer()
allfeatures = layer.getFeatures()
f = QgsFeature()

def rtreeindex_touches():
    index = QgsSpatialIndex()
    for f in layer.getFeatures():
        index.insertFeature(f)
    for feature in layer.getFeatures():
        ids = index.intersects(feature.geometry().boundingBox())
        for id in ids:
            iter = layer.getFeatures(QgsFeatureRequest().setFilterFid(id)).nextFeature(f)
            if f == feature: continue
            touches = f.geometry().touches(feature.geometry())

def rtreeindex_nearestNeighbours():
    k = 3
    index = QgsSpatialIndex()
    for f in layer.getFeatures():
        index.insertFeature(f)
    for feature in layer.getFeatures():
        nearestIds = index.nearestNeighbor(feature.geometry().asPoint(), k)
    
import timeit
print "Touches with rtree index: %s seconds " % timeit.timeit(withindex,number=1)
print "Nearest neighbours test with rtree index: %s seconds " % timeit.timeit(rtreeindex_nearestNeighbours, number=1)