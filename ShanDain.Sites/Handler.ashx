<%@ WebHandler Language="C#" Class="Handler" %>

using System;
using System.Web;

public class Handler : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/plain";
        context.Response.Write("{\"VersionCode\":\"2.0.0.0\",\"UpdateLog\":\"aaaaaaa\",\"DownloadUrl\":\"http://localhost:8856/SDSetup.zip\"}");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}