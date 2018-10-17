namespace Clamp.Webwork
{
    public class NotFoundResponse : Response
    {
        public NotFoundResponse()
        {
            this.ContentType = "text/html";
            this.StatusCode = HttpStatusCode.NotFound;
        }
    }
}