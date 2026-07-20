// ============================================
// API LAYER
// ============================================
const api = {
    async request(url, options = {}) {
        try {
            const response = await fetch(url, {
                headers: { "Content-Type": "application/json", ...options.headers },
                ...options
            });

            if (options.method === "DELETE") return true;

            if (!response.ok) {
                const err = await response.text();
                throw new Error(err);
            }

            return await response.json();
        } catch (error) {
            console.error("API Error:", error);
            throw error;
        }
    },

    get: (url) => {
        const separator = url.includes('?') ? '&' : '?';
        const antiCacheUrl = `${url}${separator}_t=${new Date().getTime()}`;
        return api.request(antiCacheUrl, { method: "GET" });
    },

    post: (url, body) => api.request(url, { method: "POST", body: JSON.stringify(body) }),
    delete: (url, body) => api.request(url, {
        method: "DELETE",
        body: JSON.stringify(body)
    }),
};

// ============================================
// UTILITIES
// ============================================
const generateGuid = () => 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
    let r = Math.random() * 16 | 0;
    let v = c === 'x' ? r : (r & 0x3 | 0x8);
    return v.toString(16);
});

// ============================================
// GLOBAL VARIABLES
// ============================================
const baseUrl = window.location.origin;
let basket = [];
let currentEditingOrderGuid = null;

// ============================================
// PRODUCT CATALOG
// ============================================
const loadCatalog = async () => {
    try {
        const products = await api.get(`${baseUrl}/Product/GetAll`);
        const container = document.querySelector("#productCatalogContainer");
        if (!container) return;

        container.innerHTML = products.map(p => {
            const id = p.id;
            const title = p.title || "";
            const desc = p.recordDescription || "-";
            const price = p.unitPrice || 0;

            return `
            <tr>
                <td class="fw-bold">${title}</td>
                <td>${desc}</td>
                <td>${price}</td>
                <td>
                    <input type="number" id="qty_${id}" value="1" min="1" class="form-control form-control-sm">
                </td>
                <td>
                    <button type="button" class="btn btn-sm btn-primary" 
                            onclick="addToBasket('${id}', '${title}', ${price})">
                        Add
                    </button>
                </td>
            </tr>`;
        }).join('');
    } catch (error) {
        console.error("Failed to load catalog:", error);
    }
};

// ============================================
// BUYER SELECTION
// ============================================
const initBuyerSelection = () => {
    const selectBuyerBtn = document.querySelector("#selectBuyerBtn");
    if (!selectBuyerBtn) return;

    selectBuyerBtn.addEventListener("click", async () => {
        try {
            const buyers = await api.get(`${baseUrl}/Customer/GetAll`);
            const container = document.querySelector("#buyerListTable");
            const listContainer = document.querySelector("#buyerListContainer");

            listContainer.style.display = "block";
            container.innerHTML = buyers.map(b => {
                const id = b.id;
                const firstName = b.firstName || "";
                const lastName = b.lastName || "";
                const phone = b.phone || "";

                return `
                <tr>
                    <td>${firstName} ${lastName}</td>
                    <td>${phone}</td>
                    <td>
                        <button type="button" class="btn btn-sm btn-outline-success"
                            onclick="setBuyer('${id}', '${firstName}', '${lastName}', '${phone}')">
                            Select
                        </button>
                    </td>
                </tr>`;
            }).join('');
        } catch (error) {
            console.error("Failed to load buyers:", error);
        }
    });
};

window.setBuyer = (id, firstName, lastName, phone) => {
    document.querySelector("#buyerId").value = id;
    document.querySelector("#buyerFirstName").value = firstName;
    document.querySelector("#buyerLastName").value = lastName;
    document.querySelector("#buyerPhone").value = phone;
    document.querySelector("#buyerListContainer").style.display = "none";
};

// ============================================
// BASKET MANAGEMENT (Single product per item)
// ============================================
window.addToBasket = (id, title, price) => {
    const existingItem = basket.find(item => item.productId === id);

    if (existingItem) {
        alert(`"${title}" is already in the basket. You cannot add it again.`);
        return;
    }

    const qtyInput = document.querySelector(`#qty_${id}`);
    const qty = qtyInput ? parseInt(qtyInput.value) || 1 : 1;

    basket.push({
        productId: id,
        title: title,
        unitPrice: price,
        amount: qty
    });

    alert(`"${title}" has been added to the basket successfully.`);
    renderBasket();
};

const renderBasket = () => {
    const container = document.querySelector("#basketContainer");
    if (!container) return;

    let totalSum = 0;

    container.innerHTML = basket.map((item, index) => {
        const itemTotal = item.unitPrice * item.amount;
        totalSum += itemTotal;

        return `
        <tr>
            <td>${item.title}</td>
            <td>${item.amount}</td>
            <td>
                <input type="number" class="form-control form-control-sm" value="${item.unitPrice}"
                    onchange="updateBasketPrice(${index}, this.value)">
            </td>
            <td class="fw-bold">${itemTotal}</td>
            <td>
                <button type="button" class="btn btn-sm btn-danger" onclick="removeFromBasket(${index})">X</button>
            </td>
        </tr>`;
    }).join('');

    document.querySelector("#grandTotal").innerText = totalSum;
};

window.updateBasketPrice = (index, value) => {
    basket[index].unitPrice = parseFloat(value) || 0;
    renderBasket();
};

window.removeFromBasket = (index) => {
    basket.splice(index, 1);
    renderBasket();
};

// ============================================
// ORDER SUBMISSION (Create & Update)
// ============================================
const initOrderSubmission = () => {
    const submitBtn = document.querySelector("#submitOrderBtn");
    if (!submitBtn) return;

    submitBtn.onclick = async () => {
        const buyerId = document.querySelector("#buyerId").value;
        if (!buyerId) return alert("Please select a buyer first!");
        if (basket.length === 0) return alert("Basket is empty!");

        const orderPayload = {
            GuidKey: currentEditingOrderGuid || generateGuid(),
            SellerId: "00000000-0000-0000-0000-000000000000",
            BuyerId: buyerId,
            OrderDate: new Date().toISOString(),
            TotalAmount: parseFloat(document.querySelector("#grandTotal").innerText),
            Details: basket.map(i => ({
                GuidKey: i.guidKey || generateGuid(),
                ProductId: i.productId,
                ProductTitle: i.title,
                UnitPrice: parseFloat(i.unitPrice),
                Amount: parseFloat(i.amount)
            }))
        };

        try {
            const url = currentEditingOrderGuid ? `${baseUrl}/Order/Put` : `${baseUrl}/Order/Post`;
            const method = currentEditingOrderGuid ? 'PUT' : 'POST';

            const response = await fetch(url, {
                method: method,
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(orderPayload)
            });

            if (!response.ok) {
                const errorText = await response.text();
                throw new Error(errorText);
            }

            alert(currentEditingOrderGuid ? "Order updated successfully!" : "Order submitted successfully!");

            basket = [];
            renderBasket();
            resetToNewOrderMode();
            loadOrders();
        } catch (error) {
            console.error(error);
            alert("Error: " + error.message);
        }
    };
};

// ============================================
// EDIT ORDER
// ============================================
window.handleEdit = async (guidKey) => {
    currentEditingOrderGuid = guidKey;

    try {
        const response = await api.get(`${baseUrl}/Order/GetById?GuidKey=${guidKey}`);
        const order = response.value || response;

        if (!order) return alert("Order not found!");

        // Fill buyer info
        document.querySelector("#buyerId").value = order.buyerId || "";
        document.querySelector("#buyerFirstName").value = order.buyerName ? order.buyerName.split(' ')[0] : "";
        document.querySelector("#buyerLastName").value = order.buyerName ? order.buyerName.split(' ').slice(1).join(' ') : "";
        document.querySelector("#buyerPhone").value = order.buyerPhone || "";

        // Fill basket - IMPORTANT: Keep original detail GuidKey for update
        basket = order.details.map(d => ({
            productId: d.productId,
            title: d.productTitle || "Unknown Product",
            unitPrice: d.unitPrice,
            amount: d.amount,
            guidKey: d.guidKey || generateGuid()
        }));

        renderBasket();

        // Switch button to Update mode
        const submitBtn = document.querySelector("#submitOrderBtn");
        submitBtn.textContent = "Update Order";
        submitBtn.classList.remove("btn-success");
        submitBtn.classList.add("btn-warning");

        alert("Order loaded for editing. Make your changes and click Update Order.");
    } catch (err) {
        console.error(err);
        alert("Error loading order for edit.");
    }
};

const resetToNewOrderMode = () => {
    currentEditingOrderGuid = null;
    const submitBtn = document.querySelector("#submitOrderBtn");
    submitBtn.textContent = "Submit Order";
    submitBtn.classList.remove("btn-warning");
    submitBtn.classList.add("btn-success");
};

// ============================================
// ORDERS LIST & ACTIONS
// ============================================
const loadOrders = async () => {
    try {
        let orders = await api.get(`${baseUrl}/Order/GetAll`);
        if (orders.value) orders = orders.value;

        const tbody = document.querySelector("#ordersListTable");
        if (!tbody) return;

        const uniqueOrders = orders.filter((order, index, self) =>
            index === self.findIndex(o => o.guidKey === order.guidKey)
        );

        tbody.innerHTML = uniqueOrders.map(o => `
            <tr>
                <td>${new Date(o.orderDate).toLocaleDateString('en-US')}</td>
                <td>${o.buyerName || "N/A"}</td>
                <td class="fw-bold">${o.totalAmount.toLocaleString()}</td>
                <td>
                    <button class="btn btn-sm btn-primary me-1" onclick="handleEdit('${o.guidKey}')">Edit</button>
                    <button class="btn btn-sm btn-info me-1" onclick="handleDetail('${o.guidKey}')">Details</button>
                    <button class="btn btn-sm btn-danger" onclick="handleDelete('${o.guidKey}')">Delete</button>
                </td>
            </tr>
        `).join('');
    } catch (e) {
        console.error("Failed to load orders:", e);
    }
};

window.handleDetail = async (guidKey) => {
    try {
        const response = await api.get(`${baseUrl}/Order/GetById?GuidKey=${guidKey}`);
        const order = response.value || response;

        if (!order) {
            alert("Order details not found.");
            return;
        }

        const dateObj = new Date(order.orderDate);
        const formattedDate = !isNaN(dateObj.getTime())
            ? dateObj.toISOString().split('T')[0]
            : order.orderDate;

        let detailsHtml = `
            <div class="card p-4 shadow-sm mt-3 bg-light">
                <div class="d-flex justify-content-between align-items-center mb-3">
                    <h4 class="mb-0">Order Details</h4>
                    <button class="btn-close" onclick="this.closest('.card').remove()"></button>
                </div>
                
                <div class="row mb-3">
                    <div class="col-md-6">
                        <p><strong>Order Date:</strong> ${formattedDate}</p>
                        <p><strong>Buyer Name:</strong> <span class="fw-bold">${order.buyerName || "N/A"}</span></p>
                        <p><strong>Phone Number:</strong> ${order.buyerPhone || "N/A"}</p>
                    </div>
                    <div class="col-md-6 text-md-end">
                        <p><strong>Total Amount:</strong> <span class="text-success fw-bold">${order.totalAmount.toLocaleString()} Toman</span></p>
                    </div>
                </div>
                
                <hr>
                
                <h5 class="mb-2">Ordered Products:</h5>
                <div class="table-responsive">
                    <table class="table table-sm table-bordered table-striped bg-white">
                        <thead class="table-dark">
                            <tr>
                                <th>Product Name</th>
                                <th>Unit Price</th>
                                <th>Amount / Qty</th>
                                <th>Total Price</th>
                            </tr>
                        </thead>
                        <tbody>
                            ${order.details && order.details.length > 0 ? order.details.map(d => `
                                <tr>
                                    <td>${d.productTitle || "Product"}</td>
                                    <td>${d.unitPrice.toLocaleString()}</td>
                                    <td>${d.amount}</td>
                                    <td>${(d.unitPrice * d.amount).toLocaleString()}</td>
                                </tr>
                            `).join('') : '<tr><td colspan="4" class="text-center text-muted">No items found for this order</td></tr>'}
                        </tbody>
                    </table>
                </div>

                <div class="text-end mt-3">
                    <button class="btn btn-secondary btn-sm px-4" onclick="this.closest('.card').remove()">Close</button>
                </div>
            </div>
        `;

        let container = document.querySelector("#orderDetailsContainer");
        if (!container) {
            container = document.createElement("div");
            container.id = "orderDetailsContainer";
            container.className = "container mt-4";
            document.body.appendChild(container);
        }

        container.innerHTML = detailsHtml;
        container.scrollIntoView({ behavior: 'smooth' });

    } catch (err) {
        console.error("Failed to load details:", err);
        alert("Error loading order details.");
    }
};

window.handleDelete = async (guidKey) => {
    if (confirm("Are you sure you want to delete this order?")) {
        try {
            await api.delete(`${baseUrl}/Order/Delete`, { GuidKey: guidKey });
            alert("Order deleted successfully.");
            loadOrders();
        } catch (err) {
            alert("Error: " + err.message);
        }
    }
};

// ============================================
// INITIALIZATION
// ============================================
window.addEventListener("DOMContentLoaded", () => {
    loadCatalog();
    initBuyerSelection();
    initOrderSubmission();
    loadOrders();
});