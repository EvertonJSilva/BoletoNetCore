using System;
using NUnit.Framework;

namespace BoletoNetCore.Testes
{
    [TestFixture]
    [Category("Banco Daycoval Carteira 121")]
    public class BancoDaycovalCarteira121
    {
        readonly IBanco _banco;
        public BancoDaycovalCarteira121()
        {
            var contaBancaria = new ContaBancaria
            {
                Agencia = "0001",
                DigitoAgencia = "9",
                Conta = "1213165",
                DigitoConta = "7",
                CarteiraPadrao = "121",
                OperacaoConta = "1934436",
                TipoCarteiraPadrao = TipoCarteira.CarteiraCobrancaSimples,
                TipoFormaCadastramento = TipoFormaCadastramento.ComRegistro,
                TipoImpressaoBoleto = TipoImpressaoBoleto.Empresa
            };
            _banco = Banco.Instancia(Bancos.BancoDaycoval);
            _banco.Beneficiario = TestUtils.GerarBeneficiario("9192132", "", "190600919213200", contaBancaria);
            _banco.FormataBeneficiario();
        }

        [Test]
        public void Daycoval_121_REM400()
        {
            TestUtils.TestarHomologacao(_banco, TipoArquivo.CNAB400, nameof(BancoDaycovalCarteira121), 3, true, "?", 84580106);
        }
        [TestCase(2711.12, "84580104", "000002A", "1", "00019/121/0084580104-5", "70791132800002711120001121193443600845801045", "70790.00118 21193.443609 08458.010454 1 13280000271112", 2026, 01, 16)]
        [TestCase(2717.12, "84580102", "000002A", "2", "00019/121/0084580102-9", "70792132800002717120001121193443600845801029", "70790.00118 21193.443609 08458.010298 2 13280000271712", 2026, 01, 16)]
        [TestCase(2717.12, "84580103", "000002A", "3", "00019/121/0084580103-7", "70793132800002717120001121193443600845801037", "70790.00118 21193.443609 08458.010371 3 13280000271712", 2026, 01, 16)]
        [TestCase(2717.12, "84580104", "000002A", "4", "00019/121/0084580104-5", "70794132800002717120001121193443600845801045", "70790.00118 21193.443609 08458.010454 4 13280000271712", 2026, 01, 16)]
        [TestCase(2712.12, "84580102", "000002A", "5", "00019/121/0084580102-9", "70795132800002712120001121193443600845801029", "70790.00118 21193.443609 08458.010298 5 13280000271212", 2026, 01, 16)]
        [TestCase(2714.12, "84580102", "000002A", "6", "00019/121/0084580102-9", "70796132800002714120001121193443600845801029", "70790.00118 21193.443609 08458.010298 6 13280000271412", 2026, 01, 16)]
        [TestCase(2711.12, "84580101", "000002A", "7", "00019/121/0084580101-1", "70797132800002711120001121193443600845801011", "70790.00118 21193.443609 08458.010116 7 13280000271112", 2026, 01, 16)]
        [TestCase(2711.12, "84580107", "000002A", "8", "00019/121/0084580107-8", "70798132800002711120001121193443600845801078", "70790.00118 21193.443609 08458.010785 8 13280000271112", 2026, 01, 16)]
        [TestCase(141.50,  "84580106", "000001A", "9", "00019/121/0084580106-0", "70799166200000141500001121193443600845801060", "70790.00118 21193.443609 08458.010603 9 16620000014150", 2026, 12, 16)]
        //                                                                                                                        70790.00118 21194.346306 08458.010868 9 10250000100000
        public void Deve_criar_boleto_daycoval_01_com_digito_verificador_Nossonr_Barra_valido(decimal valorTitulo, string nossoNumero, string numeroDocumento, string digitoVerificador, string nossoNumeroFormatado, string codigoDeBarras, string linhaDigitavel, params int[] anoMesDia)
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