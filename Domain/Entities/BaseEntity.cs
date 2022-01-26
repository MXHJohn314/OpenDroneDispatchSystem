using System;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class BaseEntity
{
    private const string chars = "abcdef0123456789";
    private const int IdLength = 24;

    private static readonly Random _random = new();

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
    public static string GenerateNewId()
    {
        return new string(Enumerable.Repeat(chars, IdLength)
            .Select(s => s[_random.Next(s.Length)]).ToArray());
    }
}