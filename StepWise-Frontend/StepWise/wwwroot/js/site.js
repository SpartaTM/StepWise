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
