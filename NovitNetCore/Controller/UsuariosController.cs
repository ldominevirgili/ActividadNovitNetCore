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
    public class UsuariosController : ControllerBase
    {
        private readonly ActividadContexto contexto;
        private readonly IConfiguration configuration;

        public UsuariosController(ActividadContexto contexto, IConfiguration configuration)
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
        public ActionResult<List<UsuariosViewModel>> Get()
        {
            return Ok(contexto.Usuario);
        }

        [HttpGet("{id}")]


        public ActionResult<UsuariosViewModel> Get(int id)
        {
            if (contexto.Usuario.ToList().Exists(usuario => usuario.IdUsuario == id))
            {
                var usuario = contexto.Usuario.ToList().Find(usuario => usuario.IdUsuario == id);
                return Ok(usuario);
            }
            else
            {
                return BadRequest($"No hay usuario en la base de datos con el id: {id}");
            }
        }



        [HttpPost]
        [Route("[action]")]
        public ActionResult<List<UsuariosViewModel>> NuevoUsuario([FromBody] UsuariosViewModel nuevoUsuario)
        {
            contexto.Usuario.Add(new Usuario { Nombre = nuevoUsuario.Nombre, Apellido = nuevoUsuario.Apellido, Username = nuevoUsuario.Username, Password = nuevoUsuario.Password, Email = nuevoUsuario.Email, Estado = nuevoUsuario.Estado });


            contexto.SaveChanges();

            return Ok(contexto.Usuario);
        }


        [HttpPut]
        [Route("[action]/{id}")]
        public ActionResult<List<UsuariosViewModel>> ModificarUsuario([FromBody] UsuariosViewModel unUsuario, int id)
        {


            if (contexto.Usuario.ToList().Exists(usuario => usuario.IdUsuario == id))

            {


                var usuario = contexto.Usuario.ToList().Find(usuario => usuario.IdUsuario == id);



                usuario.Nombre = unUsuario.Nombre;
                usuario.Estado = unUsuario.Estado;
                usuario.Username = unUsuario.Username;
                usuario.Password = unUsuario.Password;
                usuario.Email = unUsuario.Email;
                usuario.Estado = unUsuario.Estado;



                contexto.SaveChanges();
                return Ok(contexto.Usuario);
            }
            else
            {

                return BadRequest($"No hay usuario en la base de datos con el id: {id}");
            }

        }

        [HttpDelete]
        [Route("[action]/{id}")]
        public ActionResult<List<UsuariosViewModel>> EliminarUsuario(int id)
        {


            var usuarioABorrar = contexto.Usuario.ToList().Find(usuario => usuario.IdUsuario == id);



            if (usuarioABorrar != null)
            {

                var success = contexto.Usuario.Remove(usuarioABorrar);
                contexto.SaveChanges();
                return Ok($"Fue eliminado de la base de datos el id: {id}");
            }
            else
            {
                return BadRequest($"No hay usuario en la base de datos con el id: {id}.");
            }
        }

    }

}
