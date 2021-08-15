using BibliotecaAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BibliotecaAPI.Controllers
{
    [ApiController]
    [Route("api/autores")]
    public class AutoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public AutoresController(ApplicationDbContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Obtiene todos los autores
        /// </summary>
        /// <remarks>Autores asociados al sindicato.</remarks>
        /// <returns>Valor de retorno</returns>
        /// <response code="200">Respuesta correcta</response>
        /// <response code="400">Bad request</response>
        [HttpGet]
        
        public async Task<ActionResult<List<Autor>>> Get()
        {
            return await context.Autor.Include(x => x.Libros).ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Autor>> Get(int id)
        {
            return await context.Autor.Include(x => x.Libros).FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult> Post(Autor autor)
        {
            context.Autor.Add(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, Autor autor)
        {
            var existe = await context.Autor.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound("No existe el mencionado autor");

            if (id != autor.Id)
                return BadRequest("Los id no coinciden");

            context.Update(autor);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult>Delete(int id)
        {
            var existe = await context.Autor.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound("No existe el mencionado autor");


            context.Remove(new Autor { Id = id });
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
