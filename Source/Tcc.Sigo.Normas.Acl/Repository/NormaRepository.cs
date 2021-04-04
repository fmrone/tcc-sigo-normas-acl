using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Tcc.Sigo.Normas.Acl.Enumerators;
using Tcc.Sigo.Normas.Acl.Models;

namespace Tcc.Sigo.Normas.Acl.Repository
{
    public class NormaRepository : INormaRepository
    {
        private readonly IConfiguration _configuration;

        public NormaRepository(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<NormaSegurancaQAModel>> ListarNormasSegurancaQA()
        {
            var conexao = GetConnection();

            using var con = new SqlConnection(conexao);
            {
                try
                {
                    con.Open();
                    var query = "SELECT * FROM Norma";
                    var normasSegurancaQAModel = await con.QueryAsync<NormaSegurancaQAModel>(query);

                    return normasSegurancaQAModel.ToList();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        public async Task<bool> PersistirNorma(NormaMessageModel normaMessageModel) 
        {
            var conexao = GetConnection();

            using var con = new SqlConnection(conexao);
            {
                try
                {
                    con.Open();

                    //verifica se existe norma cadastrada
                    var normaSegurancaQAModel = await con.QueryAsync<NormaSegurancaQAModel>(
                        "SELECT * FROM Norma WHERE Codigo = @Codigo", 
                        new { normaMessageModel.Codigo });

                    var parametros = new DynamicParameters();
                    var cmd = string.Empty;

                    if (normaMessageModel.Operacao == (byte)EOperacao.Inserir || normaMessageModel.Operacao == (byte)EOperacao.Alterar)
                    {
                        if (normaSegurancaQAModel.Any())
                        {
                            var registroNormaSegurancaQAModel = normaSegurancaQAModel.FirstOrDefault();

                            parametros.Add("Id", registroNormaSegurancaQAModel.Id);
                            parametros.Add("Codigo", registroNormaSegurancaQAModel.Codigo);
                            parametros.Add("Descricao", normaMessageModel.Descricao);
                            parametros.Add("Area", normaMessageModel.Area);
                            parametros.Add("IntegradoEm", DateTime.Now);
                            parametros.Add("Integracao", $"Id Origem: {normaMessageModel.Id} - Operação: alteração");

                            cmd = @"UPDATE Norma
                                       SET Codigo = @Codigo,
                                           Descricao = @Descricao,
                                           Area = @Area,
                                           IntegradoEm = @IntegradoEm,
                                           Integracao = @Integracao
                                     WHERE Id = @Id";
                        }
                        else
                        {
                            parametros.Add("Codigo", normaMessageModel.Codigo);
                            parametros.Add("Descricao", normaMessageModel.Descricao);
                            parametros.Add("Area", normaMessageModel.Area);
                            parametros.Add("Status", true, DbType.Boolean);
                            parametros.Add("IntegradoEm", DateTime.Now);
                            parametros.Add("Integracao", $"Id Origem: {normaMessageModel.Id} - Operação: inclusão");

                            cmd = @"INSERT INTO Norma (Codigo, Descricao, Area, Status, IntegradoEm, Integracao)
                                    VALUES (@Codigo, @Descricao, @Area, @Status, @IntegradoEm, @Integracao)";
                        }
                    }
                    else if (normaMessageModel.Operacao == (byte)EOperacao.AtivarInativar) 
                    {
                        if (normaSegurancaQAModel.Any())
                        {
                            var registroNormaSegurancaQAModel = normaSegurancaQAModel.FirstOrDefault();

                            parametros.Add("Id", registroNormaSegurancaQAModel.Id);
                            parametros.Add("Status", normaMessageModel.Status, DbType.Boolean);
                            parametros.Add("IntegradoEm", DateTime.Now);
                            parametros.Add("Integracao", $"Id Origem: {normaMessageModel.Id} - Operação: {(normaMessageModel.Status ? "ativação" : "inativação")}");

                            cmd = @"UPDATE Norma
                                       SET Status = @Status,
                                           IntegradoEm = @IntegradoEm,
                                           Integracao = @Integracao
                                     WHERE Id = @Id";
                        }
                        else
                        {
                            throw new Exception("Não foi possível ativar/inativar uma norma inexistente");
                        }
                    }

                    await con.ExecuteAsync(cmd, parametros);

                    return true;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    con.Close();
                }
            }
        }

        private string GetConnection()
        {
            var connection = _configuration
                .GetConnectionString("IndTextBrConnectionString");

            return connection;
        }
    }
}
