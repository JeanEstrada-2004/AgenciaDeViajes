document.addEventListener('DOMContentLoaded', function() {
    fetchFestividades();
});

function fetchFestividades() {
    fetch('/api/FestividadesApi')
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            renderFestividades(data);
        })
        .catch(error => {
            console.error('Error al cargar festividades:', error);
            showError();
        });
}

function renderFestividades(data) {
    const container = document.getElementById('festividades-container');
    container.innerHTML = '';

    if (!data.length) {
        container.innerHTML = `
            <div class="no-festivals">
                <i class="fas fa-calendar-times fa-3x mb-3" style="color: #ccc;"></i>
                <h4>No se encontraron festividades</h4>
                <p>Actualmente no hay festividades disponibles</p>
            </div>`;
        return;
    }

    data.forEach(festividad => {
        container.appendChild(createFestividadCard(festividad));
    });
}

function createFestividadCard(f) {
    const card = document.createElement('div');
    card.className = 'festival-card';
    
    const fecha = new Date(f.fecha);
    const options = { year: 'numeric', month: 'long', day: 'numeric' };
    const fechaFormateada = fecha.toLocaleDateString('es-PE', options);
    
    card.innerHTML = `
        ${f.imagenUrl ? `<img src="${f.imagenUrl}" alt="${f.nombre}" class="img-fluid">` : 
          `<img src="https://via.placeholder.com/800x400?text=Festividad+Peruana" alt="Imagen predeterminada" class="img-fluid">`}
        <div class="card-body">
            <h3 class="card-title">${f.nombre}</h3>
            <span class="festival-date"><i class="fas fa-calendar-alt"></i> ${fechaFormateada}</span>
            <span class="festival-type"><i class="fas fa-tag"></i> ${f.tipo}</span>
            <p class="card-text">${f.descripcion}</p>
            <p class="festival-place"><i class="fas fa-map-marker-alt"></i> <strong>Lugar:</strong> ${f.lugar}</p>
        </div>
    `;

    return card;
}

function showError() {
    document.getElementById('festividades-container').innerHTML = `
        <div class="error-message">
            <i class="fas fa-exclamation-triangle fa-2x mb-2"></i>
            <h4>Error al cargar festividades</h4>
            <p>No se pudieron cargar las festividades. Por favor, inténtalo de nuevo más tarde.</p>
        </div>`;
}