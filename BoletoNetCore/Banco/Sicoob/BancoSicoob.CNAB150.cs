using System;
using static System.String;

namespace BoletoNetCore
{
    partial class BancoSicoob : IBancoCNAB150
    {
        public override string GerarHeaderRemessaCNAB150(ref int numeroArquivoRemessa, ref int numeroRegistro)
        {
            try
            {
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "A", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 001, 0, "1", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 020, 0, Beneficiario.Codigo, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0023, 020, 0, Beneficiario.Nome, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0043, 003, 0, "756", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0046, 020, 0, "BANCO SICOOB", ' ');
                reg.Adicionar(TTiposDadoEDI.ediDataAAAAMMDD_________, 0066, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0074, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0080, 002, 0, "08", '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0082, 017, 0, "DEBITO AUTOMATICO", ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0099, 052, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB150.", ex);
            }
        }

        public override string GerarDetalheRemessaCNAB150(Boleto boleto, ref int registro)
        {
            string detalhe = Empty;
            detalhe += GerarDetalheSegmentoERemessaCNAB150(boleto, ref registro);
            return detalhe;
        }

        private string GerarDetalheSegmentoERemessaCNAB150(Boleto boleto, ref int registro)
        {
            var reg = new TRegistroEDI();
            registro++;

            string tipoIdentificacao = (boleto.Pagador.CPFCNPJ.Trim().Length == 11) ? "1" : "2";
            string dadosConta = $"{boleto.ContaDebitada}{boleto.DigitoVerificadorAgenciaContaDebitada}";

            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "E", '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 025, 0, boleto.NumeroControleParticipante, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0027, 004, 0, boleto.AgenciaDebitada, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0031, 020, 0, dadosConta, ' ');
            reg.Adicionar(TTiposDadoEDI.ediDataAAAAMMDDWithZeros, 0051, 008, 0, boleto.DataVencimento, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0059, 015, 2, boleto.ValorTitulo, '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0074, 002, 0, boleto.CodigoMoeda, '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0076, 053, 0, boleto.MensagemArquivoRemessa, ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0129, 001, 0, "X", ' ');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0130, 001, 0, tipoIdentificacao, '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0131, 015, 0, boleto.Pagador.CPFCNPJ, '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0146, 001, 0, "3", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0147, 001, 0, "2", '0');
            reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0148, 001, 0, "2", '0');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0149, 001, 0, Empty, ' ');
            reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.CodigoMovimentoRetorno, ' ');
            reg.CodificarLinha();
            var vLinha = reg.LinhaRegistro;
            return vLinha;
        }

        public override string GerarTrailerLoteRemessaCNAB150(ref int numeroArquivoRemessa, int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O número de registros no lote é igual ao número de registros gerados + 2 (header e trailler do lote)
                var numeroRegistrosNoLote = numeroRegistroGeral + 3;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "J", ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 006, 0, numeroArquivoRemessa, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataAAAAMMDD_________, 0008, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0016, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 017, 2, valorCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediDataAAAAMMDD_________, 0039, 008, 0, DateTime.Now, ' ');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 104, 0, Empty, ' ');
                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar TRAILER do lote no arquivo de remessa do CNAB150.", ex);
            }
        }

        public override string GerarTrailerRemessaCNAB150(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                // O número de registros no lote é igual ao número de registros gerados + 2 (header e trailler do lote)
                var numeroRegistrosNoLote = numeroRegistroGeral + 3;
                var reg = new TRegistroEDI();
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0001, 001, 0, "Z", '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 006, 0, numeroRegistrosNoLote, '0');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 017, 2, valorCobrancaSimples, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0025, 119, 0, Empty, ' ');
                reg.Adicionar(TTiposDadoEDI.ediNumericoSemSeparador_, 0144, 006, 0, 0, '0');
                reg.Adicionar(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, Empty, ' ');

                reg.CodificarLinha();
                return reg.LinhaRegistro;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar TRAILER no arquivo de remessa do CNAB150.", ex);
            }
        }

        public override void LerHeaderRetornoCNAB150(ArquivoRetorno arquivoRetorno, string registro)
        {
            arquivoRetorno.Banco.Beneficiario = new Beneficiario();
            arquivoRetorno.Banco.Beneficiario.Codigo = registro.Substring(2, 20).Trim();
            arquivoRetorno.Banco.Beneficiario.Nome = registro.Substring(22, 20).Trim();

            arquivoRetorno.Banco.Beneficiario.ContaBancaria = new ContaBancaria();

            arquivoRetorno.DataGeracao = Utils.ToDateTime(Utils.ToInt32(registro.Substring(65, 8)).ToString("##-##-####"));
            arquivoRetorno.NumeroSequencial = Utils.ToInt32(registro.Substring(73, 6));
        }

        public override void LerDetalheRetornoCNAB150SegmentoF(ref Boleto boleto, string registro)
        {
            try
            {
                boleto.NumeroControleParticipante = registro.Substring(1, 25);

                boleto.AgenciaDebitada = registro.Substring(26, 4);
                boleto.ContaDebitada = registro.Substring(30, 14);

                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(44, 8)).ToString("##-##-####"));
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(44, 8)).ToString("##-##-####"));

                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(52, 15)) / 100;

                //Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(67, 2);
                boleto.DescricaoMovimentoRetorno = Cnab.MovimentoRetornoCnab150(boleto.CodigoMovimentoRetorno, boleto.Banco);

                boleto.EspecieDocumento = TipoEspecieDocumento.NaoDefinido;

                boleto.MensagemArquivoRemessa = registro.Substring(69, 60);

                boleto.Pagador = new Pagador();
                var cpfCnpj = registro.Substring(130, 15).Trim();
                if (!string.IsNullOrEmpty(cpfCnpj))
                {
                    boleto.Pagador.CPFCNPJ = cpfCnpj;
                }

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 150 / T.", ex);
            }
        }
    }
}
