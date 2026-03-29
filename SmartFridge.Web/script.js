async function analyzeFridge() {
    const fileInput = document.getElementById('imageInput');
    const btn = document.getElementById('btnAnalyze');
    
    if (!fileInput.files[0]) return alert("Сначала выберите фото!");

    btn.disabled = true;
    btn.innerText = "Нейросеть думает...";

    const formData = new FormData();
    formData.append("photo", fileInput.files[0]);

    try {
        // ВАЖНО: Проверь порт своего API (в адресе Swagger) и замени 5053 если нужно
        const response = await fetch("http://localhost:5053/api/Fridge/analyze", {
            method: "POST",
            body: formData
        });

        const data = await response.json();
        displayResults(data);
    } catch (error) {
        console.error("Ошибка:", error);
        alert("Не удалось связаться с сервером");
    } finally {
        btn.disabled = false;
        btn.innerText = "Анализировать";
    }
}

function displayResults(data) {
    document.getElementById('results').classList.remove('d-none');
    
    // Вывод ингредиентов
    const ingDiv = document.getElementById('ingredientsList');
    ingDiv.innerHTML = data.ingredients.map(i => `<span class="badge bg-success me-2 p-2">${i}</span>`).join('');

    // Вывод рецептов
    const recDiv = document.getElementById('recipesList');
    recDiv.innerHTML = data.recipes.map(r => `
        <div class="col-md-6 mb-3">
            <div class="card h-100 recipe-card p-3">
                <h5>${r.title}</h5>
                <p class="text-muted small">${r.description}</p>
                <button class="btn btn-outline-primary btn-sm mt-auto" onclick="saveRecipe('${r.title}', '${r.description}')">Сохранить</button>
            </div>
        </div>
    `).join('');
}

async function saveRecipe(title, description) {
    await fetch("http://localhost:5053/api/Fridge/save", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ title, description })
    });
    alert("Рецепт сохранен в базу!");
}