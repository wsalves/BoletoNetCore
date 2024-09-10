using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using System;

namespace BoletoNetCore
{
    internal sealed partial class BancoUnicred : BancoFebraban<BancoUnicred>, IBanco
    {
        public BancoUnicred()
        {
            Codigo = 136;
            Nome = "UNICRED DO BRASIL";
            Digito = "8";
            IdsRetornoCnab400RegistroDetalhe = new List<string> { "1" };
            RemoveAcentosArquivoRemessa = true;
        }

        public void FormataBeneficiario()
        {
            var contaBancaria = Beneficiario.ContaBancaria;

            if (!CarteiraFactory<BancoUnicred>.CarteiraEstaImplementada(contaBancaria.CarteiraPadrao))
                throw BoletoNetCoreException.CarteiraNaoImplementada(contaBancaria.CarteiraPadrao);

            contaBancaria.FormatarDados("PAGÁVEL EM QUALQUER BANCO ATÉ O VENCIMENTO.", "", "", 9);
            Beneficiario.CodigoFormatado = $"{Beneficiario.ContaBancaria.Agencia}/{Beneficiario.ContaBancaria.Conta}-{Beneficiario.ContaBancaria.DigitoConta}";
        }

        public override void FormataNossoNumero(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoUnicred>.ObterCarteira(boleto.Carteira);
            carteira.FormataNossoNumero(boleto);
        }

        public override string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            var carteira = CarteiraFactory<BancoUnicred>.ObterCarteira(boleto.Carteira);
            return carteira.FormataCodigoBarraCampoLivre(boleto);
        }


        public override string FormatarNomeArquivoRemessa(TipoArquivo TipoArquivo, IBanco Banco, int sequencial)
        {
            if (sequencial < 0 || sequencial > 10)
                throw BoletoNetCoreException.NumeroSequencialInvalido(sequencial);

            //número máximos de arquivos enviados no dia são 10 
            return string.Format("{0}_UNICRED_{1}_{2}_{3}_{4}.REM", 
                TipoArquivo == TipoArquivo.CNAB240 ? "CNAB240" : "CNAB400",  
                Beneficiario.Codigo.PadLeft(10, '0'),
                Beneficiario.ContaBancaria.Agencia.PadLeft(4, '0'),
                DateTime.Now.ToString("ddMMyyyyy"),
                $"{(sequencial == 10 ? 0 : sequencial).ToString().PadLeft(2, '0')}");
        }

        public string GerarMensagemRemessa(TipoArquivo tipoArquivo, Boleto boleto, ref int numeroRegistro)
        {
            return null;
        }
    }
}