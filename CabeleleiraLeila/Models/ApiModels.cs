using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace CabeleleiraLeila.Models;

public class Servico
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("categoria_id")] public int CategoriaId { get; set; }
    [JsonPropertyName("nome")] public string Nome { get; set; }
    [JsonPropertyName("duracao_minutos")] public int DuracaoMinutos { get; set; }
    [JsonPropertyName("preco")] public decimal Preco { get; set; }
    [JsonPropertyName("ativo")] public int Ativo { get; set; }
    [JsonPropertyName("categoria_nome")] public string CategoriaNome { get; set; }

    [JsonIgnore]
    public bool IsAtivo
    {
        get => Ativo == 1;
        set => Ativo = value ? 1 : 0;
    }
}

public partial class ServicoSelecionavel : ObservableObject
{
    public Servico Servico { get; set; }
    [ObservableProperty] private bool isSelected;
}

public class CategoriaGrupo
{
    public string NomeCategoria { get; set; }
    public ObservableCollection<ServicoSelecionavel> Servicos { get; set; } = new();
}

public class AgendamentoRequest
{
    [JsonPropertyName("nome")] public string Nome { get; set; }
    [JsonPropertyName("telefone")] public string Telefone { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("observacoes")] public string Observacao { get; set; }
    [JsonPropertyName("data_agendamento")] public string DataAgendamento { get; set; }
    [JsonPropertyName("servicos")] public List<ServicoRequestItem> Servicos { get; set; } = new();
}

public class ServicoRequestItem
{
    [JsonPropertyName("servico_id")] public int ServicoId { get; set; }
    [JsonPropertyName("hora_inicio")] public string HoraInicio { get; set; }
    [JsonPropertyName("hora_fim")] public string HoraFim { get; set; }
}

public class AgendamentoResponse
{
    [JsonPropertyName("message")] public string Message { get; set; }
    [JsonPropertyName("agendamento_id")] public int? AgendamentoId { get; set; }
    [JsonPropertyName("token")] public string Token { get; set; }
    [JsonPropertyName("sugestao")] public string Sugestao { get; set; }
    [JsonPropertyName("error")] public string Error { get; set; }
}

public class DiaDisponivel
{
    [JsonPropertyName("data")] public string Data { get; set; }
    [JsonPropertyName("dia_semana")] public int DiaSemana { get; set; }
    [JsonPropertyName("slots_livres")] public List<string> SlotsLivres { get; set; } = new();
    [JsonPropertyName("horarios_ocupados")] public List<HorarioOcupado> HorariosOcupados { get; set; } = new();
    [JsonPropertyName("funcionamento")] public FuncionamentoDia Funcionamento { get; set; }

    [JsonIgnore]
    public string DataFormatada => DateTime.TryParse(Data, out var d) ? d.ToString("dd/MM/yyyy") : Data;
}

public class HorarioOcupado
{
    [JsonPropertyName("hora_inicio")] public string HoraInicio { get; set; }
    [JsonPropertyName("hora_fim")] public string HoraFim { get; set; }
}

public class FuncionamentoDia
{
    [JsonPropertyName("manha")] public PeriodoFuncionamento Manha { get; set; }
    [JsonPropertyName("tarde")] public PeriodoFuncionamento Tarde { get; set; }
}

public class PeriodoFuncionamento
{
    [JsonPropertyName("inicio")] public string Inicio { get; set; }
    [JsonPropertyName("fim")] public string Fim { get; set; }
}


public class LoginRequest
{
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("senha")] public string Senha { get; set; }
}

public class LoginResponse
{
    [JsonPropertyName("token")] public string Token { get; set; }
    [JsonPropertyName("user")] public Usuario User { get; set; }
    [JsonPropertyName("error")] public string Error { get; set; }
}

public class Usuario
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("nome")] public string Nome { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("telefone")] public string Telefone { get; set; }
    [JsonPropertyName("tipo")] public string Tipo { get; set; }
}

public class CategoriaServico
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("nome")] public string Nome { get; set; }
    [JsonPropertyName("ativo")] public int Ativo { get; set; }

    [JsonIgnore]
    public bool IsAtivo
    {
        get => Ativo == 1;
        set => Ativo = value ? 1 : 0;
    }
    public override string ToString() => Nome;
}

public class ClienteAdmin
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("usuario_id")] public int UsuarioId { get; set; }
    [JsonPropertyName("nome")] public string Nome { get; set; }
    [JsonPropertyName("email")] public string Email { get; set; }
    [JsonPropertyName("telefone")] public string Telefone { get; set; }
    [JsonPropertyName("cpf")] public string Cpf { get; set; }
    [JsonPropertyName("data_nascimento")] public string? DataNascimento { get; set; }
    [JsonPropertyName("observacoes")] public string Observacoes { get; set; }
    [JsonPropertyName("ativo")] public int Ativo { get; set; }

    [JsonIgnore]
    public DateTime? DataNascimentoObj
    {
        get => DateTime.TryParse(DataNascimento, out var d) ? d : null;
        set => DataNascimento = value?.ToString("yyyy-MM-dd");
    }
    [JsonIgnore]
    public bool IsAtivo
    {
        get => Ativo == 1;
        set => Ativo = value ? 1 : 0;
    }
}

public class AgendamentoHistorico
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("data_agendamento")] public string DataAgendamento { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; }
    [JsonPropertyName("valor_total")] public decimal ValorTotal { get; set; }
    [JsonPropertyName("itens")] public List<AgendamentoItemCliente> Itens { get; set; } = new();

    [JsonIgnore]
    public string DataFormatada => DateTime.TryParse(DataAgendamento, out var d) ? d.ToString("dd/MM/yyyy") : DataAgendamento;

    [JsonIgnore]
    public string DataFormatadaCurta => DateTime.TryParse(DataAgendamento, out var d) ? d.ToString("dd/MM") : DataAgendamento;

    [JsonIgnore]
    public string HoraInicioGeral => Itens != null && Itens.Any() ? Itens.Min(i => i.HoraInicio).Substring(0, 5) : "--:--";

    [JsonIgnore]
    public bool PodeAlterar
    {
        get
        {
            if (DateTime.TryParse(DataAgendamento, out var dataAgendada))
            {
                var diferenca = (dataAgendada.Date - DateTime.Today).TotalDays;
                return diferenca >= 2;
            }
            return false;
        }
    }
}

public class AgendamentoItemCliente
{
    [JsonPropertyName("servico_id")] public int ServicoId { get; set; }
    [JsonPropertyName("servico_nome")] public string ServicoNome { get; set; }
    [JsonPropertyName("hora_inicio")] public string HoraInicio { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; }
    [JsonPropertyName("valor")] public decimal Valor { get; set; }
}

public class HorarioFuncionamento
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("dia_semana")] public int DiaSemana { get; set; }
    [JsonPropertyName("hora_abertura_manha")] public string HoraAberturaManha { get; set; }
    [JsonPropertyName("hora_fechamento_manha")] public string HoraFechamentoManha { get; set; }
    [JsonPropertyName("hora_abertura_tarde")] public string HoraAberturaTarde { get; set; }
    [JsonPropertyName("hora_fechamento_tarde")] public string HoraFechamentoTarde { get; set; }
    [JsonPropertyName("fechado")] public int Fechado { get; set; }

    [JsonIgnore]
    public bool IsAberto
    {
        get => Fechado == 0;
        set => Fechado = value ? 0 : 1;
    }

    [JsonIgnore]
    public string NomeDiaSemana => DiaSemana switch
    {
        0 => "Domingo",
        1 => "Segunda-feira",
        2 => "Terça-feira",
        3 => "Quarta-feira",
        4 => "Quinta-feira",
        5 => "Sexta-feira",
        6 => "Sábado",
        _ => "Desconhecido"
    };

    [JsonIgnore]
    public string ResumoHorarios => !IsAberto
        ? "Fechado"
        : $"{AberturaManhaTime:hh\\:mm} às {FechamentoManhaTime:hh\\:mm} e {AberturaTardeTime:hh\\:mm} às {FechamentoTardeTime:hh\\:mm}";

    [JsonIgnore]
    public TimeSpan AberturaManhaTime
    {
        get => TimeSpan.TryParse(HoraAberturaManha, out var t) ? t : TimeSpan.Zero;
        set => HoraAberturaManha = value.ToString(@"hh\:mm\:ss");
    }
    [JsonIgnore]
    public TimeSpan FechamentoManhaTime
    {
        get => TimeSpan.TryParse(HoraFechamentoManha, out var t) ? t : TimeSpan.Zero;
        set => HoraFechamentoManha = value.ToString(@"hh\:mm\:ss");
    }
    [JsonIgnore]
    public TimeSpan AberturaTardeTime
    {
        get => TimeSpan.TryParse(HoraAberturaTarde, out var t) ? t : TimeSpan.Zero;
        set => HoraAberturaTarde = value.ToString(@"hh\:mm\:ss");
    }
    [JsonIgnore]
    public TimeSpan FechamentoTardeTime
    {
        get => TimeSpan.TryParse(HoraFechamentoTarde, out var t) ? t : TimeSpan.Zero;
        set => HoraFechamentoTarde = value.ToString(@"hh\:mm\:ss");
    }
}

public class AgendamentoAdmin
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("cliente_id")] public int ClienteId { get; set; }
    [JsonPropertyName("cliente_nome")] public string ClienteNome { get; set; }
    [JsonPropertyName("cliente_telefone")] public string ClienteTelefone { get; set; }
    [JsonPropertyName("data_agendamento")] public string DataAgendamento { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; }
    [JsonPropertyName("valor_total")] public decimal ValorTotal { get; set; }
    [JsonPropertyName("observacoes")] public string Observacoes { get; set; }
    [JsonPropertyName("itens")] public List<AgendamentoItemAdmin> Itens { get; set; } = new();

    [JsonIgnore]
    public string DataFormatada => DateTime.TryParse(DataAgendamento, out var d) ? d.ToString("dd/MM/yyyy") : DataAgendamento;
    [JsonIgnore]
    public string HoraInicioGeral => Itens.Any() ? Itens.Min(i => i.HoraInicio).Substring(0, 5) : "--:--";
    [JsonIgnore]
    public string ResumoServicos => string.Join(", ", Itens.Select(i => i.ServicoNome));
    [JsonIgnore]
    public Color StatusColor => Status switch
    {
        "PENDENTE" => Color.FromArgb("#F59E0B"),
        "CONFIRMADO" => Color.FromArgb("#3B82F6"),
        "CONCLUIDO" => Color.FromArgb("#10B981"),
        "CANCELADO" => Color.FromArgb("#EF4444"),
        "NAO_COMPARECEU" => Color.FromArgb("#6B7280"),
        _ => Colors.Black
    };
}

public class AgendamentoItemAdmin
{
    [JsonPropertyName("id")] public int Id { get; set; }
    [JsonPropertyName("servico_nome")] public string ServicoNome { get; set; }
    [JsonPropertyName("hora_inicio")] public string HoraInicio { get; set; }
    [JsonPropertyName("hora_fim")] public string HoraFim { get; set; }
    [JsonPropertyName("valor")] public decimal Valor { get; set; }
    [JsonPropertyName("status")] public string Status { get; set; }
}

public class RelatorioDesempenho
{
    [JsonPropertyName("faturamento")] public decimal Faturamento { get; set; }
    [JsonPropertyName("qtd_atendimentos")] public int QtdAtendimentos { get; set; }
    [JsonPropertyName("servicos_mais_buscados")] public List<ServicoBuscado> ServicosMaisBuscados { get; set; } = new();
}

public class ServicoBuscado
{
    [JsonPropertyName("nome")] public string Nome { get; set; }
    [JsonPropertyName("qtd")] public int Qtd { get; set; }
}