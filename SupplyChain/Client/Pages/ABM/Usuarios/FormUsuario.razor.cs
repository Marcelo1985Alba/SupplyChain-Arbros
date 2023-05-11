using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared.BuscadorCliente;
using SupplyChain.Shared;
using SupplyChain.Shared.Login;
using Syncfusion.Blazor.DropDowns;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Syncfusion.Blazor.Inputs;
using System.Net.Http;
using System.Net.Http.Headers;
using Syncfusion.Blazor.Charts.Chart.Internal;

namespace SupplyChain.Client.Pages.ABM.Usuarios
{
    public class FormUsuarioBase : ComponentBase
    {
        [Inject] public RepositoryHttp.IRepositoryHttp Http { get; set; }
        [Inject] public HttpClient Http2 { get; set; }
        [Inject] public ClienteService ClienteService { get; set; }
        /// <summary>
        /// objecto modificado el cual tambien obtiene la id nueva en caso de agregar un nuevo
        /// </summary>
        [Parameter] public ApplicationUser ApplicationUser { get; set; } = new();
        [Parameter] public string[] RolesSeleccionados { get; set; } = Array.Empty<string>();
        [Parameter] public string CodigoClienteSeleccionado  { get; set; } = "0";
        [Parameter] public bool Show { get; set; } = false;
        [Parameter] public EventCallback<ApplicationUser> OnGuardar { get; set; }
        [Parameter] public EventCallback OnCerrar { get; set; }

        [Parameter] public List<ClienteExterno> Clientes { get; set; } = new();

        [Parameter] public string HeightDialog { get; set; } = "450px";
        protected SfMultiSelect<string[], string> refMultiSelect;
        protected SfSpinner refSpinnerCli;
        protected bool SpinnerVisible = false;
        protected SfToast ToastObj;
        protected List<string> Roles = new();

        protected MultipartFormDataContent formData = new MultipartFormDataContent();

        protected Dictionary<string, object> HtmlAttribute = new()
        {
            { "type", "button" }
        };

        protected Dictionary<string, object> HtmlAttributeSubmit = new()
        {
            { "type", "submit" },
            { "form", "form-usuario" }

        };

        
        protected async override Task OnInitializedAsync()
        {
            SpinnerVisible = true;
            CodigoClienteSeleccionado = ApplicationUser.Cg_Cli.ToString();

            if (ApplicationUser.Roles.Count > 0)
            {
                RolesSeleccionados = ApplicationUser.Roles.ToArray();
            }



            var response = await Http.GetFromJsonAsync<List<string>>("api/Cuentas/roles");
            if (response.Error)
            {

            }
            else
            {
                Roles = response.Response;
                Console.WriteLine(ApplicationUser.Roles.Select(r=> r));
            }

            await InvokeAsync(StateHasChanged);

            SpinnerVisible = false;
        }


        

        protected async Task Guardar()
        {
            SpinnerVisible = true;
            bool guardado = false;
            if (ApplicationUser.EsNuevo)
            {
                guardado = await Agregar();
            }
            else
            {
                guardado = await Actualizar();
            }

            SpinnerVisible = false;
            if (guardado)
            {
                var response2 = await Http2.PostAsync($"api/AdministracionArchivos/UploadImage/{ApplicationUser.Id}", formData);
                if (!response2.IsSuccessStatusCode)
                {
                    await ToastMensajeError("Error al guardar imagen");
                }
                else
                {

                    Show = false;
                    await OnGuardar.InvokeAsync(ApplicationUser);
                }
                
            }


        }

        //protected void OnTagging(TaggingEventArgs<string> args)
        //{
        //    System.Diagnostics.Debug.WriteLine($"OnTagging() -- selectedTags: {string.Join(',', RolesSeleccionados)}");  //Outputs value of meal.Tags no matter which tag is added
        //}


        protected async Task<bool> Agregar()
        {
            var userinfo = new UserInfo()
            {
                UserName = ApplicationUser.UserName,
                Cg_Cli = ApplicationUser.Cg_Cli = string.IsNullOrEmpty(CodigoClienteSeleccionado) ? 0 : Convert.ToInt32(CodigoClienteSeleccionado),
                Email = ApplicationUser.Email,
                Password = ApplicationUser.PasswordHash,
                Roles = RolesSeleccionados.ToList()
            };

            var response = await Http.PostAsJsonAsync("api/Cuentas/crear", userinfo);
            if (response.Error)
            {
                await ToastMensajeError("Ocurrio un Error al guaardar.");
                Console.WriteLine("ERROR! " + response.HttpResponseMessage.ReasonPhrase);
                return false;
            }


            return true;
            
        }

        protected async Task<bool> Actualizar()
        {
            var userInfo = new UserInfo()
            {
                UserName = ApplicationUser.UserName,
                Cg_Cli = ApplicationUser.Cg_Cli = string.IsNullOrEmpty(CodigoClienteSeleccionado) ? 0 : Convert.ToInt32(CodigoClienteSeleccionado),
                Email = ApplicationUser.Email,
                Password = ApplicationUser.PasswordHash,
                Roles = RolesSeleccionados.ToList()
            };

            var response = await Http.PutAsJsonAsync("api/Cuentas/actualizar", userInfo);
            if (response.Error)
            {
                await ToastMensajeError("Ocurrio un Error al guaardar.");
                return false;
            }

            return true;
        }

        protected async Task SelectedClienteChanged(ChangeEventArgs<string, ClienteExterno> args)
        {
            if (args.ItemData == null || string.IsNullOrEmpty(args.ItemData.CG_CLI))
            {
                return;
            }

            ApplicationUser.NombreCliente = args.ItemData.DESCRIPCION;

        }

        private async Task ToastMensajeExito()
        {
            await this.ToastObj.Show(new ToastModel
            {
                Title = "EXITO!",
                Content = "Guardado Correctamente.",
                CssClass = "e-toast-success",
                Icon = "e-success toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }

        private async Task ToastMensajeError(string content = "Ocurrio un Error.")
        {
            await ToastObj.ShowAsync(new ToastModel
            {
                Title = "Error!",
                Content = content,
                CssClass = "e-toast-warning",
                Icon = "e-warning toast-icons",
                ShowCloseButton = true,
                ShowProgressBar = true
            });
        }

        protected async Task OnChangeFileUpload(UploadChangeEventArgs args)
        {
            try
            {
                UploadFiles file = args.Files[0];
                var buffers = new byte[file.Stream.Length];

                file.Stream.Seek(0, System.IO.SeekOrigin.Begin);
                await file.Stream.ReadAsync(buffers, 0, buffers.Length);
                file.Stream.Dispose();

                string imageType = file.FileInfo.MimeContentType;
                ApplicationUser.Foto = buffers;
                string imgUrl = $"data:{imageType};base64,{Convert.ToBase64String(buffers)}";

                // Crea un objeto FormData para enviar la foto a la API
                formData = new MultipartFormDataContent();
                var byteArrayContent = new ByteArrayContent(buffers);
                byteArrayContent.Headers.ContentType = new MediaTypeHeaderValue(imageType);

                formData.Add(byteArrayContent, "photo", file.FileInfo.Name);

                
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public async Task Hide()
        {
            Show = false;
        }
        protected async Task OnCerrarDialog()
        {
            await OnCerrar.InvokeAsync();
        }
    }
}
