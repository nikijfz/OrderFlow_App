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
const firstNameInput = document.querySelector("#firstNameInput");
const lastNameInput = document.querySelector("#lastNameInput");
const phoneInput = document.querySelector("#phoneInput");
const selectedCustomerId = document.querySelector("#selectedCustomerId");
const customerContainer = document.querySelector("#customerContainer");
const refreshBtn = document.querySelector("#refreshBtn");
const saveBtn = document.querySelector("#saveBtn");
const updateBtn = document.querySelector("#updateBtn");
const removeBtn = document.querySelector("#removeBtn");
const baseUrl = "http://localhost:5031/Customer";

// VALIDATION LOGIC
// ========================
const validate = (first, last, phone) => {
    const regex = /^[a-zA-Zآ-ی\s]+$/;
    const phoneRegex = /^[0-9]+$/;
    if (!first.trim() || !last.trim()) return "First and Last Name are required.";
    if (first.length < 3 || last.length < 3) return "Minimum length is 3 characters.";
    if (!regex.test(first) || !regex.test(last)) return "Only Persian and English letters allowed.";
    if (!phoneRegex.test(phone))  return "Phone number must contain only numbers.";
    return null;
};

// HELPERS
// ========================
const resetForm = () => {
    selectedCustomerId.value = "";
    firstNameInput.value = "";
    lastNameInput.value = "";
    phoneInput.value = "";
};

window.selectCustomer = (id, first, last, phone) => {
    selectedCustomerId.value = id;
    firstNameInput.value = first;
    lastNameInput.value = last;
    phoneInput.value = phone;
};

// RENDER LOGIC
// ========================
const fetchCustomers = async () => {
    const customers = await api.get(`${baseUrl}/GetAll`);

    customerContainer.innerHTML = customers.map(c => {
        const first = c.firstName || c.FirstName || "";
        const last = c.lastName || c.LastName || "";
        const phone = c.phone || c.Phone || "";

        return `
        <tr>
            <td>${first}</td>
            <td>${last}</td>
            <td>${phone}</td>
            <td>
                <button class="btn btn-sm btn-outline-info" 
                    onclick="selectCustomer('${c.id}', '${first}', '${last}', '${phone}')">
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
    const error = validate(firstNameInput.value, lastNameInput.value, phoneInput.value);
    if (error) return alert(error);

    const newCustomer = {
        guidKey: generateGuid(),
        firstName: firstNameInput.value.trim(),
        lastName: lastNameInput.value.trim(),
        phone: phoneInput.value.trim()
    };

    await api.post(`${baseUrl}/Post`, newCustomer);
    resetForm();
    fetchCustomers();
});

// UPDATE LOGIC
// ========================
updateBtn.addEventListener("click", async () => {
    if (!selectedCustomerId.value) return alert("Select a Customer first!");
    const error = validate(firstNameInput.value, lastNameInput.value, phoneInput.value);
    if (error) return alert(error);
    await api.put(`${baseUrl}/Put`, { id: selectedCustomerId.value, firstName: firstNameInput.value, lastName: lastNameInput.value, phone: phoneInput.value });
    resetForm();
    fetchCustomers();
});

// DELETE LOGIC
// ========================
removeBtn.addEventListener("click", async () => {
    if (!selectedCustomerId.value) return alert("Select a Customer first!");
    await api.delete(`${baseUrl}/Delete?id=${selectedCustomerId.value}`);
    resetForm();
    fetchCustomers();
});

// INIT EVENTS
// ========================
refreshBtn.addEventListener("click", () => {
    resetForm();
    fetchCustomers();
});
phoneInput.addEventListener("input", function () {
    this.value = this.value.replace(/[^0-9]/g, '');
    if (this.value.length > 11) {
        this.value = this.value.slice(0, 11);
    }
});
window.addEventListener("load", fetchCustomers);