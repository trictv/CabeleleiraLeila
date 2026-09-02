# App Cabeleleira Leila - Projeto DSIN

Aplicativo de agendamento de serviços da Cabeleleira Leila desenvolvido com a estrutura **.NET MAUI**. 
Feito para atender as necessidades e requisitos para um desafio técnico de vaga de trabalho.

---

## Funcionalidades do Aplicativo

O sistema foi estruturado em duas experiências principais para atender tanto quem agenda quanto quem gerencia o salão:

**Para os Clientes:**
* Agendamento rápido de um ou múltiplos serviços de uma só vez.
* Sistema inteligente de sugestão de agrupamento (avisa se o cliente já tem agendamentos na mesma semana para unificar a visita).
* Consulta ao histórico de agendamentos passados com filtros por data.
* Remarcação de horários e cancelamento diretamente pelo aplicativo (com trava de segurança que exige contato telefônico caso falte menos de 2 dias).
* Recuperação de senha nativa utilizando token de segurança via e-mail.

**Para o Administrador (Painel da Leila):**
* Dashboard gerencial com relatórios de desempenho semanal (faturamento, total de atendimentos e top serviços).
* Controle total da agenda, podendo atualizar o status dos serviços (Pendente, Confirmado, Concluído, Não Compareceu, Cancelado).
* Ferramenta de unificação de agendamentos dispersos de um mesmo cliente na semana.
* Permissão de "super usuário" para realizar encaixes e forçar remarcações em horários ocupados ou com menos de 2 dias de antecedência.
* Gestão completa (Adicionar, Editar, Excluir) da lista de clientes, categorias e serviços oferecidos.
* Configuração dinâmica do expediente do salão (dias da semana e janelas de horário de funcionamento).

---

## Pré-requisitos para rodar o projeto na sua máquina

Para abrir, compilar e rodar este projeto, você precisará ter o seguinte ambiente configurado:

* **SDK do .NET 10** instalado em sua máquina.
* **Visual Studio 2026** (Certifique-se de marcar o *workload* de **Desenvolvimento com .NET MAUI** durante a instalação).

---

## Como rodar o projeto

1. Clone ou baixe este repositório para a sua máquina local.
2. Procure o arquivo `.sln` e abra com o Visual Studio, ou pelo próprio Visual Studio procure o projeto e abra-o (o arquivo `.sln` fica na raiz, dentro da primeira pasta).

---

## Acesso Administrativo

O aplicativo possui uma divisão de acesso. Para avaliar as funções gerenciais do painel, utilize as seguintes credenciais na aba "Administrador" da tela de Login:

* **E-mail:** carreira@dsin.com.br
* **Senha:** admin123

---

## Documentação e Apresentação DSIN

A explicação detalhada do desenvolvimento do projeto, bem como os materiais complementares para a avaliação da DSIN, estão disponíveis no Google Drive.

Acesse o link abaixo para conferir a explicação das decisões técnicas e da arquitetura do projeto:
https://drive.google.com/drive/folders/1Pn5AptsunT5hTpPVzOB96uIXzigGvwbp?usp=sharing

Desenvolvido para o desafio técnico DSIN.
