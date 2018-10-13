using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MongoSimpleQueue.Tests
{
	[TestClass]
	public class RealLifeTests
	{
		private int _totalConsumed;
		private Random _random = new Random(7564);

		[TestMethod]
		public async Task Test()
		{
			var queue = new SimpleQueue<SimpleDocument>(this.GetType().FullName);
			await queue.Clear();

			var producers = new List<Task>();

			for (int i = 0; i < 4; i++)
			{
				producers.Add(Task.Run(() => Produce(10)));
			}


			var consumers = new List<Task>();

			for (int i = 0; i < 8; i++)
				consumers.Add(Task.Run(Consume));


			await Task.WhenAll(producers.Concat(consumers));


			Assert.AreEqual(40, _totalConsumed);
		}

		private async Task Produce(int count)
		{
			var queue = new SimpleQueue<SimpleDocument>(this.GetType().FullName);
			for (int i = 0; i < count; i++)
			{

				await Task.Delay(_random.Next(250));
				var doc = new SimpleDocument() { Value = Guid.NewGuid().ToString().Substring(0, 6) };
				Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Produced: {doc}");

				await queue.Enqueue(doc);
			}
		}

		private async Task Consume()
		{
			var queue = new SimpleQueue<SimpleDocument>(this.GetType().FullName);
			while (_totalConsumed < 40)
			{
				var item = await queue.Dequeue();

				if (item != null)
				{
					Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId}] Consumed: {item.Payload}");
					Interlocked.Increment(ref _totalConsumed);
					await queue.Confirm(item);
				}

				//Very long work
				await Task.Delay(_random.Next(500));
			}

		}
	}
}