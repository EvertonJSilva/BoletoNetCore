using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Itau Carteira 138")]
    public class BancoItauCarteira138Tests
    {
        readonly IBanco _banco;
        public BancoItauCarteira138Tests()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "1234",
                DigitoAgencia = "",
                Conta = "56789",
                DigitoConta = "0",
                CarteiraPadrao = "138",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.Itau);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("", "", "", contaBancaria);

            _banco.FormataBeneficiario();
        }

        [Test]
        public void Itau_138_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoItauCarteira138Tests), 5, true, "N", 223344);
        }

        [Test]
        public void Itau_138_REM240()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB240, nameof(BancoItauCarteira138Tests), 5, true, "N", 223344);
        }
        [TestCase(300.00, "223347", "BB000003C", "1", "138/00223347-7", "34191154300000300001380022334771234567890000", "34191.38007 22334.771239 45678.900007 1 15430000030000", 2026, 08, 19)]
        [TestCase(600.00, "223350", "BB000001F", "2", "138/00223350-1", "34192148200000600001380022335011234567890000", "34191.38007 22335.011239 45678.900007 2 14820000060000", 2026, 06, 19)]
        [TestCase(200.00, "223346", "BB000002B", "3", "138/00223346-9", "34193151200000200001380022334691234567890000", "34191.38007 22334.691239 45678.900007 3 15120000020000", 2026, 07, 19)]
        [TestCase(214.00, "223323", "BB000002B", "4", "138/00223323-8", "34194151200000214001380022332381234567890000", "34191.38007 22332.381239 45678.900007 4 15120000021400", 2026, 07, 19)]
        [TestCase(700.01, "223351", "BB000001G", "5", "138/00223351-9", "34195148200000700011380022335191234567890000", "34191.38007 22335.191239 45678.900007 5 14820000070001", 2026, 06, 19)]
        [TestCase(100.00, "223345", "BB000001A", "6", "138/00223345-1", "34196148200000100001380022334511234567890000", "34191.38007 22334.511239 45678.900007 6 14820000010000", 2026, 06, 19)]
        [TestCase(201.00, "223387", "BB000002B", "7", "138/00223387-3", "34197151200000201001380022338731234567890000", "34191.38007 22338.731239 45678.900007 7 15120000020100", 2026, 07, 19)]
        [TestCase(900.03, "223353", "BB000001I", "8", "138/00223353-5", "34198148200000900031380022335351234567890000", "34191.38007 22335.351239 45678.900007 8 14820000090003", 2026, 06, 19)]
        [TestCase(207.00, "223385", "BB000002B", "9", "138/00223385-7", "34199151200000207001380022338571234567890000", "34191.38007 22338.571239 45678.900007 9 15120000020700", 2026, 07, 19)]

        public void Deve_criar_boleto_itau_138_com_digito_verificador_Nossonr_Barra_Linha_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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

            //Assertiva
            Assert.That(boleto.CodigoBarra.DigitoVerificador, Is.EqualTo(digitoVerificador), $"Dígito Verificador diferente de {digitoVerificador}");
            Assert.That(boleto.NossoNumeroFormatado, Is.EqualTo(nossoNumeroFormatado), "Nosso número inválido");
            Assert.That(boleto.CodigoBarra.CodigoDeBarras, Is.EqualTo(codigoDeBarras), "Código de Barra inválido");
            Assert.That(boleto.CodigoBarra.LinhaDigitavel, Is.EqualTo(linhaDigitavel), "Linha digitável inválida");
        }
    }
}