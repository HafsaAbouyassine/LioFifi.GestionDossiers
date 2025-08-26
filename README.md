# 📌 LioFifi Gestion Dossiers

Une application **WPF en C# / .NET 8** connectée à une base de données **SQL Server**, permettant de gérer des dossiers avec des fonctionnalités de statistiques, impression, export PDF et suivi des activités récentes.

🔗 **Télécharger la dernière version** : [Releases](https://github.com/HafsaAbouyassine/LioFifi.GestionDossiers/releases/tag/FinProjet)

---

## ✨ Fonctionnalités

- 📊 **Statistiques dynamiques** : suivi du nombre de dossiers en cours, en attente, terminés.
- 📝 **Création / Modification de dossiers** avec interface intuitive.
- 🖨️ **Impression & Export PDF** des statistiques et rapports via `FlowDocument`.
- 📅 **Suivi des dernières activités** (dernières modifications de dossiers).
- 👩‍💻 **Tableaux de performance des agents** (dossiers traités, taux de complétion).

---

## 🎥 Démo de l'application
![Demo Application](LioFifiApp.gif)


---

## 🛠️ Technologies utilisées

- **C# / .NET 8**
- **WPF (Windows Presentation Foundation)**
- **Entity Framework Core**
- **SQL Server (LocalDB ou SQL Server standard)**
- **FlowDocument** (impression & export PDF)

---

## 🚀 Installation

### 1️⃣ Prérequis
- Windows 10 / 11
- [SQL Server LocalDB](https://learn.microsoft.com/fr-fr/sql/database-engine/configure-windows/sql-server-express-localdb) ou un serveur SQL classique

### 2️⃣ Téléchargement
1. Rendez-vous dans la section 👉 [Releases](https://github.com/HafsaAbouyassine/LioFifi.GestionDossiers/releases/tag/FinProjet)  
2. Téléchargez la version compressée `.zip`  
3. Extrayez le contenu  
4. Lancez `CPAM.GestionDossiers.exe`

---

## ⚙️ Configuration base de données

Par défaut, l’application utilise LocalDB :
```csharp
.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=CPAMDB;Trusted_Connection=True;")
