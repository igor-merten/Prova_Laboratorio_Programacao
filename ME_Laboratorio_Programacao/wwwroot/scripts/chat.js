let sessaoAtualId = null;

document.addEventListener("DOMContentLoaded", async () => {
    await carregarOpcoes();
    await carregarSessoes();
});

async function carregarOpcoes() {
    try {
        // Carrega agentes
        const resAgentes = await fetch(`${API_URL}/agentes`, fetchOptions('GET'));
        if (resAgentes.ok) {
            const agentes = await resAgentes.json();
            const selectAgente = document.getElementById('chat-agente');
            agentes.filter(a => a.ativo).forEach(a => {
                selectAgente.innerHTML += `<option value="${a.id}">${a.nome} (${a.categoriaAgente.nome})</option>`;
            });
        }

        // Carrega canais
        const resCanais = await fetch(`${API_URL}/canais`, fetchOptions('GET'));
        if (resCanais.ok) {
            const canais = await resCanais.json();
            const selectCanal = document.getElementById('chat-canal');
            canais.filter(c => c.ativo).forEach(c => {
                selectCanal.innerHTML += `<option value="${c.id}">${c.nome}</option>`;
            });
        }
    } catch (e) {
        console.error(e);
    }
}

async function carregarSessoes() {
    try {
        const res = await fetch(`${API_URL}/chat/sessoes`, fetchOptions('GET'));
        if (!res.ok) return;

        const sessoes = await res.json();
        const lista = document.getElementById('lista-sessoes');
        lista.innerHTML = '';

        sessoes.forEach(s => {
            const div = document.createElement('div');
            div.className = 'sessao-item';
            div.id = `sessao-${s.id}`;
            div.innerHTML = `<div style="display:flex; justify-content:space-between; align-items:center;">
                                <strong><i class="fa-solid fa-comments" style="color:#94a3b8; margin-right:5px;"></i> Sessão #${s.id}</strong>
                                <span style="font-size:11px; color:#94a3b8;">${new Date(s.dataInicio).toLocaleDateString()}</span>
                             </div>
                             <small style="color:#64748b; display:block; margin-top:3px;">${s.agenteNome} (${s.canalNome})</small>`;
            div.onclick = () => abrirSessao(s.id, s.agenteNome, s.canalNome);
            lista.appendChild(div);
        });
    } catch (e) {
        console.error(e);
    }
}

async function iniciarSessao() {
    const agenteId = document.getElementById('chat-agente').value;
    const canalOrigemId = document.getElementById('chat-canal').value;

    if (!agenteId || !canalOrigemId) {
        alert('Selecione Agente e Canal.');
        return;
    }

    try {
        const res = await fetch(`${API_URL}/chat/iniciar`, fetchOptions('POST', { agenteId, canalOrigemId }));
        if (res.ok) {
            const data = await res.json();
            await carregarSessoes();
            const agenteSelect = document.getElementById('chat-agente');
            const canalSelect = document.getElementById('chat-canal');
            const nomeAgente = agenteSelect.options[agenteSelect.selectedIndex].text;
            const nomeCanal = canalSelect.options[canalSelect.selectedIndex].text;
            abrirSessao(data.sessaoId, nomeAgente, nomeCanal);
        }
    } catch (e) {
        console.error(e);
    }
}

async function abrirSessao(id, nomeAgente, nomeCanal) {
    sessaoAtualId = id;
    
    // Atualiza o CSS de ativo na sidebar
    document.querySelectorAll('.sessao-item').forEach(el => el.classList.remove('active'));
    const item = document.getElementById(`sessao-${id}`);
    if(item) item.classList.add('active');

    document.getElementById('header-text').innerText = `Sessão #${id} - ${nomeAgente} (${nomeCanal})`;
    document.getElementById('status-dot').style.background = '#22c55e'; // Verde para online
    document.getElementById('btn-deletar-sessao').classList.remove('hidden');
    document.getElementById('chat-input').disabled = false;
    document.getElementById('chat-send-btn').disabled = false;
    document.getElementById('chat-input').focus();
    
    await carregarMensagens();
}

async function carregarMensagens() {
    if (!sessaoAtualId) return;
    
    try {
        const res = await fetch(`${API_URL}/chat/${sessaoAtualId}/mensagens`, fetchOptions('GET'));
        if (res.ok) {
            const msgs = await res.json();
            const container = document.getElementById('chat-messages');
            container.innerHTML = '';

            msgs.forEach(m => adicionarBolhaMensagem(m.conteudo, m.remetente.toLowerCase() === 'usuario'));
        }
    } catch (e) {
        console.error(e);
    }
}

function adicionarBolhaMensagem(texto, isUsuario) {
    const container = document.getElementById('chat-messages');
    const div = document.createElement('div');
    
    div.className = `chat-bubble ${isUsuario ? 'user' : 'agent'}`;
    div.innerText = texto;
    
    container.appendChild(div);
    container.scrollTop = container.scrollHeight; // rola para baixo
}

async function enviarMensagem() {
    const input = document.getElementById('chat-input');
    const texto = input.value.trim();
    if (!texto || !sessaoAtualId) return;

    // Adiciona na tela instantaneamente
    adicionarBolhaMensagem(texto, true);
    input.value = '';

    try {
        const res = await fetch(`${API_URL}/chat/enviar`, fetchOptions('POST', {
            sessaoId: sessaoAtualId,
            conteudo: texto
        }));

        if (res.ok) {
            const data = await res.json();
            adicionarBolhaMensagem(data.mensagem, false);
        } else {
            alert('Erro ao enviar mensagem');
        }
    } catch (e) {
        console.error(e);
    }
}

async function deletarSessao() {
    if(!sessaoAtualId) return;
    if(!confirm("Tem certeza que deseja apagar esta conversa e limpar a memória do Agente?")) return;
    
    try {
        const res = await fetch(`${API_URL}/chat/${sessaoAtualId}`, fetchOptions('DELETE'));
        if(res.ok) {
            sessaoAtualId = null;
            document.getElementById('chat-messages').innerHTML = '';
            document.getElementById('header-text').innerText = 'Selecione ou inicie uma sessão de chat';
            document.getElementById('status-dot').style.background = '#94a3b8';
            document.getElementById('btn-deletar-sessao').classList.add('hidden');
            document.getElementById('chat-input').disabled = true;
            document.getElementById('chat-send-btn').disabled = true;
            await carregarSessoes();
        } else {
            alert('Erro ao deletar sessão.');
        }
    } catch(e){
        console.error(e);
    }
}
