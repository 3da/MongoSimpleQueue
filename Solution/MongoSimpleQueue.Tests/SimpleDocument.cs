using System;
using System.Collections.Generic;
using System.Text;

namespace MongoSimpleQueue.Tests
{
	public class SimpleDocument
	{
		public string Value { get; set; }

		public override string ToString()
		{
			return $"Value: {Value}";
		}
	}
}
