    // using DotNetEnv;
using MongoDB.Driver;

namespace Persistence.Data.Mongo;

public class ChatDbContext
{

    // private readonly IMongoDatabase _database;
    private readonly string? _connectionString;
    private readonly string? _databaseName;

    // public MongoDbContext()
    // {
    //     // _connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
    //     // if (string.IsNullOrWhiteSpace(_connectionString))
    //     // {
    //     //     throw new InvalidOperationException("MongoDB connection string is not set.");
    //     // }

    //     // _databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");
    //     // if (string.IsNullOrWhiteSpace(_databaseName))
    //     // {
    //     //     throw new InvalidOperationException("MongoDB database name is not set.");
    //     // }

    //     // var client = new MongoClient(_connectionString);
    //     // _database = client.GetDatabase(_databaseName);
    // }

}

