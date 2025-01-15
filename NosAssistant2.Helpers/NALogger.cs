using System;
using System.IO;

namespace NosAssistant2.Helpers;

public static class NALogger
{
	public static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
	{
		LogExceptionToFile(e.ExceptionObject as Exception);
	}

	public static void LogExceptionToFile(Exception exception)
	{
		using StreamWriter streamWriter = File.AppendText("error.log");
		streamWriter.WriteLine($"[{DateTime.Now}] An unhandled exception occurred:");
		streamWriter.WriteLine($"Exception Type: {exception.GetType()}");
		streamWriter.WriteLine("Message: " + exception.Message);
		streamWriter.WriteLine("Stack Trace: " + exception.StackTrace);
		streamWriter.WriteLine(new string('-', 50));
	}
}
