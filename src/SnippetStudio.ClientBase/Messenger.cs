using System;
using System.Collections.Generic;
using System.Reflection;

namespace SnippetStudio.ClientBase
{
	public class Messenger
	{
		private struct TargetAndMethodInfo
		{
			public WeakReference<object> Target;
			public MethodInfo MethodInfo;
		}

		private readonly Dictionary<Type, List<TargetAndMethodInfo>> _registrations = new Dictionary<Type, List<TargetAndMethodInfo>>();

		public void Send<T>(T message) where T : class
		{
			List<TargetAndMethodInfo> list;
			bool forceClean = false;

			lock (_registrations)
			{
				if (!_registrations.TryGetValue(typeof(T), out list))
				{
					return;
				}
			}

			var parameters = new object[] { message };

			lock (list)
			{
				foreach (var entry in list)
				{
					if (entry.Target.TryGetTarget(out var target))
					{
						entry.MethodInfo.Invoke(target, parameters);
					}
					else
					{
						forceClean = true;
					}
				}
			}

			if (forceClean)
			{
				lock (list) list.RemoveAll(e => !e.Target.TryGetTarget(out var target));
			}
		}

		public void Register<T>(Action<T> handler) where T : class
		{
			List<TargetAndMethodInfo> list;
			var type = typeof(T);

			var entry = new TargetAndMethodInfo()
			{
				Target = new WeakReference<object>(handler.Target),
				MethodInfo = handler.Method
			};

			lock (_registrations)
			{
				if (!_registrations.TryGetValue(type, out list))
				{
					list = new List<TargetAndMethodInfo>();
					_registrations.Add(type, list);
				}
			}

			lock (list) list.Add(entry);
		}

		public void Unregister<T>(T obj) where T : class
		{
			var registrationsList = new List<List<TargetAndMethodInfo>>(_registrations.Count);

			lock (_registrations)
			{
				foreach (var entry in _registrations)
				{
					registrationsList.Add(entry.Value);
				}
			}

			foreach (var list in registrationsList)
			{
				lock (list) list.RemoveAll(e => (!e.Target.TryGetTarget(out var target)) || ReferenceEquals(target, obj));
			}
		}
	}
}