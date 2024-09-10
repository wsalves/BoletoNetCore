using BoletoNetCore.Extensions;
using System;
using static System.String;

namespace BoletoNetCore
{
    [CarteiraCodigo("21")]
    internal class BancoUnicredCarteira21 : ICarteira<BancoUnicred>
    {
        internal static Lazy<ICarteira<BancoUnicred>> Instance { get; } = new Lazy<ICarteira<BancoUnicred>>(() => new BancoUnicredCarteira21());

        private BancoUnicredCarteira21()
        {

        }

        public string FormataCodigoBarraCampoLivre(Boleto boleto)
        {
            return $"{boleto.Banco.Beneficiario.ContaBancaria.Agencia.PadLeft(4, '0')}" + //Agência BENEFICIÁRIO (Sem o dígito verificador, completar com zeros à esquerda quando necessário)
                $"{boleto.Banco.Beneficiario.ContaBancaria.Conta}{boleto.Banco.Beneficiario.ContaBancaria.DigitoConta}".PadLeft(10, '0') + //Conta do BENEFICIÁRIO (Com o dígito verificador - Completar com zeros à esquerda quando necessário)
                $"{boleto.NossoNumero}{boleto.NossoNumeroDV}".PadLeft(11, '0'); //Nosso Número (Com o dígito verificador)
        }

        public void FormataNossoNumero(Boleto boleto)
        {
            if (IsNullOrWhiteSpace(boleto.NossoNumero) || boleto.NossoNumero == "0000000000")
            {
                // Banco irá gerar Nosso Número
                boleto.NossoNumero = new String('0', 10);
                boleto.NossoNumeroDV = "0";
            }
            else
            {
                // Nosso Número informado pela empresa
                if (boleto.NossoNumero.Length > 10)
                    throw new Exception($"Nosso Número ({boleto.NossoNumero}) deve conter 10 dígitos.");
                boleto.NossoNumero = boleto.NossoNumero.PadLeft(10, '0');
                boleto.NossoNumeroDV = (boleto.NossoNumero).CalcularDVUnicred();
            }
            boleto.NossoNumeroFormatado = $"{boleto.NossoNumero.PadLeft(10, '0')}-{boleto.NossoNumeroDV}";
        }
    }
}
