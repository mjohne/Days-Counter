using System.Text;

namespace DaysCounter;

/// <summary>
/// Exports calendar events in ICS format.
/// </summary>
public class CalendarExporter
{
	/// <summary>
	/// Creates an ICS file for a full-day event.
	/// </summary>
	/// <param name="title">The title of the calendar entry</param>
	/// <param name="date">The date of the event</param>
	/// <param name="description">Additional description</param>
	/// <param name="filePath">The file path to save the ICS file</param>
	public static void CreateIcsFile(string title, DateTime date, string description, string filePath)
	{
		StringBuilder sb = new();

		// 1. Calendar header
		sb.AppendLine(value: "BEGIN:VCALENDAR");
		sb.AppendLine(value: "VERSION:2.0");
		sb.AppendLine(value: "PRODID:-//DaysCounterApp//EN"); // Identification of our app

		// 2. The actual event
		sb.AppendLine(value: "BEGIN:VEVENT");

		// Unique ID (Timestamp + Title is usually unique enough for local purposes)
		sb.AppendLine(handler: $"UID:{DateTime.Now:yyyyMMddTHHmmss}-{title.GetHashCode()}@dayscounter");

		sb.AppendLine(value: "BEGIN:VCALENDAR");
		sb.AppendLine(value: "VERSION:2.0");
		sb.AppendLine(value: "PRODID:-//DaysCounterApp//EN"); // Identification of our app

		// 2. The actual event
		sb.AppendLine(value: "BEGIN:VEVENT");

		// Unique ID (Timestamp + Title is usually unique enough for local purposes)
		sb.AppendLine(handler: $"UID:{DateTime.Now:yyyyMMddTHHmmss}-{title.GetHashCode()}@dayscounter");

		// Creation date (Now)
		sb.AppendLine(handler: $"DTSTAMP:{DateTime.Now:yyyyMMddTHHmmss}");

		// Start and end date (Format yyyyMMdd for full-day events)
		// For full-day events, the end date must be the following day!
		sb.AppendLine(handler: $"DTSTART;VALUE=DATE:{date:yyyyMMdd}");
		sb.AppendLine(handler: $"DTEND;VALUE=DATE:{date.AddDays(value: 1):yyyyMMdd}");

		sb.AppendLine(handler: $"SUMMARY:{title}");
		sb.AppendLine(handler: $"DESCRIPTION:{description}");

		sb.AppendLine(value: "END:VEVENT");
		sb.AppendLine(value: "END:VCALENDAR");

		// 3. Write file (UTF8 important for umlauts)
		File.WriteAllText(path: filePath, contents: sb.ToString(), encoding: Encoding.UTF8);
	}
}