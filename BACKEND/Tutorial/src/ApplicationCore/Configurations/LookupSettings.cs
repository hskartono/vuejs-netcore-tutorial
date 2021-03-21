using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore
{
	public class LookupSettings
	{
		public string baseUri { get; set; }
		public string downloadUri { get; set; }
		public string uploadUri { get; set; }
		public string baseStoragePath { get; set; }
		public string downloadPath { get; set; }
		public string logsPath { get; set; }
		public string tempPath { get; set; }
		public string templatePath { get; set; }
		public string uploadPath { get; set; }
	}
}
