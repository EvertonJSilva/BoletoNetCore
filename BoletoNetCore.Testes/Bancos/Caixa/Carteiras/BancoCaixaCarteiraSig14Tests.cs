using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Caixa Carteira Sig14")]
    public class BancoCaixaCarteiraSig14Tests
    {
        readonly IBanco _banco;
        
        public BancoCaixaCarteiraSig14Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "SIG14",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Caixa);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("123456", "0", "", contaBancaria);

            //para testar com beneficiário 7 digitos
            //_banco.Beneficiario = TestUtils.GerarBeneficiario("1234567", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Caixa_SIG14_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoCaixaCarteiraSig14Tests), 5, true, "?", 223344);
        }

        [Test]
        public void Caixa_SIG14_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoCaixaCarteiraSig14Tests), 5, true, "?", 223344);
        }
        [TestCase(800, "9", "BO123456B", "1", "14000000000000009-0", "10491140200000800001234560000100040000000099", "10491.23456 60000.100044 00000.000992 1 14020000080000", 2026, 3, 31)]
        [TestCase(409, "5", "BO123456E", "2", "14000000000000005-7", "10492165400000409001234560000100040000000056", "10491.23456 60000.100044 00000.000562 2 16540000040900", 2026, 12, 08)]
        [TestCase(100, "2", "BO123456B", "3", "14000000000000002-2", "10493157000000100001234560000100040000000021", "10491.23456 60000.100044 00000.000216 3 15700000010000", 2026, 9, 15)]
        [TestCase(200, "3", "BO123456C", "4", "14000000000000003-0", "10494159900000200001234560000100040000000030", "10491.23456 60000.100044 00000.000307 4 15990000020000", 2026, 10, 14)]
        [TestCase(500, "6", "BO123456F", "5", "14000000000000006-5", "10495131700000500001234560000100040000000064", "10491.23456 60000.100044 00000.000646 5 13170000050000", 2026, 01, 05)]
        [TestCase(300, "4", "BO123456D", "6", "14000000000000004-9", "10496162600000300001234560000100040000000048", "10491.23456 60000.100044 00000.000489 6 16260000030000", 2026, 11, 10)]
        [TestCase(500, "5", "BO123456E", "7", "14000000000000005-7", "10497165400000500001234560000100040000000056", "10491.23456 60000.100044 00000.000562 7 16540000050000", 2026, 12, 08)]
        [TestCase(900, "1", "BO123456E", "8", "14000000000000001-4", "10498165400000900001234560000100040000000013", "10491.23456 60000.100044 00000.000133 8 16540000090000", 2026, 12, 08)]
        [TestCase(700, "8", "BO123456B", "9", "14000000000000008-1", "10499138200000700001234560000100040000000080", "10491.23456 60000.100044 00000.000802 9 13820000070000", 2026, 3, 11)]
        
        public void Deve_criar_boleto_caixa_sig14_com_digito_verificador_Nossonr_Barra_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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