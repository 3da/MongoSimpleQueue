using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MongoSimpleQueue.Tests
{
	[TestClass]
	public class SimpleTests
	{
		[TestMethod]
		public async Task TestEnqueueDeque()
		{
			var queue = new SimpleQueue<SimpleDocument>(this.GetType().FullName);

			await queue.Clear();

			var simpleStrings = new List<string>();

			for (int i = 0; i < 100; i++)
			{
				simpleStrings.Add(Guid.NewGuid().ToString().Replace("-", ""));
			}

			var documents = simpleStrings.Select(q => new SimpleDocument() { Value = q });


			await Task.WhenAll(documents.Select(simpleDocument => queue.Enqueue(simpleDocument, new EnqueOptions())));

			for (int i = 0; i < 100; i++)
			{
				Assert.IsNotNull(await queue.Dequeue(new DequeueOptions()));
			}
			var document = await queue.Dequeue(new DequeueOptions());

			Assert.IsNull(document);

		}

		[TestMethod]
		public async Task TestPriority()
		{
			var rand = new Random();

			var priorities = Enumerable.Range(1, 200).OrderBy(q => rand.Next()).ToList();

			var queue = new SimpleQueue<SimpleDocument>(this.GetType().FullName);

			await queue.Clear();

			foreach (var priority in priorities)
			{
				await queue.Enqueue(
					new SimpleDocument()
					{
						Value = priority.ToString()
					},
					new EnqueOptions()
					{
						Priority = (byte)priority
					});

			}

			for (byte i = 200; i >= 1; i--)
			{
				Assert.AreEqual(i.ToString(), (await queue.Dequeue()).Value);
			}


		}

		[TestMethod]
		public async Task TestNoPriority()
		{

			var queue = new SimpleQueue<SimpleDocument>(this.GetType().FullName);

			await queue.Clear();

			for (int i = 0; i < 100; i++)
			{
				await queue.Enqueue(
					new SimpleDocument()
					{
						Value = i.ToString()
					});

			}


			for (int i = 0; i < 100; i++)
			{
				Assert.AreEqual(i.ToString(), (await queue.Dequeue()).Value);
			}


		}
	}
}
