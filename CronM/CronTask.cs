using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronM
{
	/**	Cron task descriptor
		@descrip	
			Simple task descriptor.
		@notes
			Originally the descriptor only handled individual values per minute/hour/etc...
			Those variables are still in the descriptor, for now.
	*/
	class CronTask
	{
		//Task identifier
		public int UniqueID { get; set; }
		
		//Minutes
		public List<String> Minutes			{ get; set; }

		//Hours
		public List<String> Hours			{ get; set; }
		
		//Days of the Month
		public List<String> DaysOfMonth		{ get; set; }
		
		//Months
		public List<String> Months			{ get; set; }
		
		//Days of the Week
		public List<String> DaysOfWeek		{ get; set; }

		//User name (unused in Windows, here to match Cron format)
		public string User { get; set; }

		//Raw Command
		public string Command { get; set; }

		//Time task last run.
		public DateTime LastRun { get; set; }



		public string Minute { get; set; }
		public string Hour { get; set; }
		public string DayOfMonth { get; set; }
		public string Month { get; set; }
		public string DayOfWeek { get; set; }

		
	}
}
