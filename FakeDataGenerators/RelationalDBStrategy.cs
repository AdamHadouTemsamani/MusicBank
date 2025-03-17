public class RelationalDatabaseStrategy : IDatabaseStrategy
{
    private int _counter = 1;
    
    public object GenerateId()
    {
        return _counter++; // Auto-increment behavior
    }
}