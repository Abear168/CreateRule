document.addEventListener('keydown', function(event) {
    if (event.key === 'Escape') {
        var backButton = document.querySelector('a.btn-secondary[href]');
        if (backButton) {
            window.location.href = backButton.href;
        }
    }
});
