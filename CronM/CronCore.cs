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
	/**	Cron core
		@descrip
			The core is reponsible for running the task timer.  Cron tasks
			are evaluated every 5 seconds.  This can probably be relaxed as
			the resolution of the original Cron is 1 minute.

			Tasks are spawned in a self-closing cmd shell, error catching is
			non-existent.  :) 
		@todo
			- Allow for the evaluation of time ranges per task.  Currently only
				a single value is allowed per column in the task description.
	*/
	class CronCore
	{
		//Task match bit flags
		enum TaskMatch { Unset=0x00,Minute = 0x01, Hour = 0x02, DayOfMonth = 0x04, Month = 0x08, DayOfWeek = 0x10 ,Complete = 0x1F};

		//Cron settings file
		public CronFile CronSettings { get; set; }

		//Main timer
		private System.Timers.Timer Timer { get; set; }
		
		//Configuration Timer
		private System.Timers.Timer ConfigTimer { get; set; }

		//Currently active tasks (unused)
		private Dictionary<int,CronTask>  ActiveTasks{ get; set; }

		public CronCore()
		{
			CronSettings = new CronFile("./crontab");
			CronSettings.Load();
			CronSettings.DebugTasks();

			ActiveTasks = new Dictionary<int, CronTask>();
		}

		//Initialize Timers
		public void Start()
		{
			{	//Start worker thread (updates every second)
				Timer = new System.Timers.Timer();
				Timer.Elapsed += new ElapsedEventHandler(this.OnTimerEvent);
				Timer.Interval = 1000;
				Timer.Enabled = true;
			}

			{	//Configuration timer(updates every 10 seconds)
				ConfigTimer = new System.Timers.Timer();
				ConfigTimer.Elapsed += new ElapsedEventHandler(this.OnConfigurationEvent);
				ConfigTimer.Interval = 1000;
				ConfigTimer.Enabled = true;
			}
		}

		static void ParseMatch(String value, int expected, ref TaskMatch ret,TaskMatch match){
			if (value[0] == '*')
			{
				ret |= match;
				return;
			}
			String []values = value.Split(',');
			foreach (String val in values)
			{
				int iVal = Convert.ToInt32(val);
				if (iVal == expected)
				{
					ret |= match;
					return;
				}
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
			ParseMatch(task.Hour,	dt.Hour, ref ret, TaskMatch.Hour);
			ParseMatch(task.DayOfMonth, dt.Day, ref ret, TaskMatch.DayOfMonth);
			ParseMatch(task.Month, dt.Month, ref ret, TaskMatch.Month);
			ParseMatch(task.DayOfWeek, (int)dt.DayOfWeek, ref ret, TaskMatch.DayOfWeek);
			return ret;
			
		}
		void OnConfigurationEvent(object source, ElapsedEventArgs args)
		{
			CronSettings.File.Refresh();
			if (CronSettings.LastModified != CronSettings.File.LastWriteTime)
			{
				Console.WriteLine("Cron settings have changed, reloading tasks.");
				Timer.Stop();
				CronSettings.Load();
				Timer.Start();
			}
		}
		void OnTimerEvent(object source, ElapsedEventArgs args)
		{
			foreach (CronTask task in CronSettings.Tasks)
			{
				TaskMatch completeMask = TaskMatch.Complete;/// TaskMatch.Minute | TaskMatch.Hour | TaskMatch.Month | TaskMatch.DayOfMonth | TaskMatch.DayOfWeek;
				TaskMatch final = MatchTasks(task);
				if ((final & completeMask) == completeMask)
				{
					Console.WriteLine("Running Cmd[" + task.UniqueID.ToString() + "]: " + task.Command);
					Process proc = new Process();
					proc.StartInfo.UseShellExecute = false;
					proc.StartInfo.CreateNoWindow = false;
					proc.StartInfo.FileName = "C:\\Windows\\System32\\cmd.exe";
					proc.StartInfo.Arguments = " /C \"" + task.Command + "\"";
					proc.Start();

					task.LastRun = DateTime.Now;
					
					//Floor to last valid minute(max resolution of timer)
					TimeSpan tmp = new TimeSpan(0,0,task.LastRun.Second);
					task.LastRun -= tmp;
				}
			}
		}
	}
}
