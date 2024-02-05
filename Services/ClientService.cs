using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using coink.Context;
using coink.Models;
using Npgsql;
using NpgsqlTypes;

namespace coink.Services
{
    public class ClientService
    {
        private readonly AppDbContext _context;

        public ClientService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Client>> GetClients()
        {
            try
            {
                var clientData = await _context.Client.FromSqlRaw("SELECT * FROM get_clients_information()").ToListAsync();
                return MapClientDataToObjects(clientData);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener clientes = " + ex.Message);
            }
        }

        public async Task<Client> GetClient(int id)
        {
            try
            {
                var idParam = new NpgsqlParameter("@id", id);
                idParam.NpgsqlDbType = NpgsqlDbType.Integer;

                var clientData = await _context.Client.FromSqlRaw("SELECT * FROM get_clients_information(@id)", idParam).ToListAsync();
                return MapClientDataToObjects(clientData).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al obtener cliente", ex);
            }
        }

        public async Task CreateClient(Client client)
        {
            try
            {
                // Crear parámetros para el procedimiento almacenado
                var parameters = new[]
                {
                    new NpgsqlParameter("@name", client.name),
                    new NpgsqlParameter("@phone", client.phone),
                    new NpgsqlParameter("@id_country", client.id_country),
                    new NpgsqlParameter("@id_department", client.id_department),
                    new NpgsqlParameter("@id_municipality", client.id_municipality),
                    new NpgsqlParameter("@address", client.addres)
                };
                await _context.Database.ExecuteSqlRawAsync("CALL create_client(@name, @phone, @id_country, @id_department, @id_municipality, @address)", parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al crear cliente", ex);
            }
        }

        public async Task EditClient(int id, Client client)
        {
            try
            {
                var parameters = new[]
                {
                    new NpgsqlParameter("@id", id),
                    new NpgsqlParameter("@name", client.name),
                    new NpgsqlParameter("@phone", client.phone),
                    new NpgsqlParameter("@id_country", client.id_country),
                    new NpgsqlParameter("@id_department", client.id_department),
                    new NpgsqlParameter("@id_municipality", client.id_municipality),
                    new NpgsqlParameter("@address", client.addres)
                };
                await _context.Database.ExecuteSqlRawAsync(
                    "CALL edit_client(@id, @name, @phone, @id_country, @id_department, @id_municipality, @address)",
                    parameters);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al editar cliente", ex);
            }
        }

        public async Task DeleteClient(int id)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync("CALL delete_client(@id)", new NpgsqlParameter("@id", id));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al eliminar cliente", ex);
            }
        }

        private IEnumerable<Client> MapClientDataToObjects(IEnumerable<Client> clientData)
        {
            return clientData.Select(cd => new Client
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
                addres = cd.addres
            });
        }
    }
}
