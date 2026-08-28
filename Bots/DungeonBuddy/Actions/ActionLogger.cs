using System;
using System.Drawing;
using Bots.DungeonBuddy.Helpers;
using TreeSharp;

namespace Bots.DungeonBuddy.Actions
{
	public class ActionLogger : global::TreeSharp.Action
	{
		private readonly Color _color;
		private readonly string _format;
		private readonly object[] _args;
		private readonly bool _useColor;

		public ActionLogger(string format, params object[] args)
		{
			_format = format;
			_args = args;
		}

		public ActionLogger(Color color, string format, params object[] args)
			: this(format, args)
		{
			_color = color;
			_useColor = true;
		}

		protected override RunStatus Run(object context)
		{
			if (_useColor)
			{
				Styx.Helpers.Logging.Write(_color, _format, _args);
			}
			else
			{
				Styx.Helpers.Logging.Write(_format, _args);
			}

			return base.Parent is Selector ? RunStatus.Failure : RunStatus.Success;
		}
	}
}
