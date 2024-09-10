using System.Collections.Generic;
using BoletoNetCore.Exceptions;
using System;
using BoletoNetCore.Util;

namespace BoletoNetCore
{
    partial class BancoUnicred : IBancoCNAB400
    {
        public string GerarDetalheRemessaCNAB400(Boleto boleto, ref int registro)
        {
            string detalhe = string.Empty;
            registro++;

            //Redireciona para o Detalhe da remessa Conforme o "Tipo de Documento" = "Tipo de Cobrança do CNAB400":
            //  A = 'A' - Unicredi com Registro
            // C1 = 'C' - Unicredi sem Registro Impressão Completa pelo Unicredi
            // C2 = 'C' - Unicredi sem Registro Pedido de bloquetos pré-impressos
            if (boleto.Carteira.Equals("21"))
                detalhe = GerarDetalheRemessaCNAB400_21(boleto, registro);
            return detalhe;
        }

        public string GerarHeaderRemessaCNAB400(ref int numeroArquivoRemessa, ref int numeroRegistroGeral)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "0", ' '));                             //001-001
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 001, 0, "1", ' '));                             //002-002
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0003, 007, 0, "REMESSA", ' '));                       //003-009
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0010, 002, 0, "01", ' '));                            //010-011
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0012, 015, 0, "COBRANCA", ' '));                      //012-026
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0027, 020, 0, Beneficiario.Codigo, '0'));             //027-031
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0047, 030, 0, Beneficiario.Nome, ' '));            //032-045
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0077, 003, 0, this.Codigo.ToString(), ' '));                           //077-079
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0080, 015, 0, "UNICRED", ' '));                       //080-094
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 0095, 006, 0, DateTime.Now, ' '));                    //095-102
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0101, 007, 0, "", ' '));                              //103-110
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0108, 003, 0, "000", '0'));                              //103-110
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0111, 007, 0, numeroArquivoRemessa.ToString(), '0')); //111-117
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0118, 277, 0, "", ' '));                              //118-390
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0395, 006, 0, numeroRegistroGeral, '0'));             //395-400

                reg.CodificarLinha();

                string vLinha = reg.LinhaRegistro;
                string _header = Utils.SubstituiCaracteresEspeciais(vLinha);

                return _header;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar HEADER do arquivo de remessa do CNAB400.", ex);
            }
        }






        private string GerarDetalheRemessaCNAB400_21(Boleto boleto, int numeroRegistro)
        {
            try
            {
                //string NumeroDocumento = boleto.NossoNumero;

                TRegistroEDI reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "1", ' '));                                                    //001-001
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0002, 005, 0, boleto.Banco.Beneficiario.ContaBancaria.Agencia.OnlyNumber(), '0'));                                                    //002-002  'A' - Unicredi com Registro
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0007, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoAgencia, '0'));                                                    //003-003  'A' - Simples
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0008, 012, 0, boleto.Banco.Beneficiario.ContaBancaria.Conta.OnlyNumber(), '0'));                                                    //002-002  'A' - Unicredi com Registro
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0020, 001, 0, boleto.Banco.Beneficiario.ContaBancaria.DigitoConta, ' '));                                           //005-016
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0021, 001, 0, "0", '0'));                                                    //017-017  Tipo de moeda: 'A' - REAL
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0022, 003, 0, boleto.Banco.Beneficiario.ContaBancaria.CarteiraPadrao , '0'));                                                    //018-018  Tipo de desconto: 'A' - VALOR
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0025, 013, 0, "0", '0'));                                                    //017-017  Tipo de moeda: 'A' - REAL
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0038, 025, 0, boleto.NumeroControleParticipante, ' '));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0063, 003, 0, boleto.Banco.Codigo.ToString(), '0'));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0066, 002, 0, "0", '0'));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0068, 025, 0, " ", ' '));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0093, 001, 0, "0", '0'));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0094, 001, 0, "2", '0'));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0095, 010, 2, boleto.PercentualMulta, '0'));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0105, 001, 0, boleto.ValorJurosDia != 0 ? "1" : "5", '0')); // 118 a 118 - Código do juro de mora
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0106, 001, 0, "N", 'N'));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0107, 001, 0, "N", 'N'));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0108, 001, 0, " ", ' '));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0109, 002, 0, boleto.CodigoMovimentoRetorno, '0'));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0111, 010, 0, boleto.NumeroDocumento, ' '));
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 0121, 006, 0, boleto.DataVencimento, ' '));                                 //121-126
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0127, 013, 2, boleto.ValorTitulo, '0'));                                    //127-139
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0140, 010, 0, "0", '0'));                                          //140-148
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0150, 001, 0, boleto.ValorDesconto != 0 ? "1" : "0", '0'));                 // 142 a 142 - Código do desconto 1
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 0151, 006, 0, boleto.DataProcessamento, ' '));                              //151-156
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0157, 001, 0, "0", '0'));                                                   //140-148
                switch (boleto.CodigoProtesto)
                {
                    case TipoCodigoProtesto.NaoProtestar:
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 001, 0, "3", '0')); // 221 a 221 - Código para protesto
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, "0", '0')); // 222 a 223 - Número de dias para protesto
                        break;
                    case TipoCodigoProtesto.ProtestarDiasCorridos:
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 001, 0, "1", '0')); // 221 a 221 - Código para protesto
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.DiasProtesto, '0')); // 222 a 223 - Número de dias para protesto
                        break;
                    case TipoCodigoProtesto.ProtestarDiasUteis:
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0158, 001, 0, "1", '0')); // 221 a 221 - Código para protesto
                        reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0159, 002, 0, boleto.DiasProtesto, '0')); // 222 a 223 - Número de dias para protesto
                        break;
                    default:
                        throw new Exception("Tipo de protesto inválido para Unicredi: " + boleto.CodigoProtesto);
                }
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0161, 013, 2, boleto.ValorJurosDia, '0')); // 127 a 141 - Juros de mora por dia/taxa
                if (boleto.ValorDesconto != 0)
                    reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediDataDDMMAA___________, 0174, 006, 0, boleto.DataDesconto, '0')); // 143 a 150 - Data do desconto 1
                else
                    reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0174, 006, 0, "0", '0')); // 143 a 150 - Data do desconto 1

                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0180, 013, 2, boleto.ValorDesconto, '0'));                                  //083-092
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0193, 011, 0, boleto.NossoNumero + boleto.NossoNumeroDV, '0'));                                     //048-056
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0204, 002, 0, "0", '0'));                                     //048-056
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0206, 013, 0, "0", '0'));                                     //048-056
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0219, 002, 0, boleto.Pagador.TipoCPFCNPJ("00"), '1'));  // 018 a 018 - Tipo de inscrição
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliDireita______, 0221, 014, 0, boleto.Pagador.CPFCNPJ.OnlyNumber(), '0')); // 019 a 033 - Número de inscrição
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0235, 040, 0, boleto.Pagador.Nome, ' ')); // 034 a 073 - Nome
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0275, 040, 0, boleto.Pagador.Endereco.FormataLogradouro(40), ' ')); // 074 a 113 - Endereço
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0315, 012, 0, boleto.Pagador.Endereco.Bairro, ' ')); // 114 a 128 - Bairro
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0327, 008, 0, boleto.Pagador.Endereco.CEP.OnlyNumber(), '0')); // 129 a 133 - CEP + 134 a 136 - Sufixo do CEP
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0335, 020, 0, boleto.Pagador.Endereco.Cidade, ' ')); // 137 a 151 - Cidade
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0355, 002, 0, boleto.Pagador.Endereco.UF, ' ')); // 152 a 153 - Unidade da Federação
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0357, 038, 0, boleto.Avalista.Nome, ' ')); // 170 a 209 - Nome do sacador/avalista
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistro, '0'));                                        //395-400

                reg.CodificarLinha();

                string _detalhe = Utils.SubstituiCaracteresEspeciais(reg.LinhaRegistro);

                return _detalhe;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao gerar DETALHE do arquivo CNAB400.", ex);
            }
        }









        public string GerarTrailerRemessaCNAB400(int numeroRegistroGeral, decimal valorBoletoGeral, int numeroRegistroCobrancaSimples, decimal valorCobrancaSimples, int numeroRegistroCobrancaVinculada, decimal valorCobrancaVinculada, int numeroRegistroCobrancaCaucionada, decimal valorCobrancaCaucionada, int numeroRegistroCobrancaDescontada, decimal valorCobrancaDescontada)
        {
            try
            {
                numeroRegistroGeral++;
                TRegistroEDI reg = new TRegistroEDI();
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0001, 001, 0, "9", ' '));                         //001-001
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediAlphaAliEsquerda_____, 0002, 393, 0, " ", ' '));                         //002-002
                reg.CamposEDI.Add(new TCampoRegistroEDI(TTiposDadoEDI.ediNumericoSemSeparador_, 0395, 006, 0, numeroRegistroGeral, '0'));         //395-400

                reg.CodificarLinha();

                string vLinha = reg.LinhaRegistro;
                string _trailer = Utils.SubstituiCaracteresEspeciais(vLinha);

                return _trailer;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro durante a geração do registro TRAILER do arquivo de REMESSA.", ex);
            }
        }

        
        private TipoEspecieDocumento AjustaEspecieCnab400(string codigoEspecie)
        {
            switch (codigoEspecie)
            {
                case "A":
                    return TipoEspecieDocumento.DMI;
                case "B":
                    return TipoEspecieDocumento.DR;
                case "C":
                    return TipoEspecieDocumento.NP;
                case "D":
                    return TipoEspecieDocumento.NPR;
                case "E":
                    return TipoEspecieDocumento.NS;
                case "G":
                    return TipoEspecieDocumento.RC;
                case "H":
                    return TipoEspecieDocumento.LC;
                case "I":
                    return TipoEspecieDocumento.ND;
                case "J":
                    return TipoEspecieDocumento.DSI;
                case "K":
                    return TipoEspecieDocumento.OU;
                case "O":
                    return TipoEspecieDocumento.BP;
                default:
                    return TipoEspecieDocumento.OU;
            }
        }

        private string AjustaEspecieCnab400(TipoEspecieDocumento especieDocumento)
        {
            switch (especieDocumento)
            {
                case TipoEspecieDocumento.DMI:
                    return "A";
                case TipoEspecieDocumento.DR:
                    return "B";
                case TipoEspecieDocumento.NP:
                    return "C";
                case TipoEspecieDocumento.NPR:
                    return "D";
                case TipoEspecieDocumento.NS:
                    return "E";
                case TipoEspecieDocumento.RC:
                    return "G";
                case TipoEspecieDocumento.LC:
                    return "H";
                case TipoEspecieDocumento.ND:
                    return "I";
                case TipoEspecieDocumento.DSI:
                    return "J";
                case TipoEspecieDocumento.OU:
                    return "K";
                case TipoEspecieDocumento.BP:
                    return "O";
                default:
                    return "K";
            }
        }

        public void LerDetalheRetornoCNAB400Segmento1(ref Boleto boleto, string registro)
        {
            try
            {
                // Identificação do Título no Banco
                boleto.NossoNumero = registro.Substring(47, 8);
                boleto.NossoNumeroDV = registro.Substring(55, 1);
                boleto.NossoNumeroFormatado = string.Format("{0}/{1}-{2}", boleto.NossoNumero.Substring(0, 2), boleto.NossoNumero.Substring(2, 6), boleto.NossoNumeroDV);

                // Identificação de Ocorrência
                boleto.CodigoMovimentoRetorno = registro.Substring(108, 2);
                boleto.DescricaoMovimentoRetorno = DescricaoOcorrenciaCnab400(boleto.CodigoMovimentoRetorno);

                // Data Ocorrência no Banco
                boleto.DataProcessamento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(110, 6)).ToString("##-##-##"));

                // Número do Documento
                boleto.NumeroDocumento = registro.Substring(116, 10).Trim();
                boleto.EspecieDocumento = AjustaEspecieCnab400(registro.Substring(174, 1));

                // Seu número - Seu número enviado na Remessa
                boleto.NumeroControleParticipante = registro.Substring(116, 10).Trim();

                //Data Vencimento do Título
                boleto.DataVencimento = Utils.ToDateTime(Utils.ToInt32(registro.Substring(146, 6)).ToString("##-##-##"));

                //Valores do Título
                boleto.ValorTitulo = Convert.ToDecimal(registro.Substring(152, 13)) / 100;
                boleto.ValorTarifas = Convert.ToDecimal(registro.Substring(175, 13)) / 100;
                boleto.ValorAbatimento = Convert.ToDecimal(registro.Substring(227, 13)) / 100;
                boleto.ValorDesconto = Convert.ToDecimal(registro.Substring(240, 13)) / 100;
                boleto.ValorPago = Convert.ToDecimal(registro.Substring(253, 13)) / 100;
                boleto.ValorJurosDia = Convert.ToDecimal(registro.Substring(266, 13)) / 100;
                boleto.ValorOutrosCreditos = Convert.ToDecimal(registro.Substring(279, 13)) / 100;

                boleto.ValorPago += boleto.ValorJurosDia;

                // Data do Crédito
                boleto.DataCredito = Utils.ToDateTime(Utils.ToInt32(registro.Substring(328, 8)).ToString("####-##-##"));

                // Identificação de Ocorrência - Código Auxiliar
                boleto.CodigoMotivoOcorrencia = registro.Substring(318, 10);
                boleto.ListMotivosOcorrencia = Cnab.MotivoOcorrenciaCnab240(boleto.CodigoMotivoOcorrencia, boleto.CodigoMovimentoRetorno);

                // Registro Retorno
                boleto.RegistroArquivoRetorno = boleto.RegistroArquivoRetorno + registro + Environment.NewLine;
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler detalhe do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        private string DescricaoOcorrenciaCnab400(string codigo)
        {
            switch (codigo)
            {
                case "02":
                    return "Entrada confirmada";
                case "03":
                    return "Entrada rejeitada";
                case "06":
                    return "Liquidação normal";
                case "07":
                    return "Intenção de pagamento";
                case "09":
                    return "Baixado automaticamente via arquivo";
                case "10":
                    return "Baixado conforme instruções da cooperativa";
                case "12":
                    return "Abatimento concedido";
                case "13":
                    return "Abatimento cancelado";
                case "14":
                    return "Vencimento alterado";
                case "15":
                    return "Liquidação em cartório";
                case "17":
                    return "Liquidação após baixa";
                case "19":
                    return "Confirmação de recebimento de instrução de protesto";
                case "20":
                    return "Confirmação de recebimento de instrução de sustação de protesto";
                case "23":
                    return "Entrada de título em cartório";
                case "24":
                    return "Entrada rejeitada por CEP irregular";
                case "27":
                    return "Baixa rejeitada";
                case "28":
                    return "Tarifa";
                case "29":
                    return "Rejeição do pagador";
                case "30":
                    return "Alteração rejeitada";
                case "32":
                    return "Instrução rejeitada";
                case "33":
                    return "Confirmação de pedido de alteração de outros dados";
                case "34":
                    return "Retirado de cartório e manutenção em carteira";
                case "35":
                    return "Aceite do pagador";
                case "78":
                    return "Confirmação de recebimento de pedido de negativação";
                case "79":
                    return "Confirmação de recebimento de pedido de exclusão de negativação";
                case "80":
                    return "Confirmação de entrada de negativação";
                case "81":
                    return "Entrada de negativação rejeitada";
                case "82":
                    return "Confirmação de exclusão de negativação";
                case "83":
                    return "Exclusão de negativação rejeitada";
                case "84":
                    return "Exclusão de negativação por outros motivos";
                case "85":
                    return "Ocorrência informacional por outros motivos";
                default:
                    return "";
            }
        }

        public void LerDetalheRetornoCNAB400Segmento7(ref Boleto boleto, string registro)
        {
            throw new NotImplementedException();
        }


        public override void LerHeaderRetornoCNAB400(string registro)
        {
            try
            {
                if (registro.Substring(0, 9) != "02RETORNO")
                    throw new Exception("O arquivo não é do tipo \"02RETORNO\"");

                this.Beneficiario = new Beneficiario();
                this.Beneficiario.Codigo = registro.Substring(26, 5);
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao ler HEADER do arquivo de RETORNO / CNAB 400.", ex);
            }
        }

        public void LerTrailerRetornoCNAB400(string registro)
        {

        }
    }

}