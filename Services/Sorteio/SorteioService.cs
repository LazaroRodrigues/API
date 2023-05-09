using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace API.Services.Sorteio
{
    public class SorteioService
    {
        public List<SorteioTab> ListaSorteios()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM sorteios";

            List<SorteioTab> sorteios = new List<SorteioTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                SorteioTab sorteio = new SorteioTab();
                                sorteio.id = int.Parse(dr["id"].ToString());
                                sorteio.nome = dr["nome"].ToString();
                                sorteio.descricao = dr["descricao"].ToString();
                                sorteio.status = dr["status"].ToString();
                                sorteio.valor = decimal.Parse(dr["valor"].ToString());
                                sorteio.imagem = dr["imagem"].ToString();
                                sorteio.quantidade = int.Parse(dr["quantidade"].ToString());
                                sorteio.data_sorteio = dr["data_sorteio"].ToString();
                                sorteio.resultado = dr["resultado"].ToString();

                                sorteios.Add(sorteio);
                            }
                        }
                        else
                        {
                            sorteios = null;
                        }
                    }
                }

                return sorteios;
            }
        }

        public string CriarSorteio(SorteioTab sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"INSERT INTO sorteios (nome, descricao, status, valor, imagem, quantidade, data_sorteio)
                           VALUES (@nome, @descricao, @status, @valor, @imagem, @quantidade, @data_sorteio)";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("nome", sorteio.nome));
                    cmd.Parameters.Add(new MySqlParameter("descricao", sorteio.descricao));
                    cmd.Parameters.Add(new MySqlParameter("status", sorteio.status));
                    cmd.Parameters.Add(new MySqlParameter("valor", sorteio.valor));
                    cmd.Parameters.Add(new MySqlParameter("imagem", sorteio.imagem));
                    cmd.Parameters.Add(new MySqlParameter("quantidade", sorteio.quantidade));
                    cmd.Parameters.Add(new MySqlParameter("data_sorteio", sorteio.data_sorteio));

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

        public string EditarSorteio(SorteioTab sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"UPDATE sorteios SET nome = @nome,
                             descricao = @descricao,
                             status = @status,
                             valor = @valor,
                             imagem = @imagem,
                             quantidade = @quantidade,
                             data_sorteio = @data_sorteio
                             WHERE id = @id ";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("nome", sorteio.nome));
                    cmd.Parameters.Add(new MySqlParameter("descricao", sorteio.descricao));
                    cmd.Parameters.Add(new MySqlParameter("status", sorteio.status));
                    cmd.Parameters.Add(new MySqlParameter("valor", sorteio.valor));
                    cmd.Parameters.Add(new MySqlParameter("imagem", sorteio.imagem));
                    cmd.Parameters.Add(new MySqlParameter("quantidade", sorteio.quantidade));
                    cmd.Parameters.Add(new MySqlParameter("data_sorteio", sorteio.data_sorteio));
                    cmd.Parameters.Add(new MySqlParameter("id", sorteio.id));

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
                        mensagem = "Editou";
                    }
                }
            }
            return mensagem;
        }

        public string AtualizarStatus(SorteioTab sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"UPDATE sorteios SET status = @status WHERE id = @id ";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("status", sorteio.status));
                    cmd.Parameters.Add(new MySqlParameter("id", sorteio.id));

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
                        mensagem = "Atualizou status";
                    }
                }
            }
            return mensagem;
        }

        public string ExcluirSorteio(SorteioTab sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"DELETE FROM sorteios WHERE id = @id";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", sorteio.id));

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
                        mensagem = "Excluiu";
                    }
                }
            }

            return mensagem;
        }

        public string LancarResultado(SorteioTab sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"UPDATE sorteios SET resultado = @resultado WHERE id = @id ";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("resultado", sorteio.resultado));
                    cmd.Parameters.Add(new MySqlParameter("id", sorteio.id));

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
                        mensagem = "Atualizou resultado";
                    }
                }
            }
            return mensagem;
        }
    }
}