using System;
using System.Collections.Generic;
using System.Linq;
using API.Models;
using FluentDateTime;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace API.Services.Financeiro
{
    public class FinanceiroService
    {
        public List<FinanceiroGerenteTab> Financeiro(string id, string tipo, string sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = tipo == "Administrador" ?

                             @"SELECT u.nome as nome_caixa,
                             u.comissao as comissoes_vendedor,
                             CASE
								WHEN g.nome is not null THEN g.nome
								WHEN u.tipo = 'Apostador' THEN 'PIX'
                                WHEN u.gerente is null THEN 'Sem gerente'
							 END AS nome_gerente,
                             g.comissao comissoes_gerente,
                             if(g.id is not null, g.id, 0) as id_gerente,
                             CASE
								WHEN g.id is not null THEN g.id
                                WHEN u.tipo = 'Apostador' THEN 0
                                WHEN g.id is null THEN -1
                             END AS id_gerente,
                             sum(b.valor) as apostas
                             FROM bilhetes as b
                             JOIN usuarios as u on u.id = b.id_vendedor
                             LEFT JOIN usuarios as g on g.id = u.gerente
                             WHERE b.cancelado_por IS NULL
                             and b.id_sorteio = @id_sorteio
                             GROUP BY u.id
                             ORDER BY id_gerente" :
                             tipo == "Gerente" ?

                             @"SELECT u.nome as nome_caixa,
                             u.comissao as comissoes_vendedor,
                             CASE
								WHEN g.nome is not null THEN g.nome
								WHEN u.tipo = 'Apostador' THEN 'PIX'
                                WHEN u.gerente is null THEN 'Sem gerente'
							 END AS nome_gerente,
                             g.comissao comissoes_gerente,
                             if(g.id is not null, g.id, 0) as id_gerente,
                             CASE
								WHEN g.id is not null THEN g.id
                                WHEN u.tipo = 'Apostador' THEN 0
                                WHEN g.id is null THEN -1
                             END AS id_gerente,
                             sum(b.valor) as apostas
                             FROM bilhetes as b
                             JOIN usuarios as u on u.id = b.id_vendedor
                             LEFT JOIN usuarios as g on g.id = u.gerente
                             WHERE b.cancelado_por IS NULL
                             and b.id_sorteio = @id_sorteio
                             and g.id = @id
                             GROUP BY u.id
                             ORDER BY id_gerente" :
                             "";

            List<FinanceiroGerenteTab> gerentes = new List<FinanceiroGerenteTab>();
            List<FinanceiroVendedorTab> caixas = new List<FinanceiroVendedorTab>();
            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", sorteio));
                    if (tipo == "Gerente" || tipo == "Vendedor") cmd.Parameters.Add(new MySqlParameter("id", id));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                var idGerente = dr["id_gerente"].ToString();

                                FinanceiroVendedorTab caixa = null;

                                var nomeCaixa = dr["nome_caixa"].ToString();

                                var gerente = gerentes.FirstOrDefault(g => string.Compare(idGerente, g.id_gerente, true) == 0);

                                var addGerente = gerente == null;

                                if (addGerente)
                                {
                                    gerente = new FinanceiroGerenteTab();
                                    gerente.vendedores = new List<FinanceiroVendedorTab>();
                                    gerente.nome_gerente = dr["nome_gerente"].ToString();
                                    gerente.id_gerente = dr["id_gerente"].ToString();
                                }
                                else
                                {
                                    caixa = caixas.FirstOrDefault(c => string.Compare(nomeCaixa, c.nome, true) == 0);
                                }

                                var addCaixa = caixa == null;

                                if (addCaixa) caixa = new FinanceiroVendedorTab();

                                var apostas = decimal.Parse(dr["apostas"].ToString());
                                var comissaoVendedor = apostas * Convert.ToDecimal(dr["comissoes_vendedor"].ToString()) / 100;
                                var comissaoGerente = dr["comissoes_gerente"].ToString() == "" ? 0 : decimal.Parse(dr["comissoes_gerente"].ToString()) < 0 ? 0 : decimal.Parse(dr["comissoes_gerente"].ToString());

                                var total = apostas - comissaoVendedor;

                                caixa.nome = nomeCaixa;
                                caixa.id_gerente = idGerente;
                                caixa.comissoes_vendedor += comissaoVendedor;
                                caixa.apostas += apostas;
                                caixa.total += total;

                                gerente.comissoes_vendedor += comissaoVendedor;
                                gerente.comissoes_gerente = comissaoGerente;
                                gerente.apostas += apostas;
                                gerente.total += total;

                                if (addCaixa) gerente.vendedores.Add(caixa);

                                if (addGerente) gerentes.Add(gerente);
                            }
                        }
                    }
                }

                foreach (FinanceiroGerenteTab gerente in gerentes)
                {
                    gerente.comissoes_gerente = (gerente.apostas - gerente.comissoes_vendedor) < 0 ? 0 : (gerente.apostas - gerente.comissoes_vendedor) * gerente.comissoes_gerente / 100;
                    gerente.total -= gerente.comissoes_gerente;
                }

                return gerentes;
            }
        }

        public FinanceiroTab Caixa(string id, string tipo, string sorteio)
        {
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            string conexao = configuration.GetValue<string>("Configs:Banco");
            string query = tipo == "Administrador" ?

                             @"SELECT u.nome as nome_caixa,
                             u.comissao as comissoes_vendedor,
                             if(g.nome is not null, g.nome, 'Sem gerente') as nome_gerente,
                             g.comissao comissoes_gerente,
                             if(g.id is not null, g.id, 0) as id_gerente,
                             sum(b.valor) as apostas
                             FROM bilhetes as b
                             JOIN usuarios as u on u.id = b.id_vendedor
                             LEFT JOIN usuarios as g on g.id = u.gerente
                             WHERE b.cancelado_por IS NULL
                             and b.id_sorteio = @id_sorteio
                             GROUP BY u.id
                             ORDER BY id_gerente" :

                             tipo == "Vendedor" ?

                             @"SELECT u.nome as nome_caixa,
                             u.comissao as comissoes_vendedor,
                             if(g.nome is not null, g.nome, 'Sem gerente') as nome_gerente,
                             g.comissao comissoes_gerente,
                             if(g.id is not null, g.id, 0) as id_gerente,
                             sum(b.valor) as apostas
                             FROM bilhetes as b
                             JOIN usuarios as u on u.id = b.id_vendedor
                             LEFT JOIN usuarios as g on g.id = u.gerente
                             WHERE b.cancelado_por IS NULL
                             and b.id_sorteio = @id_sorteio
                             and u.id = @id
                             GROUP BY u.id
                             ORDER BY id_gerente" :

                             "";

            List<FinanceiroGerenteTab> gerentes = new List<FinanceiroGerenteTab>();
            List<FinanceiroVendedorTab> caixas = new List<FinanceiroVendedorTab>();
            FinanceiroTab financeiro = new FinanceiroTab();
            using (MySqlConnection con = new MySqlConnection(conexao))
            {
                con.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, con))
                {
                    cmd.Parameters.Add(new MySqlParameter("id_sorteio", sorteio));
                    if (tipo == "Gerente" || tipo == "Vendedor") cmd.Parameters.Add(new MySqlParameter("id", id));

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.HasRows)
                        {
                            while (dr.Read())
                            {
                                if (tipo == "Administrador")
                                {
                                    var idGerente = dr["id_gerente"].ToString();

                                    FinanceiroVendedorTab caixa = null;

                                    var nomeCaixa = dr["nome_caixa"].ToString();

                                    var gerente = gerentes.FirstOrDefault(g => string.Compare(idGerente, g.id_gerente, true) == 0);

                                    var addGerente = gerente == null;

                                    if (addGerente)
                                    {
                                        gerente = new FinanceiroGerenteTab();
                                        gerente.vendedores = new List<FinanceiroVendedorTab>();
                                        gerente.nome_gerente = dr["nome_gerente"].ToString();
                                        gerente.id_gerente = dr["id_gerente"].ToString();
                                    }
                                    else
                                    {
                                        caixa = caixas.FirstOrDefault(c => string.Compare(nomeCaixa, c.nome, true) == 0);
                                    }

                                    var addCaixa = caixa == null;

                                    if (addCaixa) caixa = new FinanceiroVendedorTab();

                                    var apostas = decimal.Parse(dr["apostas"].ToString());
                                    var comissaoVendedor = apostas * Convert.ToDecimal(dr["comissoes_vendedor"].ToString()) / 100;
                                    var comissaoGerente = dr["comissoes_gerente"].ToString() == "" ? 0 : decimal.Parse(dr["comissoes_gerente"].ToString()) < 0 ? 0 : decimal.Parse(dr["comissoes_gerente"].ToString());

                                    var total = apostas - comissaoVendedor;

                                    caixa.nome = nomeCaixa;
                                    caixa.id_gerente = idGerente;
                                    caixa.comissoes_vendedor += comissaoVendedor;
                                    caixa.apostas += apostas;
                                    caixa.total += total;

                                    gerente.comissoes_vendedor += comissaoVendedor;
                                    gerente.comissoes_gerente = comissaoGerente;
                                    gerente.apostas += apostas;
                                    gerente.total += total;

                                    if (addCaixa) gerente.vendedores.Add(caixa);

                                    if (addGerente) gerentes.Add(gerente);
                                }
                                else
                                {
                                    var apostas = decimal.Parse(dr["apostas"].ToString());
                                    var comissaoVendedor = apostas * Convert.ToDecimal(dr["comissoes_vendedor"].ToString()) / 100;
                                    var total = apostas - comissaoVendedor;

                                    financeiro.apostas = apostas;

                                    financeiro.comissoes_vendedor = comissaoVendedor;
                                    financeiro.total = total;
                                }
                            }
                        }
                    }
                }
                if (tipo == "Administrador")
                {
                    foreach (FinanceiroGerenteTab gerente in gerentes)
                    {
                        gerente.comissoes_gerente = (gerente.apostas - gerente.comissoes_vendedor) < 0 ? 0 : (gerente.apostas - gerente.comissoes_vendedor) * gerente.comissoes_gerente / 100;
                        gerente.total -= gerente.comissoes_gerente;

                        financeiro.apostas += gerente.apostas;
                        financeiro.comissoes_gerente += gerente.comissoes_gerente;
                        financeiro.comissoes_vendedor += gerente.comissoes_vendedor;
                        financeiro.total += gerente.total;
                    }
                }
                return financeiro;
            }
        }
    }
}