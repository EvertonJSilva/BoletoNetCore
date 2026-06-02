using System;
using System.ComponentModel;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [System.ComponentModel.Category("Bradesco Carteira 04")]
    public class BancoBradescoCarteira04
    {
        readonly IBanco _banco;
        public BancoBradescoCarteira04()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "X",
                Conta = "123456",
                DigitoConta = "X",
                CarteiraPadrao = "04",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Bradesco);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1213141", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Bradesco_04_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoBradescoCarteira04), 5, true, "?", 223344);
        }
        [Test]
        public void Bradesco_04_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBradescoCarteira04), 5, true, "?", 223344);
        }

        [TestCase(141.50,  "453", "BB943A", "8", "004/00000000453-1", "23798155600000141501234040000000045301234560", "23791.23405 40000.000048 53012.345608 8 15560000014150", 2026, 9, 1)]
        [TestCase(270,     "561", "BB932A", "3", "004/00000000561-9", "23793131300000270001234040000000056101234560", "23791.23405 40000.000055 61012.345601 3 13130000027000", 2026, 1, 1)]
        [TestCase(2717.16, "456", "BB874A", "1", "004/00000000456-6", "23791158600002717161234040000000045601234560", "23791.23405 40000.000048 56012.345601 1 15860000271716", 2026, 10, 1)]
        [TestCase(2924.11, "445", "BB874A", "6", "004/00000000445-0", "23796155700002924111234040000000044501234560", "23791.23405 40000.000048 45012.345604 6 15570000292411", 2026, 9, 2)]
        [TestCase(297.21,  "443", "BB833A", "3", "004/00000000443-4", "23793155700000297211234040000000044301234560", "23791.23405 40000.000048 43012.345609 3 15570000029721", 2026, 9, 2)]
        [TestCase(297.21,  "444", "BB834A", "1", "004/00000000444-2", "23791155700000297211234040000000044401234560", "23791.23405 40000.000048 44012.345607 1 15570000029721", 2026, 9, 2)]
        [TestCase(297.21,  "468", "BB856A", "2", "004/00000000468-P", "23792158700000297211234040000000046801234560", "23791.23405 40000.000048 68012.345606 2 15870000029721", 2026, 10, 2)]
        [TestCase(649.39,  "414", "BB815A", "2", "004/00000000414-0", "23792152500000649391234040000000041401234560", "23791.23405 40000.000048 14012.345600 2 15250000064939", 2026, 8, 1)]
        [TestCase(830,     "562", "BB933A", "5", "004/00000000562-7", "23795131300000830001234040000000056201234560", "23791.23405 40000.000055 62012.345609 5 13130000083000", 2026, 1, 1)]
        [TestCase(2924.11, "445", "BB874A", "6", "004/00000000445-0", "23796155700002924111234040000000044501234560", "23791.23405 40000.000048 45012.345604 6 15570000292411", 2026, 9, 2)]
        
        public void Deve_criar_boleto_bradesco_04_com_linha_digitavel_Digito_Nossonr_Barra_valida(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            var boleto = new Boleto(_banco)
            {
                DataVencimento = new DateTime(anoMesDia[0], anoMesDia[1], anoMesDia[2]),
                ValorTitulo = valorTitulo,
                NossoNumero = nossoNumero,
                NumeroDocumento = numeroDocumento,
                EspecieDocumento = TipoEspecieDocumento.DM,
                Pagador = TestUtils.GerarPagador()
            };

            boleto.ValidarDados();

            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}