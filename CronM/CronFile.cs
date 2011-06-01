using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
namespace CronM
{
	/** Cron settings file
		@desrip
			CronFile handles parsing the Cron settings.  I tried to use the existing Cron format
			except that we don't parse ranges, yet.
	*/
	class CronFile
	{
		//Raw file
		public FileInfo File { get; set; }

		//Tasks
		public List<CronTask> Tasks { get; set; }
		
		//DateTime last modified
		public DateTime LastModified { get; set; }

		/**	Construtor
		 *  @input
		 *		fileName - Cron settings file name
		*/
		public CronFile(String fileName)
		{
			Tasks = new List<CronTask>();
			File = new FileInfo(fileName);
		}

		///Utility function to remove inline |white space| > 1.
		private string RemoveDuplicateWhiteSpace(string input)
		{   
			return Regex.Replace(input, @"[\s]+", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		}


		public bool Load()
		{
			Tasks.Clear();
			if (!File.Exists) return false;
			
			using (StreamReader sr = File.OpenText())
			{
				int uid = 1;
				int lineCount = 1;
				while (!sr.EndOfStream)
				{
					//Strip line of inline white space
					String line = RemoveDuplicateWhiteSpace(sr.ReadLine().Trim());
					
					//Match crontab format
					Regex rgx = new Regex(@"\A(?:(?:[0-9*]+|,[0-9]+)+\s+){5}[a-zA-Z0-9\.]+\s+.+", RegexOptions.Singleline);
					Match m = rgx.Match(line);
					if (m.Success)
					{
						try
						{
							String[] tokens = line.Split(' ');
							CronTask task = new CronTask { Minute = tokens[0].Trim(), Hour = tokens[1].Trim(), DayOfMonth = tokens[2].Trim(), Month = tokens[3].Trim(), DayOfWeek = tokens[4].Trim(), User = tokens[5].Trim() };
							StringBuilder command = new StringBuilder();
							for (int i = 6; i < tokens.Length; i++)
								command.Append(tokens[i].Trim() + ' ');
							task.Command = command.ToString();
							task.UniqueID = uid++;
							task.LastRun = new DateTime(2010, 1, 1);	//arbitrary date set to some time at least 1 day in time.  Allows first execution of a task that might be in the middle of a CronCore cycle
							Tasks.Add(task);
						}
						catch (Exception ex)
						{
							Console.WriteLine("Warning, poorly formed crontab line[" + lineCount.ToString() + "]: " + line);
						}
					}
					else
					{
						
						if(line.Length >0)if(line[0] != '#')Console.WriteLine("Warning, poorly formed crontab line[" + lineCount.ToString() + "]: " + line);
					}
					lineCount++;
				}
				
			}
			//Last modified time is the last time this file was written to
			LastModified = File.LastWriteTime;

			return true;
		}
		public void DebugTasks()
		{
			Console.WriteLine("Tasks:");
			foreach (CronTask t in Tasks)
			{
				Console.WriteLine("" + t.Minute + " " + t.Hour + " " + t.DayOfMonth + " " + t.Month + " " + t.DayOfWeek + " " + t.User + " " + t.Command + "");
				Console.WriteLine("   Command: " + t.Command);
			}
		}
	}
}
