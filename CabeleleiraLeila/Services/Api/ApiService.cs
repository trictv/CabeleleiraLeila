using CabeleleiraLeila.Models;
using System.Net.Http.Json;

namespace CabeleleiraLeila.Services.Api;

public interface IApiService
{
    Task<List<Servico>> GetServicosAsync();
    Task<AgendamentoResponse> CriarAgendamentoExpressoAsync(AgendamentoRequest request);
    Task<List<DiaDisponivel>> GetDisponibilidadeAsync(int dias = 7);
    Task<LoginResponse> LoginAsync(string email, string senha);
    Task<List<CategoriaServico>> GetCategoriasAsync();
    Task<bool> SalvarCategoriaAsync(CategoriaServico categoria);
    Task<bool> ExcluirCategoriaAsync(int id);
    Task<List<Servico>> GetAdminServicosAsync();
    Task<bool> SalvarServicoAsync(Servico servico);
    Task<bool> ExcluirServicoAsync(int id);
    Task<List<ClienteAdmin>> GetAdminClientesAsync();
    Task<bool> SalvarAdminClienteAsync(ClienteAdmin cliente);
    Task<bool> ExcluirAdminClienteAsync(int id);
    Task<List<AgendamentoHistorico>> GetHistoricoClienteAsync(int clienteId);
    Task<List<AgendamentoHistorico>> GetHistoricoClientePeriodoAsync(int clienteId, DateTime inicio, DateTime fim);
    Task<bool> CancelarAgendamentoClienteAsync(int id);
    Task<bool> AlterarAgendamentoClienteAsync(int id, string novaData);

    Task<List<HorarioFuncionamento>> GetExpedienteAsync();
    Task<bool> AtualizarExpedienteAsync(HorarioFuncionamento horario);
    Task<List<AgendamentoAdmin>> GetAdminAgendamentosAsync();
    Task<bool> AtualizarStatusAgendamentoAsync(int id, string novoStatus);
    Task<bool> DeletarAgendamentoAsync(int id);
    Task<RelatorioDesempenho> GetRelatorioDesempenhoAsync(DateTime dataInicial, DateTime dataFinal);
    Task<bool> AlterarAgendamentoClienteAsync(int id, AgendamentoRequest request);

    Task<bool> EsqueciSenhaAsync(string email);
    Task<bool> ResetarSenhaAsync(string token, string novaSenha);
}

public class ApiService : IApiService
{
    private readonly HttpClient _httpClient;

    public ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    private async Task ConfigurarAutenticacao()
    {
        var token = await SecureStorage.Default.GetAsync("auth_token");
        if (!string.IsNullOrEmpty(token))
        {
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }
    }

    public async Task<List<Servico>> GetServicosAsync()
    {
        try
        {
            var servicos = await _httpClient.GetFromJsonAsync<List<Servico>>("servicos");
            return servicos ?? new List<Servico>();
        }
        catch
        {
            return new List<Servico>();
        }
    }

    public async Task<List<DiaDisponivel>> GetDisponibilidadeAsync(int dias = 7)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<DiaDisponivel>>($"disponibilidade?dias={dias}");
            return response ?? new List<DiaDisponivel>();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"FALHA AO BUSCAR DISPONIBILIDADE: {ex.Message}");
            return new List<DiaDisponivel>();
        }
    }

    public async Task<AgendamentoResponse> CriarAgendamentoExpressoAsync(AgendamentoRequest request)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.PostAsJsonAsync("agendamentos", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AgendamentoResponse>() ?? throw new InvalidOperationException("A API não retornou os dados do agendamento."); ;
            }

            var erro = await response.Content.ReadFromJsonAsync<AgendamentoResponse>();
            return erro ?? new AgendamentoResponse { Error = "Erro ao processar agendamento." };
        }
        catch (Exception ex)
        {
            return new AgendamentoResponse { Error = $"FALHA NA REQUISIÇÃO: {ex.Message}" };
        }
    }

    public async Task<bool> CancelarAgendamentoClienteAsync(int id)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.DeleteAsync($"agendamentos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> AlterarAgendamentoClienteAsync(int id, string novaData)
    {
        await ConfigurarAutenticacao();
        try
        {
            var payload = new { data_agendamento = novaData };
            var response = await _httpClient.PutAsJsonAsync($"agendamentos/{id}", payload);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<AgendamentoHistorico>> GetHistoricoClientePeriodoAsync(int clienteId, DateTime inicio, DateTime fim)
    {
        await ConfigurarAutenticacao();
        try
        {
            string start = inicio.ToString("yyyy-MM-dd");
            string end = fim.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetFromJsonAsync<List<AgendamentoHistorico>>($"clientes/{clienteId}/historico?data_inicio={start}&data_fim={end}");
            return response ?? new List<AgendamentoHistorico>();
        }
        catch
        {
            return new List<AgendamentoHistorico>();
        }
    }

    public async Task<LoginResponse> LoginAsync(string email, string senha)
    {
        try
        {
            var request = new LoginRequest { Email = email, Senha = senha };
            var response = await _httpClient.PostAsJsonAsync("auth/login", request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<LoginResponse>() ?? throw new InvalidOperationException("A API não retornou os dados de login."); ;
            }
            else
            {
                var erroResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                return erroResponse ?? new LoginResponse { Error = "Erro ao tentar fazer login." };
            }
        }
        catch
        {
            return new LoginResponse { Error = "Falha de comunicação com o servidor." };
        }
    }

    public async Task<List<CategoriaServico>> GetCategoriasAsync()
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<CategoriaServico>>("admin/categorias_servicos");
            return response ?? new List<CategoriaServico>();
        }
        catch { return new List<CategoriaServico>(); }
    }

    public async Task<bool> SalvarCategoriaAsync(CategoriaServico categoria)
    {
        await ConfigurarAutenticacao();
        try
        {
            HttpResponseMessage response;
            if (categoria.Id == 0)
                response = await _httpClient.PostAsJsonAsync("admin/categorias_servicos", categoria);
            else
                response = await _httpClient.PutAsJsonAsync($"admin/categorias_servicos/{categoria.Id}", categoria);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ExcluirCategoriaAsync(int id)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.DeleteAsync($"admin/categorias_servicos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<Servico>> GetAdminServicosAsync()
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Servico>>("admin/servicos");
            return response ?? new List<Servico>();
        }
        catch { return new List<Servico>(); }
    }

    public async Task<bool> SalvarServicoAsync(Servico servico)
    {
        await ConfigurarAutenticacao();
        try
        {
            HttpResponseMessage response;
            if (servico.Id == 0)
                response = await _httpClient.PostAsJsonAsync("admin/servicos", servico);
            else
                response = await _httpClient.PutAsJsonAsync($"admin/servicos/{servico.Id}", servico);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ExcluirServicoAsync(int id)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.DeleteAsync($"admin/servicos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<ClienteAdmin>> GetAdminClientesAsync()
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<ClienteAdmin>>("admin/clientes-completos");
            return response ?? new List<ClienteAdmin>();
        }
        catch { return new List<ClienteAdmin>(); }
    }

    public async Task<bool> SalvarAdminClienteAsync(ClienteAdmin cliente)
    {
        await ConfigurarAutenticacao();
        try
        {
            HttpResponseMessage response;
            if (cliente.Id == 0)
                response = await _httpClient.PostAsJsonAsync("admin/clientes-completos", cliente);
            else
                response = await _httpClient.PutAsJsonAsync($"admin/clientes-completos/{cliente.Id}", cliente);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ExcluirAdminClienteAsync(int id)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.DeleteAsync($"admin/clientes-completos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<AgendamentoHistorico>> GetHistoricoClienteAsync(int clienteId)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AgendamentoHistorico>>($"clientes/{clienteId}/historico");
            return response ?? new List<AgendamentoHistorico>();
        }
        catch { return new List<AgendamentoHistorico>(); }
    }

    public async Task<List<HorarioFuncionamento>> GetExpedienteAsync()
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<HorarioFuncionamento>>("admin/horarios_funcionamento");
            return response?.OrderBy(h => h.DiaSemana).ToList() ?? new List<HorarioFuncionamento>();
        }
        catch { return new List<HorarioFuncionamento>(); }
    }

    public async Task<bool> AtualizarExpedienteAsync(HorarioFuncionamento horario)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"admin/horarios_funcionamento/{horario.Id}", horario);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<List<AgendamentoAdmin>> GetAdminAgendamentosAsync()
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<AgendamentoAdmin>>("admin/agendamentos");
            return response ?? new List<AgendamentoAdmin>();
        }
        catch { return new List<AgendamentoAdmin>(); }
    }

    public async Task<bool> AtualizarStatusAgendamentoAsync(int id, string novoStatus)
    {
        await ConfigurarAutenticacao();
        try
        {
            var payload = new { status = novoStatus };
            var response = await _httpClient.PutAsJsonAsync($"admin/agendamentos/{id}/status", payload);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> DeletarAgendamentoAsync(int id)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.DeleteAsync($"admin/agendamentos/{id}");
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<RelatorioDesempenho> GetRelatorioDesempenhoAsync(DateTime dataInicial, DateTime dataFinal)
    {
        await ConfigurarAutenticacao();
        try
        {
            string inicio = dataInicial.ToString("yyyy-MM-dd");
            string fim = dataFinal.ToString("yyyy-MM-dd");
            var response = await _httpClient.GetFromJsonAsync<RelatorioDesempenho>($"admin/relatorios/desempenho-semanal?data_inicial={inicio}&data_final={fim}");
            return response ?? new RelatorioDesempenho();
        }
        catch { return new RelatorioDesempenho(); }
    }

    public async Task<bool> AlterarAgendamentoClienteAsync(int id, AgendamentoRequest request)
    {
        await ConfigurarAutenticacao();
        try
        {
            var response = await _httpClient.PutAsJsonAsync($"agendamentos/{id}", request);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> EsqueciSenhaAsync(string email)
    {
        try
        {
            var payload = new { email = email };
            var response = await _httpClient.PostAsJsonAsync("auth/esqueci-senha", payload);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }

    public async Task<bool> ResetarSenhaAsync(string token, string novaSenha)
    {
        try
        {
            var payload = new { token = token, novaSenha = novaSenha };
            var response = await _httpClient.PostAsJsonAsync("auth/resetar-senha", payload);
            return response.IsSuccessStatusCode;
        }
        catch { return false; }
    }
}