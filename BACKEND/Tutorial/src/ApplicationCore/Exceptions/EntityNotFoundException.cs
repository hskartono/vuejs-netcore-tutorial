using System;

namespace Tutorial.ApplicationCore.Exceptions
{
	public class EntityNotFoundException : Exception
	{
		public EntityNotFoundException() { }
		public EntityNotFoundException(string message) : base(message) { }
		public EntityNotFoundException(string message, Exception exception):base(message, exception) { }
		public EntityNotFoundException(string name, object key) : base($"Entity \"{name}\" ({key}) was not found.") { }
	}
}
