using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    public class RepositorioInmueble
    {
		private readonly IConfiguration configuration;
		private readonly string connectionString;

		public RepositorioInmueble(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}
		public int Alta(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Inmueble (Direccion,Tipo, Precio, Estado, IdPropietario) " +
					$"VALUES (@direccion, @tipo, @precio @estado, @idPropietario);" +
					$"SELECT SCOPE_IDENTITY();";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue("@tipo", i.Tipo);
					command.Parameters.AddWithValue("@precio", i.Precio);
					command.Parameters.AddWithValue("@estado", i.Estado);
					command.Parameters.AddWithValue("@idPropietario", i.IdPropietario);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					i.IdInmueble = res;
					connection.Close();
				}
			}
			return res;
		}
		public int Baja(int id)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"DELETE FROM Inmueble WHERE Id = @idInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idInmueble", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Inmueble i)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Inmueble SET Direccion=@direccion, Tipo=@tipo, Precio=@precio, Estado=@estado, IdPropietario=@idPropietario " +
					$"WHERE Id = @idInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@direccion", i.Direccion);
					command.Parameters.AddWithValue("@tipo", i.Tipo);
					command.Parameters.AddWithValue("@precio", i.Precio);
					command.Parameters.AddWithValue("@estado", i.Estado);
					command.Parameters.AddWithValue("@IdPropietario", i.IdPropietario);
					command.Parameters.AddWithValue("@IdInmueble", i.IdInmueble);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Inmueble> ObtenerTodos()
		{
			IList<Inmueble> res = new List<Inmueble>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.IdInmueble, Direccion,Tipo, Precio, Estado, IdPropietario," +
					" p.Nombre, p.Apellido" +
					" FROM Inmueble i INNER JOIN Propietario p ON i.IdPropietario = p.IdPropietario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Inmueble i = new Inmueble
						{
							IdInmueble = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetString(2),
							Precio = reader.GetDecimal(3),
							Estado = reader.GetBoolean(4),
							IdPropietario = reader.GetInt32(5),
							Propietario = new Propietario
							{
								IdPropietario = reader.GetInt32(6),
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							}
						};
						res.Add(i);
					}
					connection.Close();
				}
			}
			return res;
		}
		public Inmueble ObtenerPorId(int id)
		{
			Inmueble i = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT i.IdInmueble, Direccion, Tipo, Precio, Estado, IdPropietario, p.Nombre, p.Apellido" +
					$" FROM Inmueble i INNER JOIN Propietario p ON i.IdInmueble = p.IdPropietario" +
					$" WHERE i.IdInmueble=@idInmueble";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idInmueble", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						i = new Inmueble
						{
							IdInmueble = reader.GetInt32(0),
							Direccion = reader.GetString(1),
							Tipo = reader.GetString(2),
							Precio = reader.GetDecimal(3),
							Estado = reader.GetBoolean(4),
							IdPropietario= reader.GetInt32(5),
							Propietario = new Propietario
							{
								IdPropietario = reader.GetInt32(6),
								Nombre = reader.GetString(7),
								Apellido = reader.GetString(8),
							}
						};
					}
					connection.Close();
				}
			}
			return i;
		}
	}
}
