//
//  RestService.cs
//  Happy31.iOSApp
//
//  Copyright © 2017 Denis Klyucherov. All rights reserved.
//

using Plugin.Connectivity;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Happy31
{
    /// <summary>
    /// RESTful service using JSON to interchange data between local and remote databases
    /// </summary>
    public class RestService
    {
        HttpClient httpClient;
        Uri serverUri;
        Uri phpFileUri;

        public string Message { get; private set; }

        public RestService()
        {
            serverUri = new Uri($"http://52.23.248.244/happy31/");
        }

        // Login user and register using Facebook
        public async Task<UsersModel> UserLoginAndRegisterJson(UsersModel user, string action)
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected || !await CrossConnectivity.Current.IsReachable(serverUri.Host.ToString(), 3000))
                {
                    Message = "There was a network problem. Please, try again later";
                    return new UsersModel() { Status = "Error", Message = this.Message };
                }

                var user_tmp = new UsersModel();
                // Disable sending Facebook's avatar to server
                if (user.LoginProvider == "Facebook")
                {
                    user_tmp.Id = user.Id;
                    user_tmp.FirstName = user.FirstName;
                    user_tmp.LastName = user.LastName;
                    user_tmp.Email = user.Email.ToLower();
                    user_tmp.Password = user.Password;
                    user_tmp.Avatar = null;
                    user_tmp.LoginProvider = user.LoginProvider;
                    user_tmp.CreatedAt = user.CreatedAt;
                }
                else
                    user_tmp = user;

                // Serialize UserModel class into a JSON String
                var jsonUserSerialize = await Task.Run(() => JsonConvert.SerializeObject(user_tmp));

                // Wrap JSON inside a StringContent which then can be used by the HttpClient class
                var httpContent = new StringContent(jsonUserSerialize, Encoding.UTF8, "application/json");

                Console.WriteLine("INPUT content:" + jsonUserSerialize);

                using (httpClient = new HttpClient())
                {
                    if (action == "login")
                        phpFileUri = new Uri(serverUri, "userlogin.php");
                    else if (action == "register")
                        phpFileUri = new Uri(serverUri, "userregister.php");
                    else if (action == "update")
                        phpFileUri = new Uri(serverUri, "userupdate.php");

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
                            var jsonUserDeserialize = await Task.Run(() => JsonConvert.DeserializeObject<UsersModel>(responseContent));

                            return jsonUserDeserialize;
                        }
                        else
                        {
                            Message = "HTTP Content is empty";
                            return new UsersModel() { Status = "Error", Message = this.Message };
                        }
                    }
                    else
                    {
                        Message = "HTTP Response Error\n" + new HttpResponseMessage(HttpStatusCode.NotModified);
                        return new UsersModel() { Status = "Error", Message = this.Message };
                    }
                }
            }
            catch (Exception ex)
            {
                Message = ex.Message;
                return new UsersModel() { Status = "Error", Message = this.Message };
            }
        }

        // Sync prompts with server
        public async Task<List<UsersPromptsModel>> SyncUserPromptsJson(IEnumerable<UsersPromptsModel> userPrompt)
        {
            try
            {
                if (!CrossConnectivity.Current.IsConnected || !await CrossConnectivity.Current.IsReachable(serverUri.Host.ToString(), 3000))
                {
                    Message = "There was a network problem. Please, try again later";
                    return null;
                }

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
                if (!CrossConnectivity.Current.IsConnected || !await CrossConnectivity.Current.IsReachable(serverUri.Host.ToString(), 3000))
                {
                    Message = "There was a network problem. Please, try again later";
                    return null;
                }

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