using NLog;

using System.Diagnostics;

namespace DaysCounter;

/// <summary>
/// Main class of the program.
/// </summary>
internal static class Program
{
	/// <summary>
	/// Logger instance for logging messages and exceptions.
	/// </summary>
	private static readonly Logger logger = LogManager.GetCurrentClassLogger();

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	private static void Main()
	{
		// Initialize and register global handlers as early as possible
		ApplicationConfiguration.Initialize();
		RegisterGlobalExceptionHandlers();

		logger.Info(message: "Application started.");

		using MainForm mainForm = new();
		Application.Run(mainForm: mainForm);
	}

	/// <summary>
	/// Registers central exception handlers for UI, AppDomain, and Task errors.
	/// </summary>
	private static void RegisterGlobalExceptionHandlers()
	{
		// Handle UI thread exceptions
		Application.ThreadException += static (sender, e) => HandleException(ex: e.Exception, userMessage: "An invalid operation occurred. Please try again.");

		// Handle non-UI thread exceptions
		AppDomain.CurrentDomain.UnhandledException += static (sender, e) => HandleException(ex: e.ExceptionObject as Exception ?? new Exception(message: "Unhandled non-exception object"), userMessage: "An unexpected error occurred. Please contact support.");
		// Handle unobserved task exceptions
		TaskScheduler.UnobservedTaskException += static (sender, e) =>
		{
			HandleException(ex: e.Exception, userMessage: "An error occurred in a background task.");
			e.SetObserved();
		};
	}

	/// <summary>
	/// Centralized logging, user communication, and optional telemetry.
	/// </summary>
	/// <param name="ex">The exception.</param>
	/// <param name="userMessage">The generic message for the user.</param>
	private static void HandleException(Exception ex, string userMessage)
	{
		try
		{
			// Log structured and complete
			logger.Error(exception: ex, message: userMessage);
			LogError(ex: ex);

			// Inform user with a generic message
			ShowErrorMessage(message: userMessage);
		}
		catch (Exception loggingFailure)
		{
			// Minimal fallback to prevent the application from crashing during logging
			try
			{
				Debug.WriteLine(value: loggingFailure);
			}
			catch
			{
				// Nothing else we can do
			}
		}
	}

	/// <summary>
	/// Logs error details
	/// </summary>
	/// <param name="ex">The exception.</param>
	private static void LogError(Exception ex)
	{
		// Structured log message; avoid Console.WriteLine in production code.
		logger.Error(exception: ex, message: "Error: {Message}\nStackTrace: {StackTrace}", args: (ex.Message, ex.StackTrace));
	}

	/// <summary>
	/// Shows a generic error message in the UI, safely on the UI thread.
	/// </summary>
	/// <param name="message">The message to display.</param>
	private static void ShowErrorMessage(string message)
	{
		// If a form exists that owns the UI thread, use Invoke.
		if (Application.OpenForms.Count > 0)
		{
			Form? owner = Application.OpenForms[index: 0];
			if (owner != null)
			{
				if (owner.InvokeRequired)
				{
					owner.Invoke(method: () => MessageBox.Show(owner: owner, text: message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error));
					return;
				}
				_ = MessageBox.Show(owner: owner, text: message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
				return;
			}
		}

		_ = MessageBox.Show(text: message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
	}
}