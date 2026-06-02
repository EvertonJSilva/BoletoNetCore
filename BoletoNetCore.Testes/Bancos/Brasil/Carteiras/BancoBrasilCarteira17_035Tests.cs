using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Brasil Carteira 17 Var 035")]
    public class BancoBrasilCarteira17_035Tests
    {
        readonly IBanco _banco;
        public BancoBrasilCarteira17_035Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "17",
                VariacaoCarteiraPadrao = "035",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.BancoDoBrasil);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1234567", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Brasil_17_035_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBrasilCarteira17_035Tests), 5, true, "?", 223344);
        }

        [Test]
        public void Brasil_17_035_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBrasilCarteira17_035Tests), 5, true, "?", 223344);
        }
        [TestCase(1232.78, "1", "BO123456A", "8", "12345670000000001", "00198154100001232780000001234567000000000117", "00190.00009 01234.567004 00000.001172 8 15410000123278", 2026, 8, 17)]
        [TestCase(200d,    "3", "BO123456C", "9", "12345670000000003", "00199158100000200000000001234567000000000317", "00190.00009 01234.567004 00000.003178 9 15810000020000", 2026, 9, 26)]
        [TestCase(300,     "4", "BO123456D", "1", "12345670000000004", "00191161100000300000000001234567000000000417", "00190.00009 01234.567004 00000.004176 1 16110000030000", 2026, 10, 26)]
        [TestCase(306.52,  "4", "BO123456D", "1", "12345670000000004", "00191161100000306520000001234567000000000417", "00190.00009 01234.567004 00000.004176 1 16110000030652", 2026, 10, 26)]
        [TestCase(400d,    "5", "BO123456E", "5", "12345670000000005", "00195164300000400000000001234567000000000517", "00190.00009 01234.567004 00000.005173 5 16430000040000", 2026, 11, 27)]
        [TestCase(402d,    "5", "BO123456E", "6", "12345670000000005", "00196164300000402000000001234567000000000517", "00190.00009 01234.567004 00000.005173 6 16430000040200", 2026, 11, 27)]
        [TestCase(800,     "9", "BO123456I", "1", "12345670000000009", "00191139000000800000000001234567000000000917", "00190.00009 01234.567004 00000.009175 1 13900000080000", 2026, 3, 19)]
        [TestCase(609,     "7", "BO123456G", "1", "12345670000000007", "00191133900000609000000001234567000000000717", "00190.00009 01234.567004 00000.007179 1 13390000060900", 2026, 1, 27)]
        [TestCase(600,     "7", "BO123456G", "2", "12345670000000007", "00192133900000600000000001234567000000000717", "00190.00009 01234.567004 00000.007179 2 13390000060000", 2026, 1, 27)]
        public void Deve_criar_boleto_brasil_17_035_com_digito_verificador_NossoNr_Barra_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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