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
	 	@Cron Format
	 	   See existing cron settings file for format and examples 
	*/
	class CronTask
	{
		//Task identifier
		public int UniqueID { get; set; }
		
		//User name (unused in Windows, here to match Cron format)
		public string User { get; set; }

		//Raw Command
		public string Command { get; set; }

		//Time task last run.
		public DateTime LastRun { get; set; }

		//Minute
		public string Minute { get; set; }
		
		//Hour
		public string Hour { get; set; }
		
		//Day of month
		public string DayOfMonth { get; set; }
		
		//Month
		public string Month { get; set; }
		
		//Day of week
		public string DayOfWeek { get; set; }
	}
}
