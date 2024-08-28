using System;

namespace CFNReader;

public class CFNStreamReaderException(string message, Exception? innerException = null)
    : Exception(message, innerException)
{ }
