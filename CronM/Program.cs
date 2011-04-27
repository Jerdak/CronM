using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace CronM
{
	class Program
	{
		public CronFile CronSettings { get; set;}
		
		static void Main(string[] args)
		{
			if (args.Length != 0)
			{
				Console.WriteLine("Usage: CronM");
				return;
			}

			Console.WriteLine("\nWelcome to CronW (Cron for Windows)");
			Console.WriteLine("  - Currently ranges are [not working]\n");
			Console.WriteLine("  - Press 'q' and hit 'enter' to QUIT.");

			CronCore core = new CronCore();
			core.Start();

			
			
			while (Console.Read() != 'q') ;
		}

		
	}
}
