using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Diagnostics;
using System.ComponentModel;

namespace CronM
{
	class CronCore
	{
		enum TaskMatch { Unset=0x00,Minute = 0x01, Hour = 0x02, DayOfMonth = 0x04, Month = 0x08, DayOfWeek = 0x10 ,Complete = 0x1F};

		public CronFile CronSettings { get; set; }
		public System.Timers.Timer Timer { get; set; }
		private Dictionary<int,CronTask>  ActiveTasks{ get; set; }

		public CronCore()
		{
			CronSettings = new CronFile();
			CronSettings.Load("./cron.txt");
			CronSettings.DebugTasks();

			ActiveTasks = new Dictionary<int, CronTask>();
		}

		public void Start()
		{
			{	//Start worker thread
				Timer = new System.Timers.Timer();
				Timer.Elapsed += new ElapsedEventHandler(this.OnTimerEvent);
				Timer.Interval = 1000;
				Timer.Enabled = true;
			}
		}
		static void ParseMatch(String value, int expected, ref TaskMatch ret,TaskMatch match){
			if (value[0] == '*')
			{
				ret |= match;
				return;
			}

			int val = Convert.ToInt32(value);
			if (val == expected)
			{
				ret |= match;
			}
			return;
		}
		static TaskMatch MatchTasks(CronTask task)
		{
			//enum TaskMatch { Unset=0x00,Minute = 0x01, Hour = 0x02, DayOfMonth = 0x04, Month = 0x08, DayOfWeek = 0x16 ,Complete = 0x31};
			TaskMatch ret = TaskMatch.Unset;
			DateTime dt = DateTime.Now;// new DateTime(2011, 4, 26, 10, 17, 0);// DateTime.Today;

			//Console.WriteLine("Diff: " + ((TimeSpan)(dt - task.LastRun)).TotalSeconds.ToString());
			if ((dt - task.LastRun).TotalSeconds < 60) return ret;
			ParseMatch(task.Minute, dt.Minute,ref ret, TaskMatch.Minute);
			//Console.WriteLine("Minutes: " + task.Minute + "," +dt.Minute);
			
			ParseMatch(task.Hour,	dt.Hour, ref ret, TaskMatch.Hour);
			ParseMatch(task.DayOfMonth, dt.Day, ref ret, TaskMatch.DayOfMonth);
			ParseMatch(task.Month, dt.Month, ref ret, TaskMatch.Month);
			ParseMatch(task.DayOfWeek, (int)dt.DayOfWeek, ref ret, TaskMatch.DayOfWeek);
			return ret;
			
		}
		void OnProcessEvent(object source, ElapsedEventArgs args)
		{

		}
		void OnTimerEvent(object source, ElapsedEventArgs args)
		{
			foreach (CronTask task in CronSettings.Tasks)
			{
				TaskMatch completeMask = TaskMatch.Complete;/// TaskMatch.Minute | TaskMatch.Hour | TaskMatch.Month | TaskMatch.DayOfMonth | TaskMatch.DayOfWeek;
				TaskMatch final = MatchTasks(task);
				if ((final & completeMask) == completeMask)
				{
					Console.WriteLine("Valid Task[" + task.UniqueID.ToString() + "]: " + task.Command);
					Process proc = new Process();
					proc.StartInfo.UseShellExecute = false;
					proc.StartInfo.CreateNoWindow = false;
					proc.StartInfo.FileName = "C:\\Windows\\System32\\cmd.exe";
					proc.StartInfo.Arguments = " /C \"" + task.Command + "\"";
					proc.Start();

					task.LastRun = DateTime.Now;
				}
				//if (test) Console.WriteLine("Valid Task["+task.UniqueID.ToString() + "]: "  + task.Command);
				//Console.WriteLine("Complete: " + completeMask.ToString() + "," + ((int)TaskMatch.Complete).ToString());
				
				//if (Convert.ToInt32(task.Minute) == dt.Minute || task.Minute[0] == '*')
				//{
				//	Console.WriteLine("Running Task: " + task.Command);
				//}
			}
		}
	}
}
