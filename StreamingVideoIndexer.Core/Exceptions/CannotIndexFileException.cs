namespace StreamingVideoIndexer.Core.Exceptions;

public class CannotIndexFileException : Exception
{
    public CannotIndexFileException(string message) : base(message)
    { }
}
