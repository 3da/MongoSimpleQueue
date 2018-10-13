using System;
using System.Collections.Generic;
using System.Text;

namespace MongoSimpleQueue
{
	public class EnqueOptions
	{
		public byte Priority { get; set; }

		public DateTime? StartAfterDateTime { get; set; }
	}
}
