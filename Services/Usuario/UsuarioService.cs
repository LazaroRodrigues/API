using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using API.Models;
using API.Services.Cliente;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace API.Services.Usuarios
{
    public class UsuarioService
    {
        public List<UsuarioTab> ListaVendedores(string id, string tipo)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT c.*, u.nome as gerenteNome FROM usuarios as c
                             LEFT JOIN usuarios as u on u.id = c.gerente
                             WHERE c.tipo = 'Vendedor'
                             AND c.usuario NOT LIKE '%(!)'";

            if (tipo == "Gerente")
            {
                query = @"SELECT c.*, u.nome as gerenteNome FROM usuarios as c
                             LEFT JOIN usuarios as u on u.id = c.gerente
                             WHERE c.tipo = 'Vendedor'
                             AND c.usuario NOT LIKE '%(!)'
                             AND c.gerente = @id";
            }

            List<UsuarioTab> usuarios = new List<UsuarioTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", id));
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                UsuarioTab usuario = new UsuarioTab();
                                usuario.id = dr["id"].ToString();
                                usuario.nome = dr["nome"].ToString();
                                usuario.usuario = dr["usuario"].ToString();
                                usuario.senha = dr["senha"].ToString();
                                usuario.telefone = dr["telefone"].ToString();
                                usuario.cidade = dr["cidade"].ToString();
                                usuario.tipo = dr["tipo"].ToString();
                                usuario.status = dr["status"].ToString();
                                usuario.criado_por = dr["criado_por"].ToString();
                                usuario.data_criacao = DateTime.Parse(dr["data_criacao"].ToString());
                                usuario.gerente = dr["gerente"].ToString() == "" ? "-1" : dr["gerente"].ToString();
                                usuario.gerenteNome = dr["gerente"].ToString() == "" ? "Sem gerente" : dr["gerenteNome"].ToString();
                                usuario.comissao = int.Parse(dr["comissao"].ToString());

                                usuarios.Add(usuario);
                            }
                        }
                        else
                        {
                            usuarios = null;
                        }
                    }
                }

                return usuarios;
            }
        }

        public bool UsuarioBloqueado(string id)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM usuarios WHERE id = @id";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", id));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                if (dr["status"].ToString() == "Inativo")
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
            }
            return false;
        }

        public List<UsuarioTab> ListaVendedoresAtivos()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT c.id, c.nome, c.usuario FROM usuarios as c
                             WHERE c.tipo = 'Vendedor'
                             AND c.status = 'Ativo'
                             AND c.banca = @banca";

            List<UsuarioTab> usuarios = new List<UsuarioTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("banca", configuration.GetValue<string>("Configs:Banca")));
                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                UsuarioTab usuario = new UsuarioTab();
                                usuario.id = dr["id"].ToString();
                                usuario.nome = dr["nome"].ToString();
                                usuario.usuario = dr["usuario"].ToString();
                                usuarios.Add(usuario);
                            }
                        }
                        else
                        {
                            usuarios = null;
                        }
                    }
                }

                return usuarios;
            }
        }

        public List<UsuarioTab> ListaGerentes()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM usuarios WHERE tipo = 'Gerente'
                             AND usuario NOT LIKE '%(!)'";

            List<UsuarioTab> usuarios = new List<UsuarioTab>();

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
                                UsuarioTab usuario = new UsuarioTab();
                                usuario.id = dr["id"].ToString();
                                usuario.nome = dr["nome"].ToString();
                                usuario.usuario = dr["usuario"].ToString();
                                usuario.senha = dr["senha"].ToString();
                                usuario.telefone = dr["telefone"].ToString();
                                usuario.cidade = dr["cidade"].ToString();
                                usuario.tipo = dr["tipo"].ToString();
                                usuario.status = dr["status"].ToString();
                                usuario.criado_por = dr["criado_por"].ToString();
                                usuario.data_criacao = DateTime.Parse(dr["data_criacao"].ToString());
                                usuario.comissao = int.Parse(dr["comissao"].ToString());

                                usuarios.Add(usuario);
                            }
                        }
                        else
                        {
                            usuarios = null;
                        }
                    }
                }

                return usuarios;
            }
        }

        public string transferir()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = "Database=bets;Data Source=localhost;Port=3306;UserId=root;Password=357951;default command timeout=0";
            string query = @"SELECT * FROM usuarios WHERE tipo = 'Gerente'
                             AND status = 'Ativo' AND banca = 16 AND usuario NOT LIKE '%(!)'";

            List<UsuarioTab> usuarios = new List<UsuarioTab>();

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
                                UsuarioTab usuario = new UsuarioTab();
                                usuario.id = dr["id"].ToString();
                                usuario.nome = dr["nome"].ToString();
                                usuario.usuario = dr["usuario"].ToString();
                                usuario.senha = dr["senha"].ToString();
                                usuario.telefone = dr["telefone"].ToString();
                                usuario.cidade = "Santos - SP";
                                usuario.tipo = dr["tipo"].ToString();
                                usuario.status = dr["status"].ToString();
                                usuario.criado_por = dr["criado_por"].ToString();
                                usuario.data_criacao = DateTime.Parse(dr["data_criacao"].ToString());
                                usuario.comissao = 5;

                                CriarUsuarioTransferir(usuario);
                            }
                        }
                        else
                        {
                            usuarios = null;
                        }
                    }
                }
                return "Ok";
            }
        }

        public string transferirVendedor()
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = "Database=bets;Data Source=localhost;Port=3306;UserId=root;Password=357951;default command timeout=0";
            string query = @"SELECT * FROM usuarios WHERE tipo = 'Vendedor'
                             AND status = 'Ativo' AND banca = 16 AND usuario NOT LIKE '%(!)'";

            List<UsuarioTab> usuarios = new List<UsuarioTab>();

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
                                UsuarioTab usuario = new UsuarioTab();
                                usuario.id = dr["id"].ToString();
                                usuario.nome = dr["nome"].ToString();
                                usuario.usuario = dr["usuario"].ToString();
                                usuario.senha = dr["senha"].ToString();
                                usuario.telefone = dr["telefone"].ToString();
                                usuario.cidade = "Santos - SP";
                                usuario.gerente = dr["gerente"].ToString() == "" ? "-1" : dr["gerente"].ToString();
                                usuario.tipo = dr["tipo"].ToString();
                                usuario.status = dr["status"].ToString();
                                usuario.criado_por = dr["criado_por"].ToString();
                                usuario.data_criacao = DateTime.Parse(dr["data_criacao"].ToString());
                                usuario.comissao = 5;

                                CriarUsuarioTransferir(usuario);
                            }
                        }
                        else
                        {
                            usuarios = null;
                        }
                    }
                }
                return "Ok";
            }
        }

        public string CriarUsuario(UsuarioTab usuario)
        {
            if (ExisteUsuario(usuario.usuario))
            {
                return "Já existe um usuário cadastrado com este nome de usuário, por favor tente outro.";
            }

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"INSERT INTO usuarios (id, nome, usuario, senha, telefone, cidade, status, tipo, criado_por, data_criacao, gerente, comissao)
                                          VALUES (@id, @nome, @usuario, @senha, @telefone, @cidade, @status, @tipo, @criado_por, @data_criacao, @gerente, @comissao)";
            if (usuario.tipo == "Gerente")
            {
                query = @"INSERT INTO usuarios (id, nome, usuario, senha, telefone, cidade, status, tipo, criado_por, data_criacao, gerente, comissao)
                                          VALUES (@id, @nome, @usuario, @senha, @telefone, @cidade, @status, @tipo, @criado_por, @data_criacao, @gerente, @comissao)";
            }
            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", Guid.NewGuid().ToString("N")));
                    cmd.Parameters.Add(new MySqlParameter("nome", usuario.nome));
                    cmd.Parameters.Add(new MySqlParameter("usuario", usuario.usuario));
                    cmd.Parameters.Add(new MySqlParameter("senha", usuario.senha));
                    cmd.Parameters.Add(new MySqlParameter("telefone", usuario.telefone));
                    cmd.Parameters.Add(new MySqlParameter("cidade", usuario.cidade));
                    cmd.Parameters.Add(new MySqlParameter("status", usuario.status));
                    cmd.Parameters.Add(new MySqlParameter("tipo", usuario.tipo));
                    cmd.Parameters.Add(new MySqlParameter("criado_por", usuario.criado_por));
                    cmd.Parameters.Add(new MySqlParameter("data_criacao", DateTime.Now));
                    cmd.Parameters.Add(new MySqlParameter("gerente", usuario.gerente == "-1" ? null : usuario.gerente));
                    cmd.Parameters.Add(new MySqlParameter("comissao", usuario.comissao));

                    try
                    {
                        retorno = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        mensagem = e.Message;

                        if (e.Message.Contains("usuarios.idx_usuarios_usuario"))
                        {
                            return mensagem = "Já existe um usuário cadastrado com este nome de usuário, por favor tente outro.";
                        }
                    }

                    if (retorno.Equals(1))
                    {
                        mensagem = "Criou";
                    }
                }
            }
            return mensagem;
        }

        public string CriarUsuarioTransferir(UsuarioTab usuario)
        {
            if (ExisteUsuario(usuario.usuario))
            {
                return "Já existe um usuário cadastrado com este nome de usuário, por favor tente outro.";
            }

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"INSERT INTO usuarios (id, nome, usuario, senha, telefone, cidade, status, tipo, criado_por, data_criacao, gerente, comissao)
                                          VALUES (@id, @nome, @usuario, @senha, @telefone, @cidade, @status, @tipo, @criado_por, @data_criacao, @gerente, @comissao)";
            if (usuario.tipo == "Gerente")
            {
                query = @"INSERT INTO usuarios (id, nome, usuario, senha, telefone, cidade, status, tipo, criado_por, data_criacao, gerente, comissao)
                                          VALUES (@id, @nome, @usuario, @senha, @telefone, @cidade, @status, @tipo, @criado_por, @data_criacao, @gerente, @comissao)";
            }
            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", usuario.id));
                    cmd.Parameters.Add(new MySqlParameter("nome", usuario.nome));
                    cmd.Parameters.Add(new MySqlParameter("usuario", usuario.usuario));
                    cmd.Parameters.Add(new MySqlParameter("senha", usuario.senha));
                    cmd.Parameters.Add(new MySqlParameter("telefone", usuario.telefone));
                    cmd.Parameters.Add(new MySqlParameter("cidade", usuario.cidade));
                    cmd.Parameters.Add(new MySqlParameter("status", usuario.status));
                    cmd.Parameters.Add(new MySqlParameter("tipo", usuario.tipo));
                    cmd.Parameters.Add(new MySqlParameter("criado_por", usuario.criado_por));
                    cmd.Parameters.Add(new MySqlParameter("data_criacao", DateTime.Now));
                    cmd.Parameters.Add(new MySqlParameter("gerente", usuario.gerente == "-1" ? null : usuario.gerente));
                    cmd.Parameters.Add(new MySqlParameter("comissao", usuario.comissao));

                    try
                    {
                        retorno = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        mensagem = e.Message;

                        if (e.Message.Contains("usuarios.idx_usuarios_usuario"))
                        {
                            return mensagem = "Já existe um usuário cadastrado com este nome de usuário, por favor tente outro.";
                        }
                    }

                    if (retorno.Equals(1))
                    {
                        mensagem = "Criou";
                    }
                }
            }
            return mensagem;
        }

        public string CriarApostador(UsuarioTab usuario)
        {
            if (ExisteUsuario(usuario.usuario))
            {
                return "Já existe um usuário cadastrado com este nome de usuário, por favor tente outro.";
            }

            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"INSERT INTO usuarios (id, nome, usuario, senha, telefone, cidade, status, tipo, criado_por, data_criacao, gerente, comissao)
                                          VALUES (@id, @nome, @usuario, @senha, @telefone, @cidade, @status, @tipo, @criado_por, @data_criacao, @gerente, @comissao)";
            if (usuario.tipo == "Gerente")
            {
                query = @"INSERT INTO usuarios (id, nome, usuario, senha, telefone, cidade, status, tipo, criado_por, data_criacao, gerente, comissao)
                                          VALUES (@id, @nome, @usuario, @senha, @telefone, @cidade, @status, @tipo, @criado_por, @data_criacao, @gerente, @comissao)";
            }
            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    string id = Guid.NewGuid().ToString("N");
                    cmd.Parameters.Add(new MySqlParameter("id", id));
                    cmd.Parameters.Add(new MySqlParameter("nome", usuario.nome));
                    cmd.Parameters.Add(new MySqlParameter("usuario", usuario.usuario));
                    cmd.Parameters.Add(new MySqlParameter("senha", usuario.senha));
                    cmd.Parameters.Add(new MySqlParameter("telefone", usuario.telefone));
                    cmd.Parameters.Add(new MySqlParameter("cidade", usuario.cidade));
                    cmd.Parameters.Add(new MySqlParameter("status", usuario.status));
                    cmd.Parameters.Add(new MySqlParameter("tipo", usuario.tipo));
                    cmd.Parameters.Add(new MySqlParameter("criado_por", usuario.criado_por));
                    cmd.Parameters.Add(new MySqlParameter("data_criacao", DateTime.Now));
                    cmd.Parameters.Add(new MySqlParameter("gerente", usuario.gerente == "-1" ? null : usuario.gerente));
                    cmd.Parameters.Add(new MySqlParameter("comissao", usuario.comissao));

                    try
                    {
                        retorno = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        mensagem = e.Message;

                        if (e.Message.Contains("usuarios.idx_usuarios_usuario"))
                        {
                            return mensagem = "Já existe um usuário cadastrado com este nome de usuário, por favor tente outro.";
                        }
                    }

                    if (retorno.Equals(1))
                    {
                        ClienteTab cliente = new ClienteTab
                        {
                            nome = usuario.nome,
                            id_vendedor = id,
                            telefone = usuario.telefone,
                            cidade = usuario.cidade
                        };
                        new ClienteService().CriarCliente(cliente);
                        mensagem = "Criou";
                    }
                }
            }
            return mensagem;
        }

        private bool ExisteUsuario(string usuario)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT usuario FROM usuarios
                             WHERE usuario = @usuario";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (DataTable dt = new DataTable())
                    {
                        cmd.Parameters.Add(new MySqlParameter("usuario", usuario));

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

        public string AlterarStatus(UsuarioTab usuario)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"UPDATE usuarios SET status = @status WHERE id = @id";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", usuario.id));
                    cmd.Parameters.Add(new MySqlParameter("status", usuario.status));

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
                        mensagem = "Alterou status";
                    }
                }
            }
            return mensagem;
        }

        public string AlterarSenha(UsuarioTab usuario)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"UPDATE usuarios SET senha = @senha WHERE id = @id";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", usuario.id));
                    cmd.Parameters.Add(new MySqlParameter("senha", usuario.senha));

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
                        mensagem = "Alterou senha";
                    }
                }
            }
            return mensagem;
        }

        public string EditarUsuario(UsuarioTab usuario, string tipo)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"UPDATE usuarios SET nome = @nome, usuario = @usuario, senha = @senha, telefone = @telefone, cidade = @cidade, status = @status, tipo = @tipo, gerente = @gerente, comissao = @comissao
                             WHERE id = @id";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", usuario.id));
                    cmd.Parameters.Add(new MySqlParameter("nome", usuario.nome));
                    cmd.Parameters.Add(new MySqlParameter("usuario", usuario.usuario));
                    cmd.Parameters.Add(new MySqlParameter("senha", usuario.senha));
                    cmd.Parameters.Add(new MySqlParameter("telefone", usuario.telefone));
                    cmd.Parameters.Add(new MySqlParameter("cidade", usuario.cidade));
                    cmd.Parameters.Add(new MySqlParameter("status", usuario.status));
                    cmd.Parameters.Add(new MySqlParameter("tipo", usuario.tipo));
                    cmd.Parameters.Add(new MySqlParameter("criado_por", usuario.criado_por));
                    cmd.Parameters.Add(new MySqlParameter("data_criacao", DateTime.Now));
                    cmd.Parameters.Add(new MySqlParameter("gerente", usuario.gerente == "-1" ? null : usuario.gerente));
                    cmd.Parameters.Add(new MySqlParameter("comissao", usuario.comissao));

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

        public string ExcluirUsuario(UsuarioTab usuario)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"UPDATE usuarios SET usuario = CONCAT(usuario, ' (!)')  WHERE id = @id";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", usuario.id));

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
                        usuario.status = "Inativo";
                        AlterarStatus(usuario);
                    }
                }
            }

            return mensagem;
        }

        public UsuarioTab Login(UsuarioTab usuario)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM usuarios WHERE usuario = @usuario AND senha = @senha";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("senha", usuario.senha));
                    cmd.Parameters.Add(new MySqlParameter("usuario", usuario.usuario));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                usuario.id = dr["id"].ToString();
                                usuario.nome = dr["nome"].ToString();
                                usuario.usuario = dr["usuario"].ToString();
                                usuario.senha = dr["senha"].ToString();
                                usuario.telefone = dr["telefone"].ToString();
                                usuario.cidade = dr["cidade"].ToString();
                                usuario.tipo = dr["tipo"].ToString();
                                usuario.status = dr["status"].ToString();
                                usuario.criado_por = dr["criado_por"].ToString();
                                usuario.data_criacao = DateTime.Parse(dr["data_criacao"].ToString());
                                usuario.cidade = dr["cidade"].ToString();
                                usuario.comissao = int.Parse(dr["comissao"].ToString());

                                usuario.gerente = dr["tipo"].ToString() == "Vendedor" ? dr["gerente"].ToString() : null;
                            }
                        }
                        else
                        {
                            usuario = null;
                        }
                    }
                }
                return usuario;
            }
        }

        public UsuarioTab Refresh_token(UsuarioTab usuario)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM usuarios WHERE usuario = @usuario";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("usuario", usuario.usuario));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                usuario.id = dr["id"].ToString();
                                usuario.nome = dr["nome"].ToString();
                                usuario.usuario = dr["usuario"].ToString();
                                usuario.senha = dr["senha"].ToString();
                                usuario.telefone = dr["telefone"].ToString();
                                usuario.cidade = dr["cidade"].ToString();
                                usuario.tipo = dr["tipo"].ToString();
                                usuario.status = dr["status"].ToString();
                                usuario.criado_por = dr["criado_por"].ToString();
                                usuario.data_criacao = DateTime.Parse(dr["data_criacao"].ToString());
                                usuario.cidade = dr["cidade"].ToString();
                                usuario.comissao = int.Parse(dr["comissao"].ToString());

                                usuario.gerente = dr["tipo"].ToString() == "Vendedor" ? dr["gerente"].ToString() : null;
                            }
                        }
                        else
                        {
                            usuario = null;
                        }
                    }
                }

                return usuario;
            }
        }
    }
}