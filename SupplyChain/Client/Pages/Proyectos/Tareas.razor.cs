using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SupplyChain.Client.HelperService;
using SupplyChain.Client.Shared;
using Syncfusion.Blazor.Data;
using Syncfusion.Blazor.Grids;
using Syncfusion.Blazor.Kanban;
using Syncfusion.Blazor.Notifications;
using Syncfusion.Blazor.Spinner;
using Syncfusion.Blazor.Buttons;
using Syncfusion.Blazor.DropDowns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using SupplyChain.Shared;
using Syncfusion.Blazor.InPlaceEditor;
using Syncfusion.Blazor.Inputs;
using Syncfusion.Blazor.PivotView;
using Action = Syncfusion.Blazor.Gantt.Action;

namespace SupplyChain.Client.Pages.Proyectos
{
    public class TareasPageBase : ComponentBase
    {
        [Inject] protected HttpClient Http { get; set; }
        [Inject] public RepositoryHttp.IRepositoryHttp Http2 { get; set; }
        [Inject] protected IJSRuntime JsRuntime { get; set; }
        [Inject] protected PedCliService PedCliService { get; set; }
        [Parameter] public ApplicationUser ApplicationUser { get; set; } = new();
        [CascadingParameter] public MainLayout Layout { get; set; }
        [CascadingParameter] public Task<AuthenticationState> authenticationState { get; set; }
        protected SfDropDownList<string, DropDownModel> StatusRef;
        protected SfDropDownList<string, DropDownModel> AssigneeRef;
        protected SfDropDownList<string, DropDownModel> ModulosRef;
        protected List<DropDownModel> AssigneeData = new List<DropDownModel>();
        protected List<DropDownModel> ModulosData = new List<DropDownModel>();
        protected SfTextBox SummaryRef;
        protected SfToast ToastObj;
        protected SfSpinner spinnerRef;
        protected SfKanban<SupplyChain.Shared.Tareas> refKanban;
        protected List<Modulo> Modulos = new List<Modulo>();
        protected AuthenticationState authState;
        protected bool SpinnerVisible { get; set; } = false;
        protected bool isAdding = false;

        protected List<SupplyChain.Shared.Tareas> tasks = new List<SupplyChain.Shared.Tareas>();
        protected List<TareasPorUsuario> tasksPerUser = new List<TareasPorUsuario>();
        protected List<vUsuario> Usuarios = new();

        protected override async Task OnInitializedAsync()
        {
            SpinnerVisible = true;
            Layout.Titulo = "Tareas";

            await GetUsuarios();
            authState = await authenticationState;
            var userId = authState.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);
            if (authState.User.Identity!.IsAuthenticated)
            {
                Modulos = await Http.GetFromJsonAsync<List<Modulo>>(
                    $"api/ModulosUsuario/GetModulosFromUserId/{userId!.Value}");
                ModulosData = Modulos.Select(x => new DropDownModel { Id = x.Id, Value = x.Descripcion.ToUpper() }).ToList();
                
                var moduloNombres = Modulos.Select(x => x.Descripcion).ToList();
                tasks = await Http.GetFromJsonAsync<List<SupplyChain.Shared.Tareas>>(
                    $"api/Tareas/TareasByModulos?modulos={Uri.EscapeDataString(string.Join("','", moduloNombres))}");
                foreach (var task in tasks)
                {
                    task.NombreCreador = Usuarios.FirstOrDefault(x => x.Id == task.Creador)?.USUARIO;
                    if(task.Importancia == "Poco Importante")
                    {
                        task.Color = "#FFD700"; // color dorado
                    }
                    else if(task.Importancia == "Algo Importante")
                    {
                        task.Color = "#FFA500"; // color naranja
                    }
                    else if(task.Importancia == "Muy Importante")
                    {
                        task.Color = "#FF0000"; // color rojo
                    }
                    else
                    {
                        task.Color = "#FFFFFF"; // color blanco
                    }
                }

                var ids = tasks.Select(x => x.Id).ToList();
                tasksPerUser = await Http.GetFromJsonAsync<List<TareasPorUsuario>>(
                    $"api/Tareas/TareasByTaskId?taskId={Uri.EscapeDataString(string.Join("','", ids))}");
                foreach (var taskUser in tasksPerUser)
                {
                    // le agrego a asignado el nombre del usuario
                    tasks.Find(x => x.Id == taskUser.tareaId)?.Asignados
                        .Add(Usuarios.FirstOrDefault(x => x.Id == taskUser.userId)?.USUARIO);
                }
            }
            
            SpinnerVisible = false;
            await base.OnInitializedAsync();
        }

        protected async Task onDragStop(Syncfusion.Blazor.Kanban.DragEventArgs<SupplyChain.Shared.Tareas> args)
        {
            SupplyChain.Shared.Tareas tarea = args.Data[0];
            await Http.PutAsJsonAsync("api/Tareas", tarea);
        }

        protected async Task AddRecord()
        {
            var userId = authState.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);
            SupplyChain.Shared.Tareas tarea = new SupplyChain.Shared.Tareas
            {
                Id = tasks.Max(x => x.Id) + 1,
                Titulo = "Nueva tarea",
                Estado = "Planificado",
                Resumen = "Resumen",
                Modulo = "",
                FechaRequerida = DateTime.Now,
                Importancia = "Poco Importante",
                Creador = userId!.Value,
                NombreCreador = Usuarios.FirstOrDefault(x => x.Id == userId.Value)?.USUARIO
            };
            isAdding = true;
            await refKanban.OpenDialogAsync(CurrentAction.Add, tarea);
        }

        protected async Task TomarTarea(SupplyChain.Shared.Tareas tarea)
        {
            var userId = authState.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);
            TareasPorUsuario tareaPorUsuario = new TareasPorUsuario
            {
                tareaId = tarea.Id,
                userId = userId!.Value
            };
            await Http.PostAsJsonAsync("api/Tareas/TomarTarea", tareaPorUsuario);
            var task = tasks.FirstOrDefault(x => x.Id == tarea.Id);
            task?.Asignados.Add(Usuarios.FirstOrDefault(x => x.Id == userId!.Value)?.USUARIO);
            await refKanban.CloseDialogAsync();
            await refKanban.RefreshAsync();
        }
        
        protected async Task DejarTarea(SupplyChain.Shared.Tareas tarea)
        {
            var userId = authState.User.Claims.FirstOrDefault(i => i.Type == ClaimTypes.NameIdentifier);
            TareasPorUsuario tareaPorUsuario = new TareasPorUsuario
            {
                tareaId = tarea.Id,
                userId = userId!.Value
            };
            var tareaToDelete = await Http.GetFromJsonAsync<TareasPorUsuario>($"api/Tareas/GetTareaPorUsuarioByUserAndTask/{tarea.Id}?userId={userId!.Value}");
            await Http.DeleteAsync($"api/Tareas/{tareaToDelete.Id}");
            var task = tasks.FirstOrDefault(x => x.Id == tarea.Id);
            task?.Asignados.Remove(Usuarios.FirstOrDefault(x => x.Id == userId!.Value)?.USUARIO);
            await refKanban.CloseDialogAsync();
            await refKanban.RefreshAsync();
        }

        protected async Task DialogCloseHandler(DialogCloseEventArgs<SupplyChain.Shared.Tareas> args)
        {
            if (args.Interaction == "Save")
            {
                if(args.Data.Importancia == "Poco Importante")
                {
                    args.Data.Color = "#FFD700"; // color dorado
                }
                else if(args.Data.Importancia == "Algo Importante")
                {
                    args.Data.Color = "#FFA500"; // color naranja
                }
                else if(args.Data.Importancia == "Muy Importante")
                {
                    args.Data.Color = "#FF0000"; // color rojo
                }
                else
                {
                    args.Data.Color = "#FFFFFF"; // color blanco
                }
                if (isAdding)
                {
                    await Http.PostAsJsonAsync("api/Tareas", args.Data);
                    foreach (var asignado in args.Data.Asignados)
                    {
                        TareasPorUsuario tareaPorUsuario = new TareasPorUsuario
                        {
                            tareaId = args.Data.Id,
                            userId = Usuarios.FirstOrDefault(x => x.USUARIO == asignado)?.Id
                        };
                        await Http.PostAsJsonAsync("api/Tareas/TomarTarea", tareaPorUsuario);
                    }
                    isAdding = false;
                }
                else
                {
                    await Http.PutAsJsonAsync("api/Tareas", args.Data);
                }
                await refKanban.RefreshAsync();
            }
            if(args.Interaction == "Cancel" || args.Interaction == "Close")
            {
                isAdding = false;
            }
        }
        
        protected async Task GetUsuarios()
        {
            var response = await Http2.GetFromJsonAsync<List<vUsuario>>("api/Cuentas/Usuarios");
            if (response.Error)
            {
                Console.WriteLine(response.HttpResponseMessage.ReasonPhrase);
            }
            else
            {
                Usuarios = response.Response;
            }
        }

        protected class DropDownModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }

        protected List<DropDownModel> StatusData = new List<DropDownModel>()
        {
            new DropDownModel { Id = 0, Value = "Planificado" },
            new DropDownModel { Id = 1, Value = "En Progreso" },
            new DropDownModel { Id = 2, Value = "En Prueba" },
            new DropDownModel { Id = 3, Value = "Terminado" }
        };
        
        
        protected List<DropDownModel> ImportanciaData = new List<DropDownModel>()
        {
            new DropDownModel { Id = 1, Value = "Poco Importante" },
            new DropDownModel { Id = 2, Value = "Algo Importante" },
            new DropDownModel { Id = 3, Value = "Muy Importante" }
        };
    }
}