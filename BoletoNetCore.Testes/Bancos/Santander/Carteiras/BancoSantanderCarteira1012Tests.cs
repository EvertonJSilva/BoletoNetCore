using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Santander Carteira 101/2")]
    public class BancoSantanderCarteira1012Tests
    {
        readonly IBanco _banco;
        public BancoSantanderCarteira1012Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "5",
                Conta = "12345678",
                DigitoConta = "9",
                CarteiraPadrao = "101",
                VariacaoCarteiraPadrao = "2",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Santander);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("1234567", "", "123400001234567", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Santander_1012_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoSantanderCarteira1012Tests), 5, true, "N", 223344);
        }
        [TestCase(297.46, "13724", "BB834A", "1", "0000000013724-3", "03391155700000297469123456700000000137240101", "03399.12347 56700.000005 01372.401016 1 15570000029746", 2026, 9, 2)]
        [TestCase(141.50,   "453", "BB943A", "2", "0000000000453-7", "03392155600000141509123456700000000004530101", "03399.12347 56700.000005 00045.301017 2 15560000014150", 2026, 9, 1)]
        [TestCase(297.22,   "444", "BB834A", "3", "0000000000444-8", "03393155700000297229123456700000000004440101", "03399.12347 56700.000005 00044.401016 3 15570000029722", 2026, 9, 2)]
        [TestCase(2717.16,  "456", "BB874A", "4", "0000000000456-1", "03394158600002717169123456700000000004560101", "03399.12347 56700.000005 00045.601010 4 15860000271716", 2026, 10, 1)]
        [TestCase(305.16,   "460", "BB874A", "5", "0000000000460-0", "03395158600000305169123456700000000004600101", "03399.12347 56700.000005 00046.001012 5 15860000030516", 2026, 10, 1)]
        [TestCase(308.16,   "461", "BB874A", "6", "0000000000461-8", "03396158600000308169123456700000000004610101", "03399.12347 56700.000005 00046.101010 6 15860000030816", 2026, 10, 1)]
        [TestCase(297.34, "12428", "BB834A", "7", "0000000012428-1", "03397155700000297349123456700000000124280101", "03399.12347 56700.000005 01242.801015 7 15570000029734", 2026, 9, 2)]
        [TestCase(2717.16,  "459", "BB874A", "8", "0000000000459-6", "03398158600002717169123456700000000004590101", "03399.12347 56700.000005 00045.901014 8 15860000271716", 2026, 10, 1)]
        [TestCase(29.16,    "470", "BB871A", "9", "0000000000470-7", "03399158600000029169123456700000000004700101", "03399.12347 56700.000005 00047.001011 9 15860000002916", 2026, 10, 1)]

        public void Deve_criar_boleto_santander_1012_com_digito_Nossonr_barra_linha_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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