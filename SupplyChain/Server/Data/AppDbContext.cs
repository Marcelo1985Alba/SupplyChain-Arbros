using Microsoft.EntityFrameworkCore;
using SupplyChain.Shared;
using SupplyChain.Shared.Login;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;

namespace SupplyChain
{
    public class AppDbContext : DbContext
    {
        //MODULO CARGA DE MAQUINA
        public virtual DbSet<ModeloCarga> Cargas { get; set; }
        public virtual DbSet<ModeloOrdenFabricacionHojaRuta> OrdenesFabricacionHojaRuta { get; set; }
        public virtual DbSet<ModeloOrdenFabricacionSE> OrdenesFabricacionSE { get; set; }
        public virtual DbSet<ModeloOrdenFabricacionMP> OrdenesFabricacionMP { get; set; }
        public virtual DbSet<ModeloOrdenFabricacionEncabezado> OrdenesFabricacionEncabezado { get; set; }
        public virtual DbSet<ModeloOrdenFabricacion> OrdenesFabricacion { get; set; }
        public virtual DbSet<ModeloGenericoIntString> ModelosGenericosIntString { get; set; }
        public virtual DbSet<ModeloGenericoStringString> ModelosGenericosStringString { get; set; }
        public virtual DbSet<Solution> Solution { get; set; }
        public virtual DbSet<Operario> Operario { get; set; }
        public virtual DbSet<EstadosCargaMaquina> EstadosCargaMaquinas { get; set; }
        //public virtual DbSet<Prod> Prod { get; set; }
        public virtual DbSet<Cliente> Cliente { get; set; }
        //MODULO LOGÍSTICA
        public DbSet<PedCli> PedCli { get; set; }
        public DbSet<Pedidos> Pedidos { get; set; }
        public DbSet<Programa> Programa { get; set; }
        //willy
        public DbSet<Areas> Areas { get; set; }
        public DbSet<Unidades> Unidades { get; set; }
        public DbSet<Lineas> Lineas { get; set; }
        public DbSet<CatOpe> CateOperarios { get; set; }
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<TipoArea> TipoArea { get; set; }
        public DbSet<TipoCelda> TipoCelda { get; set; }
        public DbSet<TiposNoConf> TiposNoConf { get; set; }
        public DbSet<Indic> Indic { get; set; }
        public DbSet<Parada> Parada { get; set; }
        public DbSet<ProTarea> ProTarea { get; set; }
        public DbSet<TipoMat> TipoMat { get; set; }
        public DbSet<Producto> Prod { get; set; }
        //MODULO SERVICIOS
        public DbSet<Celdas> Celdas { get; set; }
        public DbSet<Estado> Estado { get; set; }
        public DbSet<Marca> Marca { get; set; }
        public DbSet<Medida> Medida { get; set; }
        public DbSet<Orificio> Orificio { get; set; }
        public DbSet<Serie> Serie { get; set; }
        public DbSet<Service> Servicios { get; set; }
        public DbSet<Sobrepresion> Sobrepresion { get; set; }
        public DbSet<Tipo> Tipo { get; set; }
        public DbSet<Trabajosefec> Trabajosefec { get; set; }
        //MODULO PCP
        public virtual DbSet<PresAnual> PresAnual { get; set; }
        public virtual DbSet<ModeloPendientesFabricar> ModeloPendientesFabricar { get; set; }
        public virtual DbSet<ModeloAbastecimiento> ModeloAbastecimiento { get; set; }
        public virtual DbSet<vPendienteFabricar> VPendientesFabricars { get; set; }
        //MODULO LOGIN
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        public virtual DbSet<Compra> Compras { get; set; }
        public DbSet<ResumenStock> ResumenStock { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public virtual DbSet<Genera> Genera { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { 
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genera>()
        .       HasKey(c => new { c.CAMP3, c.CG_CIA, c.PUNTO_VENTA });

            modelBuilder .Entity<vPendienteFabricar>(
            eb =>
            {
                eb.ToView("vPendientesFabricar");
                //eb.Property(v => v.BlogName).HasColumnName("Name");
            });

            modelBuilder.Entity<ItemAbastecimiento>().HasNoKey().ToView(null);
        }
    }
}
