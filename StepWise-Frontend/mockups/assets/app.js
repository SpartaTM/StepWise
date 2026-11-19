document.addEventListener('DOMContentLoaded', () => {
    // 1. Live search in Catalog
    const searchInput = document.getElementById('catalogSearch');
    const clearBtn = document.getElementById('clearSearchBtn');
    const catalogItems = document.querySelectorAll('.catalog-item');
    const noResults = document.getElementById('noSearchResults');

    if (searchInput && catalogItems.length > 0) {
        const filterCatalog = () => {
            const query = searchInput.value.trim().toLowerCase();
            if (clearBtn) {
                clearBtn.classList.toggle('d-none', query.length === 0);
            }

            let visibleCount = 0;
            catalogItems.forEach(card => {
                const title = card.dataset.title || '';
                const category = card.dataset.category || '';
                const match = title.includes(query) || category.includes(query);
                card.classList.toggle('d-none', !match);
                if (match) visibleCount++;
            });

            if (noResults) {
                noResults.classList.toggle('d-none', visibleCount > 0);
            }
        };

        searchInput.addEventListener('input', filterCatalog);
        if (clearBtn) {
            clearBtn.addEventListener('click', () => {
                searchInput.value = '';
                filterCatalog();
                searchInput.focus();
            });
        }
    }

    // 2. Copy share link on Details page
    const copyBtn = document.getElementById('copyShareLinkBtn');
    if (copyBtn) {
        copyBtn.addEventListener('click', async () => {
            try {
                await navigator.clipboard.writeText(window.location.href);
                const originalHtml = copyBtn.innerHTML;
                copyBtn.innerHTML = '<i class="bi bi-check2 me-2 text-success"></i>Ссылка скопирована!';
                setTimeout(() => {
                    copyBtn.innerHTML = originalHtml;
                }, 2000);
            } catch {
                alert('Скопируйте ссылку из адресной строки браузера');
            }
        });
    }

    // 3. Author Form: progressive step reveal & counter
    const stepsContainer = document.getElementById('stepsContainer');
    const addMoreBtn = document.getElementById('addMoreStepBtn');
    const filledBadge = document.getElementById('filledStepsBadge');

    if (stepsContainer) {
        const stepEditors = Array.from(stepsContainer.querySelectorAll('.sw-step-editor'));

        const updateStepStats = () => {
            let filled = 0;
            stepEditors.forEach(editor => {
                const titleInput = editor.querySelector('.step-title-input');
                if (titleInput && titleInput.value.trim().length > 0) {
                    filled++;
                }
            });
            if (filledBadge) {
                filledBadge.textContent = `Заполнено: ${filled}`;
            }

            const hiddenSteps = stepEditors.filter(el => el.classList.contains('d-none'));
            if (addMoreBtn) {
                addMoreBtn.classList.toggle('d-none', hiddenSteps.length === 0);
            }
        };

        if (addMoreBtn) {
            addMoreBtn.addEventListener('click', () => {
                const nextHidden = stepEditors.find(el => el.classList.contains('d-none'));
                if (nextHidden) {
                    nextHidden.classList.remove('d-none');
                    const input = nextHidden.querySelector('.step-title-input');
                    if (input) input.focus();
                    updateStepStats();
                }
            });
        }

        stepsContainer.addEventListener('input', (e) => {
            if (e.target.classList.contains('step-title-input')) {
                updateStepStats();
            }
        });

        stepsContainer.addEventListener('click', (e) => {
            const clearStepBtn = e.target.closest('.clear-step-btn');
            if (clearStepBtn) {
                const editor = clearStepBtn.closest('.sw-step-editor');
                if (editor) {
                    const titleInput = editor.querySelector('.step-title-input');
                    const descInput = editor.querySelector('.step-desc-input');
                    if (titleInput) titleInput.value = '';
                    if (descInput) descInput.value = '';
                    updateStepStats();
                }
            }
        });

        updateStepStats();
    }
});


// Interactive demo toggle for static mockup player
document.querySelectorAll('.mockup-toggle-step').forEach(btn => {
    btn.addEventListener('click', () => {
        const card = btn.closest('.sw-step-card');
        const isDone = card.classList.toggle('is-completed');
        card.classList.remove('is-current');
        const num = card.querySelector('.sw-step-num');
        const badge = card.querySelector('.step-status-badge');
        if (isDone) {
            num.innerHTML = '<i class="bi bi-check-lg"></i>';
            btn.className = 'btn w-100 sw-btn-step-done mockup-toggle-step';
            btn.innerHTML = '<i class="bi bi-check-circle-fill me-1"></i><span>Выполнено</span>';
            if (badge) { badge.className = 'badge bg-success-subtle text-success rounded-pill step-status-badge'; badge.textContent = 'Готово'; }
        } else {
            num.innerHTML = `<span>${card.dataset.order}</span>`;
            btn.className = 'btn w-100 sw-btn-primary mockup-toggle-step';
            btn.innerHTML = '<i class="bi bi-circle me-1"></i><span>Отметить шаг</span>';
            if (badge) { badge.className = 'badge bg-light text-muted rounded-pill step-status-badge'; badge.textContent = 'Ожидает'; }
        }
        const allCards = Array.from(document.querySelectorAll('.sw-step-card'));
        const doneCount = allCards.filter(c => c.classList.contains('is-completed')).length;
        const pct = Math.round((doneCount / allCards.length) * 100);
        const pctEl = document.getElementById('mockupPct');
        const countEl = document.getElementById('mockupCount');
        const barEl = document.getElementById('mockupBar');
        const bannerEl = document.getElementById('mockupCompleteBanner');
        if (pctEl) pctEl.textContent = pct + '%';
        if (countEl) countEl.textContent = `Выполнено ${doneCount} из ${allCards.length}`;
        if (barEl) { barEl.style.width = pct + '%'; barEl.classList.toggle('bg-success', pct === 100); }
        if (bannerEl) bannerEl.classList.toggle('d-none', pct !== 100);
    });
});
