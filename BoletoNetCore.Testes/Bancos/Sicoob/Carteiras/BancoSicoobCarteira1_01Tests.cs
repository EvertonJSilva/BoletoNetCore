using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Sicoob Carteira 1 Var 01")]
    public class BancoSicoobCarteira101Tests
    {
        readonly IBanco _banco;
        public BancoSicoobCarteira101Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "4277",
                DigitoAgencia = "3",
                Conta = "6498",
                DigitoConta = "0",
                CarteiraPadrao = "1",
                VariacaoCarteiraPadrao = "01",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Sicoob);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("17227", "8", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Sicoob_1_01_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoSicoobCarteira101Tests), 5, true, "?", 223344);
        }

        [Test]
        public void Sicoob_1_01_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoSicoobCarteira101Tests), 5, true, "?", 223344);
        }
        [TestCase(700,         "8", "BO123456B", "1", "0000008-0", "75691138200000700001427701017227800000080001", "75691.42776 01017.227800 00000.800011 1 13820000070000", 2026, 3, 11)]
        [TestCase(701,         "2", "BO123456A", "2", "0000002-6", "75692138200000701001427701017227800000026001", "75691.42776 01017.227800 00000.260018 2 13820000070100", 2026, 3, 11)]
        [TestCase(707,         "3", "BO123456K", "3", "0000003-3", "75693138200000707001427701017227800000033001", "75691.42776 01017.227800 00000.330019 3 13820000070700", 2026, 3, 11)]
        [TestCase(300,         "4", "BO123456D", "4", "0000004-0", "75694162600000300001427701017227800000040001", "75691.42776 01017.227800 00000.400010 4 16260000030000", 2026, 11, 10)]
        [TestCase(306,         "5", "BO123456G", "5", "0000005-8", "75695162600000306001427701017227800000058001", "75691.42776 01017.227800 00000.580019 5 16260000030600", 2026, 11, 10)]
        [TestCase(4011.24, "12349", "BO123456F", "6", "0012349-2", "75696167400004011241427701017227800123492001", "75691.42776 01017.227800 01234.920013 6 16740000401124", 2026, 12, 28)]
        [TestCase(400,         "4", "BO123456D", "7", "0000004-0", "75697142900000400001427701017227800000040001", "75691.42776 01017.227800 00000.400010 7 14290000040000", 2026, 04, 27)]
        [TestCase(409,         "5", "BO123456E", "8", "0000005-8", "75698165400000409001427701017227800000058001", "75691.42776 01017.227800 00000.580019 8 16540000040900", 2026, 12, 08)]
        [TestCase(900,         "9", "BO123456F", "9", "0000009-7", "75699158200000900001427701017227800000097001", "75691.42776 01017.227800 00000.970012 9 15820000090000", 2026, 09, 27)]

        public void Deve_criar_boleto_sicoob_1_01_com_digito_verificador_Nossonr_Barra_linha_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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