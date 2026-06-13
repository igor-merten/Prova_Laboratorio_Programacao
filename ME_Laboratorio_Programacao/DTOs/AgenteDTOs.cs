namespace ME_Laboratorio_Programacao.DTOs;


public record AgenteRequest(string Nome, string Descricao, int CategoriaAgenteId, bool Ativo, string TipoAgente);
