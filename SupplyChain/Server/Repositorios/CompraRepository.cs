using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Data.Repository;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SupplyChain.Server.Repositorios
{
    public class CompraRepository : Repository<Compra, int>
    {
        public CompraRepository(AppDbContext appDbContext) : base(appDbContext)
        {

        }

        public async Task<IEnumerable<Compra>> UltimasCompras(int cant, string cg_mat)
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

        //public async Task AnularOc(Compras compras)
        //{
        //        string xSql = $"UPDATE COMPRAS SET FE_EMIT='', CG_PREP='', CG_ORDEN='', CG_MAT='',DES_MAT='',TIPO='',TILDE='',NECESARIO=0,SOLICITADO=0,UNID1='', PRECIO=0 " +
        //        $"BON=0, PRECIONETO='',PRECIOTOT='',MONEDA='',NRCLTE=0,DES_PROVE='',ENTRAGA=0,FE_PREV='',FE_REAL='',FE_VENC='',FE_CIERRE='',CONDVEN='', CG_DEPOSM=0 " +
        //        $"PRECIOUC=0,PRECIOPOND=0,PEDIDO=0,CG_EST=0,CG_CUENT=0,FE_PREC='',DIASVIGE=0, CANTLOTE='', CANTMIN='',ESPEFICIFA='',NOPROD='', CG_COS=0,FE_DISP='', CG_CIA=0,MARCA1=0, FE_REG='', TILDE3='' " +
        //        $"WHERE NUMERO = {compras.NUMERO}";

        //        await DbSet.FromSqlRaw(xSql).ToListAsync();
           
        //}

        public async Task AnularOC(Compra compra)
        {
            if (compra.NUMERO > 0)
            {
                await Db.Database.ExecuteSqlRawAsync($"UPDATE COMPRAS SET NUMERO=0,PRECIO=0, " +
                $"BON=0, PRECIOTOT=0,MONEDA='',ENTREGA= 0,CONDVEN=''," +
                $"PRECIOUC=0,PRECIOPOND=0,PEDIDO=0,DIASVIGE=0, OBSERVACIONES='ANULADO' " +
                $"WHERE NUMERO = {compra.NUMERO}");
              
            }
            //else if (compra.NUMERO > 0)
            //{
            //    await Db.Database.ExecuteSqlRawAsync($"INSERT INTO COMPRAS (NUMERO,FE_EMIT,CG_ORDEN ,CG_MAT ,DES_MAT ,TIPO ,TILDE ,NECESARIO ,SOLICITADO ,UNID ,AUTORIZADO ,CG_DEN ,UNID1 ,PRECIO ,BON ,PRECIONETO ,PRECIOTOT ,MONEDA ,NROCLTE ,DES_PROVE ,ENTREGA ,FE_PREV ,FE_VENC ,FE_CIERRE ,CONDVEN ,CG_DEPOSM ,PRECIOUC ,PRECIOPOND ,PEDIDO ," +
            //        $"CG_EST ,CG_CUENT ,FE_PREC ,CANTLOTE ,CANTMIN ,ESPECIFICA ,CG_COS ,FE_DISP ,NUMANULA ,NUMCOMP ,CG_IMPORT ,CG_EXPORT ,CG_CIA ,IMPRESA ,MARCA1 ,AbiertoPreparacion ,USUARIO ,FE_REG ,NUMREQ ,FE_REQ ,FE_AUTREQ ,CG_PROVEREQ ,AVANCE ,FE_AUT ,"+
            //        $"FE_CIERREREQ ,CG_ORDF ,CG_PROY ,ESTADO_CAB ,ESTADO_IT ,NECESARIO_ORI ,NUM_SOLCOT ,SOLICITADO_ORI ,MODIF_INGRESO ,PENDIENTE ,TILDE3 ,Observaciones ,DESCUENTO)"+
            //        $"VALUES (,'{compra.NUMERO}','')" +
            //        $"WHERE NUMERO ={compra.NUMERO}") ;
            //}
        }
        
        public async Task Anular(Compra compra)
        {
            if (compra.NUMERO > 0)
            {
                await Db.Database.ExecuteSqlRawAsync($"UPDATE COMPRAS SET NUMERO =0, ");
            }
        }

        

        //public async Task InsertItem(Compra compra)
        //{
        //    if(compra.NUMERO > 0)
        //    {
        //        await Db.Database.ExecuteSqlRawAsync($"INSERT INTO COMPRAS ()")
        //    }
        //} 

    

    }

}