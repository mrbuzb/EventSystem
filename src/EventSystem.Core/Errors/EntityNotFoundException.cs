using System.Runtime.Serialization;

namespace EventSystem.Core.Errors;

[Serializable]
public class EntityNotFoundException : BaseException
{
    public EntityNotFoundException() { }
    public EntityNotFoundException(String message) : base(message) { }
    public EntityNotFoundException(String message, Exception inner) : base(message, inner) { }
    protected EntityNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
