namespace ME_Laboratorio_Programacao.Models.Agentes;

public class SuperAgente : AgenteBase
{
    public required override string Nome
    {
        get => "Super " + base.Nome;
        set => base.Nome = value;
    }
}