using Microsoft.Azure.Cosmos.Table;

namespace QuizMaker.Data;

public class ConnectionEntity : TableEntity
{
    public ConnectionEntity()
    {
    }

    public ConnectionEntity(string partitionKey, string rowKey) : base(partitionKey, rowKey)
    {
    }

    public int Count { get; set; }
}
