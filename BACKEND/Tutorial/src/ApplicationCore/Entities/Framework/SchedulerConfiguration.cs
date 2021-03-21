using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.ApplicationCore.Entities
{
	public class SchedulerConfiguration : BaseEntity
	{
		public int Id { get; set; }
		public string IntervalType { get; set; }
		public int IntervalValue { get; set; }
		public int IntervalValue2 { get; set; }
		public int IntervalValue3 { get; set; }
		public string CronExpression { get; set; }
		public string JobType { get; set; }
		public string RecurringJobId { get; set; }

	}
}
