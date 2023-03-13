    using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Config;
using SupplyChain.Shared;
using SupplyChain.Shared.CDM;
using SupplyChain.Shared.Login;
using SupplyChain.Shared.Models;
using SupplyChain.Shared.PCP;
using System;
using System.Collections.Generic;

namespace SupplyChain
{

    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        #region "DbSet"
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
        public virtual DbSet<ClienteExterno> ClientesExternos { get; set; }
        //MODULO LOGÍSTICA
        public DbSet<PedCli> PedCli { get; set; }
        public DbSet<Pedidos> Pedidos { get; set; }
        public DbSet<Programa> Programa { get; set; }
        public DbSet<Areas> Areas { get; set; }
        public DbSet<Cat> Cat { get; set; }
        public DbSet<Unidades> Unidades { get; set; }
        public DbSet<Lineas> Lineas { get; set; }
        public DbSet<CatOpe> CateOperarios { get; set; }
        public DbSet<Deposito> Depositos { get; set; }
        public DbSet<TipoArea> TipoArea { get; set; }
        public DbSet<TipoCelda> TipoCelda { get; set; }
        public DbSet<TiposNoConf> TiposNoConf { get; set; }
        public DbSet<Condven> Condven { get; set; }
        public DbSet<Indic> Indic { get; set; }
        public DbSet<Parada> Parada { get; set; }
        public DbSet<ProTarea> ProTarea { get; set; }
        public DbSet<TipoMat> TipoMat { get; set; }
        public DbSet<Producto> Prod { get; set; }

        public DbSet<PreciosArticulos> PrecioArticulo { get; set; }
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
        public virtual DbSet<Moneda> Monedas { get; set; }
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
        public DbSet<vEstadEventos> vEstadEventos { get; set; }


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
        public DbSet<vEstadPedidosIngresados> vEstadPedidosIngresados { get; set; }
        public DbSet<vEstadPedidosAlta> vEstadPedidosAltas { get; set; }
        public DbSet<vEstadFacturacion> vEstadFacturaciones { get; set; }
        public DbSet<vEstadCompras> vEstadCompras { get; set; }
        public DbSet<vEstadPresupuestos> vEstadPresupuestos { get; set; }
        public DbSet<VistasGrillas> VistasGrillas { get; set; }
        public DbSet<vEventos> vEventos { get; set; }
        public DbSet<vIngenieriaProductosFormulas> vIngenieriaProductosFormulas { get; set; }

        public DbSet<Formula> Formulas { get; set; }
        public DbSet<StockCorregido> StockCorregidos { get; set; }

        public DbSet<vEstadoPedido> vEstadoPedidos { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<vSolicitudes> vSolicitudes { get; set; }
        public DbSet<vPresupuestos> vPresupuestos { get; set; }
        public DbSet<PresupuestoAnterior> Presupuestos { get; set; }
        public DbSet<vCondicionesPago> vCondicionesPago { get; set; }
        public DbSet<vCondicionesEntrega> vCondicionesEntrega { get; set; }
        public DbSet<vTipoCambio> vTipoCambio { get; set; }
        public DbSet<NotificacionSubscripcion> NotificacionSubscripcions { get; set; }
        public DbSet<vCalculoSolicitudes> vCalculoSolicitudes { get; set; }
        public DbSet<vPresupuestoReporte> vPresupuestosReporte { get; set; }
        public DbSet<vPedidoReporte> vPedidoReporte { get; set; }
        public DbSet<vRemitoReporte> vRemitoReporte { get; set; }
        public DbSet<vTransporte> vTransportes { get; set; }
        public DbSet<ChatMessage> ChatMessages { get; set; }
        //MODULO PROYECTOS
        public DbSet<ProyectosGBPI> Proyectos { get; set; }

        public DbSet<vPedidoAlta> vPedidoAltas { get; set; }
        public DbSet<MantCeldas> MantCeldas { get; set; }

        public DbSet<Matprove_busquedaprove> Matprove_busquedaprove { get; set; }

        public DbSet<Proveedores_compras> proveedores_compras { get; set; }
        public DbSet<vUsuario> vUsuarios { get; set; }
        public DbSet<vOCompraReporte> vOCompraReporte { get; set; }
        public DbSet<ISO> ISO { get; set; }
        public DbSet<AspAmb> AspAmb { get; set; }
        public DbSet<AspNetRoles> AspNetRoles { get; set; }
        public DbSet<ProcalsMP> ProcalsMP { get; set; }
        public DbSet<vControlCalidadPendientes> vcontrolCalidadPendientes { get; set; }
        public DbSet<Procesos> Procesos { get; set; }

        #endregion

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            this.Database.SetCommandTimeout(60);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Configure domain classes using modelBuilder here   
            modelBuilder.ApplyConfiguration(new CompraConfig());
            modelBuilder.ApplyConfiguration(new PedidoConfig());
            modelBuilder.ApplyConfiguration(new ProveedorConfig());

            modelBuilder.Entity<ChatMessage>(entity =>
            {
                entity.HasOne(d => d.FromUser)
                    .WithMany(p => p.ChatMessagesFromUsers)
                    .HasForeignKey(d => d.FromUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.ToUser)
                    .WithMany(p => p.ChatMessagesToUsers)
                    .HasForeignKey(d => d.ToUserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Solicitud>(entity => {
                entity.HasOne(c => c.PresupuestoDetalle)
                    .WithOne(p => p.Solicitud);
            });


            modelBuilder.Entity<Presupuesto>(entity => {
                entity.HasMany(c => c.Items)
                    .WithOne(p => p.Presupuesto)
                    .HasForeignKey(c => c.PRESUPUESTOID)
                    .OnDelete(DeleteBehavior.Cascade); 

            });

            modelBuilder.Entity<PresupuestoDetalle>(entity=> {
                entity.HasOne(d => d.Presupuesto)
                .WithMany(p => p.Items)
                .HasForeignKey(d => d.PRESUPUESTOID)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_PRESUPUESTO_DETALLE_PRESUPUESTO_ENCABEZADO");

                entity.HasOne(d => d.Solicitud)
                .WithOne(p => p.PresupuestoDetalle)
                .OnDelete(DeleteBehavior.ClientSetNull);


                entity.Property(p => p.TOTAL).HasComputedColumnSql("[PREC_UNIT_X_CANTIDAD] - ',' [TOTAL]");
            });


            modelBuilder.Entity<Genera>()
             .HasKey(c => new { c.Id, c.CG_CIA, c.PUNTO_VENTA });

            modelBuilder.Entity<vPendienteFabricar>(
            eb =>
            {
                eb.HasNoKey();
                eb.ToView("vPendientesFabricar");
            });

            modelBuilder.Entity<vResumenStock>(
            eb =>
            {
                eb.HasNoKey();
                eb.ToView("vResumenStock");
            });

            modelBuilder.Entity<vTrazabilidad>(
            eb =>
            {
                eb.HasNoKey();
                eb.ToView("vTrazabilidad");
            });

            modelBuilder.Entity<vProdMaquinaDataCore>(
            eb =>
            {
                eb.HasNoKey();
                eb.ToView("vProdMaquinaDataCore");
            });

            modelBuilder.Entity<ItemAbastecimiento>().HasNoKey().ToView(null);
            modelBuilder.Entity<Procun>().HasNoKey().ToView(null);
            modelBuilder.Entity<EstadVenta>().HasNoKey().ToView(null);
            modelBuilder.Entity<StockSP>().HasNoKey().ToView(null);
            modelBuilder.Entity<PedCli>().ToTable(tb => tb.HasTrigger("trgPedcli"));
            modelBuilder.Entity<Pedidos>().ToTable(tb => tb.HasTrigger("trgResumenStock"));
            modelBuilder.Entity<Programa>().ToTable(tb => tb.HasTrigger("trgPrograma"));
            CreateVistasSQL(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private static void CreateVistasSQL(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<vEventos>().HasNoKey().ToView("vEventos");
            modelBuilder.Entity<vEstadPedidosIngresados>().HasNoKey().ToView("vEstad_PedidosIngresados");
            modelBuilder.Entity<vEstadPedidosAlta>().HasNoKey().ToView("vEstad_PedidosAltas");
            modelBuilder.Entity<vEstadFacturacion>().HasNoKey().ToView("vEstad_Facturacion");
            modelBuilder.Entity<vIngenieriaProductosFormulas>().HasNoKey().ToView("vIngenieria_Productos_Formulas");
            modelBuilder.Entity<vEstadCompras>().HasNoKey().ToView("vEstad_Compras");
            modelBuilder.Entity<vEstadPresupuestos>().HasNoKey().ToView("vEstad_Presupuestos");
            modelBuilder.Entity<vEstadEventos>().HasNoKey().ToView("vEstad_Eventos");
            modelBuilder.Entity<vEstadoPedido>().HasNoKey().ToView("vEstadoPedido");
            modelBuilder.Entity<vSolicitudes>().HasNoKey().ToView("vSolicitudes");
            modelBuilder.Entity<ClienteExterno>().HasNoKey().ToView("vClientesItris");
            modelBuilder.Entity<vPresupuestos>().HasNoKey().ToView("vPresupuestos");
            modelBuilder.Entity<vDireccionesEntrega>().HasNoKey().ToView("vDireccionesEntrega_Itris");
            modelBuilder.Entity<vCondicionesPago>().HasNoKey().ToView("vCondicionesPago");
            modelBuilder.Entity<vCondicionesEntrega>().HasNoKey().ToView("vCondicionesEntrega");
            modelBuilder.Entity<vTipoCambio>().HasNoKey().ToView("vTipoCambio");
            modelBuilder.Entity<vCalculoSolicitudes>().ToView("vCalculoSolicitudes");
            modelBuilder.Entity<vTransporte>().HasNoKey().ToView("vTransportes");
            modelBuilder.Entity<vPedidoAlta>().HasNoKey().ToView("vPedidosAltas");
            modelBuilder.Entity<vPresupuestoReporte>().HasNoKey().ToView("vPresupuestoReporte");
            modelBuilder.Entity<vPedidoReporte>().HasNoKey().ToView("vPedidoReporte");
            modelBuilder.Entity<vRemitoReporte>().HasNoKey().ToView("vRemitoReporte");
            modelBuilder.Entity<vUsuario>().HasNoKey().ToView("vUsuarios");
            modelBuilder.Entity<vOCompraReporte>().HasNoKey().ToView("vOCompraReporte");
            modelBuilder.Entity<vControlCalidadPendientes>().HasNoKey().ToView("vControlCalidadPendientes");
        }
    }
}
