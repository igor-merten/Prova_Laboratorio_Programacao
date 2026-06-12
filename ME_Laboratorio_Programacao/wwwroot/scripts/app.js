const API_URL = 'https://localhost:7194/api';

const btnLogout = document.getElementById('btn-logout');
const userLoggedSpan = document.getElementById('user-logged');

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
    
    if (!nomeSalvo) {
        window.location.href = 'index.html';
        return;
    }

    if (userLoggedSpan) {
        userLoggedSpan.textContent = nomeSalvo;
    }
});

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