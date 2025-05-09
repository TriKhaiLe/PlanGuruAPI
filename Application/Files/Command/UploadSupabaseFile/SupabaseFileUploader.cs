using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;

namespace Application.Files.Command.UploadSupabaseFile
{
    public class SupabaseFileUploader
    {
        private readonly HttpClient _httpClient;
        private readonly string _supabaseUrl;
        private readonly string _supabaseBucket;
        private readonly string _supabaseApiKey;

        public SupabaseFileUploader(IConfiguration configuration, HttpClient httpClient)
        {
            _httpClient = httpClient;
            _supabaseUrl = configuration["Supabase:Url"];
            _supabaseBucket = configuration["Supabase:Bucket"];
            _supabaseApiKey = configuration["Supabase:ApiKey"];
        }

        public async Task<string> UploadFileAsync(byte[] fileData, string fileName)
        {
            var url = $"{_supabaseUrl}/storage/v1/object/{_supabaseBucket}/{fileName}";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _supabaseApiKey);

            request.Content = new ByteArrayContent(fileData);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"File upload failed: {response.ReasonPhrase}");
            }

            return $"{_supabaseUrl}/storage/v1/object/public/{_supabaseBucket}/{fileName}";
        }
    }
}