using System;

namespace IntelliTestDemo
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("How is your name?");
			var name = Console.ReadLine();
			if (name.Length > 0)
			{
				Console.WriteLine($"Hello {name}!");
			}
		}
	}
}
