using System;
using System.Linq;

namespace IntelliTestDemo
{
	public class Versions
	{
		public static int Compare(string version1, string version2)
		{
			var v1 = version1.Split('.');
			var v2 = version2.Split('.');
			for (int i = 0; i < Math.Max(v1.Length, v2.Length); i++)
			{
				int v1i, v2i;
				if (i < v1.Length)
				{
					if (!v1[i].Any())
					{
						throw new FormatException();
					}
					v1i = int.Parse(v1[i]);
				}
				else
				{
					v1i = 0;
				}
				if (i < v2.Length)
				{
					if (!v2[i].Any())
					{
						throw new FormatException();
					}
					v2i = int.Parse(v2[i]);
				}
				else
				{
					v2i = 0;
				}
				if (v1i > v2i)
				{
					return 1;
				}
				else if (v1i < v2i)
				{
					return -1;
				}
			}
			return 0;
		}
	}
}
