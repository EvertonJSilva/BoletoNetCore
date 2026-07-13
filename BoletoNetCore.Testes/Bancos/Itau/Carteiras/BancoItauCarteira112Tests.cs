using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Itau Carteira 112")]
    public class BancoItauCarteira112Tests
    {
        readonly IBanco _banco;
        public BancoItauCarteira112Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "",
                Conta = "12345",
                DigitoConta = "6",
                CarteiraPadrao = "112",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Banco
            };
            _banco = Banco.Instancia(Bancos.Itau);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("", "", "", contaBancaria);

            _banco.FormataBeneficiario();
        }

        [Test]
        public void Itau_112_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoItauCarteira112Tests), 5, true, "N", 223344);
        }
        [TestCase(307.15, "223347", "BB000003C", "1", "112/00223347-4", "34191154300000307151120022334741234123456000", "34191.12002 22334.741232 41234.560005 1 15430000030715", 2026, 08, 19)]
        [TestCase(609.14, "223352", "BB000001F", "2", "112/00223352-4", "34192148200000609141120022335241234123456000", "34191.12002 22335.241232 41234.560005 2 14820000060914", 2026, 06, 19)]
        [TestCase(101.01, "223345", "BB000001A", "3", "112/00223345-8", "34193148200000101011120022334581234123456000", "34191.12002 22334.581232 41234.560005 3 14820000010101", 2026, 06, 19)]
        [TestCase(103.01, "223323", "BB000001A", "4", "112/00223323-5", "34194148200000103011120022332351234123456000", "34191.12002 22332.351232 41234.560005 4 14820000010301", 2026, 06, 19)]
        [TestCase(101.01, "223344", "BB000001A", "5", "112/00223344-1", "34195148200000101011120022334411234123456000", "34191.12002 22334.411232 41234.560005 5 14820000010101", 2026, 06, 19)]
        [TestCase(202.06, "223346", "BB000002B", "6", "112/00223346-6", "34196151200000202061120022334661234123456000", "34191.12002 22334.661232 41234.560005 6 15120000020206", 2026, 07, 19)]
        [TestCase(903.47, "223353", "BB000001I", "7", "112/00223353-2", "34197148200000903471120022335321234123456000", "34191.12002 22335.321232 41234.560005 7 14820000090347", 2026, 06, 19)]
        [TestCase(704.16, "223351", "BB000001G", "8", "112/00223351-6", "34198148200000704161120022335161234123456000", "34191.12002 22335.161232 41234.560005 8 14820000070416", 2026, 06, 19)]
        [TestCase(405.06, "223348", "BB000004D", "9", "112/00223348-2", "34199157400000405061120022334821234123456000", "34191.12002 22334.821232 41234.560005 9 15740000040506", 2026, 09, 19)]

        public void Deve_criar_boleto_itau_112_com_digito_verificador_Nossonr_Barra_Linha_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            //Ambiente
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            //Ação
            boleto.ValidarDados();

            //Assertivas
            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}