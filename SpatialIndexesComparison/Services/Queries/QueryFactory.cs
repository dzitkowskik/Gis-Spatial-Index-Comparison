namespace SpatialIndexesComparison.Services.Queries
{
    using System;

    using SpatialIndexesComparison.Enums;

    public static class QueryFactory
    {
        public static Query Get(QueryEnum queryEnum, IndexEnum index, DataEnum data, DataSizeEnum size, Random random)
        {
            switch (queryEnum)
            {
                case QueryEnum.FindPointsNearRandomPoints:
                    return new FindPointsNearRandomPointsQuery(index, data, size, random);
                case QueryEnum.FindNearestNeighbours:
                    return new FindNearestNeighboursQuery(index, data, size, random);
                case QueryEnum.TouchesAllToAll:
                    return new TouchesAllToAllQuery(index, data, size, random);
            }
            return null;
        }
    }
}