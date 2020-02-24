using BONLINE_DT.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace BONLINE_DT.Data
{
    public class DbContext : IDbContext
    {
        private readonly IMongoDatabase _db;

        public DbContext(IOptions<DatabaseSettings> options)
        {
            var client = new MongoClient(options.Value.ConnectionString);
            _db = client.GetDatabase(options.Value.DatabaseName);
        }
        
        public IMongoCollection<DocumentModel> Documents => _db.GetCollection<DocumentModel>("{CollectionName}");
    }
    
    public interface IDbContext
    {
        IMongoCollection<DocumentModel> Documents { get; }
    }
}