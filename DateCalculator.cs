namespace DaysCounter;

/// <summary>Contains pure business logic for date calculations. Independent of UI controls.</summary>
public class DateCalculator
{
	/// <summary>Calculates the absolute number of days between two dates.</summary>
	/// <param name="start">The start date.</param>
	/// <param name="end">The end date.</param>
	/// <returns>The absolute number of days between the two dates.</returns>
	public static double CalculateDifferenceInDays(DateTime start, DateTime end) =>
		// .Date ensures we ignore time components (e.g. 23:59 vs 00:01)
		Math.Abs(value: (end.Date - start.Date).TotalDays);

	/// <summary>Adds a specific number of days to a start date.</summary>
	/// <param name="start">The start date.</param>
	/// <param name="days">The number of days to add.</param>
	/// <returns>The resulting date after adding the specified number of days.</returns>
	public static DateTime AddDaysToDate(DateTime start, double days) => start.Date.AddDays(value: days);

	/// <summary>Calculates the age in days based on a birth date relative to today.</summary>
	/// <param name="birthDate">The birth date.</param>
	/// <returns>The age in days.</returns>
	public static double CalculateAgeInDays(DateTime birthDate) =>
		CalculateDifferenceInDays(start: birthDate, end: DateTime.Today);

	/// <summary>Returns the day number of the year for a given date.</summary>
	/// <param name="date">The date.</param>
	/// <returns>The day number of the year.</returns>
	public static int GetDayOfYear(DateTime date) => date.DayOfYear;

	/// <summary>Calculates a resulting date by adding or subtracting a given number of years, months and days to/from a start date.</summary>
	/// <param name="start">The start date.</param>
	/// <param name="years">The number of years to add or subtract.</param>
	/// <param name="months">The number of months to add or subtract.</param>
	/// <param name="days">The number of days to add or subtract.</param>
	/// <param name="isFuture">If <see langword="true"/>, the values are added to the start date (future); otherwise they are subtracted (past).</param>
	/// <returns>The resulting date.</returns>
	public static DateTime CalculateDateFromYearsMonthsDays(DateTime start, int years, int months, int days, bool isFuture)
	{
		// Determine the sign based on the direction (future = positive, past = negative)
		int sign = isFuture ? 1 : -1;
		return start.Date.AddYears(value: years * sign).AddMonths(months: months * sign).AddDays(value: days * sign);
	}
}
