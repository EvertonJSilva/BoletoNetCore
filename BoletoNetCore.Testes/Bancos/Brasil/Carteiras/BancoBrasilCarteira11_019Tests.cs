using NUnit.Framework;
using System;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Brasil Carteira 11 Var 019")]
    public class BancoBrasilCarteira11019Tests
    {
        readonly IBanco _banco;
        public BancoBrasilCarteira11019Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "12345",
                DigitoConta = "X",
                CarteiraPadrao = "11",
                VariacaoCarteiraPadrao = "019",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Banco
            };
            _banco = Banco.Instancia(Bancos.BancoDoBrasil);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1234567", "", "", contaBancaria);
            _banco.FormataBeneficiario();


        }

        [Test]
        public void Brasil_11_019_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBrasilCarteira11019Tests), 5, true, "?", 0);
        }

        [Test]
        public void Brasil_11_019_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBrasilCarteira11019Tests), 5, true, "?", 0);
        }
        [TestCase(200,                    "", "123456/2026-A", "1", "00000000000000000", "00191167100000200000000000000000000000000011", "00190.00009 00000.000000 00000.000117 1 16710000020000", 2026, 12, 25)]
        [TestCase(804,   "12345670000000321",      "654321RT", "2", "12345670000000321", "00192131400000804000000001234567000000032111", "00190.00009 01234.567004 00000.321117 2 13140000080400", 2026, 1, 2)]
        [TestCase(806,   "12345670000000321",      "654321RT", "3", "12345670000000321", "00193131400000806000000001234567000000032111", "00190.00009 01234.567004 00000.321117 3 13140000080600", 2026, 1, 2)]
        [TestCase(791,   "12345679999999901",      "654321VW", "4", "12345679999999901", "00194138100000791000000001234567999999990111", "00190.00009 01234.567996 99999.901111 4 13810000079100", 2026, 3, 10)]
        [TestCase(1200,                   "",     "321as1234", "5", "00000000000000000", "00195131300001200000000000000000000000000011", "00190.00009 00000.000000 00000.000117 5 13130000120000", 2026, 1, 1)]
        [TestCase(801,   "12345670000000321",      "654321RT", "6", "12345670000000321", "00196131400000801000000001234567000000032111", "00190.00009 01234.567004 00000.321117 6 13140000080100", 2026, 1, 2)]
        [TestCase(800d,  "12345670000000001",    "BAN789A123", "7", "12345670000000001", "00197137100000800000000001234567000000000111", "00190.00009 01234.567004 00000.001115 7 13710000080000", 2026, 2, 28)]
        [TestCase(805,   "12345670000000321",      "654321RT", "8", "12345670000000321", "00198131400000805000000001234567000000032111", "00190.00009 01234.567004 00000.321117 8 13140000080500", 2026, 1, 2)]
        [TestCase(220.58,"12345670000000003",      "654321WA", "9", "12345670000000003", "00199145600000220580000001234567000000000311", "00190.00009 01234.567004 00000.003111 9 14560000022058", 2026, 05, 24)]
        
        public void Deve_criar_boleto_brasil_11_019_com_digito_verificador_NossoNr_Barra_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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