namespace SpatialIndexesComparison.Services.Queries
{
    using System;

    using SpatialIndexesComparison.Enums;

    public static class QueryFactory
    {
        public static Query Get(QueryEnum queryEnum, IndexEnum index, DataSizeEnum size, Random random)
        {
            switch (queryEnum)
            {
                case QueryEnum.FindPointsNearRandomPoints:
                    return new FindPointsNearRandomPointsQuery(index, size, random);
            }
            return null;
        }
    }
}