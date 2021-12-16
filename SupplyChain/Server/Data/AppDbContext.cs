using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Config;
using SupplyChain.Shared;
using SupplyChain.Shared.CDM;
using SupplyChain.Shared.Login;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using System;

namespace SupplyChain
{
    public class AppDbContext : DbContext
    {
        //MODULO CARGA DE MAQUINA
        public virtual DbSet<ModeloCarga> Cargas { get; set; }
        public virtual DbSet<CargaMaq> CargaMaq { get; set; }
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
        public virtual DbSet<ModeloPedidosPendientes> ModeloPedidosPendientes { get; set; }
        public virtual DbSet<ModeloPendientesFabricar> ModeloPendientesFabricar { get; set; }
        public virtual DbSet<ModeloAbastecimiento> ModeloAbastecimiento { get; set; }
        public virtual DbSet<vPendienteFabricar> VPendientesFabricars { get; set; }
        public virtual DbSet<vTrazabilidad> VTrazabilidads { get; set; }
        public virtual DbSet<Procun> Procun { get; set; }
        public virtual DbSet<Fabricacion> Fabricaciones { get; set; }
        public virtual DbSet<vProdMaquinaDataCore> VProdMaquinaDataCore { get; set; }
        //MODULO LOGIN
        public DbSet<Usuarios> Usuarios { get; set; }
        public DbSet<Rol> Roles { get; set; }

        //MODULO NO CONFORMIDADES

        public virtual DbSet<ModeloPedidosDespacho> PedidosDespacho { get; set; }
        public virtual DbSet<ModeloPedidosLote> PedidosLote { get; set; }
        public DbSet<NoConformidades> NoConformidades { get; set; }
        public DbSet<NoConformidadesQuery> NoConformidadesQuery { get; set; }
        public DbSet<NoConformidadesListaAcciones> NoConformidadesListaAcciones { get; set; }
        public DbSet<NoConformidadesAcciones> NoConformidadesAcciones { get; set; }
        public DbSet<Compra> ComprasDbSet { get; set; }


        public DbSet<MovimientoStockSP> MovimientosStock { get; set; }
        public DbSet<StockSP> StocksSP { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<ResumenStock> ResumenStock { get; set; }
        public DbSet<vResumenStock> vResumenStock { get; set; }
        public DbSet<Modulo> Modulos { get; set; }
        public virtual DbSet<Genera> Genera { get; set; }
        public virtual DbSet<Planificacion> Planificaciones { get; set; }

        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<EstadVenta> EstadVentas { get; set; }
        public DbSet<VistasGrillas> VistasGrillas { get; set; }
        public DbSet<vEventos> vEventos { get; set; }

        public DbSet<Formula> Formulas { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure domain classes using modelBuilder here   
            modelBuilder.ApplyConfiguration(new CompraConfig());
            modelBuilder.ApplyConfiguration(new PedidoConfig());
            modelBuilder.ApplyConfiguration(new ProveedorConfig());

            modelBuilder.Entity<Genera>()
        .       HasKey(c => new { c.CAMP3, c.CG_CIA, c.PUNTO_VENTA });

            modelBuilder .Entity<vPendienteFabricar>(
            eb =>
            {
                eb.ToView("vPendientesFabricar");
            });
            
            modelBuilder .Entity<vResumenStock>(
            eb =>
            {
                eb.ToView("vResumenStock");
            });

            modelBuilder.Entity<vTrazabilidad>(
            eb =>
            {
                eb.ToView("vTrazabilidad");
            });
            
            modelBuilder.Entity<vProdMaquinaDataCore>(
            eb =>
            {
                eb.ToView("vProdMaquinaDataCore");
            });

            modelBuilder.Entity<ItemAbastecimiento>().HasNoKey().ToView(null);
            modelBuilder.Entity<Procun>().HasNoKey().ToView(null);
            modelBuilder.Entity<EstadVenta>().HasNoKey().ToView(null);
            modelBuilder.Entity<StockSP>().HasNoKey().ToView(null);
            modelBuilder.Entity<vEventos>().HasNoKey().ToView("vEventos");
        }
    }
}
