// Скрипт для переключения видимости формы поиска
document.getElementById('searchToggleBtn').addEventListener('click', function () {
    var form = document.getElementById('searchForm');
    // Переключаем класс d-none, чтобы показать/скрыть форму
    form.classList.toggle('d-none');
});

// Скрипт для управления отображением валидационной плашки
document.addEventListener('DOMContentLoaded', function () {
    const validationSummary = document.getElementById('validation-summary');
    console.log('Элемент validation-summary:', validationSummary);

    if (validationSummary) {
        // Используем getElementById, чтобы проверить наличие ошибок внутри summary.
        // Если там есть хотя бы один элемент ошибки, скрытие не будет выполняться.
        const errorItems = validationSummary.getElementsByTagName('li');
        console.log('Ошибки найдены:', errorItems.length);

        if (errorItems.length > 0) {
            validationSummary.classList.remove('d-none');
            validationSummary.classList.add('fade-in');
            console.log('Плашка ошибок показана');

            // Через 5 секунд скрываем плашку
            setTimeout(() => {
                validationSummary.classList.add('fade-out');
                console.log('Начато исчезновение');
                setTimeout(() => validationSummary.classList.add('d-none'), 1000); // Скрытие через 1 секунду
            }, 5000);
        }
    }
});
