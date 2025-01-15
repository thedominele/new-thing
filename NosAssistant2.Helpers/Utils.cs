using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace NosAssistant2.Helpers;

public static class Utils
{
	public static int randomizeCoord(int coord, int range)
	{
		return new Random().Next(coord - range, coord + range);
	}

	public static int randomizeDelay(int delay)
	{
		if (delay == 0)
		{
			return 0;
		}
		int num = (int)((float)delay * 0.25f);
		int num2 = new Random().Next(delay - num, delay + num);
		if (num2 < 0)
		{
			num2 = 0;
		}
		return num2;
	}

	public static double CalculateDistance(Point point1, Point point2)
	{
		return Math.Sqrt(Math.Pow(point2.X - point1.X, 2.0) + Math.Pow(point2.Y - point1.Y, 2.0));
	}

	public static bool IsInSquare(Point point, Rectangle rect)
	{
		if (point.X >= rect.X && point.X <= rect.X + rect.Width && point.Y >= rect.Y)
		{
			return point.Y <= rect.Y + rect.Height;
		}
		return false;
	}

	public static void InvokeIfRequired(Control control, Action action)
	{
		if (control.InvokeRequired)
		{
			control.Invoke(action);
		}
		else
		{
			action();
		}
	}

	public static IEnumerable<Control> GetAllControls(this Control control)
	{
		IEnumerable<Control> enumerable = control.Controls.OfType<Control>();
		return enumerable.SelectMany((Control ctrl) => ctrl.GetAllControls()).Concat(enumerable);
	}

	public static bool IsValidEmail(string email)
	{
		if (string.IsNullOrWhiteSpace(email))
		{
			return false;
		}
		try
		{
			email = Regex.Replace(email, "(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200.0));
			return Regex.IsMatch(email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250.0));
		}
		catch (RegexMatchTimeoutException)
		{
			return false;
		}
		catch (ArgumentException)
		{
			return false;
		}
	}

	private static string DomainMapper(Match match)
	{
		string ascii = new IdnMapping().GetAscii(match.Groups[2].Value);
		return match.Groups[1].Value + ascii;
	}
}
