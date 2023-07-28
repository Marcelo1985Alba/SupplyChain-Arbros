﻿using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task AnularPP(Compra compra)
        {
            if (compra.NUMERO > 0)
            {

                await Db.Database.ExecuteSqlRawAsync($"UPDATE COMPRAS SET NUMERO=0, " +
                $"MONEDA='',ENTREGA= 0,CONDVEN='',PRECIO=0, PRECIOTOT=0, NROCLTE=0, DES_PROVE=''," +
                $"OBSERVACIONES='ANULADO' " +
                $"WHERE NUMERO = {compra.NUMERO}");
              
            }
            
        }
        
        public async Task AnularOC(Compra compra)
        {
            if (compra.NUMERO > 0)
            {
               
                await Db.Database.ExecuteSqlRawAsync($"UPDATE COMPRAS SET NUMERO =0 ,ESPEGEN ='',CONDVEN='',OBSERVACIONES='ANULADO' WHERE NUMERO ={compra.NUMERO}");
            }
        }

        
    }

}