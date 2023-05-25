using System.Runtime.Serialization;

namespace DunDatApi.Data;

[Serializable]
public class ConflictException : Exception
{
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public ConflictException()
    {
    }

    public ConflictException(string message) : base(message)
    {
    }

    public ConflictException(string message, Exception inner) : base(message, inner)
    {
    }

    protected ConflictException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}