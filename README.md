# 💰 Next-Gen Expense Tracker

## 📖 Project Description
The **Next-Gen Expense Tracker** is a robust, full-stack ASP.NET Core web application that empowers users to seamlessly monitor their personal finances, analyze spending trends, and manage custom budget categories. Built with a heavy emphasis on user experience, the application features an elegant, glassmorphic UI that offers a native **Light/Dark Mode toggle**, highly responsive interactive charts, and complete data isolation between user accounts.

## 🛠️ Tech Stack
*   **Backend:** C# / ASP.NET Core MVC
*   **Database & ORM:** SQL Server & Entity Framework Core (EF Core)
*   **Authentication:** ASP.NET Core Identity (Roles, Secure Sessions)
*   **Frontend:** HTML5, Bootstrap 5, Custom CSS Variables (for Theme Scaling)
*   **Analytics:** Chart.js (Interactive Data Visualization)

## ✨ Core Features
*   **🔒 Secure Multi-Tenancy & User Isolation:** Complete data privacy. Every expense and category is firmly bound to the authenticated user's session.
*   **📂 Per-User Category Seeding:** New accounts are instantly initialized with fully editable default spending buckets (Groceries, Bills, Entertainment, etc.).
*   **🎨 Stunning Modern UI:** A premium, fully responsive interface featuring floating form labels, soft-box shadows, and borderless unified table layouts. 
*   **🌗 Adaptive Light & Dark Mode:** Global CSS variables map across every single UI element, dropdown, and calendar picker, seamlessly inverting high-contrast elements and backgrounds at the click of a button.
*   **📊 Dynamic Dashboard Analytics:** Track monthly trends via full-width line charts and monitor budget limits via circular doughnut graphs.
*   **✉️ Robust Account Verification:** Integrated mock SMTP client enabling users to request Resend Email verifications securely using Base64 tokens.

## 📸 Demo & Screenshots
> **Note:** Upload your application screenshots here once deployed.

*   *Dashboard View (Dark Mode)*: `[Insert Link to Dashboard Screenshot]`
*   *Expenses Grid*: `[Insert Link to Expenses Grid Screenshot]`
*   *Login/Register Gateway*: `[Insert Link to Auth Screenshot]`

## 🚀 Getting Started Locally
1. Clone the repository: `git clone https://github.com/your-username/ExpenseTracker.git`
2. Apply database migrations: `dotnet ef database update`
3. Run the application: `dotnet run`
