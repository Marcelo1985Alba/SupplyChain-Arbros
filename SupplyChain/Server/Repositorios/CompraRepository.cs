using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class CompraRepository : Repository<Compra, int>
    {
        public CompraRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<Compra>> UltimasCompras (int cant, string cg_mat)
        {
            string xSQL = $"SELECT TOP {cant} * " +
            $"FROM COMPRAS WHERE CG_MAT = '{cg_mat}' AND NUMERO > 0 ORDER BY FE_EMIT DESC";

            return await DbSet.FromSqlRaw(xSQL).ToListAsync();
        }

        //public async Task<IEnumerable<Compra>> OcSeleccionado(int oc, int registro)
        //{
        //    string xSQL = $"SELECT * FROM COMPRAS WHERE NUMERO= '{oc}'";
        //    return await DbSet.FromSqlRaw(xSQL).ToListAsync();
        //}

        public async Task<IEnumerable<Compra>> AnularOc(int oc)
        {
            string xSQL = $"INSERT INTO COMPRAS NUMERO, FE_EMIT,PRECIO,PRECIONETO,PRECIOTOT,MONEDA,NRCLTE,DES_PROVE,FE_PREV,FE_VENC,FE_CIERRE,CONDVEN,CG_DEPOSM,PRECIOUC,PRECIOPOND,FE_PREC,DIASVIGE,ESPECIFICA," +
                          $"FE_DISP,FE_REG,DESCUENTO VALUES (0,NULL,0,0,0,0,'',0,'',NULL,NULL,NULL,0,0,0,0,NULL,NULL,'ANULADO',NULL,NULL) WHERE NUMERO ={oc}";
            return await DbSet.FromSqlRaw(xSQL).ToListAsync();
        }

    }
}
