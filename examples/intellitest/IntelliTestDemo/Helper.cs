using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace IntelliTestDemo
{
	public class Helper
	{
		static IEnumerable<string> MatchRegex(string input)
		{
			var match = Regex.Match(input, @"(\d)+\.(\d+)");
			//var match = Regex.Match(input, @"(^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$)");
			if (!match.Success)
			{
				return null;
			}
			return match.Groups.Cast<Group>().Select(g => g.Value).ToArray();
		}

		static bool MatchHash(string arg)
		{
			var hash = string.Join(
				"",
				System.Security.Cryptography.MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(arg))
					.Select(b => b.ToString("x2"))
			);

			if (hash == "5d41402abc4b2a76b9719d911017c592")
			{
				return true;
			}
			return false;
		}
	}
}
