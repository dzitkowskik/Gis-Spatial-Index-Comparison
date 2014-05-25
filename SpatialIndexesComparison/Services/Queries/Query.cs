namespace SpatialIndexesComparison.Services.Queries
{
    using SpatialIndexesComparison.Enums;

    public abstract class Query
    {
        public IndexEnum Index { get; set; }
        protected readonly DataSizeEnum _dataSize;

        protected Query(IndexEnum index, DataSizeEnum dataSize)
        {
            this.Index = index;
            this._dataSize = dataSize;
        }

        /// <summary>
        /// Executes query on DB using PostgreDbSecvice
        /// </summary>
        /// <returns>Time of query execution</returns>
        public abstract double Execute(PostgreDbService service);

        /// <summary>
        /// Executes query on data from PostGis Db using QGis implemented in python  
        /// </summary>
        /// <param name="service"></param>
        /// <returns>Time of query execution</returns>
        public abstract double Execute(QGisService service);
    }
}