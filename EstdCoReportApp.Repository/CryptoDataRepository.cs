using EstdCoReportApp.Application.Domain;
using MongoDB.Driver;

namespace EstdCoReportApp.Repository
{
    public class MongoDbDataRepository<T>
    {
        private readonly IMongoCollection<T> _collection;

        public MongoDbDataRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task InsertAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            var entities = await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
            return entities;
        }

        // Add more CRUD operations as needed

        // For example:
        // public async Task<T> GetByIdAsync(string id)
        // {
        //     var entity = await _collection.Find(Builders<T>.Filter.Eq("_id", id)).FirstOrDefaultAsync();
        //     return entity;
        // }

        // public async Task UpdateAsync(string id, T entity)
        // {
        //     await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq("_id", id), entity);
        // }

        public async Task DeleteAsync(string id)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq("_id", id));
        }
    }
}