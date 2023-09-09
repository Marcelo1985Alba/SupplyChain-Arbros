using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;

namespace SupplyChain.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FormulasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly FormulaRepository _formulaRepository;

    public FormulasController(FormulaRepository formulaRepository)
    {
        _formulaRepository = formulaRepository;
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
        var xSQL = string.Format($"Select * From Form2 where CG_PROD = '{CG_PROD}'");
        try
        {
            return await _formulaRepository.Obtener(f => f.Cg_Prod == CG_PROD).ToListAsync();
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
                return Conflict();
            return BadRequest();
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
            var exito = await _formulaRepository.AgregarList(lista);
            if (exito)
                return Ok(lista);
            return BadRequest("Error al guardar formulas");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}