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
    public class RolesController : ControllerBase
    {
        private readonly ActividadContexto contexto;
        private readonly IConfiguration configuration;

        public RolesController(ActividadContexto contexto, IConfiguration configuration)
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
        public ActionResult<List<RolesViewModel>> Get([FromHeader] string token)
        {   
            if (ValidarToken(token))
            return Ok(contexto.Rol);
            else
                return Unauthorized();
        }

        [HttpGet("{id}")]


        public ActionResult<RolesViewModel> Get([FromHeader] string token, int id)
        {
            if (ValidarToken(token))
            { 
            if (contexto.Rol.ToList().Exists(rol => rol.IdRol == id))
            {

                var rol = contexto.Rol.ToList().Find(rol => rol.IdRol == id);
                return Ok(contexto.Rol);
            }
            else
            {

                return BadRequest($"No hay rol en la base de datos con el id: {id}");
            }
            }
            else
                return Unauthorized();
        }


        [HttpPost]
        [Route("[action]")]
        public ActionResult<List<RolesViewModel>> NuevoRol([FromHeader] string token, [FromBody] RolesViewModel nuevoRol)
        {   
            if (ValidarToken(token))
            { 
            contexto.Rol.Add(new Rol { Nombre = nuevoRol.Nombre, Estado = nuevoRol.Estado });
            contexto.SaveChanges();
            return Ok(contexto.Rol);
                }
            else
                return Unauthorized();
        }


        [HttpPut]
        [Route("[action]/{id}")]
        public ActionResult<List<RolesViewModel>> ModificarRol([FromHeader] string token, [FromBody] RolesViewModel unRol, int id)
        {
             if (ValidarToken(token))
            { 

            if (contexto.Rol.ToList().Exists(rol => rol.IdRol == id))

            {


                var rol = contexto.Rol.ToList().Find(rol => rol.IdRol == id);



                rol.Nombre = unRol.Nombre;
                rol.Estado = unRol.Estado;


                contexto.SaveChanges();
                return Ok(contexto.Rol);
            }
            else
            {

                return BadRequest($"No hay usuario en la base de datos con el id: {id}");
            }
             }
            else
                return Unauthorized();

        }

        [HttpDelete]
        [Route("[action]/{id}")]
        public ActionResult Eliminar([FromHeader] string token, int id)
        {
             if (ValidarToken(token))
            { 

            var eliminarRol = contexto.Rol.ToList().Find(rol => rol.IdRol == id);



            if (eliminarRol != null)
            {
                var success = contexto.Rol.Remove(eliminarRol);
                contexto.SaveChanges();

                return Ok($"Fue eliminado de la base de datos el id: {id} ");
            }
            else
            {
                return BadRequest($"No hay usuario en la base de datos con el id: {id}.");
            }
            }
            else
                return Unauthorized();
        }
    }
}

