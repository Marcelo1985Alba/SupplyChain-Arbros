﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SupplyChain.Client.HelperService;
using SupplyChain.Server.Repositorios;
using SupplyChain.Shared;
using SupplyChain.Shared.Models;

namespace SupplyChain.Server.Controllers.ABM
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcunProcesosController : ControllerBase
    {
        //private readonly ProcunProcesosRepository _procesosRepository;
        private readonly AppDbContext _context;

        public ProcunProcesosController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Compras
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcunProcesos>>> GetProcunProcesos()
        {
            //try
            //{
            //    var respuesta = await _procesosRepository.ObtenerTodos();
            //    return respuesta;
            //}
            //catch (Exception ex)
            //{
            //    return BadRequest(ex.Message);
            //}
            return await _context.ProcunProcesos.ToListAsync();

        }

        [HttpGet("{REGISTRO}")]
        public async Task<ActionResult<ProcunProcesos>> GetProcunProceso(int registro)
        {
            var proceso = await _context.ProcunProcesos.FindAsync(registro);
            if (proceso == null)
            {
                return NotFound();
            }
            return proceso;
            //    try
            //    {
            //        var proceso = await _procesosRepository.ObtenerPorId(id);

            //        if (proceso == null)
            //        {
            //            return NotFound();
            //        }

            //        return Ok(proceso);

            //    }
            //    catch (Exception ex)
            //    {
            //        return BadRequest(ex.Message);
            //    }

            //}

        }
    }
}

