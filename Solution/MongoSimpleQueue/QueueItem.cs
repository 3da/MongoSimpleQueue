using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoSimpleQueue
{
	public class QueueItem<TPayload> : IQueueItem where TPayload : class
	{
		[BsonId]
		public ObjectId Id { get; set; }

		public byte Priority { get; set; }

		public DateTime? MustBeCompletedUntil { get; set; }

		public DateTime? StartAfterDateTime { get; set; }

		public TPayload Payload { get; set; }
	}
}
