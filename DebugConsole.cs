using Microsoft.VisualBasic.Logging;
using MyGui.net.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyGui.net
{
	internal class DebugConsole
	{
		public enum LogLevels
		{
			Info,
			Debug,
			Warning,
			Error,
			Success,
			Handled_Exception
		}

		[DllImport("kernel32.dll")]
		private static extern bool AllocConsole();

		[DllImport("kernel32.dll")]
		private static extern bool FreeConsole();

		private static bool consoleAllocated = false;
		private static Thread consoleThread;

		// A class to store each log entry
		private class LogEntry
		{
			public DateTime Timestamp { get; set; }
			public LogLevels Level { get; set; }
			public string Message { get; set; }
		}

		// List that holds the entire console history
		private static List<LogEntry> logHistory = new List<LogEntry>();

		public static void ShowConsole()
		{
			if (!consoleAllocated)
			{
				AllocConsole();

				Console.SetOut(new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });

				Console.Title = "MyGui.net Debug Console";
				Console.CursorVisible = false;

				// Redirect Debug and Trace output
				TextWriterTraceListener listener = new TextWriterTraceListener(new TimestampTextWriter());
				Trace.Listeners.Add(listener);
				Trace.AutoFlush = true;

				// Capture handled exceptions
				AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
				{
					Log(e.Exception.ToString(), LogLevels.Handled_Exception);
				};

				// Start a thread to listen for keys
				consoleThread = new Thread(ConsoleInputListener)
				{
					IsBackground = true
				};
				consoleThread.Start();

				consoleAllocated = true;

				// Replay all previously logged entries
				foreach (var entry in logHistory)
				{
					PrintLogEntry(entry);
				}

				Console.WriteLine();
				Log("Console Attached! (Press \"E\" to detach)", LogLevels.Success);
				Log("Closing this console without pressing \"E\", or pressing \"Ctrl+C\" while having nothing selected will close the whole application no questions asked! Be warned!", LogLevels.Warning);
			}
		}

		private static void ConsoleInputListener()
		{
			while (consoleAllocated)
			{
				if (Console.KeyAvailable)
				{
					var key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.E)
					{
						Log("Console Detached!", LogLevels.Info);
						Settings.Default.ShowDebugConsole = false;
						Settings.Default.Save();
						HideConsole();
						break;
					}
				}
				Thread.Sleep(100); // Reduce CPU usage
			}
		}

		public static void HideConsole()
		{
			if (consoleAllocated)
			{
				FreeConsole();
				consoleAllocated = false;
			}
		}

		public static void CloseConsoleOnExit(Form mainForm)
		{
			mainForm.FormClosing += (s, e) => HideConsole();
		}

		// Helper to print a single log entry with appropriate color formatting
		private static void PrintLogEntry(LogEntry entry)
		{
			SetColorBasedOnLevel(entry.Level);
			Console.WriteLine($"{entry.Timestamp:HH:mm:ss} [{entry.Level.ToString().ToUpper().Replace('_', ' ')}] {entry.Message}");
			Console.ResetColor();
		}

		// Set the console color based on the log level
		private static void SetColorBasedOnLevel(LogLevels level)
		{
			switch (level)
			{
				case LogLevels.Info:
					Console.ForegroundColor = ConsoleColor.Cyan;
					break;
				case LogLevels.Success:
				case LogLevels.Debug:
					Console.ForegroundColor = ConsoleColor.Green;
					break;
				case LogLevels.Warning:
					Console.ForegroundColor = ConsoleColor.Yellow;
					break;
				case LogLevels.Error:
					Console.ForegroundColor = ConsoleColor.Red;
					break;
				case LogLevels.Handled_Exception:
					Console.ForegroundColor = ConsoleColor.DarkGreen;
					break;
				default:
					Console.ResetColor();
					break;
			}
		}

		// Custom TextWriter that logs Trace messages
		private class TimestampTextWriter : TextWriter
		{
			private static readonly object consoleLock = new object();
			public override Encoding Encoding => Encoding.UTF8;

			public override void WriteLine(string message)
			{
				try
				{
					lock (consoleLock)
					{
						// Instead of directly writing, forward to our log method
						DebugConsole.Log(message, LogLevels.Debug);
					}
				}
				catch (IOException e)
				{
					Debugger.Log(0, null, e.ToString());
				}
			}
		}

		private static bool isLoggingException = false;
		public static void Write(string message, LogLevels level = LogLevels.Debug) => Log(message, level);
		public static void WriteLine(string message, LogLevels level = LogLevels.Debug) => Log(message, level);

		public static void Log(string message, LogLevels level = LogLevels.Debug)
		{
			if (isLoggingException)
				return;

			try
			{
				isLoggingException = true;

				// Create and store the log entry
				var entry = new LogEntry
				{
					Timestamp = DateTime.Now,
					Level = level,
					Message = message
				};
				logHistory.Add(entry);

				// If the console is currently allocated, output immediately
				if (consoleAllocated)
				{
					SetColorBasedOnLevel(level);
					Console.WriteLine($"{entry.Timestamp:HH:mm:ss} [{Enum.GetName(level).ToUpper().Replace('_', ' ')}] {message}");
					Console.ResetColor();
				}

				Debugger.Log(0, null, message);
			}
			catch (Exception ex)
			{
				Debug.WriteLine("Logging failed: " + ex.Message);
			}
			finally
			{
				isLoggingException = false;
			}
		}
	}
}
