document.addEventListener('DOMContentLoaded', () => {
    const list = document.querySelector('.player-steps');
    if (!list) return;

    const instructionId = list.dataset.instructionId;
    const url = list.dataset.toggleUrl;
    const token = document.querySelector('input[name="__RequestVerificationToken"]').value;

    list.addEventListener('change', async (e) => {
        if (!e.target.matches('input[type="checkbox"]')) return;

        const checkbox = e.target;
        const item = checkbox.closest('.player-step');

        const response = await fetch(url, {
            method: 'POST',
            headers: {
                'RequestVerificationToken': token,
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: new URLSearchParams({ instructionId, stepId: item.dataset.stepId })
        });

        if (!response.ok) {
            checkbox.checked = !checkbox.checked;   // откатываем галочку
            alert('Не удалось сохранить отметку. Попробуйте ещё раз.');
            return;
        }

        const data = await response.json();
        item.classList.toggle('done', data.isDone);
        document.querySelector('.progress-bar').style.width = data.percent + '%';
        document.querySelector('.progress-text').textContent =
            `${data.done} из ${data.total} (${data.percent}%)`;
        document.querySelector('.completed-message').hidden = !data.isCompleted;
    });
});