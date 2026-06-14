document.addEventListener("DOMContentLoaded", async () => {
    await carregarEstatisticas();
});

let chartMensagens = null;
let chartCanais = null;
let chartSessoesAgente = null;

async function carregarEstatisticas() {
    try {
        const res = await fetch(`${API_URL}/dashboard/estatisticas`, fetchOptions('GET'));
        if (res.ok) {
            const data = await res.json();
            document.getElementById('kpi-sessoes').innerText = data.totalSessoes;
            document.getElementById('kpi-mensagens').innerText = data.totalMensagens;

            // Media
            const media = data.totalSessoes > 0 ? (data.totalMensagens / data.totalSessoes).toFixed(1) : 0;
            document.getElementById('kpi-media').innerText = media;

            // Agente mais usado
            if (data.sessoesPorAgente && data.sessoesPorAgente.length > 0) {
                const topAgente = data.sessoesPorAgente.reduce((max, obj) => obj.total > max.total ? obj : max);
                document.getElementById('kpi-top-agente').innerText = topAgente.agente;
            }

            renderizarGraficoMensagensAgente(data.mensagensPorAgente);
            renderizarGraficoSessoesCanal(data.sessoesPorCanal);
            renderizarGraficoSessoesAgente(data.sessoesPorAgente);
        }else console.error("Erro ao carregar dados do dashboard");
    }catch(e) {
        console.error("Erro de conexão", e);
    }
}

function renderizarGraficoMensagensAgente(dados){
    const ctx = document.getElementById('graficoMensagensAgente').getContext('2d');
    if(chartMensagens) chartMensagens.destroy();
    const labels = dados.map(d => d.agente);
    const valores = dados.map(d => d.total);

    chartMensagens = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Mensagens Trafegadas',
                data: valores,
                backgroundColor: 'rgba(79, 70, 229, 0.7)',
                borderColor: 'rgba(79, 70, 229, 1)',
                borderWidth: 1,
                borderRadius: 4
            }]
        },
        options:{
            responsive: true,
            maintainAspectRatio: false,
            scales: {
                y:{
                    beginAtZero: true,
                    ticks: { precision: 0 }
                }
            }
        }
    });
}

function renderizarGraficoSessoesCanal(dados){
    const ctx = document.getElementById('graficoSessoesCanal').getContext('2d');
    if(chartCanais) chartCanais.destroy();
    const labels = dados.map(d => d.canal);
    const valores = dados.map(d => d.total);
    const cores = ['#3b82f6', '#10b981', '#f59e0b', '#ec4899', '#8b5cf6'];

    chartCanais = new Chart(ctx,{
        type: 'doughnut',
        data:{
            labels: labels,
            datasets: [{
                data: valores,
                backgroundColor: cores.slice(0, valores.length),
                borderWidth: 2,
                borderColor: '#ffffff'
            }]
        },
        options:{
            responsive: true,
            maintainAspectRatio: false,
            plugins:{
                legend:{
                    position: 'right'
                }
            }
        }
    });
}

function renderizarGraficoSessoesAgente(dados){
    const ctx = document.getElementById('graficoSessoesAgente').getContext('2d');
    if(chartSessoesAgente) chartSessoesAgente.destroy();
    
    const labels = dados.map(d => d.agente);
    const valores = dados.map(d => d.total);

    chartSessoesAgente = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                data: valores,
                backgroundColor: ['#ef4444', '#10b981', '#3b82f6', '#f59e0b', '#8b5cf6'],
                borderWidth: 1
            }]
        },
        options:{
            responsive: true,
            maintainAspectRatio: false,
            plugins:{
                legend:{
                    position: 'bottom'
                }
            }
        }
    });
}
