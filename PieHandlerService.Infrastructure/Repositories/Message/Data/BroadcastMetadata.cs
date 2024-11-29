using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PieHandlerService.Infrastructure.Repositories.Message.Data;

[Serializable, BsonIgnoreExtraElements]
public sealed class BroadcastMetadata
{
    [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("oeidentifier"), BsonRepresentation(BsonType.String)]
    public string? OeId { get; set; }

    [BsonElement("mixnumber"), BsonRepresentation(BsonType.String)]
    public string? MixNumber { get; set; }

    [BsonElement("status"), BsonRepresentation(BsonType.String)]
    public string? Status { get; set; }

    [BsonElement("createdutcts"), BsonRepresentation(BsonType.Int64)]
    public long? CreatedUtcMs { get; set; }

    [BsonElement("modifiedutcts"), BsonRepresentation(BsonType.Int64)]
    public long? ModifiedUtcMs { get; set; }

    [BsonElement("requesttype"), BsonRepresentation(BsonType.String)]
    public string? RequestType { get; set; }

    [BsonElement("filename"), BsonRepresentation(BsonType.String)]
    public string? FileName { get; set; }

    [BsonElement("contenthash"), BsonRepresentation(BsonType.String)]
    public string? ContentHash { get; set; }

    [BsonElement("originhash"), BsonRepresentation(BsonType.String)]
    public string? OriginHash { get; set; }

    [BsonElement("ispriority")]
    public bool? IsPriority { get; set; }
}