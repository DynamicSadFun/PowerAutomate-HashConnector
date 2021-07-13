using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FunctionHash
{
    public class Result
    {
        public string hashType { get; set; }
        public string message { get; set; }
        public string hashValue { get; set; }
    }
    public static class Hash
    {
        public static string StrAppend(byte[] result)
        {
            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        public static string RSA(string text)
        {
            CspParameters CSApars = new CspParameters();
            CSApars.KeyContainerName = "RsaKey";

            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(CSApars);
            
            byte[] byteText = Encoding.UTF8.GetBytes(text);
            byte[] byteEntry = rsa.Encrypt(byteText, false);

            return Convert.ToBase64String(byteEntry);
        }

        public static string SHA256Hash(string text)
        {
            var shaM = new SHA256Managed();
            shaM.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = shaM.Hash;

            return Hash.StrAppend(result);
        }

        public static string SHA512Hash(string text)
        {
            
            SHA512 shaM = new SHA512Managed();
            shaM.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = shaM.Hash;

            return Hash.StrAppend(result);
        }

        public static string MD5Hash(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
            byte[] result = md5.Hash;

            return Hash.StrAppend(result);
        }

        private static async Task<string> ReadRequestBodyAsync(HttpRequest request)
        {
            string bodyAsText = string.Empty;
            if (request.Body.CanSeek)
            {
                request.Body.Seek(0, SeekOrigin.Begin);
                using var reader = new StreamReader(request.Body);
                bodyAsText = await reader.ReadToEndAsync();
                request.Body.Seek(0, SeekOrigin.Begin);
            }
            return bodyAsText;
        }

        [FunctionName("Hash")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"C# HTTP trigger function processed a request. RequestQuery={req.Body}");
            var identity = req.HttpContext?.User?.Identity;
            log.LogInformation("IsAuthenticated: {isAuthenticated}", identity?.IsAuthenticated);
            log.LogInformation("Identity name: {name}", identity?.Name);
            log.LogInformation("AuthenticationType: {authenticationType}",
                identity?.AuthenticationType);

            var success = true;
            string json = string.Empty;
            string message = req.Query["message"];
            string hashtype = req.Query["hashtype"];
            string error = "Please pass a hashtype (md5/sha256/sha512/rsa) and the message which you want to encode in the request body";
            string responseMessage = string.Empty;
            var requestBodyContent = await Hash.ReadRequestBodyAsync(req);
            message = requestBodyContent != string.Empty ? requestBodyContent : message;

            try
            {
                switch (hashtype)
                {
                    case "sha256":
                        responseMessage = Hash.SHA256Hash(message);
                        break;
                    case "sha512":
                        responseMessage = Hash.SHA512Hash(message);
                        break;
                    case "rsa":
                        responseMessage = Hash.RSA(message);
                        break;
                    default:
                        responseMessage = Hash.MD5Hash(message);
                        break;
                }

                // if you wanna json return
                //json = JsonConvert.SerializeObject(new
                //{
                //    results = new List<Result>()
                //    {
                //        new Result { hashType = hashtype, message = message, hashValue = responseMessage}
                //    }
                //});
                //return new OkObjectResult(json);
            }
            catch
            {
                success = false;

                // if you wanna json return
                //json = JsonConvert.SerializeObject(new
                //{
                //    results = new List<Result>()
                //    {
                //        new Result { hashType = hashtype, message = message, hashValue = error}
                //    }
                //});
            }
            log.LogInformation(json);

            return success
                ? (ActionResult)new OkObjectResult(responseMessage)
                : new BadRequestObjectResult(error);

        }
    }
}
