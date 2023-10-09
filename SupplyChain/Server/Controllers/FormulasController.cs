using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormulasController : ControllerBase
    {
        private string CadenaConexionSQL = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json").Build().GetConnectionString("DefaultConnection");

        private readonly AppDbContext _context;
        private readonly FormulaRepository _formulaRepository;

        public FormulasController(FormulaRepository formulaRepository)
        {
            this._formulaRepository = formulaRepository;
        }

        [HttpGet("EnFormula/{codigo}")]
        public async Task<ActionResult<bool>> ExisteEnFormula(string codigo)
        {
            var ExisteEnFormula = await _formulaRepository.InsumoEnFormula(codigo);
            return ExisteEnFormula;
        }

        [HttpGet]
        public async Task<IEnumerable<Formula>> Get()
        {
            try
            {
                return await _formulaRepository.ObtenerTodos();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // GET: api/Formulas/BuscarPorCgProd/{CG_PROD}
        [HttpGet("BuscarPorCgProd/{CG_PROD}")]
        public async Task<IEnumerable<Formula>> BuscarPorCgProd(string CG_PROD)
        {
            string xSQL = string.Format($"Select * From Form2 where CG_PROD = '{CG_PROD}'");
            try
            {
                return await _formulaRepository.Obtener(f => f.Cg_Prod == CG_PROD).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<Formula>();
            }
        }

        // GET: api/Formulas/BuscarPorCgProdCopiar/{CG_PROD}
        [HttpGet("BuscarPorCgProdCopiar/{CG_PROD}")]
        public async Task<IEnumerable<Formula>> BuscarPorCgProdCopiar(string CG_PROD)
        {
            List<Formula> formulas = new List<Formula>();
            ConexionSQL xConexionSQL = new ConexionSQL(CadenaConexionSQL);
            DataTable xTabla = xConexionSQL.EjecutarSQL($"SELECT * FROM Form2 F INNER JOIN (" +
                                                        $"SELECT CG_PROD, MAX(REVISION) AS MAX_REVISION" +
                                                        $" FROM Form2 WHERE CG_PROD = '{CG_PROD}' AND ACTIVO <> 'N'" +
                                                        $" GROUP BY CG_PROD) MaxRevisionSubquery ON F.CG_PROD = MaxRevisionSubquery.CG_PROD" +
                                                        $" AND F.REVISION = MaxRevisionSubquery.MAX_REVISION" +
                                                        $" WHERE F.CG_PROD = '{CG_PROD}' AND F.ACTIVO <> 'N';");

            formulas = xTabla.AsEnumerable().Select(r => new Formula
            {
                Id = r.Field<int>("Registro"),
                Cg_Prod = r.Field<string>("CG_PROD"),
                CG_FORM = r.Field<int>("CG_FORM"),
                Cg_Mat = r.Field<string>("CG_MAT"),
                Cg_Se = r.Field<string>("CG_SE"),
                CG_FORM_SE = r.Field<int>("CG_FORM_SE"),
                CANT_MAT = r.Field<decimal>("CANT_MAT"),
                CG_CLAS = r.Field<int>("CG_CLAS"),
                ACTIVO = r.Field<string>("ACTIVO"),
                CG_DEC = r.Field<int>("CG_DEC"),
                CG_DEC_OF = r.Field<int>("CG_DEC_OF"),
                FE_FORM = r.Field<DateTime>("FE_FORM"),
                MERMA = r.Field<decimal>("MERMA"),
                CANTIDAD = r.Field<decimal>("CANTIDAD"),
                REVISION = r.Field<int>("REVISION"),
                FE_VIGE = r.Field<DateTime>("FE_VIGE"),
                CG_GRUPOMP = r.Field<string>("CG_GRUPOMP"),
                OBSERV = r.Field<string>("OBSERV"),
                TIPO = r.Field<int>("TIPO"),
                Diferencial = r.Field<bool>("Diferencial"),
                CATEGORIA = r.Field<int>("CATEGORIA"),
                USUARIO = r.Field<string>("USUARIO"),
                DES_FORM = r.Field<string>("DES_FORM"),
                DES_REVISION = r.Field<string>("DES_REVISION"),
                UNIPROD = r.Field<string>("UNIPROD"),
                UNIMAT = r.Field<string>("UNIMAT"),
                DOSIS = r.Field<decimal>("DOSIS"),
                REVISION_SE = r.Field<decimal>("REVISION_SE"),
                CANT_BASE = r.Field<decimal>("CANT_BASE"),
                CANT_MAT_BASE = r.Field<decimal>("CANT_MAT_BASE"),
                CANTIDAD_BASE = r.Field<decimal>("CANTIDAD_BASE"),
                TIPOFORM = r.Field<string>("TIPOFORM"),
                CG_ORDEN = r.Field<int>("CG_ORDEN"),
                NOMBREFOTO = r.Field<string>("NOMBREFOTO"),
                FOTO = r.Field<string>("FOTO"),
                AUTORIZA = r.Field<string>("AUTORIZA"),
            }).ToList<Formula>();
            try
            {
                return formulas;
            }
            catch (Exception ex)
            {
                return new List<Formula>();
            }
        }

        [HttpGet("VerificaFormula")]
        public async Task<ActionResult<bool>> ExisteEnFormula([FromQuery] List<string> insumos)
        {
            if (insumos.Count == 0) return true;
            var ExisteEnFormula = await _formulaRepository.InsumoEnFormula(insumos);

            return ExisteEnFormula;
        }

        // POST: api/Formulas
        [HttpPost]
        public async Task<ActionResult<Formula>> PostFormula(Formula form)
        {
            try
            {
                await _formulaRepository.Agregar(form);
                return CreatedAtAction("Get", new { id = form.Id }, form);
            }
            catch (DbUpdateException exx)
            {
                if (!await _formulaRepository.Existe(form.Id))
                {
                    return Conflict();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
        //POST: api/Formulas/PostList
        [HttpPost("PostList")]
        public async Task<ActionResult<List<Formula>>> PostList([FromBody] List<Formula> lista)
        {
            try
            {
                foreach (Formula form in lista)
                {
                    await _formulaRepository.Agregar(form);
                }
                await _formulaRepository.AgregarList(lista);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}