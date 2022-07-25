using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class SolicitudRepository : Repository<Solicitud, int>
    {
        private readonly PrecioArticulosRepository _precioArticulosRepository;
        private readonly ClienteExternoRepository _clienteExternoRepository;

        public SolicitudRepository(AppDbContext appDbContext, PrecioArticulosRepository precioArticulosRepository,
            ClienteExternoRepository clienteExternoRepository) : base (appDbContext)
        {
            this._precioArticulosRepository = precioArticulosRepository;
            this._clienteExternoRepository = clienteExternoRepository;
        }

        public override async Task<Solicitud> ObtenerPorId(int Id)
        {
            var solicitud = await DbSet.FindAsync(Id);
            if (solicitud != null)
            {
                var precioArticulo = await _precioArticulosRepository.ObtenerPorId(solicitud.Producto);
                if (precioArticulo != null)
                {
                    solicitud.PrecioArticulo = precioArticulo;
                    solicitud.Des_Prod = precioArticulo.Descripcion;
                }


                if (solicitud.CG_CLI > 0)
                {
                    var cliente = await _clienteExternoRepository.ObtenerTodosQueryable()
                        .Where(c => c.CG_CLI == solicitud.CG_CLI.ToString()).FirstOrDefaultAsync();
                    if (cliente != null)
                    {
                        solicitud.Des_Cli = cliente.DESCRIPCION;
                    } 
                }
            }

            return solicitud;
        }

        public async Task<List<vSolicitudes>> ObtenerTodosFromVista()
        {
            return await Db.vSolicitudes.Where(s => !s.TienePresupuesto).ToListAsync();
        }

        public async Task AsignarClientByCuit(string cuit, Solicitud solicitud)
        {
            //cuit = cuit.Insert(2, "-");
            //cuit = cuit.Insert(cuit.Length - 1, "-");
            var cliente = await Db.ClientesExternos.FirstOrDefaultAsync(c=> c.CUIT.Trim() == cuit);
            if (cliente != null)
            {
                solicitud.CG_CLI = Convert.ToInt32(cliente.CG_CLI);
            }
        }


        /// <summary>
        /// guarda en servicio en caso de ser una reparacion
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async override Task Agregar(Solicitud entity)
        {
            await base.Agregar(entity);


            if (entity.Producto.StartsWith("0012")) //SI ES REPARACION
            {
                //TODO: ENVIAR A SERVICIO
                var servicio = new Service()
                {
                    SOLICITUD = entity.Id,
                    CG_CLI = entity.CG_CLI,
                    CLIENTE = entity.Des_Cli,
                    DESCARTICULO = entity.Des_Prod
                };

                Db.Servicios.Add(servicio);
                await Db.SaveChangesAsync();
            }

        }
    }
}
