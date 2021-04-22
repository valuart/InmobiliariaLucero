﻿using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InmobiliariaLucero.Models
{
    public class RepositorioUsuario
    {
		private readonly IConfiguration configuration;
		private readonly string connectionString;

		public RepositorioUsuario(IConfiguration configuration)
		{
			this.configuration = configuration;
			connectionString = configuration["ConnectionStrings:DefaultConnection"];
		}
		public int Alta(Usuario u)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"INSERT INTO Usuario (Nombre, Apellido, Avatar, Email, Rol, Clave) " +
					$"VALUES (@nombre, @apellido, @avatar, @email, @rol, @clave);" +
					$"SELECT SCOPE_IDENTITY();";//devuelve el id insertado
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", u.Nombre);
					command.Parameters.AddWithValue("@apellido", u.Apellido);
					command.Parameters.AddWithValue("@avatar", u.Avatar);
					command.Parameters.AddWithValue("@email", u.Email);
					command.Parameters.AddWithValue("@rol", u.Rol);
					command.Parameters.AddWithValue("@clave", u.Clave);
					connection.Open();
					res = Convert.ToInt32(command.ExecuteScalar());
					u.IdUsuario = res;
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
				string sql = $"DELETE FROM Usuario WHERE IdUsuario = @idUsuario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@idUsuario", id);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}
		public int Modificacion(Usuario u)
		{
			int res = -1;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"UPDATE Usuario SET Nombre=@nombre, Apellido=@apellido, Avatar=@avatar, Email=@email, Rol=@rol, Clave=@clave " +
					$"WHERE IdUsuario = @idUsuario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.AddWithValue("@nombre", u.Nombre);
					command.Parameters.AddWithValue("@apellido", u.Apellido);
					command.Parameters.AddWithValue("@avatar", u.Avatar);
					command.Parameters.AddWithValue("@email", u.Email);
					command.Parameters.AddWithValue("@rol", u.Rol);
					command.Parameters.AddWithValue("@clave", u.Clave);
					command.Parameters.AddWithValue("@id", u.IdUsuario);
					connection.Open();
					res = command.ExecuteNonQuery();
					connection.Close();
				}
			}
			return res;
		}

		public IList<Usuario> ObtenerTodos()
		{
			IList<Usuario> res = new List<Usuario>();
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdUsuario, Nombre, Apellido, Avatar, Email, Rol, Clave" +
					$" FROM Usuario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						Usuario u = new Usuario
						{
							IdUsuario = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Avatar = reader["Avatar"].ToString(),
							Email = reader.GetString(4),
							Rol = reader.GetInt32(5),
							Clave = reader.GetString(6),
						};
						res.Add(u);
					}
					connection.Close();
				}
			}
			return res;
		}

		public Usuario ObtenerPorId(int id)
		{
			Usuario u = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdUsuario, Nombre, Apellido, Avatar, Email, Rol, Clave FROM Usuario" +
					$" WHERE IdUsuario=@idUsuario";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@idUsuario", SqlDbType.Int).Value = id;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						u = new Usuario
						{
							IdUsuario = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Avatar = reader["Avatar"].ToString(),
							Email = reader.GetString(4),
							Rol = reader.GetInt32(5),
							Clave = reader.GetString(6),
						};
					}
					connection.Close();
				}
			}
			return u;
		}

		public Usuario ObtenerPorEmail(string email)
		{
			Usuario u = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdUsuario, Nombre, Apellido, Avatar, Email, Rol, Clave FROM Usuario" +
					$" WHERE Email=@email";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.CommandType = CommandType.Text;
					command.Parameters.Add("@email", SqlDbType.VarChar).Value = email;
					connection.Open();
					var reader = command.ExecuteReader();
					if (reader.Read())
					{
						u = new Usuario
						{
							IdUsuario = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Avatar = reader["Avatar"].ToString(),
							Email = reader.GetString(4),
							Rol = reader.GetInt32(5),
							Clave = reader.GetString(6),
						};
					}
					connection.Close();
				}
			}
			return u;
		}

		public IList<Usuario> BuscarPorNombre(string nombre)
		{
			List<Usuario> res = new List<Usuario>();
			Usuario u = null;
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				string sql = $"SELECT IdUsuario, Nombre, Apellido, Avatar, Email, Rol, Clave FROM Usuario" +
					$" WHERE Nombre LIKE %@nombre% OR Apellido LIKE %@nombre";
				using (SqlCommand command = new SqlCommand(sql, connection))
				{
					command.Parameters.Add("@nombre", SqlDbType.VarChar).Value = nombre;
					command.CommandType = CommandType.Text;
					connection.Open();
					var reader = command.ExecuteReader();
					while (reader.Read())
					{
						u = new Usuario
						{
							IdUsuario = reader.GetInt32(0),
							Nombre = reader.GetString(1),
							Apellido = reader.GetString(2),
							Avatar = reader["Avatar"].ToString(),
							Email = reader.GetString(4),
							Rol = reader.GetInt32(5),
							Clave = reader.GetString(6),
						};
						res.Add(u);
					}
					connection.Close();
				}
			}
			return res;
		}

        internal object ObtenerPorEmail(object email)
        {
            throw new NotImplementedException();
        }
    }
}
