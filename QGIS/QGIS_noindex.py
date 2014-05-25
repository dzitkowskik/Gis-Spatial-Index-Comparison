layer = qgis.utils.iface.activeLayer()
provider = layer.dataProvider()
feat = QgsFeature()

def noindex_touches():
    for feature in layer.getFeatures():
        for f in layer.getFeatures():
            touches = f.geometry().touches(feature.geometry())

import Queue
class Job(object):
    def __init__(self, priority, geom):
        self.priority = priority
        self.geom = geom
        return
    def __cmp__(self, other):
        return cmp(self.priority, other.priority)
            
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

import timeit
print "Touches test without Index: %s seconds " % timeit.timeit(noindex_touches,number=1)
print "Nearest neighbours test without Index: %s seconds " % timeit.timeit(noindex_nearestNeighbours, number=1)