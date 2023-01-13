using System;
using System.Collections.Generic;

namespace Utils
{
	public delegate void CommandHandler<T>(T command) where T : struct;

	public class CommandDispatcher
	{
		private static CommandDispatcher _instance;

		public static CommandDispatcher Instance
		{
			get
			{
				return _instance ?? (_instance = new CommandDispatcher());
			}
		}

		private Dictionary<Type, Delegate> _handlers;

		public CommandDispatcher()
		{
			_handlers = new Dictionary<Type, Delegate>();
		}

		public void Reset()
		{
			_handlers = new Dictionary<Type, Delegate>();
		}

		public void RegisterHandler<T>(CommandHandler<T> command) where T : struct
		{
			var type = typeof(T);
			if (!_handlers.ContainsKey(type)) _handlers.Add(type, new CommandHandler<T>(delegate { }));
			var commandHandler = (CommandHandler<T>)_handlers[type];
			commandHandler += command;
			_handlers[type] = commandHandler;
		}

		public void RemoveHandler<T>(CommandHandler<T> command) where T : struct
		{
			var type = typeof(T);
			if (!_handlers.ContainsKey(type)) _handlers.Add(type, new CommandHandler<T>(delegate { }));
			var commandHandler = (CommandHandler<T>)_handlers[type];
			commandHandler -= command;
			_handlers[type] = commandHandler;
		}

		public void Handle<T>(T command) where T : struct
		{
			var type = typeof(T);
			if (!_handlers.ContainsKey(type)) return;
			((CommandHandler<T>)_handlers[type]).Invoke(command);
		}
	}
}