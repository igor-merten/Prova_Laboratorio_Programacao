const categoryTableBody = document.querySelector('#category-table tbody');

window.modalCategoria = async function (c = null) {
  openModal(`
    <h3>${c ? 'Editar Categoria' : 'Nova Categoria'}</h3>
    <div class="form-group"><label>Nome</label><input id="ca-nome" value="${c?.nome ?? ''}"></div>
    <div class="form-group"><label>Cor (hex)</label><input id="ca-cor" type="color" value="${c?.corHex ?? '#6366f1'}" style="height:42px;padding:4px"></div>
    <div style="display:flex;gap:10px;justify-content:flex-end;margin-top:8px">
      <button class="btn btn-secondary" onclick="closeModal()">Cancelar</button>
      <button class="btn btn-primary" onclick="salvarCategoria(${c?.id ?? 'null'})">Salvar</button>
    </div>`);
}

window.prepararEdicao = function (id, nome, corHex) {
    window.modalCategoria({ id, nome, corHex });
};

window.salvarCategoria = async function (id = null) {
    const nome = document.getElementById('ca-nome').value;
    const corHex = document.getElementById('ca-cor').value;

    if (!nome.trim()) {
        alert('O nome da categoria é obrigatório.');
        return;
    }

    const payload = { nome, corHex };
    
    // Define a estratégia baseada na existência do ID
    const url = id ? `${API_URL}/categoriaagente/categorias/${id}` : `${API_URL}/categoriaagente/categorias`;
    const metodo = id ? 'PUT' : 'POST';

    try {
        const response = await fetch(url, fetchOptions(metodo, payload));

        if (response.ok) {
            alert(id ? 'Categoria atualizada com sucesso!' : 'Categoria criada com sucesso!');
            closeModal();
            await carregarCategorias(); // Recarrega a tabela limpa
        } else {
            const erroTexto = await response.text();
            alert(`Erro do servidor: ${erroTexto}`);
        }
    } catch (error) {
        alert('Não foi possível conectar ao servidor.');
    }
};

window.deletarCategoria = async function (id) {
    if (!confirm('Deseja realmente excluir esta categoria?')) return;

    try {
        const response = await fetch(`${API_URL}/categoriaagente/categorias/${id}`, fetchOptions('DELETE'));
        if (response.ok) {
            await carregarCategorias();
        } else {
            alert('Não foi possível excluir a categoria.');
        }
    } catch (error) {
        alert('Erro ao tentar deletar.');
    }
};

async function carregarCategorias() {
    try {
        // Altere para a rota exata definida no seu Controller .NET (ex: /categoriaagentes ou /categorias)
        const response = await fetch(`${API_URL}/categoriaagente/categorias`, fetchOptions('GET'));
        
        if (response.status === 401 || response.status === 403) {
            alert('Sessão expirada ou acesso não autorizado.');
            efetuarLogoutLocal();
            return;
        }

        const categorias = await response.json();
        categoryTableBody.innerHTML = '';

        categorias.forEach(cat => {
            console.log(cat)
            const tr = document.createElement('tr');
            tr.innerHTML = `
                <td>${cat.id}</td>
                <td>${cat.nome}</td>
                <td>
                    <span style="
                        display: inline-block; 
                        width: 12px; 
                        height: 12px; 
                        border-radius: 50%; 
                        background-color: ${cat.corHex || '#cccccc'}; 
                        margin-right: 8px;
                        border: 1px solid #ddd;
                    "></span>
                    <code>${cat.corHex || '#cccccc'}</code>
                </td>
                <td>
                    <button class="icon-btn" style="color: #00317C" onclick="prepararEdicao(${cat.id}, '${cat.nome}', '${cat.corHex}')" > <i class="fa-solid fa-pen-to-square"></i> <small>Editar </small></button> 
                    <span class="barrer">|</span>
                    <button class="icon-btn" style="color: #ca0707" onclick="deletarCategoria(${cat.id})"><i class="fa-solid fa-trash-can"></i>  <small>Deletar </small></button>
                </td>
            `;
            categoryTableBody.appendChild(tr);
        });
    } catch (error) {
        console.error('Erro ao buscar categorias:', error);
    }
}

await carregarCategorias();