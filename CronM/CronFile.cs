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
		//Tasks
		public List<CronTask> Tasks { get; set; }
		
		public CronFile()
		{
			Tasks = new List<CronTask>();	
		}

		///Utility function to remove inline |white space| > 1.
		private string RemoveDuplicateWhiteSpace(string input)
		{   
			return Regex.Replace(input, @"[\s]+", " ", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		}


		public bool Load(String fileName)
		{
			Tasks.Clear();

			FileInfo file = new FileInfo(fileName);
			if (!file.Exists) return false;

			using (StreamReader sr = file.OpenText())
			{
				int uid = 1;
				while (!sr.EndOfStream)
				{
					String line = RemoveDuplicateWhiteSpace(sr.ReadLine().Trim());
					String []tokens = line.Split(' ');

					if (line.Length > 0 && line[0] != '#')
					{
						if (tokens.Length < 7)
						{
							Console.WriteLine("Warning, poorly formed cron line: %s\n", line);
						}
						else
						{
							CronTask task = new CronTask { Minute = tokens[0].Trim(), Hour = tokens[1].Trim(), DayOfMonth = tokens[2].Trim(), Month = tokens[3].Trim(), DayOfWeek = tokens[4].Trim(), User = tokens[5].Trim() };
							StringBuilder command = new StringBuilder();
							for (int i = 6; i < tokens.Length; i++)
								command.Append(tokens[i].Trim() + ' ');
							task.Command = command.ToString();
							task.UniqueID = uid++;
							task.LastRun = new DateTime(2010, 1, 1);	//arbitrary date set to some time at least 1 day or more back in time.  Allows first execution of a task that might be in the middle of its cycle
							Tasks.Add(task);
						}
					}
				}
			}
			return false;
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
