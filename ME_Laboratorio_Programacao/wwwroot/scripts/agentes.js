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
            console.log(a)
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
                    <button class="icon-btn" style="color: #00317C" onclick="prepararEdicao(${a.id}, '${a.nome}', ${a.ativo})"> <i class="fa-solid fa-pen-to-square"></i> <small>Editar </small></button>
                    <span class="barrer">|</span>
                    <button class="icon-btn" style="color: #ca0707" onclick="deletarCanal(${a.id})"><i class="fa-solid fa-trash-can"></i>  <small>Deletar </small></button>
                </td>
            `;
            canaisTableBody.appendChild(tr);
        });
    } catch (error) {
        console.error('Erro ao buscar usuários:', error);
    }
}

window.modalAgente = async function (a = null) {
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
            <option value="1" ${a?.categoriaAgenteId == 3 ? 'selected' : ''}>Financeiro</option>
            <option value="2" ${a?.categoriaAgenteId == 4 ? 'selected' : ''}>RH</option>
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
      <button class="btn btn-success" onclick="salvarUsuario(${a?.id ?? 'null'})">Salvar</button>
    </div>
  `);
};

window.prepararEdicao = function (id, nome, ativo) {
    window.modalCanal({ id, nome, ativo });
};

// window.salvarUsuario = async function (id = null) {
//     const nome = document.getElementById('ca-nome').value;
//     const ativo = document.getElementById('ca-ativo').value === 'true'; 

//     const payload = {
//         nome,
//         ativo 
//     };

//     const url = id ? `${API_URL}/canais/${id}` : `${API_URL}/canais`;
//     const metodo = id ? 'PUT' : 'POST';

//     try {
//         const response = await fetch(url, fetchOptions(metodo, payload));

//         if (response.ok) {
//             alert(id ? 'Canal atualizado com sucesso!' : 'Canal criado com sucesso!');
//             closeModal();
//             await carregarCanais(); 
//         } else {
//             const erroTxt = await response.text();
//             alert(`Erro: ${erroTxt}`);
//         }
//     } catch (error) {
//         console.error('Erro ao salvar canal:', error);
//         alert('Não foi possível salvar o canal.');
//     }
// };

// async function deletarCanal(id) {
//     if (!confirm('Deseja realmente excluir este canal?')) return;

//     try {
//         const response = await fetch(`${API_URL}/canais/${id}`, fetchOptions('DELETE'));
//         if (response.ok) {
//             carregarCanais();
//         } else {
//             alert('Você não tem permissão para excluir (Apenas Admin).');
//         }
//     } catch (error) {
//         alert('Erro ao tentar deletar.');
//     }
// }

