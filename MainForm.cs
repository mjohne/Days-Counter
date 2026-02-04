using DaysCounter.Properties;

using NLog;

using System.Diagnostics;

namespace DaysCounter
{
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
			double days = (dateTimePickerEnd.Value.Date - dateTimePickerBegin.Value.Date).TotalDays;
			labelDaysCounted.Text = $"Difference {Math.Abs(value: days):N0} days.";
		}

		/// <summary>
		/// Calculate the days from a date with a specific span in days
		/// </summary>
		private void CalculateDateFromSpan()
			=> dateTimePickerDateOut.Value = dateTimePickerDateIn.Value.Date.AddDays(value: (double)numericUpDownDays.Value);

		/// <summary>
		/// Calculate the days from a date until today
		/// </summary>
		private void CalculateDaysOfLife()
		{
			double daysOld = (DateTime.Today - dateTimePickerDateOfTheBirth.Value.Date).TotalDays;
			labelDaysOld.Text = $"You are {Math.Abs(value: daysOld):N0} days old.";
		}

		/// <summary>
		/// Calculate the days since the start of the year until today
		/// </summary>
		private void CalculateDaysOfYear()
			=> labelDaysOfYearPassed.Text = $"Day {dateTimePickerDaysOfYear.Value.DayOfYear} of the current year.";

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

		#endregion

		#region Clipboard Operations

		/// <summary>
		/// Copies the specified text to the clipboard and displays a confirmation message
		/// </summary>
		/// <param name="text">The text to be copied</param>
		private static void CopyToClipboard(string text)
		{
			if (string.IsNullOrWhiteSpace(value: text))
			{
				return;
			}

			try
			{
				Clipboard.SetText(text: text);
				MessageBox.Show(text: "Copied to clipboard.", caption: "Information", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
			}
			catch (Exception ex)
			{
				HandleException(ex: ex, message: "An error occurred while copying to clipboard.");
			}
		}

		/// <summary>
		/// Pastes the text from the clipboard into the specified DateTimePicker
		/// </summary>
		/// <param name="dateTimePicker">The DateTimePicker to paste the text into</param>
		private static void PasteToDateTimePicker(DateTimePicker dateTimePicker)
		{
			try
			{
				if (!Clipboard.ContainsText())
				{
					MessageBox.Show(text: "The clipboard is empty or contains no text.", caption: "Information", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Information);
					return;
				}

				string clipboardText = Clipboard.GetText();

				if (DateTime.TryParse(s: clipboardText, result: out DateTime parsedDate))
				{
					dateTimePicker.Value = parsedDate;
				}
				else
				{
					MessageBox.Show(text: $"The clipboard content '{clipboardText}' is not a valid date.", caption: "Warning", buttons: MessageBoxButtons.OK, icon: MessageBoxIcon.Warning);
				}
			}
			catch (Exception ex)
			{
				HandleException(ex: ex, message: "An error occurred while reading from the clipboard.");
			}
		}

		#endregion

		#region Event Handlers: UI Interaction

		// Input Method Toggles
		private void ButtonSwitchDateBegin_Click(object sender, EventArgs e) => dateTimePickerBegin.ShowUpDown = !dateTimePickerBegin.ShowUpDown;
		private void ButtonSwitchDateEnd_Click(object sender, EventArgs e) => dateTimePickerEnd.ShowUpDown = !dateTimePickerEnd.ShowUpDown;
		private void ButtonSwitchDateDays_Click(object sender, EventArgs e) => dateTimePickerDateIn.ShowUpDown = !dateTimePickerDateIn.ShowUpDown;
		private void ButtonDateOfTheBirth_Click(object sender, EventArgs e) => dateTimePickerDateOfTheBirth.ShowUpDown = !dateTimePickerDateOfTheBirth.ShowUpDown;
		private void ButtonDaysOfYear_Click(object sender, EventArgs e) => dateTimePickerDaysOfYear.ShowUpDown = !dateTimePickerDaysOfYear.ShowUpDown;

		// TopMost Logic
		private void ToolStripMenuItemStayNotOnTop_Click(object sender, EventArgs e) => ToggleTopMost();
		private void ToolStripMenuItemStayOnTop_Click(object sender, EventArgs e) => ToggleTopMost();
		private void ToolStripSplitButtonStayOnTop_ButtonClick(object sender, EventArgs e) => ToggleTopMost();

		// Copy Handlers
		private void ButtonDateToDateCopyToClipboard_Click(object sender, EventArgs e) => CopyToClipboard(text: labelDaysCounted.Text);
		private void ButtonDaysOfSpanCopyToClipboard_Click(object sender, EventArgs e) => CopyToClipboard(text: dateTimePickerDateOut.Value.ToShortDateString());
		private void ButtonDaysOfLifeCopyToClipboard_Click(object sender, EventArgs e) => CopyToClipboard(text: labelDaysOld.Text);
		private void ButtonDaysOfYearCopyToClipboard_Click(object sender, EventArgs e) => CopyToClipboard(text: labelDaysOfYearPassed.Text);

		// Paste Handlers
		private void ButtonDateToDateCopyFromClipboard_Click(object sender, EventArgs e) => PasteToDateTimePicker(dateTimePicker: dateTimePickerBegin);
		private void ButtonSpanOfDaysCopyFromClipboard_Click(object sender, EventArgs e) => PasteToDateTimePicker(dateTimePicker: dateTimePickerDateIn);
		private void ButtonDaysOfLifeCopyFromClipboard_Click(object sender, EventArgs e) => PasteToDateTimePicker(dateTimePicker: dateTimePickerDateOfTheBirth);
		private void ButtonDaysOfYearCopyFromClipboard_Click(object sender, EventArgs e) => PasteToDateTimePicker(dateTimePicker: dateTimePickerDaysOfYear);

		// Value Changed Handlers
		private void DateTimePickerBegin_ValueChanged(object sender, EventArgs e) => CalculateDaysFromDateToDate();
		private void DateTimePickerEnd_ValueChanged(object sender, EventArgs e) => CalculateDaysFromDateToDate();
		private void DateTimePickerDateIn_ValueChanged(object sender, EventArgs e) => CalculateDateFromSpan();
		private void NumericUpDownDays_ValueChanged(object sender, EventArgs e) => CalculateDateFromSpan();
		private void DateTimePickerDateOfTheBirth_ValueChanged(object sender, EventArgs e) => CalculateDaysOfLife();
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
				SetStatusBarText(text: control.AccessibleDescription);
			}
			else if (sender is ToolStripItem item && !string.IsNullOrEmpty(value: item.AccessibleDescription))
			{
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
			if (e.KeyCode == Keys.Escape)
			{
				Close();
			}
		}

		#endregion
	}
}