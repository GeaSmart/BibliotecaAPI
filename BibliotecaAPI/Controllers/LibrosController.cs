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
    [Route("api/libros")]    
    public class LibrosController:ControllerBase
    {
        private readonly ApplicationDbContext context;

        public LibrosController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<Libro>>> Get()
        {
            return await context.Libro.ToListAsync();
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Libro>> Get(int id)
        {
            var existe = await context.Libro.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound($"No se encontró el libro solicitado con id {id}");

            return await context.Libro.Include(x => x.Autor).FirstOrDefaultAsync(x => x.Id == id);
        }

        [HttpPost]
        public async Task<ActionResult>PostLibro(Libro libro)
        {
            var existe = await context.Autor.AnyAsync(x => x.Id == libro.AutorId);
            if (!existe)
                return NotFound($"No se encontró el autor ingresado con id {libro.AutorId}");

            context.Libro.Add(libro);
            await context.SaveChangesAsync();
            return Ok();
        }

        [HttpPut("{id:int}")]        
        public async Task<ActionResult> Put(int id, Libro libro)
        {
            var existeLibro = await context.Libro.AnyAsync(x => x.Id == id);
            if (!existeLibro)
                return NotFound($"El libro con id {id} que se quiere actualizar no existe");

            var existeAutor = await context.Autor.AnyAsync(x => x.Id == libro.AutorId);
            if (!existeAutor)
                return NotFound($"El autor con id {libro.AutorId} que se quiere actualizar no existe");

            if (id != libro.Id)
                return BadRequest("Los id no coinciden");

            context.Libro.Update(libro);
            await context.SaveChangesAsync();

            return Ok();                   
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var existe = await context.Libro.AnyAsync(x => x.Id == id);
            if (!existe)
                return NotFound($"El libro con id {id} no existe.");
            context.Libro.Remove(new Libro { Id = id });
            await context.SaveChangesAsync();

            return Ok();
        }


    }
}
