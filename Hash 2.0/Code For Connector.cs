public class Script : ScriptBase
{
	private static readonly Regex _regex = new Regex(@"[?&](\w[\w.]*)=([^?&]+)");
	public static IReadOnlyDictionary<string, string> ParseQueryString(Uri uri)
	{
		var match = _regex.Match(uri.PathAndQuery);
		var parameters = new Dictionary<string, string>();
		while (match.Success)
		{
			parameters.Add(match.Groups[1].Value, match.Groups[2].Value);
			match = match.NextMatch();
		}
		return parameters;
	}
	
	public override async Task<HttpResponseMessage> ExecuteAsync()
	{
		return await this.HandleHashOperation().ConfigureAwait(false);
	}

	private async Task<HttpResponseMessage> HandleHashOperation()
	{
        	string responseMessage = string.Empty;
		HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
		var queryString = ParseQueryString(this.Context.Request.RequestUri);
		var message = queryString.First(x => x.Key == "message").Value;
		var hashtype = queryString.First(x => x.Key == "hashtype").Value;
		switch (hashtype)
		{
			case "sha1":
				responseMessage = Script.SHA1Hash(message);
				break;
			case "sha256":
				responseMessage = Script.SHA256Hash(message);
				break;
			case "sha512":
				responseMessage = Script.SHA512Hash(message);
				break;
			default:
				responseMessage = Script.MD5Hash(message);
				break;
		}
		var responseContent = new JObject
		{
			["hashValue"] = responseMessage,
			["message"] = message,
			["hashType"] = hashtype
		};

		response.Content = CreateJsonContent(responseContent.ToString());
		return response;
	}	

	public static string StrAppend(byte[] result)
	{
		StringBuilder strBuilder = new StringBuilder();
		for (int i = 0; i < result.Length; i++)
		{
			strBuilder.Append(result[i].ToString("x2"));
		}
		return strBuilder.ToString();
	}
	/// <summary>
	/// Sha1 hash algorithm
	/// </summary>
	/// <param name="text">the message you want to encrypt</param>
	/// <returns>sha1 hash</returns>
	public static string SHA1Hash(string text)
	{
		var shaM = new SHA1Managed();
		shaM.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
		byte[] result = shaM.Hash;

		return Script.StrAppend(result);
	}
	/// <summary>
	/// Sha256 hash algorithm
	/// </summary>
	/// <param name="text">the message you want to encrypt</param>
	/// <returns>sha256 hash</returns>
	public static string SHA256Hash(string text)
	{
		var shaM = new SHA256Managed();
		shaM.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
		byte[] result = shaM.Hash;

		return Script.StrAppend(result);
	}
	/// <summary>
	/// Sha512 hash algorithm
	/// </summary>
	/// <param name="text">the message you want to encrypt</param>
	/// <returns>sha512 hash</returns>
	public static string SHA512Hash(string text)
	{		
		SHA512 shaM = new SHA512Managed();
		shaM.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
		byte[] result = shaM.Hash;

		return Script.StrAppend(result);
	}

	/// <summary>
	/// MD5 hash algorithm
	/// </summary>
	/// <param name="text">the message you want to encrypt</param>
	/// <returns>MD5 hash</returns>
	public static string MD5Hash(string text)
	{
		MD5 md5 = new MD5CryptoServiceProvider();
		md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));
		byte[] result = md5.Hash;

		return Script.StrAppend(result);
	}	
}
