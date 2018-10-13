using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace MongoSimpleQueue
{
	public class SimpleQueue<TPayload> where TPayload : class
	{
		private IMongoClient _mongoClient;
		private IMongoDatabase _dataBase;
		private IMongoCollection<QueueItem<TPayload>> _collection;

		private static UpdateDefinitionBuilder<QueueItem<TPayload>> _updateDefinitionBuilder
			= Builders<QueueItem<TPayload>>.Update;

		public DequeueOptions DefaultDequeueOptions { get; set; } = new DequeueOptions();

		public EnqueOptions DefaultEnqueueOptions { get; set; } = new EnqueOptions();

		public SimpleQueue(string collectionName)
		{
			_mongoClient = new MongoClient();

			_dataBase = _mongoClient.GetDatabase("database");

			_collection = _dataBase.GetCollection<QueueItem<TPayload>>(collectionName);

			_collection.Indexes.CreateOne(new CreateIndexModel<QueueItem<TPayload>>(Builders<QueueItem<TPayload>>.IndexKeys.Descending(q => q.Priority)));
		}

		public async Task Enqueue(TPayload payload, EnqueOptions options)
		{
			var item = new QueueItem<TPayload>()
			{
				Id = ObjectId.GenerateNewId(),
				Payload = payload,
				Priority = options.Priority,
				StartAfterDateTime = options.StartAfterDateTime
			};

			await _collection.InsertOneAsync(item);
		}

		public Task Enqueue(TPayload payload)
		{
			return Enqueue(payload, DefaultEnqueueOptions);
		}

		public async Task<TPayload> Dequeue(DequeueOptions dequeueOptions)
		{
			var now = DateTime.UtcNow;
			var time = DateTime.UtcNow.AddMinutes(-1);

			var updateOptions = new FindOneAndUpdateOptions<QueueItem<TPayload>, TPayload>()
			{
				Projection = Builders<QueueItem<TPayload>>.Projection.Expression(w => w.Payload),
				Sort = Builders<QueueItem<TPayload>>.Sort.Descending(q => q.Priority)
			};

			var queueItem = await _collection.FindOneAndUpdateAsync(q =>
			(q.LastWorkTime == null || q.LastWorkTime.Value < time)
			&& (q.StartAfterDateTime == null || q.StartAfterDateTime >= now),
				_updateDefinitionBuilder.Set(q => q.LastWorkTime, now), updateOptions);

			return queueItem;
		}

		public Task<TPayload> Dequeue()
		{
			return Dequeue(DefaultDequeueOptions);
		}

		public async Task Clear()
		{
			await _collection.DeleteManyAsync(q => true);
		}
	}
}
