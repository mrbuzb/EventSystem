using System.Runtime.Serialization;

namespace EventSystem.Core.Errors;

[Serializable]
public class BaseException : Exception
{
    public BaseException() { }
    public BaseException(String message) : base(message) { }
    public BaseException(String message, Exception inner) : base(message, inner) { }
    protected BaseException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}