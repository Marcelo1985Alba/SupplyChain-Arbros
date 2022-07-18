using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SupplyChain.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FormulasController : ControllerBase
    {
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

        [HttpGet("VerificaFormula")]
        public async Task<ActionResult<bool>> ExisteEnFormula([FromQuery] List<string> insumos)
        {
            if (insumos.Count == 0) return true;
            var ExisteEnFormula = await _formulaRepository.InsumoEnFormula(insumos);

            return ExisteEnFormula;
        }
    }
}
