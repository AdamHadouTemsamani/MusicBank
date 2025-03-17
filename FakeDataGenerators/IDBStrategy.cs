using MongoDB.Bson;

public interface IDatabaseStrategy
{
    object GenerateId();
}
