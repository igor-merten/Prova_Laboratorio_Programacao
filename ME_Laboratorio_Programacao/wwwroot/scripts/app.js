const isLiveServer = window.location.port === '5500' || window.location.port === '5501';
const API_URL = isLiveServer ? 'http://localhost:5268/api' : '/api';

const btnLogout = document.getElementById('logout-link');
const userLoggedSpan = document.getElementById('user-logged');
const profileLoggedSpan = document.getElementById('profile-logged');

const fetchOptions = (method, body = null) => {
    const config = {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include'
    };
    if (body) config.body = JSON.stringify(body);
    return config;
};


// EXECUTA AO CARREGAR A PÁGINA
document.addEventListener('DOMContentLoaded', () => {
    const nomeSalvo = localStorage.getItem('usuarioNome');
    const perfilSalvo = localStorage.getItem('perfil');
    
    if (!nomeSalvo) {
        window.location.href = 'index.html';
        return;
    }

    if (userLoggedSpan) {
        userLoggedSpan.textContent = nomeSalvo;
        profileLoggedSpan.textContent = perfilSalvo;
    }
});


// 4. EVENTO DE LOGOUT
if (btnLogout) {
    btnLogout.onclick = async (e) => {
        e.preventDefault();
        try {
            await fetch(`${API_URL}/account/logout`, fetchOptions('POST'));
        } catch (error) {
            console.error('Erro ao limpar sessão no servidor:', error);
        } finally {
            efetuarLogoutLocal();
        }
    };
}

// Limpa os dados locais e joga para o index
function efetuarLogoutLocal() {
    localStorage.removeItem('usuarioNome');
    window.location.href = '../index.html';
}

// navegacao
function navegarPara(pagina) {
    window.location.href = `${pagina}.html`;
}

// modal
function openModal(html) {
  const overlay = document.createElement('div');
  overlay.className = 'modal-overlay';
  overlay.id = 'modal-overlay';
  overlay.innerHTML = `<div class="modal">${html}</div>`;
  overlay.addEventListener('click', e => {
    if (e.target === overlay) closeModal();
  });
  document.body.appendChild(overlay);
}

function closeModal() {
  document.getElementById('modal-overlay')?.remove();
}