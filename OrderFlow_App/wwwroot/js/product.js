// API LAYER
// ========================
const api = {
    async request(url, options = {}) {
        try {
            const response = await fetch(url, {
                headers: { "Content-Type": "application/json", ...options.headers },
                ...options
            });
            const data = options.method !== "DELETE" ? await response.json().catch(() => null) : true;
            if (!response.ok) throw new Error("Request Failed");
            return data;
        } catch (error) { console.error("API Error:", error); throw error; }
    },
    get: (url) => api.request(url, { method: "GET" }),
    post: (url, body) => api.request(url, { method: "POST", body: JSON.stringify(body) }),
    put: (url, body) => api.request(url, { method: "PUT", body: JSON.stringify(body) }),
    delete: (url) => api.request(url, { method: "DELETE" })
};

// HELPER: GENERATE GUID
// ========================
const generateGuid = () => {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
        var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
};


// ELEMENTS
// ========================
const titleInput = document.querySelector("#titleInput");
const descriptionInput = document.querySelector("#recordDescriptionInput");
const priceInput = document.querySelector("#unitPriceInput");
const selectedProductId = document.querySelector("#selectedProductId");
const productContainer = document.querySelector("#productContainer");
const refreshBtn = document.querySelector("#refreshBtn");
const saveBtn = document.querySelector("#saveBtn");
const updateBtn = document.querySelector("#updateBtn");
const removeBtn = document.querySelector("#removeBtn");
const baseUrl = "http://localhost:5031/Product";

// VALIDATION
// ========================
const validate = (title, description, unitPrice) => {
    if (!title.trim() || !description.trim() || !unitPrice)
        return { isValid: false, message: "All fields are required." };
    if (title.trim().length < 2)
        return { isValid: false, message: "Title must be at least 2 characters." };
    if (description.trim().length < 3)
        return { isValid: false, message: "Description must be at least 5 characters." };
    if (Number(unitPrice) <= 0)
        return { isValid: false, message: "Unit Price must be greater than zero." };
    return { isValid: true };
};

// HELPERS
// ========================
const resetForm = () => {
    selectedProductId.value = "";
    titleInput.value = "";
    descriptionInput.value = "";
    priceInput.value = "";
};

window.selectProduct = (id, title, description, price) => {
    document.querySelector("#selectedProductId").value = id;
    document.querySelector("#titleInput").value = title;
    document.querySelector("#recordDescriptionInput").value = description;
    document.querySelector("#unitPriceInput").value = price;
};

// RENDER LOGIC
// ========================
const fetchProducts = async () => {
    const products = await api.get(`${baseUrl}/GetAll`);

    productContainer.innerHTML = products.map(p => {
        const title = p.title || "";
        const desc = p.recordDescription || "";
        const price = p.unitPrice || 0;

        return `
        <tr>
            <td>${title}</td>
            <td>${desc}</td>
            <td>${price}</td>
            <td>
                <button class="btn btn-sm btn-outline-primary" 
                    onclick="selectProduct('${p.id}', '${title}', '${desc}', ${price})">
                    Select
                </button>
            </td>
        </tr>
        `;
    }).join('');
};


// CREATE LOGIC
// ========================
saveBtn.addEventListener("click", async () => {
    const result = validate(titleInput.value, descriptionInput.value, priceInput.value);
    if (!result.isValid) return alert(result.message);

    const newProduct = {
        guidKey: generateGuid(),
        title: titleInput.value.trim(),
        recordDescription: descriptionInput.value.trim(),
        unitPrice: parseFloat(priceInput.value)
    };

    await api.post(`${baseUrl}/Post`, newProduct);
    resetForm();
    fetchProducts();
});

// UPDATE LOGIC
// ========================
updateBtn.addEventListener("click", async () => {
    const id = selectedProductId.value;
    if (!id) return alert("Select a product first!");

    const title = titleInput.value.trim();
    const description = descriptionInput.value.trim();
    const price = priceInput.value.trim();

    const error = validate(title, description, price);
    if (!error.isValid) return alert(error.message);

    await api.put(`${baseUrl}/Put`, {
        id: id,
        title: title,
        recordDescription: description,
        unitPrice: parseFloat(price)
    });

    resetForm();
    fetchProducts();
});

// DELETE LOGIC
// ========================
removeBtn.addEventListener("click", async () => {
    if (!selectedProductId.value) return alert("Select a product first!");
    await api.delete(`${baseUrl}/Delete?id=${selectedProductId.value}`);
    resetForm();
    fetchProducts();
});

// INIT EVENTS
// ========================
refreshBtn.addEventListener("click", () => {
    resetForm();
    fetchProducts();
});
window.addEventListener("load", fetchProducts);