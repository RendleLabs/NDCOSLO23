using System.Net;
using System.Runtime.Serialization;

namespace DunDat.Clients;

[Serializable]
public class ApiException : Exception
{
    private const string DefaultMessage = "Error response received from API";

    public int Status { get; }

    public string ResponseText { get; }
    //
    // For guidelines regarding the creation of new exception types, see
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
    // and
    //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
    //

    public ApiException(HttpStatusCode statusCode) : this((int)statusCode)
    {
        
    }

    public ApiException(HttpStatusCode statusCode, string responseText) : this((int)statusCode, responseText)
    {
        
    }

    public ApiException(int status) : base(DefaultMessage)
    {
        Status = status;
        ResponseText = string.Empty;
    }

    public ApiException(int status, string responseText) : base(DefaultMessage)
    {
        Status = status;
        ResponseText = responseText;
    }

    public ApiException(int status, string responseText, Exception inner) : base(DefaultMessage, inner)
    {
        Status = status;
        ResponseText = responseText;
    }

    protected ApiException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
    }
}