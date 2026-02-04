namespace DaysCounter;

/// <summary>
/// Contains pure business logic for date calculations.
/// Independent of UI controls.
/// </summary>
public class DateCalculator
{
	/// <summary>
	/// Calculates the absolute number of days between two dates.
	/// </summary>
	public static double CalculateDifferenceInDays(DateTime start, DateTime end) =>
		// .Date ensures we ignore time components (e.g. 23:59 vs 00:01)
		Math.Abs(value: (end.Date - start.Date).TotalDays);

	/// <summary>
	/// Adds a specific number of days to a start date.
	/// </summary>
	public static DateTime AddDaysToDate(DateTime start, double days) =>
		start.Date.AddDays(value: days);

	/// <summary>
	/// Calculates the age in days based on a birth date relative to today.
	/// </summary>
	public static double CalculateAgeInDays(DateTime birthDate) =>
		CalculateDifferenceInDays(start: birthDate, end: DateTime.Today);

	/// <summary>
	/// Returns the day number of the year for a given date.
	/// </summary>
	public static int GetDayOfYear(DateTime date) =>
		date.DayOfYear;
}