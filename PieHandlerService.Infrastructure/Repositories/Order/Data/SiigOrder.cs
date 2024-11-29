using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using PieHandlerService.Core.Models;

namespace PieHandlerService.Infrastructure.Repositories.Order.Data;

[Serializable, BsonIgnoreExtraElements]
public sealed class SiigOrder
{
    [BsonId, BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("mixnumber"), BsonRepresentation(BsonType.String)]
    public string? MixNumber { get; set; }

    [BsonElement("oeidentifier"), BsonRepresentation(BsonType.String)]
    public string? OeIdentifier { get; set; }

    [BsonElement("orderresponse"), BsonRepresentation(BsonType.String)]
    public string? OrderResponse { get; set; }

    [BsonElement("ordertype"), BsonRepresentation(BsonType.String)]
    public SIIGOrderType? OrderType { get; set; }

    [BsonElement("orderstatus"), BsonRepresentation(BsonType.String)]
    public string? OrderStatus { get; set; }

    [BsonElement("createdutcts")]
    public long CreatedUtcTs { get; set; }

    [BsonElement("modifiedutcts")]
    public long ModifiedUtcTs { get; set; }

}