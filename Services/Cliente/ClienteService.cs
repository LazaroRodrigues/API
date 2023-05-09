using System.Collections.Generic;
using System.Data;
using API.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace API.Services.Cliente
{
    public class ClienteService
    {
        public List<ClienteTab> Clientes(string usuario)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM clientes WHERE id_vendedor = @vendedor ORDER BY nome";

            List<ClienteTab> clientes = new List<ClienteTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("vendedor", usuario));
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                ClienteTab cliente = new ClienteTab
                                {
                                    id = int.Parse(dr["id"].ToString()),
                                    nome = dr["nome"].ToString(),
                                    telefone = dr["telefone"].ToString(),
                                    cidade = dr["cidade"].ToString(),
                                    id_vendedor = dr["id_vendedor"].ToString()
                                };

                                clientes.Add(cliente);
                            }
                        }
                        else
                        {
                            clientes = null;
                        }
                    }
                }

                return clientes;
            }
        }

        public List<ClienteTab> ClientesBolao(string usuario, string bolao)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT c.id, c.nome FROM clientes as c
                             JOIN bilhetes as b on b.id_bolao = @bolao
                             WHERE c.id_vendedor = @vendedor
                             and c.id = b.id_cliente
                             group by c.id
                             ORDER BY nome";

            List<ClienteTab> clientes = new List<ClienteTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("vendedor", usuario));
                    cmd.Parameters.Add(new MySqlParameter("bolao", bolao));
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                ClienteTab cliente = new ClienteTab
                                {
                                    id = int.Parse(dr["id"].ToString()),
                                    nome = dr["nome"].ToString(),
                                };

                                clientes.Add(cliente);
                            }
                        }
                        else
                        {
                            clientes = null;
                        }
                    }
                }

                return clientes;
            }
        }

        private bool existeCliente(ClienteTab cliente)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM clientes
                            where nome = @nome
                            and cidade = @cidade
                            and id_vendedor = @id_vendedor";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (DataTable dt = new DataTable())
                    {
                        cmd.Parameters.Add(new MySqlParameter("nome", cliente.nome));
                        cmd.Parameters.Add(new MySqlParameter("id_vendedor", cliente.id_vendedor));
                        cmd.Parameters.Add(new MySqlParameter("cidade", cliente.cidade));

                        MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                        da.Fill(dt);

                        if (dt.Rows.Count > 0)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
        }

        public string CriarCliente(ClienteTab cliente)
        {
            if (existeCliente(cliente) == true)
            {
                return "Esse cliente já existe, procure na lista de clientes.";
            }

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"INSERT INTO clientes (nome, telefone, cidade, id_vendedor)
                           VALUES (@nome, @telefone, @cidade, @id_vendedor)";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("nome", cliente.nome));
                    cmd.Parameters.Add(new MySqlParameter("telefone", cliente.telefone));
                    cmd.Parameters.Add(new MySqlParameter("cidade", cliente.cidade));
                    cmd.Parameters.Add(new MySqlParameter("id_vendedor", cliente.id_vendedor));

                    try
                    {
                        retorno = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        mensagem = e.Message;
                    }

                    if (retorno.Equals(1))
                    {
                        mensagem = "Criou";
                    }
                }
            }
            return mensagem;
        }
    }
}