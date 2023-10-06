using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.JSInterop;
using SupplyChain.Client.Shared;
using SupplyChain.Shared;
using SupplyChain.Shared.Login;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Inputs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SupplyChain.Client.Pages.ChatApp
{
    public class ChatBase: ComponentBase
    {
        [Inject] public NavigationManager _navigationManager { get; set; }
        [Inject] public IJSRuntime _js { get; set; }
        [Inject] public HelperService.ChatService ChatService { get; set; }
        [CascadingParameter] public HubConnection hubConnection { get; set; }
        [CascadingParameter] public MainLayout MainLayout { get; set; }
        [CascadingParameter] public Task<AuthenticationState> authenticationState { get; set; }
        [Parameter] public string ContactEmail { get; set; }
        [Parameter] public string ContactId { get; set; }
        [Parameter] public string ContactFoto { get; set; }
        [Parameter] public string CurrentMessage { get; set; }
        [Parameter] public string CurrentUserId { get; set; }
        [Parameter] public string CurrentUserEmail { get; set; }

        [Parameter] public ApplicationUser ApplicationUser { get; set; }

        protected List<ChatMessage> messages = new List<ChatMessage>();
        protected List<Usuario> ChatUsers = new List<Usuario>();
        private AuthenticationState authState;

        private byte[]? imageUsuario;

        protected SfTextBox SfTextBox;
        protected ObservableCollection<ListDataModel> DataSource = new ObservableCollection<ListDataModel>() {
        new ListDataModel {
            Text = "Jenifer",
            Contact = "Hi",
            Id = "1",
            Avatar = "",
            Pic = "pic01",
            Chat = "sender"
        },
        new ListDataModel {
            Text = "Amenda",
            Contact = "Hello",
            Id = "2",
            Avatar = "A",
            Pic = "",
            Chat = "receiver"
        },
        new ListDataModel {
            Text = "Jenifer",
            Contact = "What Knid of application going to launch",
            Id = "4",
            Avatar = "",
            Pic = "pic02",
            Chat = "sender"
        },
        new ListDataModel {
            Text = "Amenda ",
            Contact = "A knid of Emergency broadcast App",
            Id = "5",
            Avatar = "A",
            Pic = "",
            Chat = "receiver"
        },
        new ListDataModel {
            Text = "Jacob",
            Contact = "Can you please elaborate",
            Id = "6",
            Avatar = "",
            Pic = "pic04",
            Chat = "sender"
        },
    };

        protected async override Task OnInitializedAsync()
        {
            MainLayout.Titulo = "Chat";
            if (hubConnection == null)
            {
                hubConnection = new HubConnectionBuilder().WithUrl(_navigationManager.ToAbsoluteUri("/chathub")).Build();
            }
            if (hubConnection.State == HubConnectionState.Disconnected)
            {
                await hubConnection?.StartAsync();
            }
            hubConnection?.On<ChatMessage, string>("ReceiveMessage", async (message, userName) =>
            {
                if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId) || (ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
                {

                    if ((ContactId == message.ToUserId && CurrentUserId == message.FromUserId))
                    {
                        messages.Add(new ChatMessage { Message = message.Message, CreatedDate = message.CreatedDate, FromUser = new ApplicationUser() { Email = CurrentUserEmail } });
                        await hubConnection?.SendAsync("ChatNotificationAsync", $"Nuevo Mensaje de {userName}", ContactId, CurrentUserId);
                    }
                    else if ((ContactId == message.FromUserId && CurrentUserId == message.ToUserId))
                    {
                        messages.Add(new ChatMessage { Message = message.Message, CreatedDate = message.CreatedDate, FromUser = new ApplicationUser() { Email = ContactEmail } });
                    }

                    await IrUtlimoMensaje();
                    StateHasChanged();
                }
            });

            await GetUsersAsync();
            var state = await authenticationState;
            var user = state.User;
            var claims = user.Claims.ToList();
            CurrentUserId = user.Claims.Where(a => a.Type == ClaimTypes.NameIdentifier)
                .Select(a => a.Value).FirstOrDefault();
            CurrentUserEmail = user.Claims.Where(a => a.Type == ClaimTypes.Email)
                .Select(a => a.Value).FirstOrDefault();
            

            if (!string.IsNullOrEmpty(ContactId))
            {
                await LoadUserChat(ContactId);
            }

        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await IrUtlimoMensaje();
        }

        private async Task IrUtlimoMensaje()
        {
            if (messages?.Count > 0)
            {
                var item = messages[messages.Count - 1];
                // call the JS function to move the scroller position. 
                await _js.InvokeAsync<string>("ListView", item);
            }
        }

        private async Task GetUsersAsync()
        {
            var response = await ChatService.GetUsersAsync();
            if (response.Error)
            {

            }
            else
            {
                ChatUsers = response.Response;
            }
        }

        async Task LoadUserChat(string userId)
        {
           
                var response = await ChatService.GetUserDetailsAsync(userId);
                if (response.Error)
                {

                }
                else
                {
                    var contact = response.Response;
                    ContactId = contact.Id;
                    ContactEmail = contact.Email;
                    _navigationManager.NavigateTo($"chat-app/{ContactId}");
                    messages = new List<ChatMessage>();
                    var responseConversation = await ChatService.GetConversationAsync(ContactId);
                    if (responseConversation.Error)
                    {

                    }
                    else
                    {
                        messages = responseConversation.Response;
                    }
                }
            
        }

        protected async Task EnviarMensaje()
        {
            if (!string.IsNullOrEmpty(CurrentMessage) && !string.IsNullOrEmpty(ContactId))
            {
                //Save Message to DB
                var chatHistory = new ChatMessage()
                {
                    Message = CurrentMessage,
                    ToUserId = ContactId,
                    CreatedDate = DateTime.Now

                };
                var response = await ChatService.Agregar(chatHistory);
                if (response.Error)
                {
                    Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);

                }

                chatHistory.FromUserId = CurrentUserId;
                await hubConnection.SendAsync("SendMessageAsync", chatHistory, CurrentUserEmail);
                CurrentMessage = string.Empty;
            }

        }

        protected async Task change(ListBoxChangeEventArgs<string, Usuario> args)
        {
            //Triggers when value changed
             await LoadUserChat(args.Value);
        }

        public class ListDataModel
        {
            public string Id
            {
                get;
                set;
            }
            public string Chat
            {
                get;
                set;
            }
            public string Pic
            {
                get;
                set;
            }
            public string Avatar
            {
                get;
                set;
            }
            public string Text
            {
                get;
                set;
            }
            public string Contact
            {
                get;
                set;
            }
        }
    }
}
