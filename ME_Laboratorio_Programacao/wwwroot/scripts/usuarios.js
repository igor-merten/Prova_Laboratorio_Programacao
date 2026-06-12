const usersTableBody = document.querySelector('#users-table tbody');
const userForm = document.getElementById('user-form');

document.addEventListener("DOMContentLoaded", () => {
    carregarUsuarios();

});

async function carregarUsuarios() {
    try {
        const response = await fetch(`${API_URL}/usuarios`, fetchOptions('GET'));
        
        if (response.status === 401 || response.status === 403) {
            alert('Sessão expirada ou acesso não autorizado.');
            efetuarLogoutLocal();
            return;
        }

        const usuarios = await response.json();
        usersTableBody.innerHTML = '';

        usuarios.forEach(u => {
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${u.id}</td>
                <td>${u.nome}</td>
                <td>${u.email}</td>
                <td><strong>${u.perfil}</strong></td>
                <td>
                    <span style="
                        display: inline-block; 
                        width: 12px; 
                        height: 12px; 
                        border-radius: 50%; 
                        background-color: ${u.ativo ? '#0aa50a' : '#ff0000'}; 
                        margin-right: 0px;
                        border: 1px solid #ddd;
                    "></span>
                    ${u.ativo ? 'Ativo' : 'Inativo'}
                </td>
                <td>
                    <button class="icon-btn" style="color: #00317C" onclick="prepararEdicao(${u.id}, '${u.nome}', ${u.perfilAcessoId || u.perfilId}, ${u.ativo})"> <i class="fa-solid fa-pen-to-square"></i> <small>Editar </small></button>
                    <span class="barrer">|</span>
                    <button class="icon-btn" style="color: #ca0707" onclick="deletarUsuario(${u.id})"><i class="fa-solid fa-trash-can"></i>  <small>Deletar </small></button>
                </td>
            `;
            usersTableBody.appendChild(tr);
        });
    } catch (error) {
        console.error('Erro ao buscar usuários:', error);
    }
}

window.modalUsuario = async function (u = null) {
    console.log(u)
  openModal(`
    <h3>${u ? 'Editar Usuário' : 'Novo Usuário'}</h3>
    
    <div class="form-group">
        <label>Nome Completo</label>
        <input id="mu-nome" type="text" value="${u?.nome ?? ''}" placeholder="Nome Completo" required>
    </div>
    
    <div class="form-group">
        <label>E-mail</label>
        <input id="mu-email" type="email" value="${u?.email ?? ''}" placeholder="E-mail" ${u ? 'disabled' : ''} required>
    </div>
    
    <div class="form-group">
        <label>${u ? 'Nova Senha (deixe em branco para não alterar)' : 'Senha'}</label>
        <input id="mu-senha" type="password" placeholder="••••••••" ${u ? '' : 'required'}>
    </div>
    
    <div class="form-group">
        <label>Perfil de Acesso</label>
        <select id="mu-perfil" required>
            <option value="">Selecione o Perfil</option>
            <option value="1" ${u?.perfilAcessoId == 1 ? 'selected' : ''}>Admin</option>
            <option value="2" ${u?.perfilAcessoId == 2 ? 'selected' : ''}>Operador</option>
        </select>
    </div>

    <div class="form-group">
        <label>Status da Conta</label>
        <select id="mu-ativo" required>
            <option value="true" ${u?.ativo ?? true ? 'selected' : ''}>Ativo</option>
            <option value="false" ${u?.ativo === false ? 'selected' : ''}>Inativo</option>
        </select>
    </div>
    
    <div style="display:flex;gap:10px;justify-content:flex-end;margin-top:15px">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-success" onclick="salvarUsuario(${u?.id ?? 'null'})">Salvar</button>
    </div>
  `);
};

window.prepararEdicao = function (id, nome, perfilAcessoId, ativo) {
    window.modalUsuario({ id, nome, perfilAcessoId, ativo });
};

window.salvarUsuario = async function (id = null) {
    const nome = document.getElementById('mu-nome').value;
    const perfilAcessoId = parseInt(document.getElementById('mu-perfil').value);
    const senha = document.getElementById('mu-senha').value;
    const ativo = document.getElementById('mu-ativo').value === 'true'; 
    
    if (!nome.trim() || !perfilAcessoId) {
        alert('Nome e Perfil de Acesso são obrigatórios.');
        return;
    }

    const payload = {
        nome,
        perfilAcessoId,
        ativo 
    };

    if (!id) {
        const email = document.getElementById('mu-email').value;
        if (!email.trim() || !senha.trim()) {
            alert('E-mail e Senha são obrigatórios para novos usuários.');
            return;
        }
        payload.email = email;
        payload.senha = senha;
    } else {
        if (senha.trim() !== '') {
            payload.senha = senha;
        }
    }

    const url = id ? `${API_URL}/usuarios/${id}` : `${API_URL}/usuarios`;
    const metodo = id ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, fetchOptions(metodo, payload));

        if (response.ok) {
            alert(id ? 'Usuário atualizado com sucesso!' : 'Usuário criado com sucesso!');
            closeModal();
            await carregarUsuarios(); 
        } else {
            const erroTxt = await response.text();
            alert(`Erro: ${erroTxt}`);
        }
    } catch (error) {
        console.error('Erro ao salvar usuário:', error);
        alert('Não foi possível salvar o usuário.');
    }
};

async function deletarUsuario(id) {
    if (!confirm('Deseja realmente excluir este usuário?')) return;

    try {
        const response = await fetch(`${API_URL}/usuarios/${id}`, fetchOptions('DELETE'));
        if (response.ok) {
            carregarUsuarios();
        } else {
            alert('Você não tem permissão para excluir (Apenas Admin).');
        }
    } catch (error) {
        alert('Erro ao tentar deletar.');
    }
}

