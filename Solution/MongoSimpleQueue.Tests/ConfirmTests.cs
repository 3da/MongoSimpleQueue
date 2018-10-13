using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MongoSimpleQueue.Tests
{
	[TestClass]
	public class ConfirmTests
	{
		[TestMethod]
		public async Task TestNotConfirmed()
		{
			var queue = new SimpleQueue<SimpleDocument>(GetType().FullName);
			await queue.Clear();
			await queue.Enqueue(new SimpleDocument()
			{
				Value = "test"
			});

			var value = await queue.Dequeue(new DequeueOptions()
			{
				ConfirmTime = TimeSpan.FromMilliseconds(1000)
			});

			Assert.AreEqual("test", value.Payload.Value);

			value = await queue.Dequeue();

			Assert.IsNull(value);

			await Task.Delay(1000);

			value = await queue.Dequeue();

			Assert.IsNotNull(value);
			Assert.AreEqual("test", value.Payload.Value);

		}

		[TestMethod]
		public async Task TestConfirmed()
		{
			var queue = new SimpleQueue<SimpleDocument>(GetType().FullName);
			await queue.Clear();
			await queue.Enqueue(new SimpleDocument()
			{
				Value = "test"
			});

			var value = await queue.Dequeue(new DequeueOptions()
			{
				ConfirmTime = TimeSpan.FromMilliseconds(500)
			});

			Assert.AreEqual("test", value.Payload.Value);

			await queue.Confirm(value);


			value = await queue.Dequeue();

			Assert.IsNull(value);

			await Task.Delay(1000);

			value = await queue.Dequeue();

			Assert.IsNull(value);

		}
	}
}
