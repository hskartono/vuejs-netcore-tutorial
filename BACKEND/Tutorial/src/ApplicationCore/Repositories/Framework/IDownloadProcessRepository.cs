using Tutorial.ApplicationCore.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tutorial.ApplicationCore.Repositories
{
	public interface IDownloadProcessRepository : IAsyncRepository<DownloadProcess>
	{
	}
}