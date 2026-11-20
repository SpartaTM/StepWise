// Мобильное меню
(function () {
    const header = document.getElementById('site-header');
    const burger = document.getElementById('burger');
    if (!header || !burger) return;

    const setOpen = (open) => {
        header.classList.toggle('nav-open', open);
        burger.setAttribute('aria-expanded', String(open));
        burger.setAttribute('aria-label', open ? 'Закрыть меню' : 'Открыть меню');
    };

    burger.addEventListener('click', () => setOpen(!header.classList.contains('nav-open')));
    document.addEventListener('keydown', (e) => { if (e.key === 'Escape') setOpen(false); });
    window.addEventListener('resize', () => { if (window.innerWidth > 760) setOpen(false); });
})();
