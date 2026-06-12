const usersTableBody = document.querySelector('#users-table tbody');
const userForm = document.getElementById('user-form');

document.addEventListener("DOMContentLoaded", () => {
    carregarUsuarios();

});

// 1. CARREGAR LISTA DE USUÁRIOS (READ)
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
                        margin-right: 8px;
                        border: 1px solid #ddd;
                    "></span>
                </td>
                <td>
                    <button class="btn-danger" onclick="deletarUsuario(${u.id})" style="padding: 4px 8px; font-size: 12px;">Excluir</button>
                </td>
            `;
            usersTableBody.appendChild(tr);
        });
    } catch (error) {
        console.error('Erro ao buscar usuários:', error);
    }
}

// 2. CRIAR NOVO USUÁRIO (CREATE)
if (userForm) {
    userForm.addEventListener('submit', async (e) => {
        e.preventDefault();

        const novoUsuario = {
            nome: document.getElementById('user-nome').value,
            email: document.getElementById('user-email').value,
            senha: document.getElementById('user-senha').value,
            perfilAcessoId: parseInt(document.getElementById('user-perfil').value)
        };

        try {
            const response = await fetch(`${API_URL}/usuarios`, fetchOptions('POST', novoUsuario));
            
            if (response.ok) {
                alert('Usuário cadastrado com sucesso!');
                userForm.reset();
                carregarUsuarios();
            } else {
                const erroTxt = await response.text();
                alert(`Erro: ${erroTxt}`);
            }
        } catch (error) {
            alert('Não foi possível salvar o usuário.');
        }
    });
}

// 3. DELETAR USUÁRIO (DELETE)
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

