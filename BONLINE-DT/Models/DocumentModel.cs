using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BONLINE_DT.Models
{
    public class DocumentModel
    {
        [BsonId]
        [BsonElement("_id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("createDate")]
        [DataType(DataType.Date)]
        public DateTime CreateDate { get; set; }

        [BsonIgnore]
        public string DocumentCreateDateLocal => !string.IsNullOrEmpty(Document.PostDate.ToString()) ? Document.PostDate.ToString("dd-MM-yyyy", new CultureInfo("ka-GE").DateTimeFormat) : null;
        
        [BsonElement("document")]
        public Document Document { get; set; }
        
        [BsonElement("isRead")]
        public bool IsRead { get; set; }
    }
    public class Sender
    {
        public object Name { get; set; }
        public object Inn { get; set; }
        public object AccountNumber { get; set; }
        public object BankCode { get; set; }
        public object BankName { get; set; }
    }

    public class Beneficiary
    {
        public string Name { get; set; }
        public object Inn { get; set; }
        public object AccountNumber { get; set; }
        public object BankCode { get; set; }
        public object BankName { get; set; }
    }

    public class Document
    {
        [BsonElement("Id")]
        public long DocId { get; set; }
        public long DocKey { get; set; }
        public string DocNo { get; set; }
        [DataType(DataType.Date)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime PostDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime ValueDate { get; set; }
        public string EntryType { get; set; }
        public string EntryComment { get; set; }
        public string EntryCommentEn { get; set; }
        public double Credit { get; set; }
        public double Debit { get; set; }
        public double Amount { get; set; }
        public double AmountBase { get; set; }
        public object PayerName { get; set; }
        public object PayerInn { get; set; }
        public Sender Sender { get; set; }
        public Beneficiary Beneficiary { get; set; }
    }
}