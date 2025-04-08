HttpClient client = new();

var response = await client.GetAsync("https://www.apple.com/");

WriteLine("Home page has {0:N0} bytes", response.Content.Headers.ContentLength);
