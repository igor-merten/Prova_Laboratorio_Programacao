const canaisTableBody = document.querySelector('#agentes-table tbody');
const userForm = document.getElementById('user-form');

document.addEventListener("DOMContentLoaded", () => {
    carregarAgentes();

});

async function carregarAgentes() {
    try {
        const response = await fetch(`${API_URL}/agentes`, fetchOptions('GET'));
        
        if (response.ativo === 401 || response.ativo === 403) {
            alert('Sessão expirada ou acesso não autorizado.');
            efetuarLogoutLocal();
            return;
        }

        const agentes = await response.json();
        canaisTableBody.innerHTML = '';

        agentes.forEach(a => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${a.id}</td>
                <td>${a.nome}</td>
                <td style="color: ${a.categoriaAgente.corHex}">${a.categoriaAgente.nome}</td>
                <td>
                    <span style="
                        display: inline-block; 
                        width: 12px; 
                        height: 12px; 
                        border-radius: 50%; 
                        background-color: ${a.ativo ? '#0aa50a' : '#ff0000'}; 
                        margin-right: 0px;
                        border: 1px solid #ddd;
                    "></span>
                    ${a.ativo ? 'Ativo' : 'Inativo'}
                </td>
                <td>
                    <button class="icon-btn" style="color: #00317C" onclick="prepararEdicao(${a.id}, '${a.nome}', '${a.descricao}', ${a.categoriaAgenteId}, ${a.ativo})"> <i class="fa-solid fa-pen-to-square"></i> <small>Editar </small></button>
                    <span class="barrer">|</span>
                    <button class="icon-btn" style="color: #ca0707" onclick="deletarAgente(${a.id})"><i class="fa-solid fa-trash-can"></i>  <small>Deletar </small></button>
                </td>
            `;
            canaisTableBody.appendChild(tr);
        });
    } catch (error) {
        console.error('Erro ao buscar usuários:', error);
    }
}

window.modalAgente = async function (a = null) {
    console.log(a)
  openModal(`
    <h3>${a ? 'Editar Agente' : 'Novo Agente'}</h3>
    
    <div class="form-group">
        <label>Nome</label>
        <input id="ca-nome" type="text" value="${a?.nome ?? ''}" placeholder="Nome agente">
    </div>

    <div class="form-group">
        <label>Nome</label>
        <input id="ca-descricao" type="text" value="${a?.descricao ?? ''}" placeholder="Descrição agente">
    </div>

        <div class="form-group">
        <label>Perfil de Acesso</label>
        <select id="ca-categoria" required>
            <option value="">Selecione uma categoria</option>
            <option value="1" ${a?.categoriaAgenteId == 1 ? 'selected' : ''}>Vendas</option>
            <option value="2" ${a?.categoriaAgenteId == 2 ? 'selected' : ''}>Suporte</option>
            <option value="3" ${a?.categoriaAgenteId == 3 ? 'selected' : ''}>Financeiro</option>
            <option value="4" ${a?.categoriaAgenteId == 4 ? 'selected' : ''}>RH</option>
        </select>
    </div>

    <div class="form-group">
        <label>Status</label>
        <select id="ca-ativo" required>
            <option value="true" ${a?.ativo ?? true ? 'selected' : ''}>Ativo</option>
            <option value="false" ${a?.ativo === false ? 'selected' : ''}>Inativo</option>
        </select>
    </div>
    
    <div style="display:flex;gap:10px;justify-content:flex-end;margin-top:15px">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-success" onclick="salvarAgente(${a?.id ?? 'null'})">Salvar</button>
    </div>
  `);
};

window.prepararEdicao = function (id, nome, descricao, categoriaAgenteId, ativo) {
    window.modalAgente({ id, nome, descricao, categoriaAgenteId, ativo });
};

window.salvarAgente = async function (id = null) {
    const nome = document.getElementById('ca-nome').value;
    const descricao = document.getElementById('ca-descricao').value;
    const categoriaAgenteId = document.getElementById('ca-categoria').value;
    const ativo = document.getElementById('ca-ativo').value === 'true'; 

    const payload = {
        nome,
        descricao,
        categoriaAgenteId,
        ativo 
    };

    const url = id ? `${API_URL}/agentes/${id}` : `${API_URL}/agentes`;
    const metodo = id ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, fetchOptions(metodo, payload));

        if (response.ok) {
            alert(id ? 'Agente atualizado com sucesso!' : 'Agente adicionado com sucesso!');
            closeModal();
            await carregarAgentes(); 
        } else {
            const erroTxt = await response.text();
            alert(`Erro: ${erroTxt}`);
        }
    } catch (error) {
        console.error('Erro ao salvar agente:', error);
        alert('Não foi possível salvar o agente.');
    }
};

async function deletarAgente(id) {
    if (!confirm('Deseja realmente excluir este agente?')) return;

    try {
        const response = await fetch(`${API_URL}/agentes/${id}`, fetchOptions('DELETE'));
        if (response.ok) {
            carregarAgentes();
        } else {
            alert('Você não tem permissão para excluir (Apenas Admin).');
        }
    } catch (error) {
        alert('Erro ao tentar deletar.');
    }
}

