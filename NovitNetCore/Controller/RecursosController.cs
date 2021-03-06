using System;
using System.Collections.Generic;
using System.Linq;
using JWT.Algorithms;
using JWT.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NovitNetCore.Controller.V1.ViewModels;

namespace NovitNetCore.Controller
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecursosController : ControllerBase
    {
        private readonly ActividadContexto contexto;
        private readonly IConfiguration configuration;

        public RecursosController(ActividadContexto contexto, IConfiguration configuration)
        {
            this.contexto = contexto;
            this.configuration = configuration;
        }
        private bool ValidarToken(string token)
        {
            try
            {
                var json = JwtBuilder.Create()
                         .WithAlgorithm(new HMACSHA256Algorithm())
                         .WithSecret(configuration["Jwt:secret"])
                         .MustVerifySignature()
                         .Decode(token);

                JwtViewModel jwt = JsonConvert.DeserializeObject<JwtViewModel>(json);

                if (jwt.Name != configuration["Jwt:name"] && jwt.Sub != configuration["Jwt:sub"]) return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        [HttpGet]
        public ActionResult<List<RecursosViewModel>> Get([FromHeader] string token)
        {
            if (ValidarToken(token))
            return Ok(contexto.Recurso);
            else
                return Unauthorized();
        }

        [HttpGet("{id}")]


        public ActionResult<RecursosViewModel> Get([FromHeader] string token, int id)
        {
            if (ValidarToken(token))

            {if (contexto.Recurso.ToList().Exists(recurso => recurso.IdRecurso == id))
            {

                var recurso = contexto.Recurso.ToList().Find(recurso => recurso.IdRecurso == id);
                return Ok(contexto.Recurso);
            }
            else
            {

                return BadRequest($"No hay recurso en la base de datos con el id: {id}");
            }
            }
            else
                return Unauthorized();
        }



        [HttpPost]
        [Route("[action]")]
        public ActionResult<List<RecursosViewModel>> NuevoRecurso([FromHeader] string token, [FromBody] RecursosViewModel nuevoRecurso)
        {
             if (ValidarToken(token))
            {
            contexto.Recurso.Add(new Recurso { Nombre = nuevoRecurso.Nombre, Estado = nuevoRecurso.Estado });
                contexto.SaveChanges();
                return Ok(contexto.Recurso);
               
            }
            else
                 return Unauthorized();
        }


        [HttpPut]
        [Route("[action]/{id}")]
        public ActionResult<List<RecursosViewModel>> ModificarRecurso([FromHeader] string token, [FromBody] RecursosViewModel unRecurso, int id)
        {
             if (ValidarToken(token))
            { 

            if (contexto.Recurso.ToList().Exists(recurso => recurso.IdRecurso == id))

            {


                var recurso = contexto.Recurso.ToList().Find(recurso => recurso.IdRecurso == id);



                recurso.Nombre = unRecurso.Nombre;
                recurso.Estado = unRecurso.Estado;


                contexto.SaveChanges();
                return Ok(contexto.Recurso);
            }
            else
            {

                return BadRequest($"No hay recurso en la base de datos con el id: {id}");
            }
            }
             else
                 return Unauthorized();

        }

        [HttpDelete]
        [Route("[action]/{id}")]
        public ActionResult EliminarRecurso([FromHeader] string token, int id)
        {
            if (ValidarToken(token))
            { 
            var eliminarRecurso = contexto.Recurso.ToList().Find(recurso => recurso.IdRecurso == id);


            if (eliminarRecurso != null)
            {

                var success = contexto.Recurso.Remove(eliminarRecurso);
                contexto.SaveChanges();
                return Ok($"El recurso fue eliminado de la base de datos con el id: {id}.");
            }
            else
            {
                return BadRequest($"No hay usuario en la base de datos con el id: {id}");
            }
            }
             else
                 return Unauthorized();
        }
    }
}
