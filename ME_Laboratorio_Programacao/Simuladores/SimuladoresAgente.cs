namespace ME_Laboratorio_Programacao.Simuladores
{
    public abstract class AgenteSimulador{
        public abstract string GerarResposta(string mensagemUsuario);
    }

    public class SimuladorVendas : AgenteSimulador{
        public override string GerarResposta(string mensagemUsuario){
            return $"[Vendas] Estou analisando seu interesse. Posso oferecer uma negociação especial se fecharmos agora. Qual a sua proposta?";
        }
    }

    public class SimuladorSuporte : AgenteSimulador{
        public override string GerarResposta(string mensagemUsuario){
            return $"[Suporte] Entendi a situação. Já abri um chamado interno. Por favor, reinicie o sistema ou limpe o cache do navegador. Posso ajudar em algo mais?";
        }
    }
    
    public class SimuladorRH : AgenteSimulador{
        public override string GerarResposta(string mensagemUsuario){
            return $"[RH] Nós recebemos sua mensagem. Nossas vagas ficam disponíveis na aba de Trabalhe Conosco. Deseja registrar alguma ocorrência interna?";
        }
    }

    public class SimuladorPadrao : AgenteSimulador{
        public override string GerarResposta(string mensagemUsuario){
            return $"Sou um agente de atendimento em treinamento. Recebi sua mensagem: '{mensagemUsuario}'. Em breve um humano assumirá o chat.";
        }
    }
}
