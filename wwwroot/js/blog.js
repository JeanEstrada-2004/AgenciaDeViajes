// Función para confirmar eliminación de entradas
document.addEventListener('DOMContentLoaded', function() {
    // Confirmación antes de eliminar
    const deleteForms = document.querySelectorAll('form[onsubmit="return confirm(\'¿Seguro que quieres eliminar esta entrada?\');"]');
    
    deleteForms.forEach(form => {
        form.onsubmit = function(e) {
            e.preventDefault();
            if (confirm('¿Estás seguro de que deseas eliminar esta entrada? Esta acción no se puede deshacer.')) {
                form.submit();
            }
        };
    });

    // Animación para las tarjetas
    const cards = document.querySelectorAll('.blog-card');
    cards.forEach((card, index) => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'all 0.5s ease ' + (index * 0.1) + 's';
        
        setTimeout(() => {
            card.style.opacity = '1';
            card.style.transform = 'translateY(0)';
        }, 100);
    });

    // Validación del formulario
    const blogForm = document.querySelector('.blog-form form');
    if (blogForm) {
        blogForm.addEventListener('submit', function(e) {
            const requiredFields = blogForm.querySelectorAll('[required]');
            let isValid = true;
            
            requiredFields.forEach(field => {
                if (!field.value.trim()) {
                    field.classList.add('is-invalid');
                    isValid = false;
                } else {
                    field.classList.remove('is-invalid');
                }
            });
            
            if (!isValid) {
                e.preventDefault();
                alert('Por favor complete todos los campos requeridos.');
            }
        });
    }
});