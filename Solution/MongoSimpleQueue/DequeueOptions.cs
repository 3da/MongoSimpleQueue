using System;
using System.Collections.Generic;
using System.Text;

namespace MongoSimpleQueue
{
	public class DequeueOptions
	{
		public TimeSpan ConfirmTime { get; set; } = TimeSpan.FromMinutes(5);
	}
}
