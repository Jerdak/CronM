using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CronM
{
	//@todo:  Make lists out of CronTask properties so that the user can specify comma delimited ranges.  (0,15,30,35 * * * * /cmd.exe)  Should run every 15 minutes.
	class CronTask
	{
		public int UniqueID { get; set; }
		public string Minute { get; set; }
		public string Hour { get; set; }
		public string DayOfMonth { get; set; }
		public string Month { get; set; }
		public string DayOfWeek { get; set; }

		public string User		{ get; set; }
		public string Command	{ get; set; }

		public DateTime LastRun { get; set; }
	}
}
