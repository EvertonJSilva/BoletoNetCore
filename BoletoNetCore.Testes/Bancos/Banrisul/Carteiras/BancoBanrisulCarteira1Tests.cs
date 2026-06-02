using NUnit.Framework;
using System;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Banrisul Carteira 1")]
    public class BancoBanrisulCarteira1Tests
    {
        readonly IBanco _banco;
        public BancoBanrisulCarteira1Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0340",
                DigitoAgencia = "",
                Conta = "12345606",
                DigitoConta = "",
                CarteiraPadrao = "1",
                VariacaoCarteiraPadrao = "",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro
            };
            _banco = Banco.Instancia(Bancos.Banrisul);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("0340123456063", "", "", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Banrisul_1_REM400_BancoEmite()
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBanrisulCarteira1Tests) + "_BancoEmite", 5, true, "?", 0);
        }
        [Test]
        public void Banrisul_1_REM400_EmpresaEmite()
        {
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoBanrisulCarteira1Tests) + "_EmpresaEmite", 5, true, "?", 12345);
        }

        #region Tipo Impressao Empresa

        [TestCase(131.57, "457", "BB943A", "4", "00000457-21", "04194155600000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 4 15560000013157", 2026, 9, 1)]
        [TestCase(217.12, "453", "BB874A", "1", "00000453-14", "04191155700000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 1 15570000021712", 2026, 9, 2)]
        [TestCase(270.54, "459", "BB932A", "1", "00000459-85", "04191131300000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 1 13130000027054", 2026, 1, 1)]
        [TestCase(276.15, "458", "BB874A", "7", "00000458-02", "04197158600000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 7 15860000027615", 2026, 10, 1)]
        [TestCase(287.25, "456", "BB834A", "2", "00000456-40", "04192155700000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 2 15570000028725", 2026, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "3", "00000455-78", "04193155700000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 3 15570000028826", 2026, 9, 2)]
        [TestCase(293.23, "452", "BB856A", "9", "00000452-33", "04199158700000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 9 15870000029323", 2026, 10, 2)]
        [TestCase(647.34, "451", "BB815A", "6", "00000451-52", "04196152500000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 6 15250000064734", 2026, 8, 1)]
        [TestCase(829.21, "454", "BB933A", "9", "00000454-97", "04199131300000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 9 13130000082921", 2026, 1, 1)]
        
        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_empresa_e_nosso_numero_Digito_BarraLinha_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa;
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

        #endregion

        #region Tipo Impressao Banco
    
        [TestCase(131.57, "457", "BB943A", "4", "00000457-21", "04194155600000131572103401234560000004574028", "04192.10349 01234.560009 00045.740289 4 15560000013157", 2026, 9, 1)]
        [TestCase(217.12, "453", "BB874A", "1", "00000453-14", "04191155700000217122103401234560000004534018", "04192.10349 01234.560009 00045.340189 1 15570000021712", 2026, 9, 2)]
        [TestCase(270.54, "459", "BB932A", "1", "00000459-85", "04191131300000270542103401234560000004594088", "04192.10349 01234.560009 00045.940889 1 13130000027054", 2026, 1, 1)]
        [TestCase(276.15, "458", "BB874A", "7", "00000458-02", "04197158600000276152103401234560000004584090", "04192.10349 01234.560009 00045.840907 7 15860000027615", 2026, 10, 1)]
        [TestCase(287.25, "456", "BB834A", "2", "00000456-40", "04192155700000287252103401234560000004564030", "04192.10349 01234.560009 00045.640307 2 15570000028725", 2026, 9, 2)]
        [TestCase(288.26, "455", "BB833A", "3", "00000455-78", "04193155700000288262103401234560000004554051", "04192.10349 01234.560009 00045.540515 3 15570000028826", 2026, 9, 2)]
        [TestCase(293.23, "452", "BB856A", "9", "00000452-33", "04199158700000293232103401234560000004524020", "04192.10349 01234.560009 00045.240207 9 15870000029323", 2026, 10, 2)]
        [TestCase(647.34, "451", "BB815A", "6", "00000451-52", "04196152500000647342103401234560000004514041", "04192.10349 01234.560009 00045.140415 6 15250000064734", 2026, 8, 1)]
        [TestCase(829.21, "454", "BB933A", "9", "00000454-97", "04199131300000829212103401234560000004544080", "04192.10349 01234.560009 00045.440807 9 13130000082921", 2026, 1, 1)]

        public void Deve_criar_boleto_banrisul_01_com_tipo_emissao_banco_e_nosso_numero_Digito_Linha_Barra_formatado_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
        {
            // Ambiente - Emissão pela empresa
            _banco.Beneficiario.ContaBancaria.TipoImpressaoBoleto = TipoImpressaoBoleto.Banco;
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
        #endregion
    }
}