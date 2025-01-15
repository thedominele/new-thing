using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

namespace NosAssistant2.Helpers;

public class ClipboardAddition
{
	public static Task<TResult> Run<TResult>([NotNull] Func<TResult> function)
	{
		TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				tcs.SetResult(function());
			}
			catch (Exception exception)
			{
				tcs.SetException(exception);
			}
		});
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		return tcs.Task;
	}

	public static Task Run([NotNull] Action action)
	{
		TaskCompletionSource<object> tcs = new TaskCompletionSource<object>();
		Thread thread = new Thread((ThreadStart)delegate
		{
			try
			{
				action();
				tcs.SetResult(null);
			}
			catch (Exception exception)
			{
				tcs.SetException(exception);
			}
		});
		thread.SetApartmentState(ApartmentState.STA);
		thread.Start();
		return tcs.Task;
	}
}
