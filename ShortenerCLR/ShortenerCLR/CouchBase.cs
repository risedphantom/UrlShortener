using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;
using Microsoft.SqlServer.Server;

public class CouchBase
{
    private static string _webApiUrl = @"http://vApiProd.example.com:9080";

    [SqlFunction]
    public static SqlString GetShortUrl(string longUrl)
    {
        return HttpGetText(_webApiUrl + @"/api/rest/getShortUrl?longUrl=" + Uri.EscapeDataString(longUrl));
    }

    [SqlFunction]
    public static SqlString GetLongUrl(string shortUrl)
    {
        return HttpGetText(_webApiUrl + @"/api/rest/getLongUrl?shortUrl=" + Uri.EscapeDataString(shortUrl));
    }

    private static string HttpGetText(string webApiUrl)
    {
        try
        {
            var request = (HttpWebRequest)WebRequest.Create(webApiUrl);
            request.Method = "GET";
            request.UserAgent = "SQL Server 2012 CLR web client";
            request.Accept = "text/plain";

            var response = (HttpWebResponse)request.GetResponse();
            var reader = new StreamReader(response.GetResponseStream());
            var data = reader.ReadToEnd().Trim('"');

            return data;
        }
        catch (WebException ex)
        {
            var reader = new StreamReader(ex.Response.GetResponseStream());
            var data = reader.ReadToEnd();

            return string.Format("[ERROR] - {0}", data);
        }
        catch (Exception ex)
        {
            return string.Format("[ERROR] - Exception at HttpGetText: {0}", ex.Message);
        }
    }
}
