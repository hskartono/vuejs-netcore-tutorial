using Tutorial.ApplicationCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tutorial.Infrastructure
{
	public class UriComposer : IUriComposer
	{
		private readonly LookupSettings _lookupSettings;
		private readonly string _basePath = AppDomain.CurrentDomain.BaseDirectory;
		public UriComposer(LookupSettings lookupSettings) => _lookupSettings = lookupSettings;

		public string ComposeBaseUri(string url)
		{
			var uri = new Uri(_lookupSettings.baseUri);
			var baseUri = new Uri(uri, url);
			return baseUri.AbsoluteUri;
		}

		public string ComposeDownloadUri(string url)
		{
			var completeUrl = ComposeBaseUri(_lookupSettings.downloadUri);
			var completeUri = new Uri(completeUrl);
			var downloadUri = new Uri(completeUri, url);
			return downloadUri.AbsoluteUri;
		}

		public string ComposeUploadUri(string url)
		{
			var completeUrl = ComposeBaseUri(_lookupSettings.uploadUri);
			var completeUri = new Uri(completeUrl);
			var downloadUri = new Uri(completeUri, url);
			return downloadUri.AbsoluteUri;
		}

		public string ComposeBaseStoragePath(string path)
		{
			return Path.Combine(_basePath, _lookupSettings.baseStoragePath.Replace("~", _basePath), path);
		}

		public string ComposeDownloadPath(string path)
		{
			return Path.Combine(_lookupSettings.downloadPath.Replace("~", _basePath), path);
		}

		public string ComposeLogsPath(string path)
		{
			return Path.Combine(_lookupSettings.logsPath.Replace("~", _basePath), path);
		}

		public string ComposeTemplatePath(string path)
		{
			return Path.Combine(_lookupSettings.templatePath.Replace("~", _basePath), path);
		}

		public string ComposeTempPath(string path)
		{
			return Path.Combine(_lookupSettings.tempPath.Replace("~", _basePath), path);
		}

		public string ComposeUploadPath(string path)
		{
			return Path.Combine(_lookupSettings.uploadPath.Replace("~", _basePath), path);
		}
	}
}
