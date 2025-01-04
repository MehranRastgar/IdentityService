using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class OtpSms
{
  public async Task<string> SendOtpAsync(string mobileNumber)
  {
    var postData = new
    {
      to = mobileNumber,
    };

    var json = JsonConvert.SerializeObject(postData);
    var content = new StringContent(json, Encoding.UTF8, "application/json");

    using (var client = new HttpClient())
    {
      ConfigureHttpClient(client);

      try
      {
        var response = await client.PostAsync("https://console.melipayamak.com/api/send/otp/22af6bb1402f4f5f8acb456c135e5c59", content);
        response.EnsureSuccessStatusCode();
        var responseData = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<dynamic>(responseData);
        return result?.code;
      }
      catch (Exception ex)
      {
        // Log the general exception details
        Console.WriteLine($"Exception: {ex.Message}");
        throw new ApplicationException("Error sending OTP SMS", ex);
      }
    }
  }

  private void ConfigureHttpClient(HttpClient client)
  {
    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));
    client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("gzip"));
    client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("deflate"));
    client.DefaultRequestHeaders.AcceptEncoding.Add(new System.Net.Http.Headers.StringWithQualityHeaderValue("br"));
    client.DefaultRequestHeaders.Connection.Add("keep-alive");
  }
}