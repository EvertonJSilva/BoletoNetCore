using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Safra Carteira 1")]
    public class BancoSafraCarteira1
    {
        readonly IBanco _banco;
        public BancoSafraCarteira1()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "5",
                Conta = "123456",
                DigitoConta = "7",
                CarteiraPadrao = "1",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Safra);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Safra_1_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoSafraCarteira1), 5, true, "?", 223344);
        }
        [TestCase(292.21,  "444", "BB834A", "1", "00000444-8", "42291155700000292217123450012345670000044482", "42297.12346 50012.345679 00000.444828 1 15570000029221", 2026, 9, 2)]
        [TestCase(2921.27, "443", "BB833A", "2", "00000443-0", "42292155700002921277123450012345670000044302", "42297.12346 50012.345679 00000.443028 2 15570000292127", 2026, 9, 2)]
        [TestCase(293.21,  "468", "BB856A", "3", "00000468-5", "42293158700000293217123450012345670000046852", "42297.12346 50012.345679 00000.468520 3 15870000029321", 2026, 10, 2)]
        [TestCase(305.21,  "461", "BB852A", "4", "00000461-8", "42294158700000305217123450012345670000046182", "42297.12346 50012.345679 00000.461822 4 15870000030521", 2026, 10, 2)]
        [TestCase(292.21,  "461", "BB852A", "5", "00000461-8", "42295158700000292217123450012345670000046182", "42297.12346 50012.345679 00000.461822 5 15870000029221", 2026, 10, 2)]
        [TestCase(141.50,  "453", "BB943A", "6", "00000453-7", "42296155600000141507123450012345670000045372", "42297.12346 50012.345679 00000.453720 6 15560000014150", 2026, 9, 1)]
        [TestCase(645.39,  "414", "BB815A", "7", "00000414-6", "42297152500000645397123450012345670000041462", "42297.12346 50012.345679 00000.414623 7 15250000064539", 2026, 8, 1)]
        [TestCase(2711.12, "456", "BB874A", "8", "00000456-1", "42298158600002711127123450012345670000045612", "42297.12346 50012.345679 00000.456129 8 15860000271112", 2026, 10, 1)]
        [TestCase(838,     "562", "BB933A", "9", "00000562-2", "42299131300000838007123450012345670000056222", "42297.12346 50012.345679 00000.562223 9 13130000083800", 2026, 1, 1)]

        public void Deve_criar_boleto_safra_01_com_digito_verificador_Nossonr_Barra_Linha_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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