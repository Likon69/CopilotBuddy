using System;
using System.Runtime.Serialization;

namespace Styx
{
	[Serializable]
	public class HonorbuddyUnableToStartException : UserException
	{
		public HonorbuddyUnableToStartException()
		{
		}

		public HonorbuddyUnableToStartException(string message)
			: base(message)
		{
		}

		public HonorbuddyUnableToStartException(string message, Exception inner)
			: base(message, inner)
		{
		}

		protected HonorbuddyUnableToStartException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		public override string HelpLink
		{
			get
			{
				return "";
			}
		}
	}
}
