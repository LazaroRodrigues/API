using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using API.Models;
using API.Services.Usuarios;
using MercadoPago.Client;
using MercadoPago.Client.Payment;
using MercadoPago.Resource.Payment;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace API.Services.Bilhete
{
    public class BilheteService
    {
        private bool existeCodigo(string codigo)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT codigo FROM bilhetes
                            where codigo = @codigo";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (DataTable dt = new DataTable())
                    {
                        cmd.Parameters.Add(new MySqlParameter("codigo", codigo));

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

        public List<BilheteTab> ListaBilhetesVencedoresAdmin(AuxGerarPalpite json)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"Select b.id, b.codigo, b.id_sorteio, b.data,b.status, b.valor, c.nome as clienteNome, c.cidade, c.telefone, u.nome as vendedorNome, b.cancelado_por, b.data_cancelamento
                           from bilhetes as b
                           left join clientes as c on b.id_cliente = c.id
                           left join usuarios as u on b.id_vendedor = u.id
                           where b.id_sorteio = @id_sorteio
                           and b.status like '%Lugar'
                           group by b.id
                           order by b.status = '1° Lugar' desc, b.status = '2° Lugar' desc, b.status = '3° Lugar' desc, b.status = '4° Lugar' desc, b.status = '5° Lugar' desc, b.data desc";

            List<BilheteTab> bilhetes = new List<BilheteTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", json.id_sorteio));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                BilheteTab bilhete = new BilheteTab();
                                bilhete.id = int.Parse(dr["id"].ToString());
                                bilhete.id_sorteio = int.Parse(dr["id_sorteio"].ToString());
                                bilhete.valor = decimal.Parse(dr["valor"].ToString());
                                bilhete.data = DateTime.Parse(dr["data"].ToString());
                                bilhete.nome_cliente = dr["clienteNome"].ToString();
                                bilhete.cidade_cliente = dr["cidade"].ToString();
                                bilhete.telefone_cliente = dr["telefone"].ToString();
                                bilhete.nome_vendedor = dr["vendedorNome"].ToString();
                                bilhete.cancelado_por = dr["cancelado_por"].ToString();
                                bilhete.status = dr["status"].ToString();
                                bilhete.codigo = dr["codigo"].ToString();
                                if (!dr["cancelado_por"].ToString().Equals(""))
                                {
                                    bilhete.data_cancelamento = DateTime.Parse(dr["data_cancelamento"].ToString());
                                }
                                bilhetes.Add(bilhete);
                            }
                        }
                        else
                        {
                            bilhetes = null;
                        }
                    }
                }

                return bilhetes;
            }
        }

        public List<BilheteTab> ListaBilhetesVencedores(AuxGerarPalpite json)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"Select b.id, b.codigo, b.id_sorteio, b.data,b.status, b.valor, c.nome as clienteNome, c.cidade, c.telefone, u.nome as vendedorNome, b.cancelado_por, b.data_cancelamento
                           from bilhetes as b
                           left join clientes as c on b.id_cliente = c.id
                           left join usuarios as u on b.id_vendedor = u.id
                           where b.id_sorteio = @id_sorteio
                           and b.status like '%Lugar'
                           group by b.id
                           order by b.status = '1° Lugar' desc, b.status = '2° Lugar' desc, b.status = '3° Lugar' desc, b.status = '4° Lugar' desc, b.status = '5° Lugar' desc, b.data desc";

            List<BilheteTab> bilhetes = new List<BilheteTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", json.id_sorteio));
                    cmd.Parameters.Add(new MySqlParameter("id", json.gerente));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                BilheteTab bilhete = new BilheteTab();
                                bilhete.id = int.Parse(dr["id"].ToString());
                                bilhete.id_sorteio = int.Parse(dr["id_sorteio"].ToString());
                                bilhete.valor = decimal.Parse(dr["valor"].ToString());
                                bilhete.data = DateTime.Parse(dr["data"].ToString());
                                bilhete.nome_cliente = dr["clienteNome"].ToString();
                                bilhete.cidade_cliente = dr["cidade"].ToString();
                                bilhete.telefone_cliente = dr["telefone"].ToString();
                                bilhete.nome_vendedor = dr["vendedorNome"].ToString();
                                bilhete.cancelado_por = dr["cancelado_por"].ToString();
                                bilhete.status = dr["status"].ToString();
                                bilhete.codigo = dr["codigo"].ToString();
                                if (!dr["cancelado_por"].ToString().Equals(""))
                                {
                                    bilhete.data_cancelamento = DateTime.Parse(dr["data_cancelamento"].ToString());
                                }
                                bilhetes.Add(bilhete);
                            }
                        }
                        else
                        {
                            bilhetes = null;
                        }
                    }
                }

                return bilhetes;
            }
        }

        public List<PalpitesTab> Palpites(int id_sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT p.* FROM palpites as p
                             join bilhetes as bi on p.id_bilhete = bi.id
                             where p.id_sorteio = @id_sorteio
                             and p.status <> 'Cancelado'
                             and bi.status <>'Cancelado'";

            List<PalpitesTab> palpites = new List<PalpitesTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", id_sorteio));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                PalpitesTab palpite = new PalpitesTab();
                                palpite.id = int.Parse(dr["id"].ToString());
                                palpite.id_bilhete = int.Parse(dr["id_bilhete"].ToString());
                                palpite.id_sorteio = int.Parse(dr["id_sorteio"].ToString());
                                palpite.numero = dr["numero"].ToString();
                                palpite.status = dr["status"].ToString();

                                palpites.Add(palpite);
                            }
                        }
                        else
                        {
                            palpites = null;
                        }
                    }
                }

                return palpites;
            }
        }

        public List<PalpitesTab> PalpitesChecar(int id_sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT p.numero FROM palpites as p
                             join bilhetes as bi on p.id_bilhete = bi.id
                             join usuarios as u on u.id = bi.id_vendedor
                             where p.id_sorteio = @id_sorteio
                             and p.status <> 'Cancelado'
                             and bi.cancelado_por IS NULL";

            List<PalpitesTab> palpites = new List<PalpitesTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", id_sorteio));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                PalpitesTab palpite = new PalpitesTab();
                                palpite.numero = dr["numero"].ToString();

                                palpites.Add(palpite);
                            }
                        }
                        else
                        {
                            palpites = null;
                        }
                    }
                }

                return palpites;
            }
        }

        private bool existeNumero(string numero, int id_sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT p.numero FROM palpites as p
                            join bilhetes as bi on p.id_bilhete = bi.id
                            where p.numero = @numero
                            and p.id_sorteio = @id_sorteio
                            and p.status <> 'Cancelado'
                            and bi.cancelado_por IS NULL";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    using (DataTable dt = new DataTable())
                    {
                        cmd.Parameters.Add(new MySqlParameter("numero", numero));
                        cmd.Parameters.Add(new MySqlParameter("id_sorteio", id_sorteio));

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

        public string AtualizarResultados(AuxResultados resultados)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"update palpites set status = case
                                 when numero = @primeiro then '1° Lugar'
                                 when numero = @segundo then '2° Lugar'
                                 when numero = @terceiro then '3° Lugar'
                                 when numero = @quarto then '4° Lugar'
                                 when numero = @quinto then '5° Lugar'
                                 else 'Errou'
                                 end
                             where id_sorteio = @id;";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", resultados.id_sorteio));
                    cmd.Parameters.Add(new MySqlParameter("primeiro", resultados.primeiro));
                    cmd.Parameters.Add(new MySqlParameter("segundo", resultados.segundo));
                    cmd.Parameters.Add(new MySqlParameter("terceiro", resultados.terceiro));
                    cmd.Parameters.Add(new MySqlParameter("quarto", resultados.quarto));
                    cmd.Parameters.Add(new MySqlParameter("quinto", resultados.quinto));

                    try
                    {
                        retorno = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        mensagem = e.Message;
                    }

                    if (retorno > 0)
                    {
                        mensagem = "Atualizou";
                    }
                }
            }
            AtualizarResultadosBilhetes(resultados.id_sorteio);
            return mensagem;
        }

        public string AtualizarResultadosBilhetes(int id_sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"update bilhetes
                                       set status = case when exists (select null from palpites p where p.id_bilhete = bilhetes.id and p.status = '1° Lugar')
                                        then '1° Lugar'

                                       when exists (select null from palpites p where p.id_bilhete = bilhetes.id and p.status = '2° Lugar')
                                        then '2° Lugar'

                                       when exists (select null from palpites p where p.id_bilhete = bilhetes.id and p.status = '3° Lugar')
                                        then '3° Lugar'

                                       when exists (select null from palpites p where p.id_bilhete = bilhetes.id and p.status = '4° Lugar')
                                        then '4° Lugar'

                                       when exists (select null from palpites p where p.id_bilhete = bilhetes.id and p.status = '5° Lugar')
                                        then '5° Lugar'
                                       else 'Perdeu' end

                             where id_sorteio = @id
                             and status <> 'Cancelado'";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", id_sorteio));

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
                        mensagem = "Atualizou";
                    }
                }
            }

            return mensagem;
        }

        public string ReabrirResultados(int id_sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"update bilhetes
                                       set status = 'Aberto'

                             where id_sorteio = @id;
                             update palpites
                                       set status = 'Aberto'

                             where id_sorteio = @id;";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", id_sorteio));

                    try
                    {
                        retorno = cmd.ExecuteNonQuery();
                    }
                    catch (MySqlException e)
                    {
                        mensagem = e.Message;
                    }

                    if (retorno > 0)
                    {
                        mensagem = "Reabriu";
                    }
                }
            }

            return mensagem;
        }

        public string GerarCodigo(int tamanho)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string codigo = "";

            do
            {
                codigo = new string(chars.Select(c => chars[random.Next(chars.Length)]).Take(tamanho).ToArray());
            } while (existeCodigo(codigo));

            return codigo;
        }

        public string gerarPalpite()
        {
            Random _random = new Random();
            return _random.Next(0, 10000).ToString("D4");
        }

        public List<PalpitesTab> gerarPalpites(int quantidade, int id_sorteio, string gerente)
        {
            List<PalpitesTab> palpites = new List<PalpitesTab>();
            List<PalpitesTab> palpitesNoBanco = PalpitesChecar(id_sorteio);

            if (palpitesNoBanco != null)
            {
                if (palpitesNoBanco.Count > 9999)
                {
                    return new List<PalpitesTab>();
                }
                int quantidadeDisponivel = 10000 - palpitesNoBanco.Count;

                if (quantidadeDisponivel < quantidade)
                {
                    quantidade = quantidadeDisponivel;
                }
            }

            for (int i = 0; i < quantidade; i++)
            {
                PalpitesTab palpite = new PalpitesTab();
                palpite.numero = gerarPalpite();

                if (palpitesNoBanco != null)
                {
                    while (palpitesNoBanco.Any(p => p.numero == palpite.numero) || palpites.Any(p => p.numero == palpite.numero))
                    {
                        palpite = new PalpitesTab();
                        palpite.numero = gerarPalpite();
                    }
                }

                palpites.Add(palpite);
            }

            return palpites;
        }

        public string StatusRifa(int id)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM sorteios WHERE id = @id";

            string status = "";

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
                                status = dr["status"].ToString();
                            }
                        }
                        else
                        {
                            status = "";
                        }
                    }
                }

                return status;
            }
        }

        public decimal ValorRifa(int id)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT * FROM sorteios WHERE id = @id";

            decimal valor = 0;

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
                                valor = Decimal.Parse(dr["valor"].ToString());
                            }
                        }
                        else
                        {
                            valor = 0;
                        }
                    }
                }

                return valor;
            }
        }

        public async Task<BilheteTab> CriarBilheteApostador(BilheteTab bilhete)
        {
            if (bilhete.id_cliente == 0)
            {
                bilhete = new BilheteTab();
                bilhete.erro = true;
                bilhete.erroMensagem = "Por favor, selecione um cliente.";
                return bilhete;
            }

            if (StatusRifa(bilhete.id_sorteio) != "Ativo")
            {
                bilhete = new BilheteTab();
                bilhete.erro = true;
                bilhete.erroMensagem = "A rifa não está disponível para apostas.";
                return bilhete;
            }

            foreach (PalpitesTab palpite in bilhete.palpites)
            {
                if (existeNumero(palpite.numero, bilhete.id_sorteio))
                {
                    bilhete.erro = true;
                    bilhete.erroMensagem = "O número " + palpite.numero + " já foi escolhido, por favor escolha outro número.";
                    return bilhete;
                }
            }

            bilhete.valor = bilhete.palpites.Count * ValorRifa(bilhete.id_sorteio);

            string codigo = GerarCodigo(10);
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            int id_bilhete = 0;
            string MercadoPagoToken = configuration.GetValue<string>("Configs:MercadoPagoToken");
            bilhete.status = "Reservado";
            bilhete.origem = "Apostador";
            string query = @"INSERT INTO bilhetes (valor, id_cliente, id_vendedor, id_sorteio, data, codigo,status,origem)
                           VALUES (@valor, @id_cliente, @id_vendedor, @id_sorteio, @data, @codigo,@status,@origem)";

            string queryPalpites = @"INSERT INTO palpites (numero, id_bilhete, id_sorteio, status)
                                   VALUES (@numero, @id_bilhete, @id_sorteio, @status)";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                MySqlTransaction transaction;
                transaction = con.BeginTransaction();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con, transaction))
                    {
                        cmd.Parameters.Add(new MySqlParameter("valor", bilhete.valor));
                        cmd.Parameters.Add(new MySqlParameter("id_cliente", bilhete.id_cliente));
                        cmd.Parameters.Add(new MySqlParameter("id_vendedor", bilhete.id_vendedor));
                        cmd.Parameters.Add(new MySqlParameter("id_sorteio", bilhete.id_sorteio));
                        cmd.Parameters.Add(new MySqlParameter("data", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("codigo", codigo));
                        cmd.Parameters.Add(new MySqlParameter("status", bilhete.status));
                        cmd.Parameters.Add(new MySqlParameter("origem", bilhete.origem));

                        retorno += cmd.ExecuteNonQuery();

                        id_bilhete = (int)cmd.LastInsertedId;

                        foreach (PalpitesTab palpite in bilhete.palpites)
                        {
                            using (MySqlCommand cmdPalpites = new MySqlCommand(queryPalpites, con, transaction))
                            {
                                cmdPalpites.Parameters.Add(new MySqlParameter("numero", palpite.numero));
                                cmdPalpites.Parameters.Add(new MySqlParameter("id_bilhete", id_bilhete));
                                cmdPalpites.Parameters.Add(new MySqlParameter("id_sorteio", bilhete.id_sorteio));
                                cmdPalpites.Parameters.Add(new MySqlParameter("status", bilhete.status));

                                retorno += cmdPalpites.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (MySqlException e)
                {
                    bilhete.erro = true;
                    transaction.Rollback();
                    bilhete.erroMensagem = e.Message;
                }

                if (retorno.Equals(bilhete.palpites.Count + 1) && bilhete.erro == false)
                {
                    transaction.Commit();
                }
                else
                {
                    bilhete.erro = true;
                    bilhete.erroMensagem = "Por favor, preencha os placares dos jogos corretamente";
                    transaction.Rollback();
                }
            }
            if (bilhete.erro == true)
            {
                return bilhete;
            }
            else
            {
                BilheteTab returnBilhete = BilhetePorCodigo(codigo);

                var requestOptions = new RequestOptions
                {
                    AccessToken = MercadoPagoToken
                };
                var request = new PaymentCreateRequest
                {
                    TransactionAmount = bilhete.valor,
                    Description = "Bilhete",
                    ExternalReference = returnBilhete.id.ToString(),
                    Installments = 1,
                    PaymentMethodId = "pix",
                    DateOfExpiration = DateTime.Now.AddMinutes(10),
                    Payer = new PaymentPayerRequest
                    {
                        Email = "email@email.com",
                        FirstName = returnBilhete.nome_cliente
                    }
                };

                var client = new PaymentClient();
                Payment payment = await client.CreateAsync(request, requestOptions);

                returnBilhete.imagemPix = "data:image/jpg;base64," + payment.PointOfInteraction.TransactionData.QrCodeBase64;
                returnBilhete.textoPix = payment.PointOfInteraction.TransactionData.QrCode;
                returnBilhete.sitePix = payment.PointOfInteraction.TransactionData.TicketUrl;

                Console.WriteLine(payment.ToString());
                return returnBilhete;
            }
        }

        public BilheteTab CriarBilhete(BilheteTab bilhete)
        {
            if (bilhete.id_cliente == 0)
            {
                bilhete = new BilheteTab();
                bilhete.erro = true;
                bilhete.erroMensagem = "Por favor, selecione um cliente.";
                return bilhete;
            }

            if (StatusRifa(bilhete.id_sorteio) != "Ativo")
            {
                bilhete = new BilheteTab();
                bilhete.erro = true;
                bilhete.erroMensagem = "O sorteio não está disponível para apostas.";
                return bilhete;
            }

            foreach (PalpitesTab palpite in bilhete.palpites)
            {
                if (existeNumero(palpite.numero, bilhete.id_sorteio))
                {
                    bilhete.erro = true;
                    bilhete.erroMensagem = "O número " + palpite.numero + " já foi escolhido, por favor escolha outro número.";
                    return bilhete;
                }
            }

            bilhete.valor = bilhete.palpites.Count * ValorRifa(bilhete.id_sorteio);

            string codigo = GerarCodigo(10);
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            int id_bilhete = 0;

            string query = @"INSERT INTO bilhetes (valor, id_cliente, id_vendedor, id_sorteio, data, codigo,status,origem)
                           VALUES (@valor, @id_cliente, @id_vendedor, @id_sorteio, @data, @codigo,@status,@origem)";

            string queryPalpites = @"INSERT INTO palpites (numero, id_bilhete, id_sorteio, status)
                                   VALUES (@numero, @id_bilhete, @id_sorteio, @status)";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                MySqlTransaction transaction;
                transaction = con.BeginTransaction();
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con, transaction))
                    {
                        cmd.Parameters.Add(new MySqlParameter("valor", bilhete.valor));
                        cmd.Parameters.Add(new MySqlParameter("id_cliente", bilhete.id_cliente));
                        cmd.Parameters.Add(new MySqlParameter("id_vendedor", bilhete.id_vendedor));
                        cmd.Parameters.Add(new MySqlParameter("id_sorteio", bilhete.id_sorteio));
                        cmd.Parameters.Add(new MySqlParameter("data", DateTime.Now));
                        cmd.Parameters.Add(new MySqlParameter("codigo", codigo));
                        cmd.Parameters.Add(new MySqlParameter("status", "Aberto"));
                        cmd.Parameters.Add(new MySqlParameter("origem", "Vendedor"));

                        retorno += cmd.ExecuteNonQuery();

                        id_bilhete = (int)cmd.LastInsertedId;

                        foreach (PalpitesTab palpite in bilhete.palpites)
                        {
                            using (MySqlCommand cmdPalpites = new MySqlCommand(queryPalpites, con, transaction))
                            {
                                cmdPalpites.Parameters.Add(new MySqlParameter("numero", palpite.numero));
                                cmdPalpites.Parameters.Add(new MySqlParameter("id_bilhete", id_bilhete));
                                cmdPalpites.Parameters.Add(new MySqlParameter("id_sorteio", bilhete.id_sorteio));
                                cmdPalpites.Parameters.Add(new MySqlParameter("status", "Aberto"));

                                retorno += cmdPalpites.ExecuteNonQuery();
                            }
                        }
                    }
                }
                catch (MySqlException e)
                {
                    bilhete.erro = true;
                    transaction.Rollback();
                    bilhete.erroMensagem = e.Message;
                }

                if (retorno.Equals(bilhete.palpites.Count + 1) && bilhete.erro == false)
                {
                    transaction.Commit();
                }
                else
                {
                    bilhete.erro = true;
                    bilhete.erroMensagem = "Por favor, preencha o bilhete corretamente";
                    transaction.Rollback();
                }
            }
            if (bilhete.erro == true)
            {
                return bilhete;
            }
            else
            {
                return BilhetePorId(id_bilhete);
            }
        }

        public BilheteTab BilhetePorId(int idBilhete)
        {
            int id = 0;
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"Select b.id, b.origem, b.codigo, b.id_sorteio,b.status, b.data, b.valor, c.nome as clienteNome, c.cidade, c.telefone, u.nome as vendedorNome,b.id_vendedor, b.cancelado_por, b.data_cancelamento, r.descricao as premio, r.data_sorteio as data_sorteio
                           from bilhetes as b
                           left join clientes as c on b.id_cliente = c.id
                           left join usuarios as u on c.id_vendedor = u.id
                           left join sorteios as r on b.id_sorteio = r.id
                           where b.id = @id";

            string queryPalpites = @"SELECT * from palpites WHERE id_bilhete = @id and status <> 'Cancelado'";

            BilheteTab bilhete = new BilheteTab();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", idBilhete));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                bilhete.id = int.Parse(dr["id"].ToString());
                                bilhete.valor = decimal.Parse(dr["valor"].ToString());
                                bilhete.data = DateTime.Parse(dr["data"].ToString());
                                bilhete.nome_cliente = dr["clienteNome"].ToString();
                                bilhete.cidade_cliente = dr["cidade"].ToString();
                                bilhete.telefone_cliente = dr["telefone"].ToString();
                                bilhete.nome_vendedor = dr["vendedorNome"].ToString();
                                bilhete.id_vendedor = dr["id_vendedor"].ToString();
                                bilhete.premio = dr["premio"].ToString();
                                bilhete.status = dr["status"].ToString();
                                bilhete.data_sorteio = dr["data_sorteio"].ToString();
                                bilhete.codigo = dr["codigo"].ToString();
                                bilhete.origem = dr["origem"].ToString();
                                bilhete.id_sorteio = int.Parse(dr["id_sorteio"].ToString());
                                id = int.Parse(dr["id"].ToString());
                            }
                        }
                        else
                        {
                            bilhete.erro = true;
                            bilhete.erroMensagem = "Erro ao exibir bilhete";
                        }
                    }
                }

                using (MySqlCommand cmd = new MySqlCommand(queryPalpites, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", id));

                    using (var drPalpites = cmd.ExecuteReader())
                    {
                        List<PalpitesTab> palpites = new List<PalpitesTab>();

                        if (drPalpites.HasRows)
                        {
                            while (drPalpites.Read())
                            {
                                PalpitesTab palpite = new PalpitesTab
                                {
                                    id = int.Parse(drPalpites["id"].ToString()),
                                    numero = drPalpites["numero"].ToString(),
                                    id_bilhete = int.Parse(drPalpites["id_bilhete"].ToString()),
                                    id_sorteio = int.Parse(drPalpites["id_sorteio"].ToString()),
                                    status = drPalpites["id"].ToString()
                                };

                                palpites.Add(palpite);
                            }

                            bilhete.palpites = palpites;
                        }
                        else
                        {
                            bilhete.erro = true;
                            bilhete.erroMensagem = "Erro ao exibir bilhete";
                        }
                    }
                }

                return bilhete;
            }
        }

        public List<BilheteTab> PalpitesPorTelefone(string telefone, int sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"SELECT c.nome, c.cidade, p.numero, p.status FROM palpites as p
                             JOIN bilhetes as b on b.id = p.id_bilhete
                             JOIN clientes as c on c.id = b.id_cliente
                             WHERE c.telefone = @telefone
                             AND p.id_sorteio = @id_sorteio
                             AND p.status <> 'Cancelado'";

            List<PalpitesTab> palpites = new List<PalpitesTab>();
            List<BilheteTab> bilhetes = new List<BilheteTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("telefone", telefone));
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", sorteio));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                //PalpitesTab palpite = new PalpitesTab();
                                //palpite.numero = dr["numero"].ToString();
                                //palpite.status = dr["status"].ToString();

                                //palpites.Add(palpite);

                                var nomeCliente = dr["nome"].ToString();

                                PalpitesTab palpite = null;

                                var palpiteNumero = dr["numero"].ToString();

                                var bilhete = bilhetes.FirstOrDefault(g => string.Compare(nomeCliente, g.nome_cliente, true) == 0);

                                var addBilhete = bilhete == null;

                                if (addBilhete)
                                {
                                    bilhete = new BilheteTab();
                                    bilhete.palpites = new List<PalpitesTab>();
                                    bilhete.nome_cliente = dr["nome"].ToString();
                                    bilhete.cidade_cliente = dr["cidade"].ToString();
                                }
                                else
                                {
                                    palpite = palpites.FirstOrDefault(c => string.Equals(palpiteNumero, c.numero, StringComparison.CurrentCultureIgnoreCase));
                                }

                                var addPalpite = palpite == null;

                                if (addPalpite) palpite = new PalpitesTab();

                                palpite.numero = dr["numero"].ToString();
                                palpite.status = dr["status"].ToString();

                                if (addPalpite) bilhete.palpites.Add(palpite);

                                if (addBilhete) bilhetes.Add(bilhete);
                            }
                        }
                        else
                        {
                            palpites = null;
                        }
                    }
                }

                return bilhetes;
            }
        }

        public BilheteTab BilhetePorCodigo(string codigo)
        {
            int id = 0;
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"Select b.id, b.origem, b.codigo,b.status, b.id_sorteio, b.data, b.valor, c.nome as clienteNome, c.cidade, c.telefone, u.nome as vendedorNome, b.cancelado_por, b.data_cancelamento, r.descricao as premio, r.data_sorteio as data_sorteio
                           from bilhetes as b
                           left join clientes as c on b.id_cliente = c.id
                           left join usuarios as u on c.id_vendedor = u.id
                           left join sorteios as r on b.id_sorteio = r.id
                           where b.codigo = @codigo";

            string queryPalpites = @"SELECT * from palpites WHERE id_bilhete = @id and status <> 'Cancelado'";

            BilheteTab bilhete = new BilheteTab();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("codigo", codigo));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                bilhete.id = int.Parse(dr["id"].ToString());
                                bilhete.valor = decimal.Parse(dr["valor"].ToString());
                                bilhete.data = DateTime.Parse(dr["data"].ToString());
                                bilhete.nome_cliente = dr["clienteNome"].ToString();
                                bilhete.cidade_cliente = dr["cidade"].ToString();
                                bilhete.telefone_cliente = dr["telefone"].ToString();
                                bilhete.nome_vendedor = dr["vendedorNome"].ToString();
                                bilhete.premio = dr["premio"].ToString();
                                bilhete.status = dr["status"].ToString();
                                bilhete.data_sorteio = dr["data_sorteio"].ToString();
                                bilhete.codigo = dr["codigo"].ToString();
                                bilhete.origem = dr["origem"].ToString();
                                bilhete.id_sorteio = int.Parse(dr["id_sorteio"].ToString());
                                id = int.Parse(dr["id"].ToString());
                            }
                        }
                        else
                        {
                            bilhete.erro = true;
                            bilhete.erroMensagem = "Erro ao exibir bilhete";
                        }
                    }
                }

                using (MySqlCommand cmd = new MySqlCommand(queryPalpites, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id", id));

                    using (var drPalpites = cmd.ExecuteReader())
                    {
                        List<PalpitesTab> palpites = new List<PalpitesTab>();

                        if (drPalpites.HasRows)
                        {
                            while (drPalpites.Read())
                            {
                                PalpitesTab palpite = new PalpitesTab
                                {
                                    id = int.Parse(drPalpites["id"].ToString()),
                                    numero = drPalpites["numero"].ToString(),
                                    id_bilhete = int.Parse(drPalpites["id_bilhete"].ToString()),
                                    id_sorteio = int.Parse(drPalpites["id_sorteio"].ToString()),
                                    status = drPalpites["id"].ToString()
                                };

                                palpites.Add(palpite);
                            }

                            bilhete.palpites = palpites;
                        }
                        else
                        {
                            bilhete.erro = true;
                            bilhete.erroMensagem = "Erro ao exibir bilhete";
                        }
                    }
                }

                return bilhete;
            }
        }

        public List<BilheteTab> ListaBilhetes(SorteioTab rifa)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"Select b.id, b.codigo, b.id_sorteio, b.status, b.data, b.valor, c.nome as clienteNome, c.cidade, c.telefone, u.nome as vendedorNome, u.gerente as idGerente, b.cancelado_por, b.data_cancelamento
                           from bilhetes as b
                           left join clientes as c on b.id_cliente = c.id
                           left join usuarios as u on c.id_vendedor = u.id
                           where b.id_sorteio = @id_sorteio
                           group by b.id
                           order by b.status = '1° Lugar' desc, b.status = '2° Lugar' desc, b.status = '3° Lugar' desc, b.status = '4° Lugar' desc, b.status = '5° Lugar' desc, b.data desc";

            List<BilheteTab> bilhetes = new List<BilheteTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", rifa.id));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                BilheteTab bilhete = new BilheteTab();
                                bilhete.id = int.Parse(dr["id"].ToString());
                                bilhete.id_sorteio = int.Parse(dr["id_sorteio"].ToString());
                                bilhete.valor = decimal.Parse(dr["valor"].ToString());
                                bilhete.data = DateTime.Parse(dr["data"].ToString());
                                bilhete.nome_cliente = dr["clienteNome"].ToString();
                                bilhete.cidade_cliente = dr["cidade"].ToString();
                                bilhete.telefone_cliente = dr["telefone"].ToString();
                                bilhete.nome_vendedor = dr["vendedorNome"].ToString();
                                bilhete.id_gerente = dr["idGerente"].ToString().Equals("") ? "-1" : dr["idGerente"].ToString();
                                bilhete.cancelado_por = dr["cancelado_por"].ToString();
                                bilhete.status = dr["status"].ToString();
                                bilhete.codigo = dr["codigo"].ToString();
                                if (!dr["cancelado_por"].ToString().Equals(""))
                                {
                                    bilhete.data_cancelamento = DateTime.Parse(dr["data_cancelamento"].ToString());
                                }
                                bilhetes.Add(bilhete);
                            }
                        }
                        else
                        {
                            bilhetes = null;
                        }
                    }
                }

                return bilhetes;
            }
        }

        public List<BilheteTab> ListaBilhetesVendedor(SorteioTab rifa, string id)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"Select b.id, b.codigo, b.id_sorteio, b.data,b.status, b.valor, c.nome as clienteNome, c.cidade, c.telefone, u.nome as vendedorNome, b.cancelado_por, b.data_cancelamento
                           from bilhetes as b
                           left join clientes as c on b.id_cliente = c.id
                           left join usuarios as u on c.id_vendedor = u.id
                           where b.id_sorteio = @id_sorteio
                           and u.id = @id
                           group by b.id
                           order by b.status = '1° Lugar' desc, b.status = '2° Lugar' desc, b.status = '3° Lugar' desc, b.status = '4° Lugar' desc, b.status = '5° Lugar' desc, b.data desc";

            List<BilheteTab> bilhetes = new List<BilheteTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", rifa.id));
                    cmd.Parameters.Add(new MySqlParameter("id", id));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                BilheteTab bilhete = new BilheteTab();
                                bilhete.id = int.Parse(dr["id"].ToString());
                                bilhete.id_sorteio = int.Parse(dr["id_sorteio"].ToString());
                                bilhete.valor = decimal.Parse(dr["valor"].ToString());
                                bilhete.data = DateTime.Parse(dr["data"].ToString());
                                bilhete.nome_cliente = dr["clienteNome"].ToString();
                                bilhete.cidade_cliente = dr["cidade"].ToString();
                                bilhete.telefone_cliente = dr["telefone"].ToString();
                                bilhete.nome_vendedor = dr["vendedorNome"].ToString();
                                bilhete.cancelado_por = dr["cancelado_por"].ToString();
                                bilhete.status = dr["status"].ToString();
                                bilhete.codigo = dr["codigo"].ToString();
                                if (!dr["cancelado_por"].ToString().Equals(""))
                                {
                                    bilhete.data_cancelamento = DateTime.Parse(dr["data_cancelamento"].ToString());
                                }
                                bilhetes.Add(bilhete);
                            }
                        }
                        else
                        {
                            bilhetes = null;
                        }
                    }
                }

                return bilhetes;
            }
        }

        public List<BilheteTab> ListaBilhetesGerente(SorteioTab rifa, string id)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = @"Select b.id, b.codigo, b.id_sorteio, b.data,b.status, b.valor, c.nome as clienteNome, c.cidade, c.telefone, u.nome as vendedorNome, b.cancelado_por, b.data_cancelamento
                           from bilhetes as b
                           left join clientes as c on b.id_cliente = c.id
                           left join usuarios as u on b.id_vendedor = u.id
                           where b.id_sorteio = @id_sorteio
                           and u.gerente = @id
                           group by b.id
                           order by b.status = '1° Lugar' desc, b.status = '2° Lugar' desc, b.status = '3° Lugar' desc, b.status = '4° Lugar' desc, b.status = '5° Lugar' desc, b.data desc";

            List<BilheteTab> bilhetes = new List<BilheteTab>();

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", rifa.id));
                    cmd.Parameters.Add(new MySqlParameter("id", id));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                BilheteTab bilhete = new BilheteTab();
                                bilhete.id = int.Parse(dr["id"].ToString());
                                bilhete.id_sorteio = int.Parse(dr["id_sorteio"].ToString());
                                bilhete.valor = decimal.Parse(dr["valor"].ToString());
                                bilhete.data = DateTime.Parse(dr["data"].ToString());
                                bilhete.nome_cliente = dr["clienteNome"].ToString();
                                bilhete.cidade_cliente = dr["cidade"].ToString();
                                bilhete.telefone_cliente = dr["telefone"].ToString();
                                bilhete.nome_vendedor = dr["vendedorNome"].ToString();
                                bilhete.cancelado_por = dr["cancelado_por"].ToString();
                                bilhete.status = dr["status"].ToString();
                                bilhete.codigo = dr["codigo"].ToString();
                                if (!dr["cancelado_por"].ToString().Equals(""))
                                {
                                    bilhete.data_cancelamento = DateTime.Parse(dr["data_cancelamento"].ToString());
                                }
                                bilhetes.Add(bilhete);
                            }
                        }
                        else
                        {
                            bilhetes = null;
                        }
                    }
                }

                return bilhetes;
            }
        }

        public string CancelarBilhete(BilheteTab bilhete)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string mensagem = "";
            string query = @"UPDATE bilhetes SET cancelado_por = @cancelado_por,
                             data_cancelamento = @data_cancelamento,
                             status = 'Cancelado'
                             WHERE id = @id ";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("cancelado_por", bilhete.cancelado_por));
                    cmd.Parameters.Add(new MySqlParameter("data_cancelamento", DateTime.Now));
                    cmd.Parameters.Add(new MySqlParameter("id", bilhete.id));

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
                        mensagem = "Cancelou";
                    }
                }
            }
            return mensagem;
        }

        public async Task<string> NotificacaoBilheteAsync(NotificacaoMP notificacao)
        {
            string retornoMSG = "";
            using (HttpClient client = new HttpClient())
            {
                IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                string MercadoPagoToken = configuration.GetValue<string>("Configs:MercadoPagoToken");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", MercadoPagoToken);
                var response = client.GetAsync("https://api.mercadopago.com/v1/payments/" + notificacao.data.id.ToString()).Result;

                string JsonString = await response.Content.ReadAsStringAsync();
                JObject Raw = (JObject)JsonConvert.DeserializeObject(JsonString);

                RetornoMP retorno = Raw.ToObject<RetornoMP>();

                switch (retorno.status)
                {
                    case "approved":
                        AlterarStatusBilhete("Pago", retorno.external_reference);
                        break;

                    case "rejected":
                        AlterarStatusBilhete("Cancelado", retorno.external_reference);
                        break;

                    case "cancelled":
                        AlterarStatusBilhete("Cancelado", retorno.external_reference);
                        break;

                    case "refunded":
                        AlterarStatusBilhete("Cancelado", retorno.external_reference);
                        break;

                    case "partially_refunded":
                        AlterarStatusBilhete("Cancelado", retorno.external_reference);
                        break;

                    default:
                        // código 3
                        break;
                }

                retornoMSG = retorno.status;
            }
            return retornoMSG;
        }

        public string AlterarStatusBilhete(string status, string id)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            int retorno = 0;
            string query = @"UPDATE bilhetes SET status = @status
                             WHERE id = @id ";

            string queryPalpites = @"UPDATE palpites SET status = @status
                             WHERE id_bilhete = @id";

            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();
                MySqlTransaction transaction;
                transaction = con.BeginTransaction();

                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(query, con))
                    {
                        cmd.Parameters.Add(new MySqlParameter("status", status));
                        cmd.Parameters.Add(new MySqlParameter("id", id));

                        retorno = cmd.ExecuteNonQuery();
                    }

                    using (MySqlCommand cmdPalpites = new MySqlCommand(queryPalpites, con))
                    {
                        cmdPalpites.Parameters.Add(new MySqlParameter("status", status));
                        cmdPalpites.Parameters.Add(new MySqlParameter("id", id));

                        retorno += cmdPalpites.ExecuteNonQuery();
                    }
                }
                catch (MySqlException e)
                {
                    transaction.Rollback();
                }

                if (retorno > 1)
                {
                    transaction.Commit();
                }
                else
                {
                    transaction.Rollback();
                }
            }
            return "Alterou";
        }
    }
}