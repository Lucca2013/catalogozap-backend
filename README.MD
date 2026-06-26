<div align="center">
<h1>CatálogoZap</h1>

![GitHub Stars](https://img.shields.io/github/stars/Lucca2013/catalogozap-backend?style=social)
![GitHub Forks](https://img.shields.io/github/forks/Lucca2013/catalogozap-backend?style=social)
![GitHub Issues](https://img.shields.io/github/issues/Lucca2013/catalogozap-backend)
![GitHub License](https://img.shields.io/github/license/Lucca2013/catalogozap-backend)
<br>
CatálogoZap is a simple and practical solution to create digital product catalogs integrated with WhatsApp.
</div>

## about
CatálogoZap is a website project where you can create a store (of any type) and list your products, including name, price, image, and description. When a customer wants to buy your product, they click a "Buy via WhatsApp" button and are redirected to a conversation with the store owner, who can then send a message indicating which product they want.

CatálogoZap is a project created for training purposes by <a href="https://github.com/Lucca2013">Lucca</a>

## technologies/stack
 - Backend: C# ASP.NET
 - JWT
 - Dapper
 - Postgresql (at neon.com)
 - Cloudinary (Image Database)
 - Repository pattern

## Features
- Accounts system
- Create and manage stores with name, image and description 
- Register products with name, price, image, and description
- WhatsApp integration for direct sales
- Image upload using Cloudinary
- JWT authentication

##  How to run the project

### Prerequisites
- .NET 9.0 SDK
- PostgreSQL
- Cloudinary account

### Steps
```bash
git clone https://github.com/pandasoli/catalogozap-backend
cd catalogozap-backend
dotnet restore
```
configure .env
```bash
JWT_KEY=
JWT_ISSUER=
JWT_AUDIENCE=
CONNECTION_STRING=
CLOUDINARY_KEY=
```
and run
```bash
dotnet run
```
