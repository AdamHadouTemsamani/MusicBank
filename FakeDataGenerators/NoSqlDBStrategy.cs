public class NoSqlDatabaseStrategy : IDatabaseStrategy
{
    public object GenerateId()
    {
        return ObjectId.GenerateNewId();
    }
}