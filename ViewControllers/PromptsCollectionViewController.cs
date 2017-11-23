using Foundation;
using System;
using UIKit;
using CoreGraphics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Collections;

namespace Happy31
{
    public partial class PromptsCollectionViewController : UICollectionViewController
    {
        //static NSString promptCellId = new NSString("PromptCell");
        //static NSString headerId = new NSString("Header");
        List<string> collectionItems;
        List<string> promptItems;

        PromptsRepository promptRepository;
        UsersPromptsRepository userPromptRepository;

        RestService restService;

        NSUserDefaults plist;
        string userId;

        public PromptsCollectionViewController(IntPtr handle) : base(handle)
        {
            collectionItems = new List<string>();
            collectionItems.Add("avnvbnvbnvbnvbn");
            collectionItems.Add("bvbnvbnvbnvbnvbnvb bvbnvbnvbnvbnvbnvb bvbnvbnvbnvbnvbnvb");
            collectionItems.Add("cvbnvbnvbnvbnvnvbnv");
            collectionItems.Add("dvbnvbnvbnn ljkf fjdhg kjnbk jcvb");
            collectionItems.Add("egfh fghfg hfghfghf");

            promptRepository = new PromptsRepository();
            userPromptRepository = new UsersPromptsRepository();

            plist = NSUserDefaults.StandardUserDefaults;
            userId = plist.StringForKey("userId");

            promptItems = new List<string>();
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // To use a Supplementary View, it first needs to be registered in the ViewDidLoad method
            CollectionView.RegisterClassForCell(typeof(PromptsCollectionCell), PromptsCollectionCell.cellId);
            //CollectionView.RegisterClassForSupplementaryView(typeof(PromptsCollectionCell), UICollectionElementKindSection.Header, headerId);
            CollectionView.Source = new PromptsCollectionSource(collectionItems);

            CollectionView.ReloadData();
        }

        // Generating a new prompt and inserting into table "users_prompts"
        private bool GenerateNewPrompt()
        {
            // Getting random promptId
            int promptId = userPromptRepository.GetCurrentPromptId(userId);

            if (promptId == 0)
            {
                Console.WriteLine("There is no any prompt available");
                return false;
            }

            // Insert current prompt into table "users_prompts"
            int res = userPromptRepository.InsertCurrentPromptIntoTable(new UsersPromptsModel()
            {
                UserId = userId,
                PromptId = promptId,
                CreatedAt = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

            if (res == 0)
            {
                Console.WriteLine("There was a problem when insert prompt");
                return false;
            }

            return true;
        }

        // After first login, get full table "user_prompts" from remote database and insert it in local database
        async void SyncAllTableUserPrompts()
        {
            IEnumerable<UsersPromptsModel> userPromptsToSync;

            // Firstly, check if there are any data in "users_prompts" table for the current user
            // If there are no any data (could be user just registered or logged in from another device), send request "sync_all" to server to check if there are any saved prompts
            // If server contains any prompts, get them
            var data = userPromptRepository.DisplayUserPrompts(userId);

            if (data.Any())
                return;

            // If local table "user-prompts" is empty, send request "sync_all" to server
            userPromptsToSync = new UsersPromptsModel[] { new UsersPromptsModel() { UserId = userId, IsSync = "sync_all" } };

            // Send Json and get response from remote server
            var jsonResponse = await RestService(userPromptsToSync);

            // If there was an error in PostJsonDataAsync class, display message
            if (jsonResponse == null)
            {
                Console.WriteLine("Something went wrong:" + restService.Message);
                return;
            }

            // Insert prompts from server into table "users_prompts"
            foreach (var item in jsonResponse)
            {
                Console.WriteLine(item.CreatedAt);
                int res = userPromptRepository.InsertCurrentPromptIntoTable(new UsersPromptsModel()
                {
                    UserId = item.UserId,
                    PromptId = item.PromptId,
                    CreatedAt = item.CreatedAt,
                    IsSync = "True" // Change attrubute "is_sync=true" for the new users promts received from the remote server
                });
                if (res == 0)
                    Console.WriteLine("There was a problem when inserting prompt id=" + item.UserPromptId);
            }
        }

        // This method starts after user gets prompts and/or there is an internect connection
        // Retrieve prompts from local table "users_prompts" with attrubute "is_sync=False" - that is prompts, which haven't been synchronized with remote server
        // Send these prompts to the remote server, insert them and get response from server for every prompt if they were inserted without errors
        // If for the prompt there was not any error, update the current prompt, changing attribute "is_sync=True"
        async void SyncCurrentUserPrompts()
        {
            IEnumerable<UsersPromptsModel> userPromptsToSync;

            // Retrieve not sync prompts from local database
            userPromptsToSync = userPromptRepository.RetrieveNotSyncPromptsFromLocalDb(userId);

            if (!userPromptsToSync.Any())
            {
                Console.WriteLine("No data to sync");
                return;
            }

            // Send Json and get response from remote server
            var jsonResponse = await RestService(userPromptsToSync);

            // If there was an error in PostJsonDataAsync class, display message
            if (jsonResponse == null)
            {
                Console.WriteLine("Something went wrong:" + restService.Message);
                return;
            }

            //foreach (var obj in jsonResponse)
            //    Console.WriteLine(obj.IsSync);

            foreach (var status in jsonResponse) // Loop through prompts statuses received from server
            {
                if (status.UserId != userId) // Check if response has been received from the same requested user 
                {
                    Console.WriteLine("Requested and responsed userId are not the same!");
                    return;
                }

                string currentSyncStatusFromServer = status.IsSync;
                if (currentSyncStatusFromServer == "True") // If promt has been successfully inserted in the table on server successfully
                    foreach (var item in userPromptsToSync) // Loop through prompts
                        if (item.UserPromptId == status.UserPromptId) // Check id between prompt and hos status
                            userPromptRepository.UpdateUserPromptSyncStatus(new UsersPromptsModel() // Update the current prompt (set "is_sync=True")
                            {
                                UserPromptId = item.UserPromptId,
                                UserId = item.UserId,
                                PromptId = item.PromptId,
                                CreatedAt = item.CreatedAt,
                                IsSync = "True"
                            });
            }
        }

        // Call REST service to send Json data
        async Task<List<UsersPromptsModel>> RestService(IEnumerable<UsersPromptsModel> userPromptsToSync)
        {
            restService = new RestService();

            // Get Json data from server in JsonResponseModel format
            Task<List<UsersPromptsModel>> jsonResponeTask = restService.SyncUserPromptsJson(userPromptsToSync);

            Console.WriteLine(restService.Message);

            // Create instance of jsonResponeTask and pass jsonResponeTask there
            return await jsonResponeTask;
        }

        // Dsiplay user's prompts
        void DisplayUserPrompts()
        {
            var data = userPromptRepository.DisplayUserPrompts(userId);

            if (!data.Any())
                foreach (var str in data)
                {
                    string promptCategory = promptRepository.GetPrompt(str.PromptId).Category;
                    string promptTask = promptRepository.GetPrompt(str.PromptId).Task;
                    string promptDate = str.CreatedAt;
                    //promptItems.Add(str.PromptId.Id + ". " + str.CreatedAt);
                    Console.WriteLine(promptCategory + "," + promptTask + "," + promptDate);
                }
            else
                Console.WriteLine("No data");
        }

        partial void LoadPrompts_Activated(UIBarButtonItem sender)
        {
            //userPrompt.RetrieveUserPromptsFromServer(new UsersPromptsModel() { UserId = userId });
            // userPrompt.RemoveTableUsersPrompts();
            SyncAllTableUserPrompts();

            //GenerateNewPrompt();

            //DisplayUserPrompts();

            //SyncCurrentUserPrompts();
        }

        partial void GetTablePromptsButton_Activated(UIBarButtonItem sender)
        {
            promptRepository.GetTablePrompts();
            Console.WriteLine(promptRepository.Message);
        }
    }
}