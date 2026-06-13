const isLiveServer = window.location.port === '5500' || window.location.port === '5501';
const API_URL = isLiveServer ? 'http://localhost:5268/api' : '/api';

const loginForm = document.getElementById('login-form');
const loginErro = document.getElementById('login-erro');

const fetchOptions = (method, body = null) => {
    const config = {
        method: method,
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include' 
    };
    if (body) config.body = JSON.stringify(body);
    return config;
};

// 1. EVENTO DE LOGIN
loginForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    loginErro.innerText = '';

    const email = document.getElementById('email').value;
    const senha = document.getElementById('senha').value;

    try {
        const response = await fetch(`${API_URL}/auth/login`, fetchOptions('POST', { email, senha }));
        const data = await response.json();

        if (response.ok) {
            // Guarda o nome do usuário para usar na tela de dashboard
            localStorage.setItem('usuarioNome', data.usuario);
            localStorage.setItem('perfil', data.perfil);
            
            // Redireciona o navegador para a página de usuários
            window.location.href = '../usuarios.html';
        } else {
            loginErro.innerText = data.mensagem || 'Falha na autenticação.';
        }
    } catch (error) {
        loginErro.innerText = 'Erro ao conectar com o servidor.';
    }
});