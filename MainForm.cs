using DaysCounter.Properties;

using NLog;

using System.Diagnostics;

namespace DaysCounter;

/// <summary>
/// Show the main window of the application
/// </summary>
[DebuggerDisplay(value: $"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public partial class MainForm : Form
{
	/// <summary>
	/// Logger instance for logging messages and exceptions
	/// </summary>
	private static readonly Logger logger = LogManager.GetCurrentClassLogger();

	#region Constructor

	/// <summary>
	/// Constructor
	/// </summary>
	public MainForm()
	{
		// Initialize the form
		InitializeComponent();
		// Setup Key Handling
		KeyDown += MainForm_KeyDown;
		// Ensures the form receives key events before the controls
		KeyPreview = true;
		// Initial Calculations & UI Setup
		ClearStatusBar_Leave(sender: null, e: null);
		UpdateAllCalculations();
		// Set Labels from AssemblyInfo
		labelTitle.Text = $"{AssemblyInfo.AssemblyProduct} {AssemblyInfo.AssemblyVersion}";
		labelDescription.Text = AssemblyInfo.AssemblyDescription;
		labelCopyright.Text = $"{AssemblyInfo.AssemblyCopyright}";
	}

	#endregion

	#region Calculation Logic

	/// <summary>
	/// Updates all calculations
	/// </summary>
	private void UpdateAllCalculations()
	{
		CalculateDaysFromDateToDate();
		CalculateDateFromSpan();
		CalculateDaysOfLife();
		CalculateDaysOfYear();
	}

	/// <summary>
	/// Calculate the days from a date to another date
	/// </summary>
	private void CalculateDaysFromDateToDate()
	{
		// Get the start and end dates from the date pickers
		DateTime start = dateTimePickerBegin.Value;
		DateTime end = dateTimePickerEnd.Value;
		// Calculate the difference in days using the DateCalculator class
		double days = DateCalculator.CalculateDifferenceInDays(start: start, end: end);
		// Update the label to show the result
		labelDaysCounted.Text = $"Difference {days:N0} days.";
	}

	/// <summary>
	/// Calculate the days from a date with a specific span in days
	/// </summary>
	private void CalculateDateFromSpan()
	{
		// Get the start date and the number of days to add from the controls
		DateTime start = dateTimePickerDateIn.Value;
		double daysToAdd = (double)numericUpDownDays.Value;
		// Calculate the resulting date using the DateCalculator class
		DateTime resultDate = DateCalculator.AddDaysToDate(start: start, days: daysToAdd);
		// Update the date picker to show the resulting date
		dateTimePickerDateOut.Value = resultDate;
	}

	/// <summary>
	/// Calculate the days from a date until today
	/// </summary>
	private void CalculateDaysOfLife()
	{
		// Get the birth date from the date picker
		DateTime birthDate = dateTimePickerDateOfTheBirth.Value;
		// Calculate the age in days using the DateCalculator class
		double daysOld = DateCalculator.CalculateAgeInDays(birthDate: birthDate);
		// Update the label to show the result
		labelDaysOld.Text = $"You are {daysOld:N0} days old.";
	}

	/// <summary>
	/// Calculate the days since the start of the year until today
	/// </summary>
	private void CalculateDaysOfYear()
	{
		// Get the date from the date picker
		DateTime date = dateTimePickerDaysOfYear.Value;
		// Get the day of the year using the DateCalculator class
		int dayOfYear = DateCalculator.GetDayOfYear(date);
		// Update the label to show the result
		labelDaysOfYearPassed.Text = $"Day {dayOfYear} of the current year.";
	}

	#endregion

	#region Helpers

	/// <summary>
	/// Handles exceptions by logging the error and showing a message box
	/// </summary>
	/// <param name="ex">The exception that occurred</param>
	/// <param name="message">The message to log and display</param>
	/// <param name="sender">The source of the event that caused the exception</param>
	/// <param name="e">The event data associated with the exception</param>
	private static void HandleException(Exception ex, string message, object? sender = null, EventArgs? e = null)
	{
		// Structured logging; detailed information is in the log
		logger.Error(exception: ex, message: "Exception occurred. Message: {Message} | Sender: {Sender}", args: (message, sender));
		// Show only a generic message to the user (details are in the log)
		_ = MessageBox.Show(text: message, caption: "Error", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Error);
	}

	/// <summary>
	/// Get the debugger display
	/// </summary>
	/// <returns>debugger display</returns>
	private string GetDebuggerDisplay() => ToString();

	/// <summary>
	/// Set a specific text to the status bar
	/// </summary>
	/// <param name="text">text with some information</param>
	private void SetStatusBarText(string text)
	{
		// Enable the label only if there is text to show
		labelInformation.Enabled = !string.IsNullOrEmpty(value: text);
		labelInformation.Text = text;
	}

	/// <summary>
	/// Clears the status bar
	/// </summary>
	private void ClearStatusBar() => SetStatusBarText(text: string.Empty);

	/// <summary>
	/// Toggle the "Always on Top" status of the application
	/// </summary>
	private void ToggleTopMost()
	{
		TopMost = !TopMost;
		// UI update based on the new status
		toolStripMenuItemStayNotOnTop.Checked = !TopMost;
		toolStripMenuItemStayOnTop.Checked = TopMost;
		toolStripSplitButtonStayOnTop.Image = TopMost ? Resources.application_blue : Resources.application;
		toolStripSplitButtonStayOnTop.Text = TopMost ? Resources.stayOnTop : Resources.stayNotOnTop;
	}

	/// <summary>
	/// Opens a file with the default application.
	/// </summary>
	/// <param name="filePath">The path to the file to open.</param>
	private static void OpenFile(string filePath)
	{
		// Use ProcessStartInfo to open the file with the default associated application
		try
		{
			// UseShellExecute must be true to use the default application
			using Process? _ = Process.Start(startInfo: new ProcessStartInfo(fileName: filePath) { UseShellExecute = true });
		}
		// Catch any exceptions that may occur during the process start
		catch (Exception ex)
		{
			// Log the exception and show a user-friendly message
			HandleException(ex: ex, message: "The file could not be opened automatically.");
		}
	}

	#endregion

	#region Clipboard Operations

	/// <summary>
	/// Copies the specified text to the clipboard and displays a confirmation message
	/// </summary>
	/// <param name="text">The text to be copied</param>
	private static void CopyToClipboard(string text)
	{
		// Do not attempt to copy if the text is null, empty, or whitespace
		if (string.IsNullOrWhiteSpace(value: text))
		{
			return;
		}
		// Attempt to copy the text to the clipboard and handle any potential exceptions
		try
		{
			// Clipboard operations can fail if the clipboard is being used by another process, so we catch exceptions to prevent crashes
			Clipboard.SetText(text: text);
			MessageBox.Show(text: "Copied to clipboard.", caption: "Information", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
		}
		// Catching general exceptions here to prevent the application from crashing due to clipboard access issues
		catch (Exception ex)
		{
			// Log the exception and show a user-friendly message without exposing technical details
			HandleException(ex: ex, message: "An error occurred while copying to clipboard.");
		}
	}

	/// <summary>
	/// Pastes the text from the clipboard into the specified DateTimePicker
	/// </summary>
	/// <param name="dateTimePicker">The DateTimePicker to paste the text into</param>
	private static void PasteToDateTimePicker(DateTimePicker dateTimePicker)
	{
		// Attempt to read text from the clipboard and set it as the value of the DateTimePicker, handling any potential exceptions
		try
		{
			// Check if the clipboard contains text before attempting to read it
			if (!Clipboard.ContainsText())
			{
				// Inform the user that there is no text in the clipboard to paste
				MessageBox.Show(text: "The clipboard is empty or contains no text.", caption: "Information", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
				return;
			}
			// Read the text from the clipboard
			string clipboardText = Clipboard.GetText();
			// Attempt to parse the clipboard text as a date and set it to the DateTimePicker if successful
			if (DateTime.TryParse(s: clipboardText, result: out DateTime parsedDate))
			{
				// Set the parsed date as the value of the DateTimePicker
				dateTimePicker.Value = parsedDate;
			}
			// If parsing fails, inform the user that the clipboard content is not a valid date
			else
			{
				// Log the invalid clipboard content for debugging purposes
				MessageBox.Show(text: $"The clipboard content '{clipboardText}' is not a valid date.", caption: "Warning", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Warning);
			}
		}
		// Catching general exceptions here to prevent the application from crashing due to clipboard access issues or parsing errors
		catch (Exception ex)
		{
			// Log the exception and show a user-friendly message without exposing technical details
			HandleException(ex: ex, message: "An error occurred while reading from the clipboard.");
		}
	}

	#endregion

	#region Event Handlers: UI Interaction

	/// <summary>
	/// Handles the Click event of the button and toggles the ShowUpDown property of the dateTimePickerBegin control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonSwitchDateBegin_Click(object sender, EventArgs e) => dateTimePickerBegin.ShowUpDown = !dateTimePickerBegin.ShowUpDown;

	/// <summary>
	/// Handles the Click event of the button and toggles the ShowUpDown property of the dateTimePickerEnd control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonSwitchDateEnd_Click(object sender, EventArgs e) => dateTimePickerEnd.ShowUpDown = !dateTimePickerEnd.ShowUpDown;

	/// <summary>
	/// Handles the Click event of the button and toggles the ShowUpDown property of the dateTimePickerDateIn control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonSwitchDateDays_Click(object sender, EventArgs e) => dateTimePickerDateIn.ShowUpDown = !dateTimePickerDateIn.ShowUpDown;

	/// <summary>
	/// Handles the Click event of the button and toggles the ShowUpDown property of the dateTimePickerDateOfTheBirth control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDateOfTheBirth_Click(object sender, EventArgs e) => dateTimePickerDateOfTheBirth.ShowUpDown = !dateTimePickerDateOfTheBirth.ShowUpDown;

	/// <summary>
	/// Handles the Click event of the button and toggles the ShowUpDown property of the dateTimePickerDaysOfYear control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDaysOfYear_Click(object sender, EventArgs e) => dateTimePickerDaysOfYear.ShowUpDown = !dateTimePickerDaysOfYear.ShowUpDown;

	/// <summary>
	/// Handles the Click event of the menu item and toggles the TopMost property of the form.
	/// </summary>
	/// <param name="sender">The source of the event, typically the menu item that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ToolStripMenuItemStayNotOnTop_Click(object sender, EventArgs e) => ToggleTopMost();

	/// <summary>
	/// Handles the Click event of the menu item and toggles the TopMost property of the form.
	/// </summary>
	/// <param name="sender">The source of the event, typically the menu item that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ToolStripMenuItemStayOnTop_Click(object sender, EventArgs e) => ToggleTopMost();

	/// <summary>
	/// Handles the Click event of the split button and toggles the TopMost property of the form.
	/// </summary>
	/// <param name="sender">The source of the event, typically the split button that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ToolStripSplitButtonStayOnTop_ButtonClick(object sender, EventArgs e) => ToggleTopMost();

	/// <summary>
	/// Handles the Click event of the button and copies the date to the clipboard.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDateToDateCopyToClipboard_Click(object sender, EventArgs e) => CopyToClipboard(text: labelDaysCounted.Text);

	/// <summary>
	/// Handles the Click event of the button and copies the date to the clipboard.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDaysOfSpanCopyToClipboard_Click(object sender, EventArgs e) => CopyToClipboard(text: dateTimePickerDateOut.Value.ToShortDateString());

	/// <summary>
	/// Handles the Click event of the button and copies the date to the clipboard.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDaysOfLifeCopyToClipboard_Click(object sender, EventArgs e) => CopyToClipboard(text: labelDaysOld.Text);

	/// <summary>
	/// Handles the Click event of the button and copies the date to the clipboard.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDaysOfYearCopyToClipboard_Click(object sender, EventArgs e) => CopyToClipboard(text: labelDaysOfYearPassed.Text);

	/// <summary>
	/// Handles the Click event of the button and pastes the clipboard content into the specified DateTimePicker control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDateToDateCopyFromClipboard_Click(object sender, EventArgs e) => PasteToDateTimePicker(dateTimePicker: dateTimePickerBegin);

	/// <summary>
	/// Handles the Click event of the button and pastes the clipboard content into the specified DateTimePicker control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonSpanOfDaysCopyFromClipboard_Click(object sender, EventArgs e) => PasteToDateTimePicker(dateTimePicker: dateTimePickerDateIn);

	/// <summary>
	/// Handles the Click event of the button and pastes the clipboard content into the specified DateTimePicker control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDaysOfLifeCopyFromClipboard_Click(object sender, EventArgs e) => PasteToDateTimePicker(dateTimePicker: dateTimePickerDateOfTheBirth);

	/// <summary>
	/// Handles the Click event of the button and pastes the clipboard content into the specified DateTimePicker control.
	/// </summary>
	/// <param name="sender">The source of the event, typically the button control that was clicked.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void ButtonDaysOfYearCopyFromClipboard_Click(object sender, EventArgs e) => PasteToDateTimePicker(dateTimePicker: dateTimePickerDaysOfYear);

	/// <summary>
	/// Handles the ValueChanged event of the begin date picker control and updates the calculated date range accordingly.
	/// </summary>
	/// <param name="sender">The source of the event, typically the begin date picker control.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void DateTimePickerBegin_ValueChanged(object sender, EventArgs e) => CalculateDaysFromDateToDate();

	/// <summary>
	/// Handles the ValueChanged event of the end date picker control and updates the calculated date accordingly.
	/// </summary>
	/// <param name="sender">The source of the event, typically the date picker control whose value has changed.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void DateTimePickerEnd_ValueChanged(object sender, EventArgs e) => CalculateDaysFromDateToDate();

	/// <summary>
	/// Handles the ValueChanged event of the date-in date picker control and updates the calculated date accordingly.
	/// </summary>
	/// <param name="sender">The source of the event, typically the date picker control whose value has changed.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void DateTimePickerDateIn_ValueChanged(object sender, EventArgs e) => CalculateDateFromSpan();

	/// <summary>
	/// Handles the ValueChanged event of the numeric up-down control and updates the calculated date accordingly.
	/// </summary>
	/// <param name="sender">The source of the event, typically the numeric up-down control whose value has changed.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void NumericUpDownDays_ValueChanged(object sender, EventArgs e) => CalculateDateFromSpan();

	/// <summary>
	/// Handles the ValueChanged event of the date-of-birth date picker control and updates the calculated number of days
	/// accordingly.
	/// </summary>
	/// <param name="sender">The source of the event, typically the date picker control whose value has changed.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void DateTimePickerDateOfTheBirth_ValueChanged(object sender, EventArgs e) => CalculateDaysOfLife();

	/// <summary>
	/// Handles the ValueChanged event of the days-of-year date picker control and updates the calculated number of days
	/// accordingly.
	/// </summary>
	/// <param name="sender">The source of the event, typically the date picker control whose value has changed.</param>
	/// <param name="e">An EventArgs object that contains the event data.</param>
	private void DateTimePickerDaysOfYear_ValueChanged(object sender, EventArgs e) => CalculateDaysOfYear();

	#endregion

	#region Event Handlers: General

	/// <summary>
	/// Sets the status bar text when the control is entered.
	/// </summary>
	/// <param name="sender">sender object</param>
	/// <param name="e">event arguments</param>
	private void SetStatusBar_Enter(object sender, EventArgs e)
	{
		// Pattern matching for clearer syntax
		if (sender is Control control && !string.IsNullOrEmpty(value: control.AccessibleDescription))
		{
			// Set the status bar text to the AccessibleDescription of the control, if it exists
			SetStatusBarText(text: control.AccessibleDescription);
		}
		// If the sender is a ToolStripItem (e.g., menu item, toolbar button) and has an AccessibleDescription, use it for the status bar
		else if (sender is ToolStripItem item && !string.IsNullOrEmpty(value: item.AccessibleDescription))
		{
			// Set the status bar text to the AccessibleDescription of the ToolStripItem, if it exists
			SetStatusBarText(text: item.AccessibleDescription);
		}
	}

	/// <summary>
	/// Clears the status bar text when the control is left.
	/// </summary>
	/// <param name="sender">sender object</param>
	/// <param name="e">event arguments</param>
	private void ClearStatusBar_Leave(object? sender, EventArgs? e) => ClearStatusBar();

	/// <summary>
	/// Handles the key down event for the main form.
	/// </summary>
	/// <param name="sender">sender object</param>
	/// <param name="e">event arguments</param>
	private void MainForm_KeyDown(object? sender, KeyEventArgs e)
	{
		// If the Escape key is pressed, close the form
		if (e.KeyCode == Keys.Escape)
		{
			Close();
		}
	}

	/// <summary>
	/// Handles the click event of the "Export to Calendar" button.
	/// </summary>
	/// <param name="sender">sender object</param>
	/// <param name="e">event arguments</param>
	private void ButtonExportToCalendar_Click(object sender, EventArgs e)
	{
		// 1. Collect data
		// We take the calculated date from the "Date with Span" tab.
		DateTime targetDate = dateTimePickerDateOut.Value;
		// Generate title (can later be entered via a TextBox)
		string title = $"DaysCounter: Event after {numericUpDownDays.Value} days";
		string description = $"Calculated from {dateTimePickerDateIn.Value:d} plus {numericUpDownDays.Value} days.";
		// 2. Open the save dialog.
		using SaveFileDialog saveFileDialog = new();
		saveFileDialog.Filter = "iCalendar File (*.ics)|*.ics";
		saveFileDialog.Title = "Save calendar entry";
		saveFileDialog.FileName = "event.ics";
		// Show the dialog and process the result
		if (saveFileDialog.ShowDialog() == DialogResult.OK)
		{
			try
			{
				// 3. Call export
				CalendarExporter.CreateIcsFile(title: title, date: targetDate, description: description, filePath: saveFileDialog.FileName);
				// 4. Success message & option to open
				DialogResult result = MessageBox.Show(
					text: "File successfully exported.\nDo you want to open it directly (in Outlook/Calendar)?",
					caption: "Export successful",
					buttons: MessageBoxButtons.YesNo,
					icon: MessageBoxIcon.Question);
				// If the user chooses to open the file, attempt to do so
				if (result == DialogResult.Yes)
				{
					// Try to open the file with the default app (e.g. Outlook)
					OpenFile(filePath: saveFileDialog.FileName);
				}
			}
			// Catch any exceptions that may occur during the export process
			catch (Exception ex)
			{
				// Log the exception and show a user-friendly message
				HandleException(ex: ex, message: "Error exporting calendar entry.");
			}
		}
	}

	#endregion
}