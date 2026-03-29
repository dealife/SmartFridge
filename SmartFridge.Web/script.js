// Основная функция анализа содержимого холодильника
async function analyzeFridge() {
    const fileInput = document.getElementById('imageInput');
    const btn = document.getElementById('btnAnalyze');
    
    // 1. Проверка: выбрал ли пользователь файл
    if (!fileInput.files[0]) {
        return alert("Сначала выберите фото!");
    }

    // Блокируем кнопку, чтобы пользователь не нажал её сто раз, пока сервер просыпается
    btn.disabled = true;
    btn.innerText = "Нейросеть думает...";

    // Подготовка данных для отправки (картинка)
    const formData = new FormData();
    formData.append("photo", fileInput.files[0]);

    try {
        // 2. Отправка запроса на твой НОВЫЙ сервер Render
        // ВАЖНО: На бесплатном тарифе первый запрос может занять до 50 секунд
        const response = await fetch("https://smartfridge-229t.onrender.com/api/Fridge/analyze", {
            method: "POST",
            body: formData
        });

        // Если сервер ответил ошибкой
        if (!response.ok) {
            throw new Error(`Ошибка сервера: ${response.status}`);
        }

        // Получаем результат в формате JSON
        const data = await response.json();
        
        // 3. Отрисовка результатов
        displayResults(data);

    } catch (error) {
        console.error("Критическая ошибка:", error);
        alert("Не удалось связаться с сервером. Если это первый запуск, подожди минуту, пока Render 'прогреет' сервер, и попробуй снова.");
    } finally {
        // Возвращаем кнопку в рабочее состояние
        btn.disabled = false;
        btn.innerText = "Анализировать";
    }
}

// Функция для отображения ингредиентов и рецептов в HTML
function displayResults(data) {
    document.getElementById('results').classList.remove('d-none');
    
    const ingDiv = document.getElementById('ingredientsList');
    ingDiv.innerHTML = data.ingredients.map(item => 
        `<span class="badge bg-success me-2 p-2">${item}</span>`
    ).join('');

    const recDiv = document.getElementById('recipesList');
    recDiv.innerHTML = data.recipes.map(recipe => `
        <div class="col-md-6 mb-3">
            <div class="card h-100 shadow-sm p-3">
                <div class="card-body d-flex flex-column">
                    <h5 class="card-title text-primary">${recipe.title}</h5>
                    <p class="card-text text-muted small">${recipe.description}</p>
                    <button class="btn btn-sm btn-outline-success mt-auto" 
                            onclick="saveRecipe('${recipe.title.replace(/'/g, "\\'")}', '${recipe.description.replace(/'/g, "\\'")}')">
                        ❤ Сохранить рецепт
                    </button>
                </div>
            </div>
        </div>
    `).join('');
}

// Функция сохранения рецепта в базу данных на Render
async function saveRecipe(title, description) {
    try {
        const response = await fetch("https://smartfridge-229t.onrender.com/api/Fridge/save", {
            method: "POST",
            headers: { 
                "Content-Type": "application/json" 
            },
            body: JSON.stringify({ title, description })
        });
        
        if (response.ok) {
            alert("Готово! Рецепт сохранен в базу на Render.");
        } else {
            alert("Ошибка при сохранении.");
        }
    } catch (error) {
        console.error("Ошибка сохранения:", error);
        alert("Сервер недоступен.");
    }
}