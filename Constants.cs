using MongoDB.Bson;
using MongoDB.Driver;

public class constants
{
    public static string botId = "6271901154:AAGQxey2sFyiuprU0Vti44Vq8KRqBJkcZGk";
    public static string host = "racebtapi20230529230204.azurewebsites.net";
    public static MongoClient mongoClient;
    public static IMongoDatabase database;
    public static IMongoCollection<BsonDocument> collection;
}