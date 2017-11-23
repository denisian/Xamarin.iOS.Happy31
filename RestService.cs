using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Happy31
{
    public class RestService
    {
        HttpClient httpClient;
        Uri serverUri;
        Uri phpFileUri;

        public string Message { get; private set; }

        public RestService()
        {
            serverUri = new Uri($"http://52.23.248.244/happy31/");
            //httpClient = new HttpClient
            //{
            //    MaxResponseContentBufferSize = 256000
            //};
        }

        public async Task<JsonResponseModel> PostJsonDataAsync(UsersModel user, string action)
        {
            try
            {
                // Serialize UserModel class into a JSON String
                var jsonUserSerialize = await Task.Run(() => JsonConvert.SerializeObject(user));

                // Wrap JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(jsonUserSerialize, Encoding.UTF8, "application/json");

                Console.WriteLine(jsonUserSerialize);

                using (httpClient = new HttpClient())
                {
                    if (action == "login")
                        phpFileUri = new Uri(serverUri, "userlogin.php");
                    else if (action == "register")
                        phpFileUri = new Uri(serverUri, "userregister.php");

                    // Send JSON using HTTP request and receiving HTTP response
                    HttpResponseMessage response = await httpClient.PostAsync(phpFileUri, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        // If the response contains content, read it
                        if (response.Content != null)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();

                            Console.WriteLine(responseContent);

                            // Deserialize JSON
                            var jsonUserDeserialize = await Task.Run(() => JsonConvert.DeserializeObject<JsonResponseModel>(responseContent));

                            return jsonUserDeserialize;
                        }
                        else
                        {
                            Message = "HTTP Content is empty";
                            return new JsonResponseModel() { Message = this.Message};
                        }
                    }
                    else
                    {
                        Message = "HTTP Response Error\n" + new HttpResponseMessage(HttpStatusCode.NotModified);
                        return new JsonResponseModel() { Message = this.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return new JsonResponseModel() { Message = this.Message };
            }
        }

        // Sync prompts with server
        public async Task<List<UsersPromptsModel>> SyncUserPromptsJson(IEnumerable<UsersPromptsModel> userPrompt)
        {
            try
            {
                // Serialize user id into a JSON String
                var jsonUserSerialize = await Task.Run(() => JsonConvert.SerializeObject(userPrompt));

                // Wrap JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(jsonUserSerialize, Encoding.UTF8, "application/json");

                Console.WriteLine("INPUT content:" + jsonUserSerialize);

                using (httpClient = new HttpClient())
                {
                    phpFileUri = new Uri(serverUri, "syncuserprompts.php");

                    // Send JSON using HTTP request and receiving HTTP response
                    HttpResponseMessage response = await httpClient.PostAsync(phpFileUri, httpContent);

                    if (response.IsSuccessStatusCode)
                    {
                        // If the response contains content, read it
                        if (response.Content != null)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();

                            Console.WriteLine("OUTPUT content:" + responseContent);

                            // Deserialize JSON
                            var jsonDataDeserialize = await Task.Run(() => JsonConvert.DeserializeObject<List<UsersPromptsModel>>(responseContent));

                            return jsonDataDeserialize;
                        }
                        else
                        {
                            Message = "HTTP Content is empty";
                            return null;
                        }
                    }
                    else
                    {
                        Message = "HTTP Response Error\n" + new HttpResponseMessage(HttpStatusCode.NotModified);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }

        // Retrieve Table Prompts from the server
        public async Task<List<PromptsModel>> RetrieveTablePromptsJson()
        {
            try
            {
                using (httpClient = new HttpClient())
                {
                    phpFileUri = new Uri(serverUri, "gettableprompts.php");

                    // Send JSON using HTTP request and receiving HTTP response
                    HttpResponseMessage response = await httpClient.GetAsync(phpFileUri);

                    if (response.IsSuccessStatusCode)
                    {
                        // If the response contains content, read it
                        if (response.Content != null)
                        {
                            var responseContent = await response.Content.ReadAsStringAsync();

                            //string test = "{\"prompt_id\":[\"1\",\"2\"],\"prompt_category\":[\"Cognitive\",\"Social\"],\"prompt_task\":[\"Find something\",\"HHH\"]}";
                           //  "{\"prompt_id\":\"2\",\"prompt_category\":\"Cognitive\",\"prompt_task\":\"Take picture of somethi\"}]";

                            Console.WriteLine("Content:" + responseContent);

                            // Deserialize JSON
                            var jsonPromptsDeserialize = await Task.Run(() => JsonConvert.DeserializeObject<List<PromptsModel>>(responseContent));

                            return jsonPromptsDeserialize;
                        }
                        else
                        {
                            Message = "HTTP Content is empty";
                            return null;
                        }
                    }
                    else
                    {
                        Message = "HTTP Response Error\n" + new HttpResponseMessage(HttpStatusCode.NotModified);
                        return null;
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return null;
            }
        }
    }
}