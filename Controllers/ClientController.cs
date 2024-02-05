using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using coink.Context;
using coink.Models;
using Microsoft.Data.SqlClient;
using Npgsql;
using NpgsqlTypes;

namespace coink.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientController(AppDbContext context)
        {
            _context = context;
        }

        //api/Client
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
        {
            try
            {
                var clientData = await _context.Client.FromSqlRaw("SELECT * FROM get_clients_information()").ToListAsync();

                // Mapear los resultados a objetos Client
                var clients = clientData.Select(cd => new Client
                {
                    id = cd.id,
                    name = cd.name,
                    phone = cd.phone,
                    id_country = cd.id_country,
                    id_department = cd.id_department,
                    id_municipality = cd.id_municipality,
                    country_name = cd.country_name,
                    department_name = cd.department_name,
                    municipality_name = cd.municipality_name,
                    addres = cd.addres,
                }).ToList();

                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener clientes: " + ex.Message);
            }
        }

        // GET: api/Client/id
        [HttpGet("{id}")]
        public async Task<ActionResult<Client>> GetClient(int id)
        {
            try
            {
                var idParam = new NpgsqlParameter("@id", id);
                idParam.NpgsqlDbType = NpgsqlDbType.Integer;

                var clientData = await _context.Client.FromSqlRaw("SELECT * FROM get_clients_information(@id)", idParam).ToListAsync();

                var clients = clientData.Select(cd => new Client
                {
                    id = cd.id,
                    name = cd.name,
                    phone = cd.phone,
                    id_country = cd.id_country,
                    id_department = cd.id_department,
                    id_municipality = cd.id_municipality,
                    country_name = cd.country_name,
                    department_name = cd.department_name,
                    municipality_name = cd.municipality_name,
                    addres = cd.addres,
                }).ToList();

                return Ok(clients);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener clientes: " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] Client client)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Crear parámetros para el procedimiento almacenado
                var parameters = new[]
                {
                    new NpgsqlParameter("@name", client.name),
                    new NpgsqlParameter("@phone", client.phone),
                    new NpgsqlParameter("@id_country", client.id_country),
                    new NpgsqlParameter("@id_department", client.id_department),
                    new NpgsqlParameter("@id_municipality", client.id_municipality),
                    new NpgsqlParameter("@address", client.addres),
                };
                await _context.Database.ExecuteSqlRawAsync("CALL create_client(@name, @phone, @id_country, @id_department, @id_municipality, @address)", parameters);
                return CreatedAtAction(nameof(GetClients), new { id = client.id }, client);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear cliente: " + ex.Message);
            }
        }


        // PUT: api/Client/id
        [HttpPut("{id}")]
        public async Task<IActionResult> EditClient(int id, [FromBody] Client client)
        {
            try
            {
                // Validar modelo
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Crear parámetros para el procedimiento almacenado
                var parameters = new[]
                {
                    new NpgsqlParameter("@id", id),
                    new NpgsqlParameter("@name", client.name),
                    new NpgsqlParameter("@phone", client.phone),
                    new NpgsqlParameter("@id_country", client.id_country),
                    new NpgsqlParameter("@id_department", client.id_department),
                    new NpgsqlParameter("@id_municipality", client.id_municipality),
                    new NpgsqlParameter("@address", client.addres),
                };

                // Llamar al procedimiento almacenado usando ExecuteSqlCommandAsync
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL edit_client(@id, @name, @phone, @id_country, @id_department, @id_municipality, @address)",
                    parameters);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al editar cliente: " + ex.Message);
            }
        }

        // DELETE: api/Client/id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            try
            {
                // Buscar el cliente a eliminar
                var client = await _context.Client.FindAsync(id);

                if (client == null)
                {
                    return NotFound();
                }

                await _context.Database.ExecuteSqlRawAsync("CALL delete_client(@id)", new NpgsqlParameter("@id", id));

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al eliminar cliente: " + ex.Message);
            }
        }
    }
}


//using Microsoft.AspNetCore.Mvc;
//using coink.Models;
//using coink.Services;

//namespace coink.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ClientController : ControllerBase
//    {
//        private readonly ClientService _clientService;

//        public ClientController(ClientService clientService)
//        {
//            _clientService = clientService;
//        }

//        // GET: api/Client
//        [HttpGet]
//        public async Task<ActionResult<IEnumerable<Client>>> GetClients()
//        {
//            try
//            {
//                var clients = await _clientService.GetClients();
//                return Ok(clients);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // GET: api/Client/id
//        [HttpGet("{id}")]
//        public async Task<ActionResult<Client>> GetClient(int id)
//        {
//            try
//            {
//                var client = await _clientService.GetClient(id);
//                if (client == null)
//                {
//                    return NotFound();
//                }
//                return Ok(client);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // POST: api/Client
//        [HttpPost]
//        public async Task<IActionResult> CreateClient([FromBody] Client client)
//        {
//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                await _clientService.CreateClient(client);

//                return CreatedAtAction(nameof(GetClients), new { id = client.id }, client);
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // PUT: api/Client/id
//        [HttpPut("{id}")]
//        public async Task<IActionResult> EditClient(int id, [FromBody] Client client)
//        {
//            try
//            {
//                if (!ModelState.IsValid)
//                {
//                    return BadRequest(ModelState);
//                }

//                if (id != client.id)
//                {
//                    return BadRequest();
//                }

//                await _clientService.EditClient(id, client);

//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }

//        // DELETE: api/Client/id
//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteClient(int id)
//        {
//            try
//            {
//                await _clientService.DeleteClient(id);

//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, ex.Message);
//            }
//        }
//    }
//}